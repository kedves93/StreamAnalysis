import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-container',
  templateUrl: './container.component.html',
  styleUrls: ['./container.component.css']
})
export class ContainerComponent implements OnInit {

  public stepItems: MenuItem[];

  constructor() { }

  ngOnInit() {
    this.stepItems = [
      { label: 'Repository' },
      { label: 'Push image' },
      { label: 'Configure' },
      { label: 'Run image' }
    ];
  }

}
