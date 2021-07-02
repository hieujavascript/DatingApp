import { TemplateRef } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RoleModalsComponent } from 'src/app/modals/role-modals/role-modals.component';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_service/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  constructor(private adminService: AdminService ,  private modalService: BsModalService) { }
  users: Partial<User[]> = [];
  bsModalRef: BsModalRef;
  ngOnInit(): void {
    this.getUserWithRole();
  }
  getUserWithRole() {
    this.adminService.getuserWithRole().subscribe(users => {
      this.users = users;
    //  console.log(users);
    })
  }

  openRoleModals(user: User) {
    const config = {
    class: "modal-dialog-centered" , 
    initialState : {
      user , 
      roles: this.getCurrentRoleArray(user) //  [{name: "Admin" , value: "Admin" , true or false}]
    }};
    this.bsModalRef = this.modalService.show(RoleModalsComponent, config);    
    // khi click Button Update của Component Model thi nó phát ra 1 evenEmitter
    // thông báo cho các lớp subscribe nó rằng ĐÃ CÓ SỰ KIỆN 
    
    this.bsModalRef.content.updateSelectedRoles.subscribe(value =>{
    //  console.log(value);
      const roleToUpdate = {
        roles: [...value.filter(el => el.checked === true).map(el => el.name)]
      }
      if(roleToUpdate) {
        this.adminService.updateUserRole(user.username , roleToUpdate.roles).subscribe(() => {
          user.roles = [...roleToUpdate.roles];
        });
      }
    });
    
    // tra về cho component Role Modals một content
    //this.bsModalRef.content.closeBtnName = 'Close';
  } 
  getCurrentRoleArray(user: User) {
    const roles = [];
    const userRoles = user.roles;
    const availableRole:Irole[] = [
      {name: "Admin" , value: "Admin"},
      {name: "Moderator" , value: "Moderator"},
      {name: "Member" , value: "Member"}
    ];
    availableRole.forEach(role => {
      let isMatch = false;
        for(const userRole of userRoles) {
          if(role.name === userRole) {
            isMatch = true;
            role.checked = true; // true false của select
            roles.push(role);
            break; // thoat khoi vong for
          }
        }
        if(!isMatch) {
          role.checked = false;
          roles.push(role);
        }
    });
    return roles;
  }
}

interface  Irole {
   name?: string;
   value?: string;
   checked?: boolean;
}