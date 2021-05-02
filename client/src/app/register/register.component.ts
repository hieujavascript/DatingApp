import { Component, Input, OnInit, Output , EventEmitter   } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_service/account.service';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  models: any = {};
  @Output() canceRegister = new EventEmitter<boolean>();
  constructor(private accountService: AccountService , private toastr: ToastrService) { }

  ngOnInit(): void {
  }
  register() {
   this.accountService.register(this.models).subscribe((user) => {
    console.log(user);
    this.cance();
   } , error => {
       console.log(error);
       this.toastr.error(error.error);
    });
  }
  cance() {
    this.canceRegister.emit(false);
  }

}
