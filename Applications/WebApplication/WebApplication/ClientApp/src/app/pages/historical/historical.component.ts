import { HistoricalData } from './../../models/historical-data';
import { QueueData } from './../../models/queue-data';
import { HistoricalService } from './../../services/historical/historical.service';
import { Queue } from './../../models/queue';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AuthService } from './../../services/auth/auth.service';
import { Router } from '@angular/router';
import { SelectItem } from 'primeng/api';

@Component({
  selector: 'app-historical',
  templateUrl: './historical.component.html',
  styleUrls: ['./historical.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class HistoricalComponent implements OnInit {

  public get availableQueues(): Queue[] {
    return this.historicalService.availableQueues;
  }

  public set availableQueues(value: Queue[]) {
    this.historicalService.availableQueues = value;
  }

  public get historicalDataList(): HistoricalData[] {
    return this.historicalService.historicalDataList;
  }

  public set historicalDataList(value: HistoricalData[]) {
    this.historicalService.historicalDataList = value;
  }

  public selectedQueue: Queue;

  public addQueueDialogShow = false;

  constructor(private auth: AuthService, private router: Router, private historicalService: HistoricalService) {
  }

  ngOnInit() {
  }

  public onOkAddQueue(): void {
    if (this.availableQueues.length === 0) {
      return;
    }

    // add historical data
    this.historicalService.addHistoricalData(this.selectedQueue);

    // delete topic from available list
    const aux = this.availableQueues;
    const index = aux.findIndex(t => t.name === this.selectedQueue.name);
    aux.splice(index, 1);
    this.availableQueues = [];
    for (let i = 0; i < aux.length; i++) {
      this.availableQueues.push(aux[i]);
    }

    // close dialog
    this.addQueueDialogShow = false;
  }

  public onSignOut(): void {
    this.auth.signOutUser();
    this.router.navigate(['/login']);
  }

  public onAddQueue(): void {
    this.addQueueDialogShow = true;
  }

  public onDeleteZoomChart(queue: string) {
    this.historicalService.deleteHistoricalData(queue);

    // add queue to available list
    const aux = this.availableQueues;
    aux.push(new Queue(queue));
    this.availableQueues = [];
    for (let i = 0; i < aux.length; i++) {
      this.availableQueues.push(aux[i]);
    }
  }

}
