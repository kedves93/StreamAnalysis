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

  public timeframeOptions: SelectItem[];

  public selectedQueue: Queue;

  public selectedTimeframe: number;

  public addQueueDialogShow = false;

  constructor(private auth: AuthService, private router: Router, private historicalService: HistoricalService) {
    this.timeframeOptions = [
      { label: 'Last hour', value: 1 },
      { label: 'Last day', value: 24 },
      { label: 'Last week', value: 168 }
    ];
  }

  ngOnInit() {
  }

  public onOkAddQueue(): void {
    this.historicalService.getQueueData(this.selectedQueue.name, this.selectedTimeframe).subscribe(x => console.log(x));

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
