import { Component, EventEmitter, Injectable, Input, Output } from '@angular/core';
import {
    NgbDateStruct,
    NgbCalendar,
    NgbDatepickerI18n,
    NgbCalendarPersian,
    NgbDate,
    NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CancellationInfo, CancellationInfoData } from 'src/app/models/CancellationInfo';

const WEEKDAYS_SHORT = ['د', 'س', 'چ', 'پ', 'ج', 'ش', 'ی'];
const MONTHS = ['حمل', 'ثور', 'جوزا', 'سرطان', 'اسد', 'سنبله', 'میزان', 'عقرب', 'قوس', 'جدی', 'دلو', 'حوت'];

@Injectable()
export class NgbDatepickerI18nPersian extends NgbDatepickerI18n {
    getWeekdayLabel(weekday: number) {
        return WEEKDAYS_SHORT[weekday - 1];
    }
    getMonthShortName(month: number) {
        return MONTHS[month - 1];
    }
    getMonthFullName(month: number) {
        return MONTHS[month - 1];
    }
    getDayAriaLabel(date: NgbDateStruct): string {
        return `${date.year}-${this.getMonthFullName(date.month)}-${date.day}`;
    }
}

@Component({
    selector: 'app-cancellationinfo',
    templateUrl: './cancellationinfo.component.html',
    styleUrls: ['./cancellationinfo.component.scss'],
    providers: [
        { provide: NgbCalendar, useClass: NgbCalendarPersian },
        { provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
    ],
})
export class CancellationinfoComponent {
    maxDate = { year: 1410, month: 12, day: 31 };
    minDate = { year: 1320, month: 12, day: 31 };

    cancellationForm!: FormGroup;
    selectedId: number = 0;
    selectedLicenseCancellationLetterDate!: NgbDate;
    cancellationInfo: CancellationInfo | null = null;

    @Input() id: number = 0;
    @Output() next = new EventEmitter<void>();

    onNextClick() {
        this.next.emit();
    }

    constructor(
        private fb: FormBuilder,
        private toastr: ToastrService,
        private comservice: CompnaydetailService,
        private ngbDateParserFormatter: NgbDateParserFormatter,
        private calendarConversionService: CalendarConversionService,
        private calendarService: CalendarService
    ) {
        this.cancellationForm = this.fb.group({
            id: [0],
            companyId: [''],
            licenseCancellationLetterNumber: ['', [Validators.maxLength(100)]],
            revenueCancellationLetterNumber: ['', [Validators.maxLength(100)]],
            licenseCancellationLetterDate: [''],
            remarks: ['', [Validators.maxLength(1000)]],
        });
    }

    ngOnInit() {
        this.loadCancellationInfo();
    }

    loadCancellationInfo() {
        const companyId = this.comservice.mainTableId || this.id;
        if (companyId === 0) return;

        const currentCalendar = this.calendarService.getSelectedCalendar();
        this.comservice.getCancellationInfoByCompanyId(companyId, currentCalendar)
            .subscribe({
                next: (info) => {
                    if (info) {
                        this.cancellationInfo = info;
                        this.cancellationForm.setValue({
                            id: info.id || 0,
                            companyId: info.companyId,
                            licenseCancellationLetterNumber: info.licenseCancellationLetterNumber || '',
                            revenueCancellationLetterNumber: info.revenueCancellationLetterNumber || '',
                            licenseCancellationLetterDate: info.licenseCancellationLetterDate || '',
                            remarks: info.remarks || '',
                        });
                        this.selectedId = info.id || 0;

                        if (info.licenseCancellationLetterDate) {
                            const dateString = info.licenseCancellationLetterDate.toString();
                            const parsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(dateString);
                            if (parsedDateStruct) {
                                this.selectedLicenseCancellationLetterDate = new NgbDate(
                                    parsedDateStruct.year,
                                    parsedDateStruct.month,
                                    parsedDateStruct.day
                                );
                            }
                        }
                    }
                },
                error: (error) => {
                    if (error.status !== 404) {
                        console.error('Error loading cancellation info:', error);
                    }
                }
            });
    }

    private formatDateForBackend(dateValue: any): string {
        const currentCalendar = this.calendarService.getSelectedCalendar();

        if (dateValue instanceof Date) {
            const calendarDate = this.calendarConversionService.fromGregorian(dateValue, currentCalendar);
            const year = calendarDate.year;
            const month = String(calendarDate.month).padStart(2, '0');
            const day = String(calendarDate.day).padStart(2, '0');
            return `${year}-${month}-${day}`;
        } else if (typeof dateValue === 'object' && dateValue.year) {
            const year = dateValue.year;
            const month = String(dateValue.month).padStart(2, '0');
            const day = String(dateValue.day).padStart(2, '0');
            return `${year}-${month}-${day}`;
        } else if (typeof dateValue === 'string') {
            return dateValue.replace(/\//g, '-');
        }
        return '';
    }

    updateData(): void {
        if (this.cancellationForm.invalid) {
            this.toastr.error('لطفاً فورم را درست پر کنید');
            return;
        }

        const formValue = this.cancellationForm.value;
        const licenseCancellationLetterDateValue = this.cancellationForm.get('licenseCancellationLetterDate')?.value;

        const data: CancellationInfoData = {
            id: this.selectedId,
            companyId: this.comservice.mainTableId,
            licenseCancellationLetterNumber: formValue.licenseCancellationLetterNumber || undefined,
            revenueCancellationLetterNumber: formValue.revenueCancellationLetterNumber || undefined,
            licenseCancellationLetterDate: licenseCancellationLetterDateValue ? this.formatDateForBackend(licenseCancellationLetterDateValue) : undefined,
            remarks: formValue.remarks || undefined,
        };

        this.comservice.updateCancellationInfo(this.selectedId, data).subscribe({
            next: (result) => {
                if (result.id !== 0) {
                    this.selectedId = result.id;
                }
                this.toastr.info('معلومات موفقانه تغیر یافت');
            },
            error: (error) => {
                console.error('Error updating cancellation info:', error);
                this.toastr.error('خطا در ذخیره معلومات');
            }
        });
    }

    addData(): void {
        if (this.cancellationForm.invalid) {
            this.toastr.error('لطفاً فورم را درست پر کنید');
            return;
        }

        const formValue = this.cancellationForm.value;
        const licenseCancellationLetterDateValue = this.cancellationForm.get('licenseCancellationLetterDate')?.value;

        const data: CancellationInfoData = {
            companyId: this.comservice.mainTableId,
            licenseCancellationLetterNumber: formValue.licenseCancellationLetterNumber || undefined,
            revenueCancellationLetterNumber: formValue.revenueCancellationLetterNumber || undefined,
            licenseCancellationLetterDate: licenseCancellationLetterDateValue ? this.formatDateForBackend(licenseCancellationLetterDateValue) : undefined,
            remarks: formValue.remarks || undefined,
        };

        this.comservice.createCancellationInfo(data).subscribe({
            next: (result) => {
                if (result.id !== 0) {
                    this.toastr.success('معلومات موفقانه ثبت شد');
                    this.selectedId = result.id;
                    this.onNextClick();
                }
            },
            error: (error) => {
                console.error('Error creating cancellation info:', error);
                if (error.error?.message) {
                    this.toastr.error(error.error.message);
                } else {
                    this.toastr.error('خطا در ذخیره معلومات');
                }
            }
        });
    }

    resetForms(): void {
        this.selectedId = 0;
        this.cancellationForm.reset();
    }

    // Form control getters
    get licenseCancellationLetterNumber() { return this.cancellationForm.get('licenseCancellationLetterNumber'); }
    get revenueCancellationLetterNumber() { return this.cancellationForm.get('revenueCancellationLetterNumber'); }
    get licenseCancellationLetterDate() { return this.cancellationForm.get('licenseCancellationLetterDate'); }
    get remarks() { return this.cancellationForm.get('remarks'); }
}
