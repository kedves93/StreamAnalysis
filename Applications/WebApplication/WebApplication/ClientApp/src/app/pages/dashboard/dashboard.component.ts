import { TopicData } from './../../models/topic-data';
import { HubService } from './../../services/hub/hub.service';
import { DashboardService } from './../../services/dashboard/dashboard.service';
import { AuthService } from './../../services/auth/auth.service';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Topic } from './../../models/topic';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class DashboardComponent implements OnInit {

  public get availableTopics(): Topic[] {
    return this.dashboardService.availableTopics;
  }

  public set availableTopics(value: Topic[]) {
    this.dashboardService.availableTopics = value;
  }

  public get topicsData(): TopicData[] {
    return this.hubService.topicsData;
  }

  public set topicsData(value: TopicData[]) {
    this.hubService.topicsData = value;
  }

  public selectedTopic: Topic;

  public addTopicDialogShow = false;

  constructor(private dashboardService: DashboardService, private auth: AuthService, private hubService: HubService,
    private router: Router) {

  }

  ngOnInit() {

  }

  public onOkAddTopic(): void {
    if (this.availableTopics.length === 0) {
      return;
    }

    // add card
    this.hubService.addTopicData(new TopicData(this.selectedTopic.name));

    // delete topic from available list
    const aux = this.availableTopics;
    const index = aux.findIndex(t => t.name === this.selectedTopic.name);
    aux.splice(index, 1);
    this.availableTopics = [];
    for (let i = 0; i < aux.length; i++) {
      this.availableTopics.push(aux[i]);
    }

    // close dialog
    this.addTopicDialogShow = false;
  }

  public onSignOut(): void {
    this.auth.signOutUser();
    this.auth.loggedInStatus = false;
    this.router.navigate(['/login']);
  }

  public onAddTopic(): void {
    this.addTopicDialogShow = true;
  }

  public onDeleteDashcard(topic: string): void {
    this.hubService.deleteTopicData(topic);

    // add topic to available list
    const aux = this.availableTopics;
    aux.push(new Topic(topic));
    this.availableTopics = [];
    for (let i = 0; i < aux.length; i++) {
      this.availableTopics.push(aux[i]);
    }
  }

}
