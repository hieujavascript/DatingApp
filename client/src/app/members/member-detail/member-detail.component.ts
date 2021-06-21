import {  Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
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
  constructor(private memberService: MembersService , 
    private route:ActivatedRoute ,
    private messageService: MessageService 
    ) { }
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
    if(this.activeTab.heading === "Messages" && this.messages.length === 0 ) {
      this.loadMessage();
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
  ngOnDestroy(): void {
   // this.messageService.stopHubConnection();
  }
}
