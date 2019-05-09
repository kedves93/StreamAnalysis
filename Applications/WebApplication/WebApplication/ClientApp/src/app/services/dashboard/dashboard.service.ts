import { Topic } from './../../models/topic';
import { Injectable, Inject } from '@angular/core';
import { Subscription, Observable, timer } from 'rxjs';
import { take, map, merge } from 'rxjs/operators';
import { HubService } from '../hub/hub.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private baseUrl: string;

  private httpOptions: object;

  public availableTopics: Topic[];

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private hub: HubService) {
    this.baseUrl = baseUrl;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };

    const topicsArr = [];
    this.getAllTopics().subscribe(topics => {
      for (let i = 0; i < topics.length; i++) {
        topicsArr.push(new Topic(topics[i]));
        this.availableTopics = topicsArr;
      }
    });
  }

  public getAllTopics(): Observable<string[]> {
    return this.http.get<string[]>(this.baseUrl + 'api/Dashboard/GetAllTopics', this.httpOptions);
  }
}
