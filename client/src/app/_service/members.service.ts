import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {Member} from '../_models/member';
import {MemberUpdate} from '../_models/memberUpdate';
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  // HttpOptions = {
  //   headers : new HttpHeaders ({
  //     Authorization: 'Bearer '+ JSON.parse(localStorage.getItem('user'))?.token
  //   })
  // }

  members: Member[] = [];
  baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient ) { }

  getMembers(): Observable<Member[]> {
    // khi mà Mảng Member co dữ liệu thi khong can goi ve API nữa
    if(this.members.length  > 0 ) return of(this.members);
    // khi mà mảnh members khong co du lieu thi quay ve Api lay
    return this.http.get<Member[]>(this.baseUrl + "users").pipe(
      map( members => {
        this.members = members ;
        return this.members; // return trong ham Map Operator sẽ trả về 1 mảng Obsevable<member[]>
      })
    )
  }
  getMember(username: string): Observable<Member> {

    const member =  this.members.find(x => x.username == username);
    if(member !== undefined) // neu co member
    return of(member);

    // nguoc lại ko co member thi quay ve goi API
    return this.http.get<Member>(this.baseUrl + "users/" + username);
  }
  updateMember(member: Member) {
   return this.http.put(this.baseUrl + "users" , member).pipe(
     map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
     })
   )
  }
  setMainPhoto(photoId: number) {
    // return ve mot  new UserDto gom {
                //Username = user.UserName , 
              //  Token = this._tokenService.CreateToken(user),
             //   photoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
          //  };
    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId , {});
  }
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + "users/delete-photo/"+photoId);
  }
}
