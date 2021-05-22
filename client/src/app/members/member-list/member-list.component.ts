import { Component, OnInit } from '@angular/core';
import {MembersService} from '../../_service/members.service';
import {Member} from '../../_models/member';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members$: Observable<Member[]>;
  constructor(private memberService: MembersService ) { 
  }
  ngOnInit(): void {
    this.loadMembers();
  }
  public loadMembers(): void {
   this.members$ = this.memberService.getMembers();
  }

}
