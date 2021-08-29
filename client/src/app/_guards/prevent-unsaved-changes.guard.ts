import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_service/confirm.service';

@Injectable({
  providedIn: 'root'
})
// CanDeactivate  cho phép chúng ta có quyết định có rời Route hay ko
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  constructor(private confirm: ConfirmService) {}

  canDeactivate(component: MemberEditComponent ): Observable<boolean> | boolean {
    if(component.editForm.dirty) {
        //    return confirm("du lieu dang thay doi , ban co muon thoat nhung khong save khong ?")
        // canDeactivate tự thực thi Obsevable
        return this.confirm.confirm("Confirm" , "do you want to do that ?");
    }
    return true;
  }
  
}
