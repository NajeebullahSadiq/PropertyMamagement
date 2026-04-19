import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CalendarType, CALENDAR_OPTIONS } from '../models/calendar-type';

/**
 * Calendar Service - SYSTEM-WIDE HIJRI SHAMSI ONLY
 * The entire system uses only Hijri Shamsi calendar.
 * Calendar selection has been removed.
 */
@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  // SYSTEM ALWAYS USES HIJRI SHAMSI - No other calendars supported
  private readonly FIXED_CALENDAR = CalendarType.HIJRI_SHAMSI;
  
  private selectedCalendarSubject: BehaviorSubject<CalendarType>;
  public selectedCalendar$: Observable<CalendarType>;

  constructor() {
    // Always initialize with Hijri Shamsi
    this.selectedCalendarSubject = new BehaviorSubject<CalendarType>(this.FIXED_CALENDAR);
    this.selectedCalendar$ = this.selectedCalendarSubject.asObservable();
  }

  /**
   * Always returns Hijri Shamsi
   */
  getSelectedCalendar(): CalendarType {
    return this.FIXED_CALENDAR;
  }

  /**
   * Ignored - system always uses Hijri Shamsi
   */
  setSelectedCalendar(calendarType: CalendarType): void {
    // Do nothing - calendar is fixed to Hijri Shamsi
    console.warn('Calendar selection is disabled. System uses Hijri Shamsi only.');
  }

  getCalendarOptions() {
    // Return only Hijri Shamsi option
    return CALENDAR_OPTIONS.filter(opt => opt.value === CalendarType.HIJRI_SHAMSI);
  }

  getCalendarLabel(calendarType: CalendarType): string {
    return 'تقویم هجری شمسی';
  }
}
