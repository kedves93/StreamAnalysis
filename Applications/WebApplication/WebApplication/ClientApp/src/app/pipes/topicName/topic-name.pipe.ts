import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'topicName'
})
export class TopicNamePipe implements PipeTransform {

  transform(value: string, args?: any): any {
    return value.split('-')[1];
  }

}
