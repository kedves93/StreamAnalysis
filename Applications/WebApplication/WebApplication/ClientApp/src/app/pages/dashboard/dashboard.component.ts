import { AuthService } from './../../services/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { ContainerService } from './../../services/container/container.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(private containerService: ContainerService, private auth: AuthService) { }

  ngOnInit() {
  }

  public onSignOut(): void {
    this.auth.signOutUser().subscribe(result => console.log('signed out'));
  }

}
