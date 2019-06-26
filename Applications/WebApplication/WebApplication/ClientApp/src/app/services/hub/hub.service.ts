import { TopicData } from './../../models/topic-data';
import { Injectable, Inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class HubService {

  public connection: HubConnection;

  public topicsData: TopicData[] = [];

  private baseUrl: string;

  constructor(@Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.connection = new HubConnectionBuilder().withUrl(this.baseUrl + 'hub/RealTimeMessages').build();
    this.connection.on('OnNewMessageArrived', (data) => {
      if (this.topicsData.find(t => t.topic === data.topic) !== undefined) {
        this.topicsData.find(t => t.topic === data.topic).stream.next(data.value);
        this.topicsData.find(t => t.topic === data.topic).measurement = data.measurement;
      }
    });
    this.connection.start()
      .then(() => console.log('Connection started'))
      .catch(error => console.log('Error while starting connection: ' + error));
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
    this.connection.invoke('StartRealTimeMessagesFromTopic', topic)
      .then(() => console.log('Started listening on topic: ' + topic))
      .catch(error => console.log(error));
  }

  private stopRealTimeMessagesFromTopic(topic: string) {
    this.connection.invoke('StopRealTimeMessagesFromTopic', topic)
      .then(() => console.log('Stopped listening on topic: ' + topic))
      .catch(error => console.log(error));
  }
}
