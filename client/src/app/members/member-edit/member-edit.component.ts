import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { MemberUpdate } from 'src/app/_models/memberUpdate';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_service/account.service';
import { MembersService } from 'src/app/_service/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  user: User; // username va Token co duoc sau khi login
  member: Member ; // Schema AppUser mô tả trong Database
  // xac nhan co muon thoat khoi man hinh khi form dang thay doi
  @HostListener('window:beforeunload' , ['$event']) onBeforeUnload($event: any) {
    $event.returnValue = true;
  }

  constructor(private accountService: AccountService , 
    private memberService: MembersService ,
    private toast: ToastrService
    ) { }

  ngOnInit(): void {
   
    this.getMember();
  }
  // get User
  getUser() {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user =>
      this.user = user
      );
  }
  // su dung UserName trong User de lay ra Member theo username
  getMember() {
    this.getUser(); // lay ra User , de su dung username trong User
    this.memberService.getMember(this.user.username).pipe(take(1)).subscribe(
      member => this.member = member
    );
  }
  updateMember() {
    this.memberService.updateMember(this.member).subscribe(val => {
      this.toast.success('Profile update successful');
      // reset form nay xac nhan la FROM o trang thai DA NHAN NUT SAVE 
      this.editForm.reset(this.member);
    });
  }
}
