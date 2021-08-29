import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalRef: BsModalRef;
  constructor(private modalService: BsModalService) {}
  confirm(title: string =  "Confirmation" ,  
          message: string = "Are you sure you want to do  this ?",
          btnOkText = "Ok",
          btnCanceText = "Cance"): Observable<boolean>
          {
            const config = {
              initialState: {
                title , 
                message ,
                btnOkText , 
                btnCanceText
              }
            }
            // this.bsModalRef = this.modalService.show('confirm' , config);
            this.bsModalRef = this.modalService.show(ConfirmDialogComponent , config);
    return new Observable<boolean>(this.getResult()); // 
  }
// 4:10
    // viết ra 1 Observable để truyền biến Result có thể dụng bên trong mọi Component
  private getResult() {
      return (observer) => {
        // khi model đóng thì phát ra 1 Evenemitter
          const subscription = this.bsModalRef.onHidden.subscribe(() => {
            // sau truyền Thành công Value (true hoặc false) thì sẽ complete và hủy unsubscribe
            observer.next(this.bsModalRef.content.result);
            observer.complete();
          })
          return {
            unsubscribe() {
              subscription.unsubscribe();
            }
          }
      }
  }
}
