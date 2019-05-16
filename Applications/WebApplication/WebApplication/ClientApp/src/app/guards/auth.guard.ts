import { AuthService } from './../services/auth/auth.service';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private authService: AuthService) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
      if (this.authService.loggedInStatus) {
        return true;
      }
      if (this.authService.isDashboardUserLoggedIn()) {
        this.authService.loggedInStatus = true;
        this.authService.currentUser = localStorage.getItem('currentUser');
        this.authService.userType = localStorage.getItem('userType');
        return true;
      } else {
        this.router.navigate(['/login']);
        return false;
      }
  }
}
