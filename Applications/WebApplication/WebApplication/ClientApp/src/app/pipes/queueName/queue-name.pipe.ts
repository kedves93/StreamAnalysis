import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'queueName'
})
export class QueueNamePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    const index = value.indexOf('-');
    return value.substr(index + 1, value.length);
  }

}
