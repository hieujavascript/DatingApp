import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent implements OnInit {

  title: string; 
  message: string;
  btnOkText: string
  btnCanceText: string;
  result: boolean;
  // bsModelRef sẽ giúp chúng ta truy xuất và truyền dữ liệu title , message....(khởi tạo trong service) 
  //và lấy Result (khởi tạo trong component) qua lại giữa Sevice và Component
  constructor(public bsModelRef: BsModalRef) { } 

  ngOnInit(): void {
  }
  confirm() {
    this.result = true ;
    this.bsModelRef.hide();
  }
  decline() {
    this.result = false;
    this.bsModelRef.hide();
  }

  // openModal(template: TemplateRef<any>) {
  //   this.bsModelRef = this.bsModalService.show(template, {class: 'modal-sm'})
  // }

}
