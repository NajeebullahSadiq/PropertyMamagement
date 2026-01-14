import { Component, Injectable, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import {
    NgbDateStruct,
    NgbCalendar,
    NgbDatepickerI18n,
    NgbCalendarPersian,
    NgbDate,
    NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import { SecuritiesControlService } from 'src/app/shared/securities-control.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { 
    SecuritiesControlData,
    SecurityDocumentTypes,
    SecuritiesTypes
} from 'src/app/models/SecuritiesControl';

const WEEKDAYS_SHORT = ['د', 'س', 'چ', 'پ', 'ج', 'ش', 'ی'];
const MONTHS = ['حمل', 'ثور', 'جوزا', 'سرطان', 'اسد', 'سنبله', 'میزان', 'عقرب', 'قوس', 'جدی', 'دلو', 'حوت'];

@Injectable()
export class NgbDatepickerI18nPersian extends NgbDatepickerI18n {
    getWeekdayLabel(weekday: number) { return WEEKDAYS_SHORT[weekday - 1]; }
    getMonthShortName(month: number) { return MONTHS[month - 1]; }
    getMonthFullName(month: number) { return MONTHS[month - 1]; }
    getDayAriaLabel(date: NgbDateStruct): string {
        return `${date.year}-${this.getMonthFullName(date.month)}-${date.day}`;
    }
}

@Component({
    selector: 'app-securities-control-form',
    templateUrl: './securities-control-form.component.html',
    styleUrls: ['./securities-control-form.component.scss'],
    providers: [
        { provide: NgbCalendar, useClass: NgbCalendarPersian },
        { provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
    ],
})
export class SecuritiesControlFormComponent implements OnInit {
    maxDate = { year: 1410, month: 12, day: 31 };
    minDate = { year: 1320, month: 12, day: 31 };

    securitiesControlForm!: FormGroup;
    isEditMode = false;
    editId: number | null = null;
    selectedTabIndex = 0;

    // Dropdown options
    securityDocumentTypes = SecurityDocumentTypes;
    securitiesTypes = SecuritiesTypes;

    // Conditional visibility flags
    showSingleTypeFields = false;
    showCombinedTypeFields = false;

    // Single Type Visibility Flags
    showPropertySaleFields = false;
    showBayWafaFields = false;
    showRentFields = false;
    showVehicleSaleFields = false;
    showExchangeFields = false;
    showRegistrationBookFields = false;
    showPrintedPetitionFields = false;

    // Combined Type Visibility Flags
    showPropertySaleBayWafaFields = false;
    showPropertySaleRentFields = false;
    showBayWafaRentFields = false;
    showAllPropertyFields = false;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        private securitiesControlService: SecuritiesControlService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private ngbDateParserFormatter: NgbDateParserFormatter
    ) {
        this.initForm();
    }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isEditMode = true;
            this.editId = parseInt(id, 10);
            this.loadData(this.editId);
        } else {
            // Generate next serial number for new records
            this.generateNextSerialNumber();
        }
    }

    initForm(): void {
        this.securitiesControlForm = this.fb.group({
            id: [null],
            // Tab 1
            serialNumber: ['', [Validators.required, Validators.maxLength(50)]],
            securityDocumentType: [null, Validators.required],
            proposalNumber: ['', [Validators.maxLength(100)]],
            proposalDate: [null],
            distributionTicketNumber: ['', [Validators.maxLength(100)]],
            deliveryDate: [null],
            // Tab 2
            securitiesType: [null],
            // Single Type Fields
            propertySaleCount: [null, [Validators.min(0)]],
            propertySaleSerialStart: ['', [Validators.maxLength(100)]],
            propertySaleSerialEnd: ['', [Validators.maxLength(100)]],
            bayWafaCount: [null, [Validators.min(0)]],
            bayWafaSerialStart: ['', [Validators.maxLength(100)]],
            bayWafaSerialEnd: ['', [Validators.maxLength(100)]],
            rentCount: [null, [Validators.min(0)]],
            rentSerialStart: ['', [Validators.maxLength(100)]],
            rentSerialEnd: ['', [Validators.maxLength(100)]],
            vehicleSaleCount: [null, [Validators.min(0)]],
            vehicleSaleSerialStart: ['', [Validators.maxLength(100)]],
            vehicleSaleSerialEnd: ['', [Validators.maxLength(100)]],
            exchangeCount: [null, [Validators.min(0)]],
            exchangeSerialStart: ['', [Validators.maxLength(100)]],
            exchangeSerialEnd: ['', [Validators.maxLength(100)]],
            registrationBookCount: [null, [Validators.min(0)]],
            registrationBookSerialStart: ['', [Validators.maxLength(100)]],
            registrationBookSerialEnd: ['', [Validators.maxLength(100)]],
            printedPetitionCount: [null, [Validators.min(0)]],
            printedPetitionSerialStart: ['', [Validators.maxLength(100)]],
            printedPetitionSerialEnd: ['', [Validators.maxLength(100)]],
            // Tab 3
            distributionStartNumber: ['', [Validators.maxLength(100)]],
            distributionEndNumber: ['', [Validators.maxLength(100)]],
            distributedPersonsCount: [null, [Validators.min(0)]],
            // Tab 4
            remarks: ['', [Validators.maxLength(2000)]]
        });
    }

    generateNextSerialNumber(): void {
        this.securitiesControlService.getNextSerialNumber().subscribe({
            next: (response) => {
                this.securitiesControlForm.patchValue({ serialNumber: response.serialNumber });
            },
            error: (err) => {
                // If there's an error, generate a simple serial number
                this.securitiesControlForm.patchValue({ serialNumber: 'SEC-' + Date.now().toString().slice(-6) });
            }
        });
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.securitiesControlService.getById(id, calendar).subscribe({
            next: (data) => {
                this.securitiesControlForm.patchValue({
                    id: data.id,
                    serialNumber: data.serialNumber,
                    securityDocumentType: data.securityDocumentType,
                    proposalNumber: data.proposalNumber,
                    distributionTicketNumber: data.distributionTicketNumber,
                    securitiesType: data.securitiesType,
                    propertySaleCount: data.propertySaleCount,
                    propertySaleSerialStart: data.propertySaleSerialStart,
                    propertySaleSerialEnd: data.propertySaleSerialEnd,
                    bayWafaCount: data.bayWafaCount,
                    bayWafaSerialStart: data.bayWafaSerialStart,
                    bayWafaSerialEnd: data.bayWafaSerialEnd,
                    rentCount: data.rentCount,
                    rentSerialStart: data.rentSerialStart,
                    rentSerialEnd: data.rentSerialEnd,
                    vehicleSaleCount: data.vehicleSaleCount,
                    vehicleSaleSerialStart: data.vehicleSaleSerialStart,
                    vehicleSaleSerialEnd: data.vehicleSaleSerialEnd,
                    exchangeCount: data.exchangeCount,
                    exchangeSerialStart: data.exchangeSerialStart,
                    exchangeSerialEnd: data.exchangeSerialEnd,
                    registrationBookCount: data.registrationBookCount,
                    registrationBookSerialStart: data.registrationBookSerialStart,
                    registrationBookSerialEnd: data.registrationBookSerialEnd,
                    printedPetitionCount: data.printedPetitionCount,
                    printedPetitionSerialStart: data.printedPetitionSerialStart,
                    printedPetitionSerialEnd: data.printedPetitionSerialEnd,
                    distributionStartNumber: data.distributionStartNumber,
                    distributionEndNumber: data.distributionEndNumber,
                    distributedPersonsCount: data.distributedPersonsCount,
                    remarks: data.remarks
                });

                // Parse dates
                if (data.proposalDateFormatted) {
                    const parsed = this.ngbDateParserFormatter.parse(data.proposalDateFormatted);
                    if (parsed) {
                        this.securitiesControlForm.patchValue({ proposalDate: parsed });
                    }
                }
                if (data.deliveryDateFormatted) {
                    const parsed = this.ngbDateParserFormatter.parse(data.deliveryDateFormatted);
                    if (parsed) {
                        this.securitiesControlForm.patchValue({ deliveryDate: parsed });
                    }
                }

                // Set visibility based on loaded data
                this.onSecuritiesTypeChange();
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });
    }

    onSecurityDocumentTypeChange(): void {
        // When document type changes, clear securities type and fields
        this.securitiesControlForm.patchValue({ securitiesType: null });
        this.resetSecuritiesFields();
    }

    onSecuritiesTypeChange(): void {
        const securitiesType = this.securitiesControlForm.get('securitiesType')?.value;
        
        // Reset all visibility flags
        this.resetVisibilityFlags();
        
        if (securitiesType >= 1 && securitiesType <= 7) {
            // Single type options
            this.showSingleTypeFields = true;
            this.showCombinedTypeFields = false;
            
            switch(securitiesType) {
                case 1: // ستههای خرید و فروش جایداد
                    this.showPropertySaleFields = true;
                    break;
                case 2: // ستههای بیع وفا
                    this.showBayWafaFields = true;
                    break;
                case 3: // ستههای کرایی
                    this.showRentFields = true;
                    break;
                case 4: // ستههای خرید و فروش وسایط نقلیه
                    this.showVehicleSaleFields = true;
                    break;
                case 5: // ستههای تبادله
                    this.showExchangeFields = true;
                    break;
                case 6: // کتاب ثبت معاملات
                    this.showRegistrationBookFields = true;
                    break;
                case 7: // عرایض مطبوع
                    this.showPrintedPetitionFields = true;
                    break;
            }
        } else if (securitiesType >= 8 && securitiesType <= 11) {
            // Combined type options
            this.showSingleTypeFields = false;
            this.showCombinedTypeFields = true;
            
            switch(securitiesType) {
                case 8: // ستههای خرید و فروش جایداد و بیع وفا
                    this.showPropertySaleBayWafaFields = true;
                    break;
                case 9: // ستههای خرید و فروش جایداد و کرایی
                    this.showPropertySaleRentFields = true;
                    break;
                case 10: // ستههای بیع وفا و کرایی
                    this.showBayWafaRentFields = true;
                    break;
                case 11: // تمام انواع ستههای جایداد
                    this.showAllPropertyFields = true;
                    break;
            }
        }
    }

    private resetVisibilityFlags(): void {
        // Single Type Visibility Flags
        this.showPropertySaleFields = false;
        this.showBayWafaFields = false;
        this.showRentFields = false;
        this.showVehicleSaleFields = false;
        this.showExchangeFields = false;
        this.showRegistrationBookFields = false;
        this.showPrintedPetitionFields = false;

        // Combined Type Visibility Flags
        this.showPropertySaleBayWafaFields = false;
        this.showPropertySaleRentFields = false;
        this.showBayWafaRentFields = false;
        this.showAllPropertyFields = false;

        // Show/hide flags
        this.showSingleTypeFields = false;
        this.showCombinedTypeFields = false;
    }

    private resetSecuritiesFields(): void {
        // Reset all securities fields
        this.securitiesControlForm.patchValue({
            propertySaleCount: null,
            propertySaleSerialStart: '',
            propertySaleSerialEnd: '',
            bayWafaCount: null,
            bayWafaSerialStart: '',
            bayWafaSerialEnd: '',
            rentCount: null,
            rentSerialStart: '',
            rentSerialEnd: '',
            vehicleSaleCount: null,
            vehicleSaleSerialStart: '',
            vehicleSaleSerialEnd: '',
            exchangeCount: null,
            exchangeSerialStart: '',
            exchangeSerialEnd: '',
            registrationBookCount: null,
            registrationBookSerialStart: '',
            registrationBookSerialEnd: '',
            printedPetitionCount: null,
            printedPetitionSerialStart: '',
            printedPetitionSerialEnd: ''
        });
    }

    formatDateForBackend(date: NgbDate | null): string | undefined {
        if (!date) return undefined;
        const calendar = this.calendarService.getSelectedCalendar();
        const calendarDate = {
            year: date.year,
            month: date.month,
            day: date.day,
            calendarType: calendar
        };
        const gregorianDate = this.calendarConversionService.toGregorian(calendarDate);
        if (gregorianDate) {
            const year = gregorianDate.getFullYear();
            const month = String(gregorianDate.getMonth() + 1).padStart(2, '0');
            const day = String(gregorianDate.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        }
        return undefined;
    }

    onSubmit(): void {
        if (this.securitiesControlForm.invalid) {
            this.toastr.error('لطفاً تمام فیلدهای الزامی را پر کنید');
            this.markFormGroupTouched();
            return;
        }

        const formValue = this.securitiesControlForm.value;
        const data: SecuritiesControlData = {
            ...formValue,
            proposalDate: this.formatDateForBackend(formValue.proposalDate),
            deliveryDate: this.formatDateForBackend(formValue.deliveryDate)
        };

        if (this.isEditMode && this.editId) {
            this.securitiesControlService.update(this.editId, data).subscribe({
                next: (res) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت بروزرسانی شد');
                    // Stay on page for edit mode - just show success message
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در بروزرسانی اطلاعات');
                }
            });
        } else {
            this.securitiesControlService.create(data).subscribe({
                next: (res) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت ثبت شد');
                    // Redirect to list for create mode
                    this.router.navigate(['/securities-control/list']);
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در ثبت اطلاعات');
                }
            });
        }
    }

    markFormGroupTouched(): void {
        Object.keys(this.securitiesControlForm.controls).forEach(key => {
            this.securitiesControlForm.get(key)?.markAsTouched();
        });
    }

    resetForm(): void {
        this.securitiesControlForm.reset();
        this.resetVisibilityFlags();
        this.resetSecuritiesFields();
        
        // Generate a new serial number for reset form
        this.generateNextSerialNumber();
        
        // If in edit mode, navigate to create page
        if (this.isEditMode) {
            this.isEditMode = false;
            this.editId = null;
            this.router.navigate(['/securities-control']);
        }
    }

    goToList(): void {
        this.router.navigate(['/securities-control/list']);
    }

    nextTab(): void {
        if (this.selectedTabIndex < 3) {
            this.selectedTabIndex++;
        }
    }

    prevTab(): void {
        if (this.selectedTabIndex > 0) {
            this.selectedTabIndex--;
        }
    }

    // Form control getters
    get serialNumber() { return this.securitiesControlForm.get('serialNumber'); }
    get securityDocumentType() { return this.securitiesControlForm.get('securityDocumentType'); }
    get proposalNumber() { return this.securitiesControlForm.get('proposalNumber'); }
}