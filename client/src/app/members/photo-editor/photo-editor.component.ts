import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_service/account.service';
import { MembersService } from 'src/app/_service/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  user: User;
  constructor(private accountService: AccountService, private memberService: MembersService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }
  ngOnInit(): void {
    this.initializeUploader();
    console.log(this.member);
  }
  fileOverBase(e: any) { // error
    this.hasBaseDropzoneOver = e;
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }
    this.uploader.onSuccessItem = (item, response, status, headers) => {
        const photo:Photo = JSON.parse(response);
        this.member.photos.push(photo);
        if(photo.isMain) {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
        }
      }
  }
    setIsMainPhoto(photo: Photo) {
      this.memberService.setMainPhoto(photo.id).subscribe(
        () => {
          this.user.photoUrl = photo.url;      
          this.accountService.setCurrentUser(this.user);   // cap nhat localStore gom Token , url , username
          this.member.photoUrl = photo.url; // cap nhat url member
          this.member.photos.forEach(p => {
            // neu hien tai co isMain
            if(p.isMain) p.isMain = false
            if(p.id == photo.id) photo.isMain = true;
          })
        }
      )
    }
    deletePhoto(photoId: number) {
      this.memberService.deletePhoto(photoId).subscribe(() => {
        // tra ve cho chung ta cai mang ko co cai photo bang photoId do
        // vi vay chung ta ko phai cap nhat tai client
       this.member.photos =  this.member.photos.filter(x =>x.id !== photoId);
        //this.member.photos = this.member.photos.filter(x =>x.id == photoId);
      })
    }
}
