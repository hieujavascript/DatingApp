import { Component, OnInit } from '@angular/core';
import {MembersService} from '../../_service/members.service';
import {Member} from '../../_models/member';
import { Observable } from 'rxjs';
import {Pagination} from '../../_models/Pagination';
import { UserParams } from 'src/app/_models/UserParams';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_service/account.service';
import { take } from 'rxjs/operators';
@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList = [{value: 'male' , display: 'Males'} , {value: 'female' , display: 'FeMales'}]
  // pageNumber: number = 1;
  // pageSize:number = 5;
  constructor(private memberService: MembersService , private accountService: AccountService ) { 
    // this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
    //   this.user = user;
    //   this.userParams = new UserParams(user);
    // })
    this.userParams =  this.memberService.getUserParam();
  }
  ngOnInit(): void {
    this.loadMembers();
  }
  public loadMembers(): void {
   // console.log(this.userParams);
    // do UserParam la 1 Class , muon dung duoc ở đây phai khoi tao doi tuong new trong Contructor.
    this.memberService.setUserParam(this.userParams);
    this.memberService.getMembers(this.userParams).subscribe(
      response => {
        this.members = response.result;
        this.pagination = response.pagination;
      }
    )
  }
  resetFilters() {
    // user dc khoi tao trogn constructor
    this.userParams = this.memberService.resetUserParam();
    this.loadMembers();
  }
  pageChanged(event: any) {
    this.userParams.pageNumber  = event.page;
    this.loadMembers();
  }

}
