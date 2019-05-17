import { TopicData } from './../../models/topic-data';
import { Injectable, Inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subscription, Observable, timer } from 'rxjs';
import { take, map, merge } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HubService {

  public connection: HubConnection;

  public topicsData: TopicData[] = [];

  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.connection = new HubConnectionBuilder().withUrl(this.baseUrl + 'hub/RealTimeMessages').build();
  }

  public addTopicData(topicData: TopicData): void {
    this.topicsData.push(topicData);

    // start messages
    this.getRealTimeMessagesFromTopic(topicData.topic);
  }

  public deleteTopicData(topic: string): void {
    // stop messages
    this.stopRealTimeMessagesFromTopic(topic);

    const index = this.topicsData.findIndex(t => t.topic === topic);
    this.topicsData.splice(index, 1);
  }

  private getRealTimeMessagesFromTopic(topic: string) {
    this.connection = new HubConnectionBuilder().withUrl(this.baseUrl + 'hub/RealTimeMessages').build();
    this.connection
      .start()
      .then(() => {
        // on message
        this.connection.on(topic, (data) => {
          if (this.topicsData.find(t => t.topic === topic) !== undefined) {
            this.topicsData.find(t => t.topic === topic).stream.next(data.value);
            this.topicsData.find(t => t.topic === topic).measurement = data.measurement;
          }
        });

        // initiate messaging
        const body = '"' + topic + '"';
        const httpOptions = {
          headers: new HttpHeaders({
            'Content-Type': 'application/json'
          })
        };
        this.http.post(this.baseUrl + 'api/Dashboard/StartRealTimeMessagesFromTopic', body, httpOptions)
          .subscribe(res => {
            console.log(res);
          });
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  private stopRealTimeMessagesFromTopic(topic: string) {
    this.connection.off(topic);

    // stop messaging
    const body = '"' + topic + '"';
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    this.http.post(this.baseUrl + 'api/Dashboard/StopRealTimeMessagesFromTopic', body, httpOptions)
      .subscribe(res => {
        console.log(res);
      });
  }
}
