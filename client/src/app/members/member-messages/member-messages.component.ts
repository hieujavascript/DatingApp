import {Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { take } from 'rxjs/operators';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_service/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit  {
  
  //tao Input lay UserName tu Message detail component  
 
   @Input() messages: Message[] = [];
   @Input() username: string = "";
   @ViewChild("messageForm") public messageForm: NgForm;
   messageContent:string; // tow way databiding [()] in html
  
  constructor(private messageService: MessageService) {}


  ngOnInit(): void {
    
  }
  sendMessage() {
    this.messageService.sendMessage(this.username , this.messageContent).pipe(take(1)).subscribe(
      message => {

        this.messages.push(message);     
        this.messageForm.reset();
      }
    )
  }
 
}
