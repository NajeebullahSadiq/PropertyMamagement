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
import { PetitionWriterSecuritiesService } from 'src/app/shared/petition-writer-securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { PetitionWriterSecuritiesData } from 'src/app/models/PetitionWriterSecurities';

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
    selector: 'app-petition-writer-securities-form',
    templateUrl: './petition-writer-securities-form.component.html',
    styleUrls: ['./petition-writer-securities-form.component.scss'],
    providers: [
        { provide: NgbCalendar, useClass: NgbCalendarPersian },
        { provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
    ],
})
export class PetitionWriterSecuritiesFormComponent implements OnInit {
    maxDate = { year: 1410, month: 12, day: 31 };
    minDate = { year: 1320, month: 12, day: 31 };

    petitionForm!: FormGroup;
    isEditMode = false;
    editId: number | null = null;
    selectedTabIndex = 0;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        private petitionService: PetitionWriterSecuritiesService,
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
        }
    }

    initForm(): void {
        this.petitionForm = this.fb.group({
            id: [null],
            // Tab 1: مشخصات عریضه‌نویس
            registrationNumber: ['', [Validators.required, Validators.maxLength(50)]],
            petitionWriterName: ['', [Validators.required, Validators.maxLength(200)]],
            petitionWriterFatherName: ['', [Validators.required, Validators.maxLength(200)]],
            licenseNumber: ['', [Validators.required, Validators.maxLength(50)]],
            // Tab 2: مشخصات سند بهادار عریضه
            petitionCount: [null, [Validators.required, Validators.min(1)]],
            amount: [null, [Validators.required, Validators.min(0)]],
            bankReceiptNumber: ['', [Validators.required, Validators.maxLength(100)]],
            serialNumberStart: ['', [Validators.required, Validators.maxLength(100)]],
            serialNumberEnd: ['', [Validators.required, Validators.maxLength(100)]],
            distributionDate: [null, [Validators.required]]
        });
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.petitionService.getById(id, calendar).subscribe({
            next: (data) => {
                this.petitionForm.patchValue({
                    id: data.id,
                    registrationNumber: data.registrationNumber,
                    petitionWriterName: data.petitionWriterName,
                    petitionWriterFatherName: data.petitionWriterFatherName,
                    licenseNumber: data.licenseNumber,
                    petitionCount: data.petitionCount,
                    amount: data.amount,
                    bankReceiptNumber: data.bankReceiptNumber,
                    serialNumberStart: data.serialNumberStart,
                    serialNumberEnd: data.serialNumberEnd
                });

                // Parse date
                if (data.distributionDateFormatted) {
                    const parsed = this.ngbDateParserFormatter.parse(data.distributionDateFormatted);
                    if (parsed) {
                        this.petitionForm.patchValue({ distributionDate: parsed });
                    }
                }
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
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

    validateSerialNumbers(): boolean {
        const start = this.petitionForm.get('serialNumberStart')?.value;
        const end = this.petitionForm.get('serialNumberEnd')?.value;
        
        if (!start || !end) return true;

        // Try numeric comparison first
        const startNum = parseInt(start, 10);
        const endNum = parseInt(end, 10);
        
        if (!isNaN(startNum) && !isNaN(endNum)) {
            if (startNum > endNum) {
                this.toastr.error('آغاز سریال نمبر باید کمتر یا مساوی ختم سریال نمبر باشد');
                return false;
            }
        } else {
            // String comparison
            if (start > end) {
                this.toastr.error('آغاز سریال نمبر باید کمتر یا مساوی ختم سریال نمبر باشد');
                return false;
            }
        }
        return true;
    }

    onSubmit(): void {
        if (this.petitionForm.invalid) {
            this.toastr.error('لطفاً تمام فیلدهای الزامی را پر کنید');
            this.markFormGroupTouched();
            return;
        }

        if (!this.validateSerialNumbers()) {
            return;
        }

        const formValue = this.petitionForm.value;
        const data: PetitionWriterSecuritiesData = {
            ...formValue,
            distributionDate: this.formatDateForBackend(formValue.distributionDate)
        };

        if (this.isEditMode && this.editId) {
            this.petitionService.update(this.editId, data).subscribe({
                next: (res) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت بروزرسانی شد');
                    this.router.navigate(['/petition-writer-securities/list']);
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در بروزرسانی اطلاعات');
                }
            });
        } else {
            this.petitionService.create(data).subscribe({
                next: (res) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت ثبت شد');
                    this.router.navigate(['/petition-writer-securities/list']);
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در ثبت اطلاعات');
                }
            });
        }
    }

    markFormGroupTouched(): void {
        Object.keys(this.petitionForm.controls).forEach(key => {
            this.petitionForm.get(key)?.markAsTouched();
        });
    }

    resetForm(): void {
        this.petitionForm.reset();
    }

    goToList(): void {
        this.router.navigate(['/petition-writer-securities/list']);
    }

    nextTab(): void {
        if (this.selectedTabIndex < 1) {
            this.selectedTabIndex++;
        }
    }

    prevTab(): void {
        if (this.selectedTabIndex > 0) {
            this.selectedTabIndex--;
        }
    }

    // Form control getters
    get registrationNumber() { return this.petitionForm.get('registrationNumber'); }
    get petitionWriterName() { return this.petitionForm.get('petitionWriterName'); }
    get petitionWriterFatherName() { return this.petitionForm.get('petitionWriterFatherName'); }
    get licenseNumber() { return this.petitionForm.get('licenseNumber'); }
    get petitionCount() { return this.petitionForm.get('petitionCount'); }
    get amount() { return this.petitionForm.get('amount'); }
    get bankReceiptNumber() { return this.petitionForm.get('bankReceiptNumber'); }
    get serialNumberStart() { return this.petitionForm.get('serialNumberStart'); }
    get serialNumberEnd() { return this.petitionForm.get('serialNumberEnd'); }
    get distributionDate() { return this.petitionForm.get('distributionDate'); }
}
