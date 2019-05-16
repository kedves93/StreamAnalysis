import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MenuItem, SelectItem, ConfirmationService } from 'primeng/api';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';
import { ContainerService } from './../../services/container/container.service';
import { ClipboardService } from 'ngx-clipboard';
import { AuthService } from './../../services/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-container',
  templateUrl: './container.component.html',
  styleUrls: ['./container.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ContainerComponent implements OnInit {

  public currentUserId: string;

  public stepItems: MenuItem[];

  public activeIndex = 0;

  public isCronExpression: boolean;

  public isScheduledRunType: boolean;

  public get repositoryName() {
    return this.currentUserId + '-' + this.repositoryForm.value.repositoryName;
  }

  public brokerForm = new FormGroup({
    topics: new FormArray([]),
    queues: new FormArray([])
  });

  public repositoryForm = new FormGroup({
    repositoryName: new FormControl('', Validators.required)
  });

  public pushImageContinueButtonDisable = true;

  public configurationForm = new FormGroup({
    configName: new FormControl('', Validators.required),
    imageUri: new FormControl('', Validators.required),
    containerName: new FormControl('', Validators.required),
    interactive: new FormControl(''),
    pseudoTerminal: new FormControl(''),
    cronExpression: new FormControl({value: '', disabled: true})
  });

  public runImageForm = new FormGroup({
    ruleName: new FormControl(''),
    fixedRateValue: new FormControl('5'),
    fixedRateMeasurement: new FormControl(''),
    cronExpression: new FormControl({value: '', disabled: true})
  });

  public timeMeasurements: SelectItem[];

  constructor(
    private containerService: ContainerService,
    private auth: AuthService,
    private router: Router,
    private clipboardService: ClipboardService,
    private confirmationService: ConfirmationService) { }

  ngOnInit() {
    this.stepItems = [
      {
        label: 'Broker',
        command: (event: any) => {
          this.activeIndex = 0;
        }
      },
      {
        label: 'Repository',
        command: (event: any) => {
          this.activeIndex = 1;
        }
      },
      {
        label: 'Push image',
        command: (event: any) => {
          this.activeIndex = 2;
        }
      },
      {
        label: 'Configure',
        command: (event: any) => {
          this.activeIndex = 3;
        }
      },
      {
        label: 'Run image',
        command: (event: any) => {
          this.activeIndex = 4;
        }
      }
    ];

    this.timeMeasurements = [
      {label: 'Minutes', value: 'minutes'},
      {label: 'Hours', value: 'hours'},
      {label: 'Days', value: 'days'}
    ];

    this.isScheduledRunType = false;

    this.isCronExpression = false;

    this.getCurrentUserId();

    this.getCurrentUserQueues();

    this.getCurrentUserTopics();
  }

  public onSubmitBrokerForm(): void {
    this.auth.getCurrentUserId(this.auth.currentUser).subscribe(id => {
      const t = <FormArray>this.brokerForm.get('topics');
      const topics = t.value.map(topic => {
        return id + '-' + topic;
      });
      const q = <FormArray>this.brokerForm.get('queues');
      const queues = q.value.map(queue => {
        return id + '-' + queue;
      });
      this.containerService.updateUserChannels(id, topics, queues).subscribe(result => {
        console.log(result + ' Updated channels for' + id);
      });
    });
    this.activeIndex = 1;
  }

  public onAddTopic(): void {
    const topics = <FormArray>this.brokerForm.get('topics');
    topics.push(new FormControl('', Validators.required));
  }

  public onDeleteTopic(index: number): void {
    const topics = <FormArray>this.brokerForm.get('topics');
    topics.removeAt(index);
  }

  public onDeleteQueue(index: number): void {
    const queues = <FormArray>this.brokerForm.get('queues');
    queues.removeAt(index);
  }

  public onAddQueue(): void {
    const queues = <FormArray>this.brokerForm.get('queues');
    queues.push(new FormControl('', Validators.required));
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
    this.containerService.createRepository(this.repositoryName)
    .subscribe(
      result => {
        console.log(result);
        this.activeIndex = 2;
      },
      error => console.log(error)
    );
  }

  public onCheckImage(): void {
    const imageUri = '526110916966.dkr.ecr.eu-central-1.amazonaws.com/' + this.repositoryName + ':latest';

    this.containerService.checkImage(this.repositoryName)
    .subscribe(
      result => {
        if (result === true) {
          this.pushImageContinueButtonDisable = false;
          this.configurationForm.patchValue({
            imageUri: imageUri
          });
        }
      },
      error => console.log(error)
    );
  }

  public onContinuePushImage(): void {
    this.activeIndex = 3;
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

    this.activeIndex = 4;
  }

  public onSubmitRunImageForm(): void {

    this.confirmationService.confirm({
      message: 'Are you sure you want to run the image?',
      accept: () => {
        this.auth.getCurrentUserId(this.auth.currentUser).subscribe(userId => {
          if (this.isScheduledRunType) {
            if (this.isCronExpression) {
              this.containerService.scheduleImageCronExp(
                userId,
                this.configurationForm.value.configName,
                userId + '.' + this.runImageForm.value.ruleName,
                this.runImageForm.value.cronExpression
              )
              .subscribe(
                result => console.log(result),
                error => console.log(error)
              );
            } else {
              this.containerService.scheduleImageFixedRate(
                userId,
                this.configurationForm.value.configName,
                userId + '.' + this.runImageForm.value.ruleName,
                this.runImageForm.value.fixedRateValue,
                this.runImageForm.value.fixedRateMeasurement
              )
              .subscribe(
                result => console.log(result),
                error => console.log(error)
              );
            }
          } else {
            this.containerService.runImage(
              userId,
              this.configurationForm.value.configName
            )
            .subscribe(
              result => console.log(result),
              error => console.log(error)
            );
          }

          // start flushing the queues
          if (this.brokerForm.controls['queues'].value.length > 0) {
            const queues = [];
            for (let i = 0; i < this.brokerForm.controls['queues'].value.length; i++) {
              queues[i] = this.currentUserId + '-' + this.brokerForm.controls['queues'].value[i];
            }

            this.containerService.startFlushingQueues(this.currentUserId, queues).subscribe(
              result => console.log(result),
              error => console.log(error)
            );
          }

          this.router.navigate(['/container/home']);
        });
      }
    });
  }

  public copyBuildToClipboard(): void {
    this.clipboardService.copyFromContent('docker build -t ' + this.repositoryName + ' .');
  }

  public copyTagToClipboard(): void {
    this.clipboardService.copyFromContent('docker tag ' + this.repositoryName +
      ':latest 526110916966.dkr.ecr.eu-central-1.amazonaws.com/' + this.repositoryName + ':latest');
  }

  public copyPushToClipboard(): void {
    this.clipboardService.copyFromContent('docker push 526110916966.dkr.ecr.eu-central-1.amazonaws.com/' +
      this.repositoryName + ':latest');
  }

  private getCurrentUserId(): void {
    this.auth.getCurrentUserId(this.auth.currentUser).subscribe(result => {
      this.currentUserId = result;
    });
  }

  private getCurrentUserQueues(): void {
    this.auth.getCurrentUserId(this.auth.currentUser).subscribe(id => {
      this.containerService.getUserQueues(id).subscribe(result => {
        if (result === null) {
          return;
        }
        const queues = <FormArray>this.brokerForm.get('queues');
        result.forEach(q => {
          const queue = q.split('-')[1];
          queues.push(new FormControl(queue, Validators.required));
        });
      });
    });
  }

  private getCurrentUserTopics(): void {
    this.auth.getCurrentUserId(this.auth.currentUser).subscribe(id => {
      this.containerService.getUserTopics(id).subscribe(result => {
        if (result === null) {
          return;
        }
        const topics = <FormArray>this.brokerForm.get('topics');
        result.forEach(t => {
          const topic = t.split('-')[1];
          topics.push(new FormControl(topic, Validators.required));
        });
      });
    });
  }

}
