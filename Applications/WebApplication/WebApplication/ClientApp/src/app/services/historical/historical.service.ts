import { Chart } from './../../models/chart';
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

  public charts: Chart[] = [];

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

  public addChart(chart: Chart): void {
    this.getQueueData(chart.queueName, chart.timeframe).pipe(map(queueData => {
      if (queueData.length === 0) {
        return {
          datasets: [
            {
              label: chart.queueName
            }
          ]
        };
      }

      chart.measurement = queueData[0].measurement;

      const labels = [];
      const dataset = [];

      switch (chart.timeframe) {
        case 1: {
          for (let i = 0; i < 12; i++) {
            labels.push((i + 1) * 5  + 'm ago');
            dataset.push(0);
          }
          for (let i = 0; i < queueData.length; i++) {
            for (let j = 0; j < dataset.length; j++) {
              if ((j * 5) === +queueData[i].lifetimeInMinutes.toFixed()) {
                dataset[j] = queueData[i].value;
              }
            }
          }
          break;
        }
        case 24: {
          for (let i = 0; i < 24; i++) {
            labels.push((i + 1) + 'h ago');
            dataset.push(0);
          }
          for (let i = 0; i < queueData.length; i++) {
            for (let j = 0; j < dataset.length; j++) {
              if (j === +queueData[i].lifetimeInHours.toFixed()) {
                dataset[j] = queueData[i].value;
              }
            }
          }
          break;
        }
        case 168: {
          for (let i = 0; i < 7; i++) {
            labels.push(i + 'days ago');
            dataset.push(0);
          }
          for (let i = 0; i < queueData.length; i++) {
            for (let j = 0; j < dataset.length; j++) {
              if (j === +queueData[i].lifetimeInDays.toFixed()) {
                dataset[j] = queueData[i].value;
              }
            }
          }
          break;
        }
      }

      return {
        labels: labels,
        datasets: [
          {
            label: chart.queueName,
            backgroundColor: 'rgb(17,102,187,0.4)',
            borderColor: 'rgb(17,102,187)',
            data: dataset
          }
        ]
      };
    })).subscribe(chartData => {
      chart.dataset = chartData;
      this.charts.push(chart);
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
