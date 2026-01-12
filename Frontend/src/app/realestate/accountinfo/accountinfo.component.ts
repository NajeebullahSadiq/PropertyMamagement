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
import { AccountInfo, AccountInfoData } from 'src/app/models/AccountInfo';

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
    selector: 'app-accountinfo',
    templateUrl: './accountinfo.component.html',
    styleUrls: ['./accountinfo.component.scss'],
    providers: [
        { provide: NgbCalendar, useClass: NgbCalendarPersian },
        { provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
    ],
})
export class AccountinfoComponent {
    maxDate = { year: 1410, month: 12, day: 31 };
    minDate = { year: 1320, month: 12, day: 31 };

    accountForm!: FormGroup;
    selectedId: number = 0;
    selectedTaxPaymentDate!: NgbDate;
    accountInfo: AccountInfo | null = null;

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
        this.accountForm = this.fb.group({
            id: [0],
            companyId: [''],
            settlementInfo: ['', [Validators.maxLength(500)]],
            taxPaymentAmount: ['', [Validators.required, Validators.min(0)]],
            settlementYear: ['', [Validators.min(1300), Validators.max(2100)]],
            taxPaymentDate: [''],
            transactionCount: ['', [Validators.min(0)]],
            companyCommission: ['', [Validators.min(0)]],
        });
    }


    ngOnInit() {
        this.loadAccountInfo();
    }

    loadAccountInfo() {
        const companyId = this.comservice.mainTableId || this.id;
        if (companyId === 0) return;

        const currentCalendar = this.calendarService.getSelectedCalendar();
        this.comservice.getAccountInfoByCompanyId(companyId, currentCalendar)
            .subscribe({
                next: (info) => {
                    if (info) {
                        this.accountInfo = info;
                        this.accountForm.setValue({
                            id: info.id || 0,
                            companyId: info.companyId,
                            settlementInfo: info.settlementInfo || '',
                            taxPaymentAmount: info.taxPaymentAmount || '',
                            settlementYear: info.settlementYear || '',
                            taxPaymentDate: info.taxPaymentDate || '',
                            transactionCount: info.transactionCount || '',
                            companyCommission: info.companyCommission || '',
                        });
                        this.selectedId = info.id || 0;

                        // Parse tax payment date
                        if (info.taxPaymentDate) {
                            const dateString = info.taxPaymentDate.toString();
                            const parsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(dateString);
                            if (parsedDateStruct) {
                                this.selectedTaxPaymentDate = new NgbDate(
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
                        console.error('Error loading account info:', error);
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
        if (this.accountForm.invalid) {
            this.toastr.error('لطفاً فورم را درست پر کنید');
            return;
        }

        const formValue = this.accountForm.value;
        const taxPaymentDateValue = this.accountForm.get('taxPaymentDate')?.value;

        const data: AccountInfoData = {
            id: this.selectedId,
            companyId: this.comservice.mainTableId,
            settlementInfo: formValue.settlementInfo || undefined,
            taxPaymentAmount: parseFloat(formValue.taxPaymentAmount) || 0,
            settlementYear: formValue.settlementYear ? parseInt(formValue.settlementYear) : undefined,
            taxPaymentDate: taxPaymentDateValue ? this.formatDateForBackend(taxPaymentDateValue) : undefined,
            transactionCount: formValue.transactionCount ? parseInt(formValue.transactionCount) : undefined,
            companyCommission: formValue.companyCommission ? parseFloat(formValue.companyCommission) : undefined,
        };

        this.comservice.updateAccountInfo(this.selectedId, data).subscribe({
            next: (result) => {
                if (result.id !== 0) {
                    this.selectedId = result.id;
                }
                this.toastr.info('معلومات موفقانه تغیر یافت');
            },
            error: (error) => {
                console.error('Error updating account info:', error);
                this.toastr.error('خطا در ذخیره معلومات');
            }
        });
    }

    addData(): void {
        if (this.accountForm.invalid) {
            this.toastr.error('لطفاً فورم را درست پر کنید');
            return;
        }

        const formValue = this.accountForm.value;
        const taxPaymentDateValue = this.accountForm.get('taxPaymentDate')?.value;

        const data: AccountInfoData = {
            companyId: this.comservice.mainTableId,
            settlementInfo: formValue.settlementInfo || undefined,
            taxPaymentAmount: parseFloat(formValue.taxPaymentAmount) || 0,
            settlementYear: formValue.settlementYear ? parseInt(formValue.settlementYear) : undefined,
            taxPaymentDate: taxPaymentDateValue ? this.formatDateForBackend(taxPaymentDateValue) : undefined,
            transactionCount: formValue.transactionCount ? parseInt(formValue.transactionCount) : undefined,
            companyCommission: formValue.companyCommission ? parseFloat(formValue.companyCommission) : undefined,
        };

        this.comservice.createAccountInfo(data).subscribe({
            next: (result) => {
                if (result.id !== 0) {
                    this.toastr.success('معلومات موفقانه ثبت شد');
                    this.selectedId = result.id;
                    this.onNextClick();
                }
            },
            error: (error) => {
                console.error('Error creating account info:', error);
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
        this.accountForm.reset();
    }

    // Form control getters
    get settlementInfo() { return this.accountForm.get('settlementInfo'); }
    get taxPaymentAmount() { return this.accountForm.get('taxPaymentAmount'); }
    get settlementYear() { return this.accountForm.get('settlementYear'); }
    get taxPaymentDate() { return this.accountForm.get('taxPaymentDate'); }
    get transactionCount() { return this.accountForm.get('transactionCount'); }
    get companyCommission() { return this.accountForm.get('companyCommission'); }
}
