import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public loggedInStatus = false;

  public containerLoggedInStatus = false;

  public currentUser: string;

  public userType: string;

  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public getCurrentUserId(username: string): Observable<string> {
    const body = '"' + username + '"';
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<string>(this.baseUrl + 'api/Auth/GetCurrentUserId', body, httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  public signInUser(username: string, password: string, containerUser: boolean): Observable<boolean> {
    const body = JSON.stringify({
      username: username,
      password: password,
      containerUser: containerUser
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Auth/SignIn', body, httpOptions).pipe(
      tap(result => {
        if (result) {
          console.log('Valid user');
        } else {
          console.log('Invalid user');
        }
      }),
      catchError(this.handleError)
    );
  }

  public registerUser(email: string, username: string, password: string, containerUser: boolean): Observable<boolean> {
    const body = JSON.stringify({
      email: email,
      username: username,
      password: password,
      containerUser: containerUser
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Auth/Register', body, httpOptions).pipe(
      tap(result => console.log(result)),
      catchError(this.handleError)
    );
  }

  public isContainerUserLoggedIn(): boolean {
    if (localStorage.getItem('currentUser') !== null && localStorage.getItem('userType') === 'container') {
      return true;
    } else {
      return false;
    }
  }

  public isDashboardUserLoggedIn(): boolean {
    if (localStorage.getItem('currentUser') !== null && localStorage.getItem('userType') === 'dashboard') {
      return true;
    } else {
      return false;
    }
  }

  public signOutUser(): void {
    this.currentUser = null;
    localStorage.clear();
  }

  private handleError(err: HttpErrorResponse) {
    const errorMessage = `Server returned code: ${err.status}, error message is: ${err.error}`;
    console.log(errorMessage);
    return throwError(errorMessage);
  }

}
