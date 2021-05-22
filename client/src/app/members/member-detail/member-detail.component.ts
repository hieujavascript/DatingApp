import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { MembersService } from 'src/app/_service/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  member: Member ;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  constructor(private memberService: MembersService , private route:ActivatedRoute ) { }
  ngOnInit(): void {
    this.loadMember();
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }]
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

  loadMember() {
    this.memberService.getMember(this.route.snapshot.paramMap.get("username"))
        .pipe(take(1))
        .subscribe(user => {
          this.member = user;
          this.galleryImages = this.getImage();
        })
  }
}
