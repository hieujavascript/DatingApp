import { HttpClient } from '@angular/common/http';

import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Toast, ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { getPaginationHeaders, getPaginationResult } from './paginationhelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  private hubConection: HubConnection; // dùng Private chặn connec từ bên ngoài


  constructor(private http: HttpClient , private toast: ToastrService) { }
  getMesage(pageNumber: number , pageSize: number , container) {
    var params = getPaginationHeaders(pageNumber , pageSize); // param pagenumber va pagesize
    params = params.append("container" , container); // THEM Param mặc định container là Unread va2 httpParam
    var message_And_Pagination_And_Params$  = getPaginationResult<Message[]>(this.baseUrl + "messages" ,  params , this.http);
    return  message_And_Pagination_And_Params$;
  }
  getMessageThread(username: string) {
    
    return this.http.get<Message[]>(this.baseUrl + "messages/thread/" + username );
  }

  deleteMessage(id: Number) {
    return this.http.delete(this.baseUrl + "messages/"+id);
  }

  createHubConnection(user:User , otherUsername: string) {
    console.log("connection lai");
    // tạo k ết nối tới Hub
    // this.busyService.busy();
    this.hubConection = new HubConnectionBuilder()
                        //.configureLogging(LogLevel.Debug)
                        .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
                          accessTokenFactory: () => user.token
                        }) 
                        .withAutomaticReconnect()
                        .build();
    this.hubConection.start().catch(error => console.log(error));    
    
    this.hubConection.on("ReceiveMessageThread" , messages => {    
      this.messageThreadSource.next(messages);  
    });

    // cập nhật tình trạng đã đọc khi 1 user connec vào
    this.hubConection.on("UpdateGroup" , (group) => {
      // some chỉ cần trong mảng connection có otherUsername sẽ trả về true        
      if(group.connections.some(x => x.username === otherUsername)) {
        // lấy ra messages cũ từ Obsevable
        this.messageThread$.pipe(take(1)).subscribe(messages => {
            messages.forEach(message => {
              if(!message.dateRead) // nếu nó ko là kiểu Date type
              {
                message.dateRead = new Date(Date.now());
              }              
            });
            this.messageThreadSource.next([...messages]);
        });
      }
    });

    // trả về 1 mang message cho Component (Cập nhật Message vào messageThreadSource)
    this.hubConection.on("NewMessage" , messageDTO => {
      this.messageThread$.pipe(take(1)).subscribe(message => {
          this.messageThreadSource.next([...message , messageDTO]);          
      });
    })

  }
  stopHubConnection() {
    if(this.hubConection){
      console.log("disconnec");
      this.hubConection.stop();
    }
  }
  async sendMessage(username: string , content: string) {
    // send message từ API
    // return this.http.post<Message>(this.baseUrl + "messages" , {recipientUsername: username , content} )
    
    // send message từ Hub Message , gọi hàm CreateMessage từ HubMessage 
    // nó ko trả về Obsevable mà trả về Promise , vì vậy Http Interceptor ko bắt đc cái lỗi và phải Catch
    return this.hubConection.invoke("CreateMessage" , {recipientUsername: username , content} )
                                  .catch(error => console.log(error));
                                  
  }
}
