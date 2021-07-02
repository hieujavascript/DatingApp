import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }
  getuserWithRole() {
    // su dung Partial vi chi tra ve 1 phan cua User 
   return this.http.get<Partial<User[]>>(this.baseUrl + "admin/users-with-roles");
  }

  public updateUserRole(username: string , roles: any  ) {
    return this.http.post(this.baseUrl + "admin/edit-roles/" + username + "?roles="+roles , {});
  } 


}
