import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  public items: MenuItem[];

  constructor() { }

  ngOnInit() {
    this.items = [
      { label: 'Home', icon: 'fa fa-fw fa-bar-chart' },
      { label: 'Cale', icon: 'fa fa-fw fa-calendar' },
      { label: 'Contact', icon: 'fa fa-fw fa-book' }
    ];
  }

}
