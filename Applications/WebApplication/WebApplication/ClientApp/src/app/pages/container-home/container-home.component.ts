import { SchedulerRule } from './../../models/scheduler-rule';
import { MenuItem } from 'primeng/api';
import { ContainerService } from './../../services/container/container.service';
import { Container } from './../../models/container';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AuthService } from './../../services/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-container-home',
  templateUrl: './container-home.component.html',
  styleUrls: ['./container-home.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ContainerHomeComponent implements OnInit {

  public containerTableCols: any[];

  public schedulerRuleTableCols: any[];

  public containers: Container[];

  public schedulerRules: SchedulerRule[];

  public selectedContainer: Container;

  public selectedSchedulerRule: SchedulerRule;

  public containerContextMenuItems: MenuItem[];

  public schedulerRulesContextMenuItems: MenuItem[];

  constructor(private auth: AuthService, private router: Router, private containerService: ContainerService) { }

  ngOnInit() {
    this.containerTableCols = [
      { field: 'id', header: 'Id' },
      { field: 'status', header: 'Status' },
      { field: 'startedAt', header: 'Started Running At' }
    ];

    this.schedulerRuleTableCols = [
      { field: 'id', header: 'Id' },
      { field: 'state', header: 'State' },
    ];

    this.containerContextMenuItems = [
      { label: 'Stop', icon: 'fa fa-stop', command: (event) => this.stopContainer(this.selectedContainer) }
    ];

    this.schedulerRulesContextMenuItems = [
      { label: 'Enable', icon: 'fa fa-play', command: (event) => this.enableSchedulerRule(this.selectedSchedulerRule) },
      { separator: true },
      { label: 'Disable', icon: 'fa fa-pause', command: (event) => this.disableSchedulerRule(this.selectedSchedulerRule) },
      { separator: true },
      { label: 'Delete', icon: 'pi pi-trash', command: (event) => this.deleteSchedulerRule(this.selectedSchedulerRule) }
    ];

    this.onRefreshContainers();
    this.onRefreshSchedulerRules();
  }

  public onLogOut(): void {
    this.auth.signOutUser();
    this.auth.containerLoggedInStatus = false;
    this.router.navigate(['/container/login']);
  }

  public onAddContainers(): void {
    this.router.navigate(['/container/create']);
  }

  public onRefreshContainers(): void {
    this.auth.getCurrentUserId(this.auth.currentUser).subscribe(userId => {
      this.containerService.listContainers(userId).subscribe(containers => {
        this.containers = containers;
      });
    });
  }

  public onRefreshSchedulerRules(): void {
    this.auth.getCurrentUserId(this.auth.currentUser).subscribe(userId => {
      this.containerService.listSchedulerRules(userId).subscribe(rules => {
        this.schedulerRules = rules;
      });
    });
  }

  private stopContainer(container: Container) {
    this.containerService.stopTask(container.taskId).subscribe();
  }

  private enableSchedulerRule(schedulerRule: SchedulerRule) {
    this.containerService.enableSchedulerRule(schedulerRule.id).subscribe();
  }

  private disableSchedulerRule(schedulerRule: SchedulerRule) {
    this.containerService.disableSchedulerRule(schedulerRule.id).subscribe();
  }

  private deleteSchedulerRule(schedulerRule: SchedulerRule) {
    this.containerService.deleteSchedulerRule(schedulerRule.id).subscribe();
  }
}
