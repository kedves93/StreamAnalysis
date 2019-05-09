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

    this.topicsData.find(t => t.topic === topic).lastUpdateSubscription.unsubscribe();
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
          this.topicsData.find(t => t.topic === topic).value = data.value;
          this.topicsData.find(t => t.topic === topic).measurement = data.measurement;
          this.topicsData.find(t => t.topic === topic).icon = data.icon;
          this.topicsData.find(t => t.topic === topic).lastUpdateSubscription.unsubscribe();
          this.topicsData.find(t => t.topic === topic).lastUpdateSubscription = this.updateTimer().subscribe(x => {
            this.topicsData.find(t => t.topic === topic).lastUpdate = x;
          });
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

  private updateTimer(): Observable<string> {
    const seconds = timer(0, 5 * 1000).pipe(take(12)).pipe(map(x => {
      if (x === 0) {
        return 'Just updated';
      } else {
        return 'updated ' + x * 5 + ' seconds ago';
      }
    }));
    const minutes = timer(60 * 1000, 60 * 1000).pipe(take(59)).pipe(map(x => {
      return 'updated ' + (x + 1) + ' minutes ago';
    }));
    const hours = timer(60 * 60 * 1000, 60 * 60 * 1000).pipe(map(x => {
      return 'updated ' + (x + 1) + ' hours ago';
    }));
    return seconds.pipe(merge(minutes)).pipe(merge(hours));
  }
}
