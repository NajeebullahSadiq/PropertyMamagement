import { Injectable } from '@angular/core';
import { CalendarType } from '../models/calendar-type';

declare const require: any;
const momentJalaali = require('moment-jalaali');
const momentHijri = require('moment-hijri');

export interface CalendarDate {
  year: number;
  month: number;
  day: number;
  calendarType: CalendarType;
}

@Injectable({
  providedIn: 'root'
})
export class CalendarConversionService {

  constructor() {
    if (momentJalaali.loadPersian) {
      momentJalaali.loadPersian({ usePersianDigits: false });
    }
  }

  toGregorian(date: CalendarDate): Date {
    if (!date || !date.year || !date.month || !date.day) {
      throw new Error('Invalid date provided');
    }

    switch (date.calendarType) {
      case CalendarType.GREGORIAN:
        return new Date(date.year, date.month - 1, date.day);

      case CalendarType.HIJRI_SHAMSI:
        const jalaali = momentJalaali(`${date.year}/${this.padZero(date.month)}/${this.padZero(date.day)}`, 'jYYYY/jMM/jDD');
        return jalaali.toDate();

      case CalendarType.HIJRI_QAMARI:
        const hijri = momentHijri(`${date.year}/${this.padZero(date.month)}/${this.padZero(date.day)}`, 'iYYYY/iMM/iDD');
        return hijri.toDate();

      default:
        throw new Error('Unknown calendar type');
    }
  }

  fromGregorian(gregorianDate: Date | string, targetCalendar: CalendarType): CalendarDate {
    if (!gregorianDate) {
      throw new Error('Invalid date provided');
    }

    const date = typeof gregorianDate === 'string' ? new Date(gregorianDate) : gregorianDate;

    switch (targetCalendar) {
      case CalendarType.GREGORIAN:
        return {
          year: date.getFullYear(),
          month: date.getMonth() + 1,
          day: date.getDate(),
          calendarType: CalendarType.GREGORIAN
        };

      case CalendarType.HIJRI_SHAMSI:
        const jalaali = momentJalaali(date);
        return {
          year: jalaali.jYear(),
          month: jalaali.jMonth() + 1,
          day: jalaali.jDate(),
          calendarType: CalendarType.HIJRI_SHAMSI
        };

      case CalendarType.HIJRI_QAMARI:
        const hijri = momentHijri(date);
        return {
          year: hijri.iYear(),
          month: hijri.iMonth() + 1,
          day: hijri.iDate(),
          calendarType: CalendarType.HIJRI_QAMARI
        };

      default:
        throw new Error('Unknown calendar type');
    }
  }

  formatDate(date: Date | string, calendarType: CalendarType): string {
    if (!date) {
      return '';
    }

    const d = typeof date === 'string' ? new Date(date) : date;
    const calendarDate = this.fromGregorian(d, calendarType);

    return `${calendarDate.year}/${this.padZero(calendarDate.month)}/${this.padZero(calendarDate.day)}`;
  }

  formatDateForInput(date: Date | string, calendarType: CalendarType): string {
    if (!date) {
      return '';
    }

    const d = typeof date === 'string' ? new Date(date) : date;

    if (calendarType === CalendarType.GREGORIAN) {
      const year = d.getFullYear();
      const month = this.padZero(d.getMonth() + 1);
      const day = this.padZero(d.getDate());
      return `${year}-${month}-${day}`;
    } else {
      const calendarDate = this.fromGregorian(d, calendarType);
      return `${calendarDate.year}/${this.padZero(calendarDate.month)}/${this.padZero(calendarDate.day)}`;
    }
  }

  parseInputDate(inputValue: string, calendarType: CalendarType): Date | null {
    if (!inputValue) {
      return null;
    }

    try {
      // Normalize Persian/Arabic digits to Western to allow Dari inputs
      const normalized = this.normalizeDigits(inputValue.trim());

      let year: number, month: number, day: number;

      if (calendarType === CalendarType.GREGORIAN) {
        const parts = normalized.split('-');
        if (parts.length !== 3) {
          return null;
        }
        year = parseInt(parts[0], 10);
        month = parseInt(parts[1], 10);
        day = parseInt(parts[2], 10);
      } else {
        const parts = normalized.split('/');
        if (parts.length !== 3) {
          return null;
        }
        year = parseInt(parts[0], 10);
        month = parseInt(parts[1], 10);
        day = parseInt(parts[2], 10);
      }

      if (isNaN(year) || isNaN(month) || isNaN(day)) {
        return null;
      }

      return this.toGregorian({ year, month, day, calendarType });
    } catch (error) {
      console.error('Error parsing date:', error);
      return null;
    }
  }

  isValidDate(year: number, month: number, day: number, calendarType: CalendarType): boolean {
    try {
      const date = this.toGregorian({ year, month, day, calendarType });
      return date instanceof Date && !isNaN(date.getTime());
    } catch {
      return false;
    }
  }

  private padZero(num: number): string {
    return num < 10 ? `0${num}` : `${num}`;
  }

  getMonthName(month: number, calendarType: CalendarType): string {
    const gregorianMonths = ['January', 'February', 'March', 'April', 'May', 'June', 
                             'July', 'August', 'September', 'October', 'November', 'December'];
    
    const shamsiMonths = ['حمل', 'ثور', 'جوزا', 'سرطان', 'اسد', 'سنبله', 
                          'میزان', 'عقرب', 'قوس', 'جدی', 'دلو', 'حوت'];
    
    const qamariMonths = ['محرم', 'صفر', 'ربیع الاول', 'ربیع الثانی', 'جمادی الاول', 'جمادی الثانی',
                          'رجب', 'شعبان', 'رمضان', 'شوال', 'ذی القعده', 'ذی الحجه'];

    switch (calendarType) {
      case CalendarType.GREGORIAN:
        return gregorianMonths[month - 1] || '';
      case CalendarType.HIJRI_SHAMSI:
        return shamsiMonths[month - 1] || '';
      case CalendarType.HIJRI_QAMARI:
        return qamariMonths[month - 1] || '';
      default:
        return '';
    }
  }

  compareDates(date1: CalendarDate, date2: CalendarDate): number {
    const greg1 = this.toGregorian(date1);
    const greg2 = this.toGregorian(date2);
    
    if (greg1 < greg2) return -1;
    if (greg1 > greg2) return 1;
    return 0;
  }

  /**
   * Convert Persian/Arabic-Indic digits to Western digits
   */
  private normalizeDigits(value: string): string {
    const persianDigits = ['۰','۱','۲','۳','۴','۵','۶','۷','۸','۹'];
    const arabicDigits = ['٠','١','٢','٣','٤','٥','٦','٧','٨','٩'];

    return value
      .split('')
      .map(char => {
        const persianIndex = persianDigits.indexOf(char);
        if (persianIndex > -1) return String(persianIndex);

        const arabicIndex = arabicDigits.indexOf(char);
        if (arabicIndex > -1) return String(arabicIndex);

        return char;
      })
      .join('');
  }
}
