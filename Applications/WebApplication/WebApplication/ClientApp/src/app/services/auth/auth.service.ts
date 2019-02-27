import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl: string;

  private _loggedInStatus: boolean;

  public get loggedInStatus(): boolean {
    return this._loggedInStatus;
  }

  public set loggedInStatus(value: boolean) {
    this._loggedInStatus = value;
  }

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.loggedInStatus = false;
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
        this.loggedInStatus = result;
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

  public isUserLoggedIn(): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.get<boolean>(this.baseUrl + 'api/Auth/IsLoggedIn', httpOptions).pipe(
      tap(result => {
        console.log(result);
      }),
      catchError(this.handleError)
    );
  }

  public signOutUser(): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.get<boolean>(this.baseUrl + 'api/Auth/SignOut', httpOptions).pipe(
      tap(result => {
        console.log(result);
      }),
      catchError(this.handleError)
    );
  }

  private handleError(err: HttpErrorResponse) {
    const errorMessage = `Server returned code: ${err.status}, error message is: ${err.error}`;
    console.log(errorMessage);
    return throwError(errorMessage);
  }

}
