import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class ContainerService {

  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public createTaskDefinition(imageUri: string, taskDefinitionFamily: string,
    containerName: string, interactive: boolean, pseudoTerminal: boolean): Observable<boolean> {
    const body = JSON.stringify({
      imageUri: imageUri,
      taskDefinitionFamily: taskDefinitionFamily,
      containerName: containerName,
      interactive: interactive,
      pseudoTerminal: pseudoTerminal
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Container/CreateTaskDefinition', body, httpOptions).pipe(
      tap(result => console.log(result)),
      catchError(this.handleError)
    );
  }

  public runTask(taskDefinitionName: string): Observable<boolean> {
    const body = JSON.stringify({
      taskDefinitionName: taskDefinitionName
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Container/RunTask', body, httpOptions).pipe(
      tap(result => console.log(result)),
      catchError(this.handleError)
    );
  }

  private handleError(err: HttpErrorResponse) {
    const errorMessage = `Server returned code: ${err.status}, error message is: ${err.error}`;
    console.log(errorMessage);
    return throwError(errorMessage);
  }
}
