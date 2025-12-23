import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CalendarService } from '../calendar.service';
import { CalendarConversionService } from '../calendar-conversion.service';
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
  
  private destroy$ = new Subject<void>();
  private onChange: (value: Date | null) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(
    private calendarService: CalendarService,
    private conversionService: CalendarConversionService
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
      });
    
    this.updatePlaceholder();
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

  getInputType(): string {
    return this.currentCalendar === CalendarType.GREGORIAN ? 'date' : 'text';
  }

  getPattern(): string {
    if (this.currentCalendar === CalendarType.GREGORIAN) {
      return '';
    }
    return '\\d{4}/\\d{2}/\\d{2}';
  }
}
