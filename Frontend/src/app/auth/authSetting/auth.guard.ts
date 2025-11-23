
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from 'src/app/shared/auth.service';


@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {


  constructor(private router: Router,private service : AuthService) {
  }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): boolean {
    if (localStorage.getItem('token') != null){
      let roles = next.data['permittedRoles'] as Array<string>;
      if(roles){
        if(this.service.roleMatch(roles)) return true;
        else{
          this.router.navigate(['/forbidden']);
          return false;
        }
      }
      return true;
    }
    else {
      this.router.navigate(['/Auth']);
      return false;
    }

  }
}
