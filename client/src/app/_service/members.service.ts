import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {Member} from '../_models/member';
import {MemberUpdate} from '../_models/memberUpdate';
import { PaginatedResult } from '../_models/Pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/UserParams';
import { AccountService } from './account.service';
import { getPaginationHeaders, getPaginationResult } from './paginationhelper';
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  // HttpOptions = {
  //   headers : new HttpHeaders ({
  //     Authorization: 'Bearer '+ JSON.parse(localStorage.getItem('user'))?.token
  //   })
  // }
  memberCache = new Map();
  members: Member[] = [];
  baseUrl: string = environment.apiUrl;
  user: User;
  userParams: UserParams;
  // tạo ra class và Interface 
  // interface có các property giống y chang Header sẽ nhận từ Response.header.get('Pagination')
  // class<T> là tập kết quả nhận đc thỏa với điều kiện đc trả về từ Server api router

  constructor(private http: HttpClient , private accountService: AccountService ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
   }

  getMembers(userParams: UserParams) {
    // // khi mà Mảng Member co dữ liệu thi khong can goi ve API nữa
    // if(this.members.length  > 0 ) return of(this.members);
    // // khi mà mảnh members khong co du lieu thi quay ve Api lay
    // CACHE dữ liệu
    let userParamValue = Object.values(userParams).join('-');
    var response = this.memberCache.get(userParamValue);  
    // neu co response thi tra ve tu day , khong co thi tra ve tu server sau do luu vao cache
    if(response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber , userParams.pageSize);
    params = params.append("minAge" , userParams.minAge.toString());
    params = params.append("maxAge" , userParams.maxAge.toString());
    params = params.append("gender" , userParams.gender);
    params = params.append("OrderBy" , userParams.orderBy);
    return getPaginationResult<Member[]>(this.baseUrl + "users" , params , this.http)
               .pipe(map(response => {
                this.memberCache.set(userParamValue , response); 
                return response;
               }));
  }



  getMember(username: string): Observable<Member> {

    // const member =  this.members.find(x => x.username == username);
    // if(member !== undefined) // neu co member
    // return of(member);
    // // nguoc lại ko co member thi quay ve goi API
    const member = [...this.memberCache.values()]
                   .reduce((arr , elem) => arr.concat(elem.result) , [] ) // return ve 1 mảng Cache
                   .find((member: Member) => {
                    if(member.username === username)
                    return member.username;
                   } );
   
     if(member) {
      return of(member);
     }
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


  getUserParam() {
    // duoc tao tren Contructor
    return this.userParams;
  }
  setUserParam(param: UserParams) {
    this.userParams = param;
  }
  resetUserParam() {
    // user lay tren Contructor
    this.userParams = new UserParams(this.user);
    return this.userParams;
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

  addLike(username: string) {
    return this.http.post(this.baseUrl + "likes/" + username , {});
  }
  getLike(predicate: string , pageNumber: number , pageSize:number) {
    // predicate=" + predicate truyền trực tiếp trên câu Query
    let params = getPaginationHeaders(pageNumber , pageSize);
    // định nghĩa giá trị tham số Prediacate
    params = params.append('predicate' , predicate); // noi params vao trong header
    //params = params.append('iloveyou' , 'pac pac');
    return getPaginationResult<Partial<Member[]>>(this.baseUrl + "likes" , params , this.http);
    // return this.http.get<Partial<Member[]>>(this.baseUrl + "likes?predicate=" + predicate);
  } // 2:27

}
