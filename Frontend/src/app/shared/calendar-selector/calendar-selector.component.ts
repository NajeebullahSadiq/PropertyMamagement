import { Component, OnInit } from '@angular/core';
import { CalendarService } from '../calendar.service';
import { CalendarType, CalendarOption, CALENDAR_OPTIONS } from '../../models/calendar-type';

/**
 * Calendar Selector Component - HIDDEN
 * System uses only Hijri Shamsi calendar.
 * This component is hidden but kept for compatibility.
 */
@Component({
  selector: 'app-calendar-selector',
  template: `<!-- Calendar selector hidden - System uses Hijri Shamsi only -->`,
  styleUrls: ['./calendar-selector.component.scss']
})
export class CalendarSelectorComponent implements OnInit {
  calendarOptions: CalendarOption[] = [];
  selectedCalendar: CalendarType = CalendarType.HIJRI_SHAMSI;

  constructor(private calendarService: CalendarService) {}

  ngOnInit(): void {
    // System always uses Hijri Shamsi
    this.selectedCalendar = CalendarType.HIJRI_SHAMSI;
  }

  onCalendarChange(event: any): void {
    // Disabled - calendar is fixed
  }
}
