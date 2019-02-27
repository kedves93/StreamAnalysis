import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MenuItem, SelectItem } from 'primeng/api';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ContainerService } from './../../services/container/container.service';
import { ClipboardService } from 'ngx-clipboard';

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

  public repositoryForm = new FormGroup({
    repositoryName: new FormControl('', Validators.required)
  });

  public configurationForm = new FormGroup({
    configName: new FormControl('', Validators.required),
    imageUri: new FormControl('', Validators.required),
    containerName: new FormControl('', Validators.required),
    interactive: new FormControl(''),
    pseudoTerminal: new FormControl(''),
    fixedRateValue: new FormControl('5'),
    fixedRateMeasurement: new FormControl(''),
    cronExpression: new FormControl({value: '', disabled: true})
  });

  public runImageForm = new FormGroup({
    fixedRateValue: new FormControl('5'),
    fixedRateMeasurement: new FormControl(''),
    cronExpression: new FormControl({value: '', disabled: true})
  });

  public timeMeasurements: SelectItem[];

  constructor(private containerService: ContainerService, private clipboardService: ClipboardService) { }

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
      this.runImageForm.controls['fixedRateValue'].disable();
      this.runImageForm.controls['fixedRateMeasurement'].disable();
      this.runImageForm.controls['cronExpression'].enable();
    } else {
      this.runImageForm.controls['fixedRateValue'].enable();
      this.runImageForm.controls['fixedRateMeasurement'].enable();
      this.runImageForm.controls['cronExpression'].disable();
    }
  }

  public onSubmitRepositoryForm(): void {
    this.containerService.createRepository(this.repositoryForm.value.repositoryName)
    .subscribe(
      result => console.log(result),
      error => console.log(error)
    );

    this.activeIndex = 1;
  }

  public onContinue(): void {
    this.activeIndex = 2;

    this.configurationForm.patchValue({
      imageUri: '526110916966.dkr.ecr.eu-central-1.amazonaws.com/' + this.repositoryForm.controls['repositoryName'].value + ':latest'
    });
  }

  public onSubmitConfigurationForm(): void {
    this.containerService.createConfiguration(
      this.configurationForm.value.configName,
      this.configurationForm.value.imageUri,
      this.configurationForm.value.containerName,
      this.configurationForm.value.interactive,
      this.configurationForm.value.pseudoTerminal
      )
    .subscribe(
      result => console.log(result),
      error => console.log(error)
    );

    this.activeIndex = 3;
  }

  public onSubmitRunImageForm(): void {
    if (this.isScheduledRunType) {
      if (this.isCronExpression) {
        this.containerService.scheduleImageCronExp(
          this.configurationForm.value.configName,
          this.runImageForm.value.cronExpression
        )
        .subscribe(
          result => console.log(result),
          error => console.log(error)
        );
      } else {
        this.containerService.scheduleImageFixedRate(
          this.configurationForm.value.configName,
          this.configurationForm.value.fixedRateValue,
          this.configurationForm.value.fixedRateMeasurement
        )
        .subscribe(
          result => console.log(result),
          error => console.log(error)
        );
      }
    } else {
      this.containerService.runImage(
        this.configurationForm.value.configName
      )
      .subscribe(
        result => console.log(result),
        error => console.log(error)
      );
    }
  }

  public copyBuildToClipboard(): void {
    this.clipboardService.copyFromContent('docker build -t ' + this.repositoryForm.value.repositoryName + ' .');
  }

  public copyTagToClipboard(): void {
    this.clipboardService.copyFromContent('docker tag ' + this.repositoryForm.value.repositoryName +
      ':latest 526110916966.dkr.ecr.eu-central-1.amazonaws.com/' + this.repositoryForm.value.repositoryName + ':latest');
  }

  public copyPushToClipboard(): void {
    this.clipboardService.copyFromContent('docker push 526110916966.dkr.ecr.eu-central-1.amazonaws.com/' +
      this.repositoryForm.value.repositoryName + ':latest');
  }

}
