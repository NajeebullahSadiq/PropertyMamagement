import { Component, EventEmitter, Injectable, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CancellationInfo, CancellationInfoData } from 'src/app/models/CancellationInfo';

@Component({
    selector: 'app-cancellationinfo',
    templateUrl: './cancellationinfo.component.html',
    styleUrls: ['./cancellationinfo.component.scss'],
})
export class CancellationinfoComponent {

    cancellationForm!: FormGroup;
    selectedId: number = 0;
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
                        
                        // Parse licenseCancellationLetterDate properly for the multi-calendar datepicker
                        let licenseCancellationLetterDateValue: Date | null = null;
                        if (info.licenseCancellationLetterDate) {
                            const dateString: any = info.licenseCancellationLetterDate;
                            if (typeof dateString === 'string') {
                                licenseCancellationLetterDateValue = new Date(dateString);
                                if (isNaN(licenseCancellationLetterDateValue.getTime())) {
                                    licenseCancellationLetterDateValue = null;
                                }
                            } else if (dateString instanceof Date) {
                                licenseCancellationLetterDateValue = dateString;
                            }
                        }
                        
                        this.cancellationForm.setValue({
                            id: info.id || 0,
                            companyId: info.companyId,
                            licenseCancellationLetterNumber: info.licenseCancellationLetterNumber || '',
                            revenueCancellationLetterNumber: info.revenueCancellationLetterNumber || '',
                            licenseCancellationLetterDate: licenseCancellationLetterDateValue,
                            remarks: info.remarks || '',
                        });
                        this.selectedId = info.id || 0;
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
