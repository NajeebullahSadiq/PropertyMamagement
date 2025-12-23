import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CalendarType, CALENDAR_OPTIONS } from '../models/calendar-type';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private readonly STORAGE_KEY = 'selectedCalendar';
  
  private selectedCalendarSubject: BehaviorSubject<CalendarType>;
  public selectedCalendar$: Observable<CalendarType>;

  constructor() {
    const storedCalendar = this.getStoredCalendar();
    this.selectedCalendarSubject = new BehaviorSubject<CalendarType>(storedCalendar);
    this.selectedCalendar$ = this.selectedCalendarSubject.asObservable();
  }

  getSelectedCalendar(): CalendarType {
    return this.selectedCalendarSubject.value;
  }

  setSelectedCalendar(calendarType: CalendarType): void {
    this.selectedCalendarSubject.next(calendarType);
    localStorage.setItem(this.STORAGE_KEY, calendarType);
  }

  private getStoredCalendar(): CalendarType {
    const stored = localStorage.getItem(this.STORAGE_KEY);
    if (stored && Object.values(CalendarType).includes(stored as CalendarType)) {
      return stored as CalendarType;
    }
    return CalendarType.HIJRI_SHAMSI;
  }

  getCalendarOptions() {
    return CALENDAR_OPTIONS;
  }

  getCalendarLabel(calendarType: CalendarType): string {
    const option = CALENDAR_OPTIONS.find(opt => opt.value === calendarType);
    return option ? option.labelFa : '';
  }
}
