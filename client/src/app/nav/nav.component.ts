import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import {AccountService} from '../_service//account.service'
@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit  {
  
  model: any = {};
  f: boolean = true;
  constructor(
    public accountService: AccountService , 
    private router: Router,
    private toastr: ToastrService
    ) 
    { }
 

  ngOnInit(): void {
  
  }
  login () {
    this.accountService.login(this.model).subscribe(response => {
      this.f = true;
      this.router.navigateByUrl('/members');     
    });
  }
  logout() {
    this.f = false;
    this.accountService.logout();
    this.router.navigateByUrl("/");
  }
}
