import { Component, OnInit } from '@angular/core';
import { CalendarService } from '../calendar.service';
import { CalendarType, CalendarOption, CALENDAR_OPTIONS } from '../../models/calendar-type';

@Component({
  selector: 'app-calendar-selector',
  templateUrl: './calendar-selector.component.html',
  styleUrls: ['./calendar-selector.component.scss']
})
export class CalendarSelectorComponent implements OnInit {
  calendarOptions: CalendarOption[] = CALENDAR_OPTIONS;
  selectedCalendar: CalendarType = CalendarType.HIJRI_SHAMSI;

  constructor(private calendarService: CalendarService) {}

  ngOnInit(): void {
    this.selectedCalendar = this.calendarService.getSelectedCalendar();
    
    this.calendarService.selectedCalendar$.subscribe(calendar => {
      this.selectedCalendar = calendar;
    });
  }

  onCalendarChange(event: any): void {
    const selectedValue = event.target.value as CalendarType;
    this.calendarService.setSelectedCalendar(selectedValue);
  }
}
