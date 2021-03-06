import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_service/account.service';

@Injectable({
  providedIn: 'root'
})
// CanActivate nếu nó là False , Route sẽ không được thông qua 
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService , private toastr: ToastrService) {}
  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if(user)
        return true;
        
        this.toastr.error("you can't take member");
      })
    )
  }
  
}
