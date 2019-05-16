import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'scheduleRuleId'
})
export class ScheduleRuleIdPipe implements PipeTransform {

  transform(value: string, args?: any): any {
    const index = value.indexOf('.');
    return value.substr(index + 1, value.length);
  }

}
