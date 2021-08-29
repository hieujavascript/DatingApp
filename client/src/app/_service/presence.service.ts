import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
// kiểm trả một người là Online hay không
export class PresenceService { 
  hubUrl = environment.hubUrl;
  hubConnection: HubConnection; // goi trong Signalr
  // new BehaviorSubject<string[]>([]);  chứa Mảng string những người đang online khở tạo là Rỗng
  private userOnlineSource = new BehaviorSubject<string[]>([]);
  // lấy dữ liệu UserOnlineSource khi nó next
  onlineUser$ = this.userOnlineSource.asObservable();

  constructor(private toast: ToastrService , private router: Router) { }
  createHubConnection(user: User) {
    // chúng ta ko thể sử dụng Token trong Header 
    // do đó phải tạo HubConnectionBuilder để sử dụng

    this.hubConnection = new HubConnectionBuilder()
    // địa chỉ API của this.hubUrl + "presence" được 
    // định nghĩa trong Startup.cs endpoints.MapHub<PresenceHub>("hubs/presence");
                          .withUrl(this.hubUrl + "presence" , {
    // sử dụng token của user curent truyền như là QueryString cho Hub API
    // //cấu hinh IdentityServiceExtension cho phép Client send Token
                            accessTokenFactory: () => user.token 
                          })
                          // khi có vấn đề về mạng , sẽ tự động kết nối lại cho Hub
                          .withAutomaticReconnect()
                          .build()
    
    //Start hubConnection khi Login và Register
    this.hubConnection.start().catch(error => console.log(error));
    
    // listen "UseOnlineMethod"
    //// username là danh sách những người đang online mới , ko bao gồm trong danh sách cũ
    this.hubConnection.on("UserIsOnline" , username => {   
      // this.toast.info(username + "had connected");
      // thay bằng cập nhật Username OnlineisOnlien trong PresentTrack
      this.onlineUser$.pipe(take(1)).subscribe(usernames => { // messages là 1 array user đang online
        // cập nhật lại ds người đang online khi họ có connection 
        this.userOnlineSource.next([...usernames , username]);
      }) 
    });
    this.hubConnection.on("UserIsOffline" , username => {
     //  this.toast.warning(username + "had disconnected");
     this.onlineUser$.pipe(take(1)).subscribe(usernames => { // messages là 1 array user đang Offline
      // cập nhật lại ds người đang online khi họ mất connection 
      // danh sách online sẽ còn lại những usernames(online) mà ko nằm trong bảng usernae (offline)
      this.userOnlineSource.next([...usernames.find(x => x != username )]);
    }) 

    })
    // nó sẽ tự động kết nối với Presencehub.CS và gọi phương thức GetOnlineUsers
    this.hubConnection.on("GetOnlineUsers" , (username: string[]) => {
        this.userOnlineSource.next(username);
    } )

    // hub.clients.clients sẽ liên kết với hàm on ở đây
    this.hubConnection.on("NewMessageRecived" , ({username , knownas}) => {
      this.toast.info(username + "had send a messages for you" )
                .onTap // trigger khi click vào Toast
                .pipe(take(1))
                .subscribe(() => this.router.navigateByUrl("/members/" + username + "?tab=3"));
    });
    
  }
  stopHubConnection() {
    this.hubConnection.stop().catch(error => console.log(error));
  }
  
}
