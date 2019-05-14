import { QueueData } from './../../models/queue-data';
import { Queue } from './../../models/queue';
import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HistoricalService {

  private baseUrl: string;

  private httpOptions: object;

  public availableQueues: Queue[];

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };

    const queuesArr = [];
    this.getAllQueues().subscribe(queues => {
      for (let i = 0; i < queues.length; i++) {
        queuesArr.push(new Queue(queues[i]));
        this.availableQueues = queuesArr;
      }
    });
  }

  public getAllQueues(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'api/History/GetAllQueues', this.httpOptions);
  }

  public getQueueData(queueName: string, timeframe: number): Observable<QueueData[]> {
    const body = JSON.stringify({
      queueName: queueName,
      timeframeInHours: timeframe
    });
    return this.http.post<QueueData[]>(this.baseUrl + 'api/History/GetHistoricalData', body, this.httpOptions);
  }
}
