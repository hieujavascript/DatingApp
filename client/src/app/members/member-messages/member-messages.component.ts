import {ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { take } from 'rxjs/operators';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_service/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
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
  
  //constructor(private messageService: MessageService) {}
  constructor(public messageService: MessageService) {} // chuyen thanh public de lấy trực tiếp bên html


  ngOnInit(): void {
    // this.messageService.messageThread$.subscribe(message => {
    //   console.log(message);
    // })
  }
  sendMessage() {
    this.messageService.sendMessage(this.username , this.messageContent).then(() => {
        //this.messages.push(message);     
        this.messageForm.reset();
      }
    )
  }
 
}
