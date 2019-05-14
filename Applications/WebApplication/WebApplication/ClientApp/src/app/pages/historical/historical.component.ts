import { Chart } from './../../models/chart';
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

  public get charts(): Chart[] {
    return this.historicalService.charts;
  }

  public set charts(value: Chart[]) {
    this.historicalService.charts = value;
  }

  public timeframeOptions: SelectItem[];

  public selectedQueue: Queue;

  public selectedTimeframe: number;

  public addQueueDialogShow = false;

  public chartOptions: any;

  constructor(private auth: AuthService, private router: Router, private historicalService: HistoricalService) {
    this.timeframeOptions = [
      { label: 'Last hour', value: 1 },
      { label: 'Last day', value: 24 },
      { label: 'Last week', value: 168 }
    ];

    this.chartOptions = {
      legend: {
        display: false
      },
      layout: {
        padding: 3
      },
      scales: {
        yAxes: [{
          ticks: {
            beginAtZero: true,
            fontColor: '#333333',
            fontSize: 12,
            maxTicksLimit: 5
          }
        }],
        xAxes: [{
          ticks: {
            fontColor: '#333333',
            fontSize: 12
          }
        }]
      }
    };
  }

  ngOnInit() {
  }

  public onOkAddQueue(): void {
    if (this.availableQueues.length === 0) {
      return;
    }

    // add chart
    this.historicalService.addChart(new Chart(this.selectedQueue.name, this.selectedTimeframe));

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

}
