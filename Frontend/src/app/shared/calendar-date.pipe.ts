import { Pipe, PipeTransform } from '@angular/core';
import { CalendarService } from './calendar.service';
import { CalendarConversionService } from './calendar-conversion.service';

@Pipe({
  name: 'calendarDate',
  pure: false
})
export class CalendarDatePipe implements PipeTransform {
  constructor(
    private calendarService: CalendarService,
    private conversionService: CalendarConversionService
  ) {}

  transform(value: Date | string | null | undefined): string {
    if (!value) {
      return '';
    }

    const d = typeof value === 'string' ? new Date(value) : value;
    if (!(d instanceof Date) || Number.isNaN(d.getTime())) {
      return '';
    }

    const calendarType = this.calendarService.getSelectedCalendar();
    return this.conversionService.formatDate(d, calendarType);
  }
}
