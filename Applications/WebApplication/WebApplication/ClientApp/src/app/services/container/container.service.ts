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

  public createRepository(repositoryName: string): Observable<any> {
    const body = JSON.stringify({
      name: repositoryName
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<any>(this.baseUrl + 'api/Container/CreateRepository', body, httpOptions);
  }

  public createConfiguration(configName: string, imageUri: string,
    containerName: string, interactive: boolean, pseudoTerminal: boolean): Observable<any> {
    const body = JSON.stringify({
      name: configName,
      imageUri: imageUri,
      containerName: containerName,
      interactive: interactive,
      pseudoTerminal: pseudoTerminal
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Container/CreateConfiguration', body, httpOptions);
  }

  public runImage(configName: string): Observable<any> {
    const body = '"' + configName + '"';
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Container/RunImage', body, httpOptions);
  }

  public scheduleImageFixedRate(configName: string, rate: number, time: string): Observable<any> {
    const body = JSON.stringify({
      configName: configName,
      rate: rate,
      time: time
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Container/ScheduleImageFixedRate', body, httpOptions);
  }

  public scheduleImageCronExp(configName: string, cronExp: string): Observable<any> {
    const body = JSON.stringify({
      configName: configName,
      cronExp: cronExp
    });
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post<boolean>(this.baseUrl + 'api/Container/ScheduleImageCronExp', body, httpOptions);
  }
}
