import { SchedulerRule } from './../../models/scheduler-rule';
import { Container } from './../../models/container';
import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class ContainerService {

  private baseUrl: string;

  private httpOptions: object;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
  }

  public startFlushingQueues(currentUserId: string, queues: string[]): Observable<any> {
    const body = JSON.stringify({
      userId: currentUserId,
      queues: queues
    });
    return this.http.post<any>(this.baseUrl + 'api/Container/StartFlushingQueues', body, this.httpOptions);
  }

  public createRepository(repositoryName: string): Observable<any> {
    const body = JSON.stringify({
      name: repositoryName
    });
    return this.http.post<any>(this.baseUrl + 'api/Container/CreateRepository', body, this.httpOptions);
  }

  public checkImage(repositoryName: string): Observable<boolean> {
    const body = JSON.stringify({
      name: repositoryName
    });
    return this.http.post<any>(this.baseUrl + 'api/Container/CheckImage', body, this.httpOptions);
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
    return this.http.post<boolean>(this.baseUrl + 'api/Container/CreateConfiguration', body, this.httpOptions);
  }

  public runImage(userId: string, configName: string): Observable<any> {
    const body = JSON.stringify({
      configName: configName,
      taskGroupName: userId
    });
    return this.http.post<boolean>(this.baseUrl + 'api/Container/RunImage', body, this.httpOptions);
  }

  public stopTask(taskId: string): Observable<any> {
    const body = '"' + taskId + '"';
    return this.http.post<boolean>(this.baseUrl + 'api/Container/StopTask', body, this.httpOptions);
  }

  public scheduleImageFixedRate(userId: string, configName: string, ruleName: string, rate: number, time: string): Observable<any> {
    const body = JSON.stringify({
      configName: configName,
      ruleName: ruleName,
      tasksGroupName: userId,
      rate: rate,
      time: time
    });
    return this.http.post<boolean>(this.baseUrl + 'api/Container/ScheduleImageFixedRate', body, this.httpOptions);
  }

  public scheduleImageCronExp(userId: string, configName: string, ruleName: string, cronExp: string): Observable<any> {
    const body = JSON.stringify({
      configName: configName,
      ruleName: ruleName,
      tasksGroupName: userId,
      cronExpression: cronExp
    });
    return this.http.post<boolean>(this.baseUrl + 'api/Container/ScheduleImageCronExp', body, this.httpOptions);
  }

  public listSchedulerRules(userId: string): Observable<SchedulerRule[]> {
    const body = '"' + userId + '"';
    return this.http.post<SchedulerRule[]>(this.baseUrl + 'api/Container/ListSchedulerRules', body, this.httpOptions);
  }

  public listContainers(userId: string): Observable<Container[]> {
    const body = '"' + userId + '"';
    return this.http.post<Container[]>(this.baseUrl + 'api/Container/ListContainers', body, this.httpOptions);
  }

  public enableSchedulerRule(ruleName: string): Observable<any> {
    const body = '"' + ruleName + '"';
    return this.http.post<any>(this.baseUrl + 'api/Container/EnableSchedulerRule', body, this.httpOptions);
  }

  public disableSchedulerRule(ruleName: string): Observable<any> {
    const body = '"' + ruleName + '"';
    return this.http.post<any>(this.baseUrl + 'api/Container/DisableSchedulerRule', body, this.httpOptions);
  }

  public deleteSchedulerRule(ruleName: string): Observable<any> {
    const body = '"' + ruleName + '"';
    return this.http.post<any>(this.baseUrl + 'api/Container/DeleteSchedulerRule', body, this.httpOptions);
  }

  public stopScheduledImage(configName: string): Observable<any> {
    const body = '"' + configName + '"';
    return this.http.post<boolean>(this.baseUrl + 'api/Container/StopScheduledImage', body, this.httpOptions);
  }

  public getUserQueues(userId: string): Observable<string[]> {
    const body = '"' + userId + '"';
    return this.http.post<string[]>(this.baseUrl + 'api/Container/GetUserQueues', body, this.httpOptions);
  }

  public getUserTopics(userId: string): Observable<string[]> {
    const body = '"' + userId + '"';
    return this.http.post<string[]>(this.baseUrl + 'api/Container/GetUserTopics', body, this.httpOptions);
  }

  public updateUserChannels(userId: string, topics: string[], queues: string[]): Observable<boolean> {
    const body = JSON.stringify({
      userId: userId,
      topics: topics,
      queues: queues
    });
    return this.http.post<boolean>(this.baseUrl + 'api/Container/UpdateUserChannels', body, this.httpOptions);
  }
}
