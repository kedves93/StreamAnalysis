import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { webSocket } from 'rxjs/webSocket';
import { Observable, Subject } from 'rxjs';
import * as signalR from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {

  public connection: signalR.HubConnection;

  private baseUrl: string;

  constructor(@Inject('BASE_URL') baseUrl: string, private http: HttpClient) {
    this.baseUrl = baseUrl;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'hub/Dashboard')
      .build();

    this.connection
      .start()
      .then(() => {
        console.log('Connection started');
        this.connection.on('s3', (data) => {
          console.log(data);
        });
        this.http.get(this.baseUrl + 'api/Dashboard/SendMessage')
        .subscribe(res => {
          console.log(res);
        });
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  }
}
