import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl; 
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient , private presence: PresenceService) { } // dung httpClient de thuc hien viec Request CURD den server  
  // model se chua username va password khi chung ta gui request
  // account : controller ; 
  // login : HttpPost("login") 
  login(model: any) {
  return this.http.post(this.baseUrl + "account/login" , model).pipe(
    map((response: User ) => {
      const user = response;
      if(user) {
      // localStorage.setItem("user" , JSON.stringify(user));  
      // this.currentUserSource.next(user);
      this.setCurrentUser(user);
      // SignalR Create connection
      this.presence.createHubConnection(user);
      }
    }));
  }

  register(model : any) {
   return this.http.post(this.baseUrl +"account/register" , model).pipe(
      map((user: User) => {
        if(user) {
          // localStorage.setItem("user" , JSON.stringify(user));
          //this.currentUserSource.next(user);
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
        return user;
      })
    )
  }

  setCurrentUser(user: User) {
   // if(!user) return

    user.roles = []; // khai báo mảng rổng để nếu token tra về 1 chuổi thì ta biến nó thành 1 mảng
    // "role": ["Member","Moderator"] user.token.role , role nằm trong token  sau khi dc decodeToken
    const roles = this.getDecodedToken(user.token).role;
    // kiem tra xem Role là 1 chuỗi hay 1 mảng , nếu ko là mảng thì ta add nó vào mảng rỗng đã tạo
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    //JSON.stringify(user) chuyển user thành Json
    localStorage.setItem("user" , JSON.stringify(user));
    this.currentUserSource.next(user); // currentUser$ sẽ có chứa User có token  bên trong luôn
  }
  logout() {
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
    this.presence.stopHubConnection()
  }
  getDecodedToken(token: string) { //  giải mã token và trả về 1 JSON 
      return JSON.parse(atob(token.split(".")[1])); // bỏ bean . và lấy hết token phái sau TRẢ VỀ Object
  }
}
