import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public loggedInStatus = false;

  public currentUser: string;

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
      tap(result => {
        console.log(result);
      }),
      catchError(this.handleError)
    );
  }

  public signInUser(username: string, password: string, rememberMe: boolean): Observable<boolean> {
    const body = JSON.stringify({
      username: username,
      password: password,
      rememberMe: rememberMe
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Auth/SignIn', body, httpOptions).pipe(
      tap(result => {
        console.log(result);
      }),
      catchError(this.handleError)
    );
  }

  public registerUser(email: string, username: string, password: string): Observable<boolean> {
    const body = JSON.stringify({
      email: email,
      username: username,
      password: password
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

  public isUserLoggedIn(): boolean {
    if (localStorage.getItem('currentUser') === null) {
      return false;
    } else {
      return true;
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
