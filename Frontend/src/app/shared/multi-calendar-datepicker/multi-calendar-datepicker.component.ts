import { Component, forwardRef, Input, OnDestroy, OnInit, HostListener, ElementRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CalendarService } from '../calendar.service';
import { CalendarConversionService, CalendarDate } from '../calendar-conversion.service';
import { CalendarType } from '../../models/calendar-type';

@Component({
  selector: 'app-multi-calendar-datepicker',
  templateUrl: './multi-calendar-datepicker.component.html',
  styleUrls: ['./multi-calendar-datepicker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MultiCalendarDatepickerComponent),
      multi: true
    }
  ]
})
export class MultiCalendarDatepickerComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() label: string = 'تاریخ';
  @Input() required: boolean = false;
  @Input() disabled: boolean = false;
  @Input() placeholder: string = '';

  currentCalendar: CalendarType = CalendarType.HIJRI_SHAMSI;
  inputValue: string = '';
  CalendarType = CalendarType;
  
  showCalendarPopup: boolean = false;
  currentYear: number = 1403;
  currentMonth: number = 1;
  selectedDate: CalendarDate | null = null;
  calendarDays: (number | null)[][] = [];
  
  private destroy$ = new Subject<void>();
  private onChange: (value: Date | null) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(
    private calendarService: CalendarService,
    private conversionService: CalendarConversionService,
    private elementRef: ElementRef
  ) {}

  ngOnInit(): void {
    this.calendarService.selectedCalendar$
      .pipe(takeUntil(this.destroy$))
      .subscribe(calendar => {
        const oldCalendar = this.currentCalendar;
        this.currentCalendar = calendar;
        
        if (this.inputValue && oldCalendar !== calendar) {
          this.convertInputToNewCalendar(oldCalendar, calendar);
        }
        
        this.updatePlaceholder();
        this.initializeCalendar();
      });
    
    this.updatePlaceholder();
    this.initializeCalendar();
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.showCalendarPopup = false;
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  writeValue(value: Date | string | null): void {
    if (value) {
      const date = typeof value === 'string' ? new Date(value) : value;
      this.inputValue = this.conversionService.formatDateForInput(date, this.currentCalendar);
    } else {
      this.inputValue = '';
    }
  }

  registerOnChange(fn: (value: Date | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onInputChange(event: any): void {
    const value = event.target.value;
    this.inputValue = value;
    
    if (!value) {
      this.onChange(null);
      return;
    }

    const gregorianDate = this.conversionService.parseInputDate(value, this.currentCalendar);
    this.onChange(gregorianDate);
  }

  onBlur(): void {
    this.onTouched();
  }

  private convertInputToNewCalendar(oldCalendar: CalendarType, newCalendar: CalendarType): void {
    if (!this.inputValue) {
      return;
    }

    try {
      const gregorianDate = this.conversionService.parseInputDate(this.inputValue, oldCalendar);
      if (gregorianDate) {
        this.inputValue = this.conversionService.formatDateForInput(gregorianDate, newCalendar);
      }
    } catch (error) {
      console.error('Error converting date:', error);
    }
  }

  private updatePlaceholder(): void {
    switch (this.currentCalendar) {
      case CalendarType.GREGORIAN:
        this.placeholder = this.placeholder || 'YYYY-MM-DD';
        break;
      case CalendarType.HIJRI_SHAMSI:
        this.placeholder = this.placeholder || 'YYYY/MM/DD (شمسی)';
        break;
      case CalendarType.HIJRI_QAMARI:
        this.placeholder = this.placeholder || 'YYYY/MM/DD (قمری)';
        break;
    }
  }

  initializeCalendar(): void {
    const today = new Date();
    const calendarDate = this.conversionService.fromGregorian(today, this.currentCalendar);
    this.currentYear = calendarDate.year;
    this.currentMonth = calendarDate.month;
    this.generateCalendar();
  }

  toggleCalendarPopup(): void {
    if (this.disabled) return;
    
    if (this.currentCalendar === CalendarType.GREGORIAN) {
      return;
    }
    
    this.showCalendarPopup = !this.showCalendarPopup;
    if (this.showCalendarPopup) {
      if (this.selectedDate) {
        this.currentYear = this.selectedDate.year;
        this.currentMonth = this.selectedDate.month;
      }
      this.generateCalendar();
    }
  }

  generateCalendar(): void {
    this.calendarDays = [];
    
    const firstDay = this.conversionService.toGregorian({
      year: this.currentYear,
      month: this.currentMonth,
      day: 1,
      calendarType: this.currentCalendar
    });
    
    const lastDayOfMonth = this.getDaysInMonth(this.currentYear, this.currentMonth);
    
    const firstDayOfWeek = firstDay.getDay();
    
    let week: (number | null)[] = [];
    for (let i = 0; i < firstDayOfWeek; i++) {
      week.push(null);
    }
    
    for (let day = 1; day <= lastDayOfMonth; day++) {
      week.push(day);
      
      if (week.length === 7) {
        this.calendarDays.push(week);
        week = [];
      }
    }
    
    if (week.length > 0) {
      while (week.length < 7) {
        week.push(null);
      }
      this.calendarDays.push(week);
    }
  }

  getDaysInMonth(year: number, month: number): number {
    return this.conversionService.getDaysInMonth(year, month, this.currentCalendar);
  }

  isLeapYearShamsi(year: number): boolean {
    const breaks = [1, 5, 9, 13, 17, 22, 26, 30];
    const mod = year % 33;
    return breaks.includes(mod);
  }

  selectDate(day: number | null): void {
    if (!day || this.disabled) return;
    
    this.selectedDate = {
      year: this.currentYear,
      month: this.currentMonth,
      day: day,
      calendarType: this.currentCalendar
    };
    
    const gregorianDate = this.conversionService.toGregorian(this.selectedDate);
    this.inputValue = this.conversionService.formatDateForInput(gregorianDate, this.currentCalendar);
    this.onChange(gregorianDate);
    this.showCalendarPopup = false;
  }

  isSelectedDay(day: number | null): boolean {
    if (!day || !this.selectedDate) return false;
    return this.selectedDate.year === this.currentYear &&
           this.selectedDate.month === this.currentMonth &&
           this.selectedDate.day === day;
  }

  previousMonth(): void {
    if (this.currentMonth === 1) {
      this.currentMonth = 12;
      this.currentYear--;
    } else {
      this.currentMonth--;
    }
    this.generateCalendar();
  }

  nextMonth(): void {
    if (this.currentMonth === 12) {
      this.currentMonth = 1;
      this.currentYear++;
    } else {
      this.currentMonth++;
    }
    this.generateCalendar();
  }

  previousYear(): void {
    this.currentYear--;
    this.generateCalendar();
  }

  nextYear(): void {
    this.currentYear++;
    this.generateCalendar();
  }

  getMonthName(): string {
    return this.conversionService.getMonthName(this.currentMonth, this.currentCalendar);
  }

  getWeekDayNames(): string[] {
    if (this.currentCalendar === CalendarType.HIJRI_QAMARI) {
      return ['الأحد', 'الاثنین', 'الثلاثاء', 'الأربعاء', 'الخمیس', 'الجمعة', 'السبت'];
    }
    return ['یکشنبه', 'دوشنبه', 'سه‌شنبه', 'چهارشنبه', 'پنج‌شنبه', 'جمعه', 'شنبه'];
  }

  getInputType(): string {
    return this.currentCalendar === CalendarType.GREGORIAN ? 'date' : 'text';
  }

  getPattern(): string {
    if (this.currentCalendar === CalendarType.GREGORIAN) {
      return '';
    }
    return '\\d{4}/\\d{2}/\\d{2}';
  }

  shouldShowCalendarIcon(): boolean {
    return this.currentCalendar !== CalendarType.GREGORIAN;
  }
}
