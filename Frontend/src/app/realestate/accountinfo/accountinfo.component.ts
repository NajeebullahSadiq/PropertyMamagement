import { Component, EventEmitter, Injectable, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { NumeralService } from 'src/app/shared/numeral.service';
import { AccountInfo, AccountInfoData } from 'src/app/models/AccountInfo';

@Component({
    selector: 'app-accountinfo',
    templateUrl: './accountinfo.component.html',
    styleUrls: ['./accountinfo.component.scss'],
})
export class AccountinfoComponent {

    accountForm!: FormGroup;
    selectedId: number = 0;
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
        private calendarConversionService: CalendarConversionService,
        private calendarService: CalendarService,
        private numeralService: NumeralService
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
                        
                        // Parse taxPaymentDate properly for the multi-calendar datepicker
                        let taxPaymentDateValue: Date | null = null;
                        if (info.taxPaymentDate) {
                            const dateString: any = info.taxPaymentDate;
                            if (typeof dateString === 'string') {
                                taxPaymentDateValue = new Date(dateString);
                                if (isNaN(taxPaymentDateValue.getTime())) {
                                    taxPaymentDateValue = null;
                                }
                            } else if (dateString instanceof Date) {
                                taxPaymentDateValue = dateString;
                            }
                        }
                        
                        this.accountForm.setValue({
                            id: info.id || 0,
                            companyId: info.companyId,
                            settlementInfo: info.settlementInfo || '',
                            taxPaymentAmount: info.taxPaymentAmount || '',
                            settlementYear: info.settlementYear || '',
                            taxPaymentDate: taxPaymentDateValue,
                            transactionCount: info.transactionCount || '',
                            companyCommission: info.companyCommission || '',
                        });
                        this.selectedId = info.id || 0;
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

        // Helper function to parse numbers safely with Dari/Pashto numeral support
        const parseNumber = (value: any): number | undefined => {
            if (value === null || value === undefined || value === '') return undefined;
            // Convert Eastern Arabic numerals to Western before parsing
            const westernValue = this.numeralService.toWesternArabic(value);
            const parsed = parseFloat(westernValue);
            return isNaN(parsed) ? undefined : parsed;
        };

        const parseInteger = (value: any): number | undefined => {
            if (value === null || value === undefined || value === '') return undefined;
            // Convert Eastern Arabic numerals to Western before parsing
            const westernValue = this.numeralService.toWesternArabic(value);
            const parsed = parseInt(westernValue, 10);
            return isNaN(parsed) ? undefined : parsed;
        };

        const data: AccountInfoData = {
            id: this.selectedId,
            companyId: this.comservice.mainTableId,
            settlementInfo: formValue.settlementInfo || undefined,
            taxPaymentAmount: parseNumber(formValue.taxPaymentAmount) || 0,
            settlementYear: parseInteger(formValue.settlementYear),
            taxPaymentDate: taxPaymentDateValue ? this.formatDateForBackend(taxPaymentDateValue) : undefined,
            transactionCount: parseInteger(formValue.transactionCount),
            companyCommission: parseNumber(formValue.companyCommission),
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

        // Helper function to parse numbers safely with Dari/Pashto numeral support
        const parseNumber = (value: any): number | undefined => {
            if (value === null || value === undefined || value === '') return undefined;
            // Convert Eastern Arabic numerals to Western before parsing
            const westernValue = this.numeralService.toWesternArabic(value);
            const parsed = parseFloat(westernValue);
            return isNaN(parsed) ? undefined : parsed;
        };

        const parseInteger = (value: any): number | undefined => {
            if (value === null || value === undefined || value === '') return undefined;
            // Convert Eastern Arabic numerals to Western before parsing
            const westernValue = this.numeralService.toWesternArabic(value);
            const parsed = parseInt(westernValue, 10);
            return isNaN(parsed) ? undefined : parsed;
        };

        const data: AccountInfoData = {
            companyId: this.comservice.mainTableId,
            settlementInfo: formValue.settlementInfo || undefined,
            taxPaymentAmount: parseNumber(formValue.taxPaymentAmount) || 0,
            settlementYear: parseInteger(formValue.settlementYear),
            taxPaymentDate: taxPaymentDateValue ? this.formatDateForBackend(taxPaymentDateValue) : undefined,
            transactionCount: parseInteger(formValue.transactionCount),
            companyCommission: parseNumber(formValue.companyCommission),
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
