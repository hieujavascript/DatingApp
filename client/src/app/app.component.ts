import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client App component';
  url: string = "https://localhost:5001/api/Users";
  users: any;
  constructor(private http: HttpClient) {
  } 
  ngOnInit(): void {
   this.http.get(this.url).subscribe(
     response => {
      this.users = response;
     },error => console.log(error)
   );
  }
}
