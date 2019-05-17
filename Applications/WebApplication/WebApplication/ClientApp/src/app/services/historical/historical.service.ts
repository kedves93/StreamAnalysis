import { HistoricalData } from './../../models/historical-data';
import { QueueData } from './../../models/queue-data';
import { Queue } from './../../models/queue';
import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HistoricalService {

  private baseUrl: string;

  private httpOptions: object;

  public availableQueues: Queue[];

  public historicalDataList: HistoricalData[] = [];

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

  public addHistoricalData(queue: Queue): void {
    this.getQueueData(queue.name).subscribe(queueData => {
      // new historicalData
      const newHistoricalData = new HistoricalData();
      newHistoricalData.queue = queueData[0].queue;
      newHistoricalData.measurement = queueData[0].measurement;
      newHistoricalData.values = [];
      for (let i = 0; i < queueData.length; i++) {
        const d = new Date(0);
        newHistoricalData.values.push({
          value: queueData[i].value,
          date: d.setSeconds(queueData[i].timestampEpoch)
        });
      }

      // update historicalDataList
      const aux = this.historicalDataList;
      this.historicalDataList = [];
      for (let i = 0; i < aux.length; i++) {
        this.historicalDataList.push(aux[i]);
      }
      this.historicalDataList.push(newHistoricalData);

    });
  }

  public deleteHistoricalData(queue: string): void {
    const index = this.historicalDataList.findIndex(h => h.queue === queue);
    this.historicalDataList.splice(index, 1);
  }

  public getAllQueues(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'api/History/GetAllQueues', this.httpOptions);
  }

  public getQueueData(queue: string): Observable<QueueData[]> {
    const body = '"' + queue + '"';
    return this.http.post<QueueData[]>(this.baseUrl + 'api/History/GetHistoricalData', body, this.httpOptions);
  }
}
