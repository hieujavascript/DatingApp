import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable, of } from "rxjs";
import { Member } from "../_models/member";
import { MembersService } from "../_service/members.service";

// load dữ liệu trước Constructor
@Injectable({
    providedIn:'root'
})
export class MemberDetailedResolver implements Resolve<Member> { 
    constructor(private memberService: MembersService) {}
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):  Observable<Member> {
        // lay param member in app-routing
        var usernameRouter = this.memberService.getMember(route.paramMap.get('username'));  
        return usernameRouter;
    }    
}   