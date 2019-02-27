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
      return this.authService.isUserLoggedIn().pipe(tap(result => {
        if (result) {
          this.authService.loggedInStatus = true;
          return true;
        } else {
          this.router.navigate(['/login']);
          return false;
        }
      }));
  }
}
