import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/Pagination';
import { MembersService } from '../_service/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members: Partial<Member[]>;
  predicate:string = "liked"; // default
  pageNumber = 1;
  pageSize = 2;
  pagination: Pagination;
  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadLike();
  }
  loadLike() {
    this.memberService.getLike(this.predicate , this.pageNumber , this.pageSize).subscribe(response => {
      // response.result; du lieu lay trong response.body cua service
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }
  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadLike();
  }

}
