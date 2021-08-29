import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_service/account.service';
import { PresenceService } from './_service/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client App component';
  constructor(private http: HttpClient , 
    private accountService: AccountService ,
    private presence: PresenceService
    ) {
  } 
  ngOnInit(): void {
    this.setCurrentUser();
  }
  setCurrentUser(){
    const user = JSON.parse(localStorage.getItem("user"));
    if(user)
    {
      this.accountService.setCurrentUser(user);      
      this.presence.createHubConnection(user);
    }
  }
}
