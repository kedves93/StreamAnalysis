import { Component, OnInit } from '@angular/core';
import { ContainerService } from './../../services/container/container.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(private containerService: ContainerService) { }

  ngOnInit() {
  }

  public onCreateTaskDefinition() {
    this.containerService.createTaskDefinition(
      '526110916966.dkr.ecr.eu-central-1.amazonaws.com/stream-analysis/apixu:latest',
      'TaskDefinitionSzilard',
      'ContainerNameSzilard',
      true,
      true)
      .subscribe(
        result => console.log(result),
        error => console.error(error)
    );
  }

  public onRunTask() {
    this.containerService.runTask('sometaskDefinitionName')
      .subscribe(
        result => console.log(result),
        error => console.error(error)
    );
  }

}
