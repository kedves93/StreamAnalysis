import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MenuItem, SelectItem } from 'primeng/api';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-container',
  templateUrl: './container.component.html',
  styleUrls: ['./container.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ContainerComponent implements OnInit {

  public stepItems: MenuItem[];

  public activeIndex = 0;

  public isCronExpression: boolean;

  public isScheduledRunType: boolean;

  public containerForm = new FormGroup({
    repositoryName: new FormControl('', Validators.required),
    configName: new FormControl('', Validators.required),
    imageUri: new FormControl('', Validators.required),
    containerName: new FormControl('', Validators.required),
    interactive: new FormControl(''),
    pseudoTerminal: new FormControl(''),
    fixedRateValue: new FormControl('5'),
    fixedRateMeasurement: new FormControl(''),
    cronExpression: new FormControl({value: '', disabled: true})
  });

  public timeMeasurements: SelectItem[];

  constructor() { }

  ngOnInit() {
    this.stepItems = [
      {
        label: 'Repository',
        command: (event: any) => {
          this.activeIndex = 0;
        }
      },
      {
        label: 'Push image',
        command: (event: any) => {
          this.activeIndex = 1;
        }
      },
      {
        label: 'Configure',
        command: (event: any) => {
          this.activeIndex = 2;
        }
      },
      {
        label: 'Run image',
        command: (event: any) => {
          this.activeIndex = 3;
        }
      }
    ];

    this.timeMeasurements = [
      {label: 'Minutes', value: 'Minutes'},
      {label: 'Hours', value: 'Hours'},
      {label: 'Days', value: 'Days'}
    ];

    this.isScheduledRunType = false;

    this.isCronExpression = false;
  }

  public onScheduleTypeClick(): void {
    if (this.isCronExpression) {
      this.containerForm.controls['fixedRateValue'].disable();
      this.containerForm.controls['fixedRateMeasurement'].disable();
      this.containerForm.controls['cronExpression'].enable();
    } else {
      this.containerForm.controls['fixedRateValue'].enable();
      this.containerForm.controls['fixedRateMeasurement'].enable();
      this.containerForm.controls['cronExpression'].disable();
    }
  }

  public onSubmit(): void {

  }

}
