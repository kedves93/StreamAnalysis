import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'queueName'
})
export class QueueNamePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return null;
  }

}
