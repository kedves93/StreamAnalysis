import { DashboardService } from './../../services/dashboard/dashboard.service';
import { AuthService } from './../../services/auth/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(private dashboardService: DashboardService, private auth: AuthService) { }

  ngOnInit() {
    this.dashboardService.startMessages();
  }

  public onSignOut(): void {
    this.auth.signOutUser();
  }

}
