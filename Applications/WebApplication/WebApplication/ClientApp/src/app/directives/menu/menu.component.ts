import { Component, OnInit, ViewEncapsulation, HostListener } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { trigger, transition, style, animate, state } from '@angular/animations';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css'],
  animations: [
    trigger('navbarHoverTrigger', [
      state('false', style({ width: '*' })),
      state('true', style({ width: '250px' })),
      transition('false <=> true', animate(100))
    ])
  ],
  encapsulation: ViewEncapsulation.None
})
export class MenuComponent implements OnInit {

  public isNavbarHovered: boolean;

  public activeItem: string;

  @HostListener('mouseenter')
  onMouseEnter() {
    this.isNavbarHovered = true;
  }

  @HostListener('mouseleave')
  onMouseLeave() {
    this.isNavbarHovered = false;
  }

  constructor() { }

  ngOnInit() {
  }

  public onNavbarItemClick(futureNavbarItem: string): void {
    this.activeItem = futureNavbarItem;
  }

  public isActive(currentNavbarItem: string) {
    return this.activeItem === currentNavbarItem;
  }

}
