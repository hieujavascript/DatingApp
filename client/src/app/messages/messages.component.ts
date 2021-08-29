
import {Component, OnDestroy, OnInit } from '@angular/core';
import {ToastrService } from 'ngx-toastr';
import { Subject, Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';
import { ConfirmService } from '../_service/confirm.service';
import { MessageService } from '../_service/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit , OnDestroy  {

  // messages:Message[];
  // pageNumber: number = 1;
  // pageSize: number = 5;
  // container: string = "Unread"; // đây là mặc định ở server
  // pagination: Pagination;
  // isloading = false;
  private subscription: Subscription;
  messages: Message[] ;

  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;
  mysubject: any;

  constructor(
    private messageService: MessageService ,    
    private confirmService: ConfirmService  
    ) { }


    ngOnInit(): void {
      this.loadMessages();
    }
  // loadMessage() {
  //   this.isloading = true;
  //   this.messageService.getMesage(this.pageNumber , this.pageSize , this.container)
  //   .subscribe(response => {
  //     this.pagination = response.pagination;
  //     this.messages =  response.result;
  //     this.isloading = false;
  //   })
  // }
  loadMessages() {
    this.loading = true;
    this.messageService.getMesage(this.pageNumber, this.pageSize, this.container).subscribe(response => {
      this.messages = response.result;
     // this.mySubjectMessage.next(this.messages);
      this.pagination = response.pagination;
      this.loading = false;
      
    })
  }
  // deleteMessage(id: number) {

  //    // this.toast.success(" delete successful"+ id );
  //     //slice tra ve 1  mang tu chi so index den .leng
  //     //const animals = ['ant', 'bison', 'camel', 'duck', 'elephant']
  //     //animals.slice(2)
  //   // expected output: Array ["camel", "duck", "elephant"]
    
  //   this.confirm.confirm("confirm" , "are you sure delete?").subscribe((result) => {
  //     if(result) {
  //       this.messageService.deleteMessage(id).subscribe(() => {
  //         this.messages.slice(this.messages.findIndex(m => m.Id ===  id) , 1);
  //          console.log("message : ");
  //          console.log(this.messages);
  //       });
  //     }
  //   })
  // }
  // pageChange(event: any) {
  //   this.pageNumber = event.page;
  //   this.loadMessage();
  // }
  deleteMessage(id: number) {
    
    // this.confirmService.confirm('Confirm delete message', 'This cannot be undone').subscribe(result => {
    //   if (result) {
    //     // this.messageService.deleteMessage(id).subscribe(() => {          
    //     //   this.messages.splice(this.messages.findIndex(m => m.Id === id) , 1);  
    //     //   console.log(this.messages);       
    //     // })
    //   }
    // })
   this.mysubject = this.messageService.deleteMessage(id).subscribe(() => {          
       this.messages.splice(this.messages.findIndex(m => m.id === id) , 1);  
      // this.loadMessages();
      //this.messages = this.messages.filter(x => x.Id !== id);
    })
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadMessages();
  }
  ngOnDestroy(): void {
   // this.subscription.unsubscribe();
  }
  // ko dung ko sao
  trackMessage(index , message) {
    console.log(message);
    return message ? message.id : undefined;
  }
}
