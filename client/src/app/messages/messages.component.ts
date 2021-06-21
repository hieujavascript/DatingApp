
import {Component, OnInit } from '@angular/core';
import {ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_service/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages:Message[];
  pageNumber: number = 1;
  pageSize: number = 5;
  container: string = "Unread"; // đây là mặc định ở server
  pagination: Pagination;
  isloading = false;
  constructor(private messageService: MessageService , public toast: ToastrService) { }

  ngOnInit(): void {
    this.loadMessage();
  }
  loadMessage() {
    this.isloading = true;
    this.messageService.getMesage(this.pageNumber , this.pageSize , this.container)
    .subscribe(response => {
      this.pagination = response.pagination;
      this.messages =  response.result;
      this.isloading = false;
    })
  }
  deleteMessage(id: number) {
   
   
     // this.toast.success(" delete successful"+ id );
      //slice tra ve 1  mang tu chi so index den .leng
      //const animals = ['ant', 'bison', 'camel', 'duck', 'elephant']
      //animals.slice(2)
    // expected output: Array ["camel", "duck", "elephant"]
    this.messageService.deleteMessage(id).subscribe(() => {
      this.messages.slice(this.messages.findIndex(m => m.Id ===  id) , 1);
    } );
  }
  pageChange(event: any) {
    this.pageNumber = event.page;
    this.loadMessage();
  }

}
