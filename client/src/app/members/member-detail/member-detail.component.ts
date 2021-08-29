import {  Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_service/account.service';
import { MembersService } from 'src/app/_service/members.service';
import { MessageService } from 'src/app/_service/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit , OnDestroy {
  @ViewChild("memberTabs" , {static: true}) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  messages: Message[] = [] ; // neu khong khoi tao se nhan error khi truy xuat message.length
  member: Member ;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  paramSelectTab: number = -1;
  user: User;
  constructor(private memberService: MembersService , 
    private route:ActivatedRoute ,
    private messageService: MessageService,
    private acccountService: AccountService , 
    private router: Router
    ) {
        this.acccountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
        // không lưu lại cache của router , vi vây khi Hub gửi message Angular sẽ làm mới router
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
     }
  ngOnInit(): void {
  
    this.route.data.subscribe(data => {
      //resolve: {member: MemberDetailedResolver} trong app.routing.module.ts
      this.member = data.member; // member sẽ lấy trước khi contructor khoi tao template
    });

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    });
   // this.loadMember();
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }]
      this.galleryImages = this.getImage();
  }
  getImage(): NgxGalleryImage[] {
    const imgUrls = [];
    for(const photo of this.member.photos) {
      imgUrls.push({
        small: photo?.url ,
        medium: photo?.url,
        big: photo?.url
      })
    }
    return imgUrls;
  }
 onTabActivated(data: TabDirective) {
    this.activeTab = data;
    // nếu Tag là message thì Load Messsign từ SingR
    if(this.activeTab.heading === "Messages" && this.messages.length === 0 ) {
      // load message từ Api
      // this.loadMessage(); 
      //load Message từ Singl R
      this.messageService.createHubConnection(this.user , this.member.username);
    }
    else 
    { 
      this.messageService.stopHubConnection(); 
    }

  }
  loadMessage() {
    this.messageService.getMessageThread(this.member.username).subscribe(message => {
      this.messages = message;      
    })
  }
  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }
  // khi không activite Tag thì phải hủy Conection của SignR
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
