import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_service/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  user: User;
  constructor(private viewContainerRef: ViewContainerRef, 
    private templateRef: TemplateRef<any>, 
    private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
        this.user = user;
     //   console.log("Step1");
      })
     }

  ngOnInit(): void {
  //  console.log("Step2");
    // clear view if no roles
    if (!this.user?.roles || this.user == null) {
    //  console.log("not role");
      this.viewContainerRef.clear();
      return;
    }

    if (this.user?.roles.some(r  => this.appHasRole.includes(r))) {
    //  console.log("có role admin");
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
    //  console.log("có role nhung ko co admin");
      this.viewContainerRef.clear();
    }
  }
}

// coi lai phan update state directive 
