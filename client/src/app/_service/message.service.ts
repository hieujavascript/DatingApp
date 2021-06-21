import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { getPaginationHeaders, getPaginationResult } from './paginationhelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }
  getMesage(pageNumber: number , pageSize: number , container) {
    var params = getPaginationHeaders(pageNumber , pageSize); // param pagenumber va pagesize
    params = params.append("container" , container); // THEM Param mặc định container là Unread va2 httpParam
    var message_And_Pagination_And_Params$  = getPaginationResult<Message[]>(this.baseUrl + "messages" ,  params , this.http);
    return  message_And_Pagination_And_Params$;
  }
  getMessageThread(username: string) {
    
    return this.http.get<Message[]>(this.baseUrl + "messages/thread/" + username );
  }
  sendMessage(username: string , content: string) {
    return this.http.post<Message>(this.baseUrl + "messages" , {recipientUsername: username , content} )
  }
  deleteMessage(id: Number) {
    return this.http.delete(this.baseUrl + "messages/"+id);
  }
}
