import { Pipe, PipeTransform } from '@angular/core';
import { CalendarConversionService } from './calendar-conversion.service';
import { CalendarType } from '../models/calendar-type';

@Pipe({
  name: 'calendarDate',
  pure: false
})
export class CalendarDatePipe implements PipeTransform {
  constructor(
    private conversionService: CalendarConversionService
  ) {}

  transform(value: Date | string | null | undefined): string {
    if (!value) {
      return '';
    }

    if (typeof value === 'string') {
      const normalized = value.trim();
      const dateParts = normalized.match(/^(\d{4})[-\/](\d{2})[-\/](\d{2})/);

      if (dateParts) {
        const year = Number(dateParts[1]);
        if (year >= 1300 && year <= 1599) {
          return `${dateParts[1]}/${dateParts[2]}/${dateParts[3]}`;
        }
      }

      if (/^\d{4}\/\d{2}\/\d{2}$/.test(normalized)) {
        return normalized;
      }
    }

    const d = typeof value === 'string' ? new Date(value) : value;
    if (!(d instanceof Date) || Number.isNaN(d.getTime())) {
      return '';
    }

    // The application displays dates in Hijri Shamsi regardless of browser locale/calendar settings.
    return this.conversionService.formatDate(d, CalendarType.HIJRI_SHAMSI);
  }
}
