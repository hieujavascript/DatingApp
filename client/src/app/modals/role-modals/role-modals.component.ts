import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subject } from 'rxjs';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-role-modals',
  templateUrl: './role-modals.component.html',
  styleUrls: ['./role-modals.component.css']
})
export class RoleModalsComponent implements OnInit {
 
  // @Input() updateSelectedRoles = new EventEmitter();
  updateSelectedRoles = new Subject<Irole[]>();
  user: User;  
  roles:Irole[] = [];
  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    
  }
  updateRoles() {
    //truyền ngược cái roles cho user-management component đã subscribe nó 
    this.updateSelectedRoles.next(this.roles);
    this.bsModalRef.hide();
  }

}
interface  Irole {
  name?: string;
  value?: string;
  checked?: boolean;
}
