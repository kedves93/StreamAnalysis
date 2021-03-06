import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'topicName'
})
export class TopicNamePipe implements PipeTransform {

  transform(value: string, args?: any): any {
    const index = value.indexOf('-');
    return value.substr(index + 1, value.length);
  }

}
