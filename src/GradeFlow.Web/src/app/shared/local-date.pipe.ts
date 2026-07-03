import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'localDate' })
export class LocalDatePipe implements PipeTransform {
  transform(value: string | null | undefined) {
    if (!value) return value;
    return value.endsWith('Z') ? value : `${value}Z`;
  }
}
