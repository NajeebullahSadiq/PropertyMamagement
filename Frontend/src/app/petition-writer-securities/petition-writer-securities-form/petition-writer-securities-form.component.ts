import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PetitionWriterSecuritiesService } from 'src/app/shared/petition-writer-securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { PetitionWriterSecuritiesData } from 'src/app/models/PetitionWriterSecurities';

@Component({
    selector: 'app-petition-writer-securities-form',
    templateUrl: './petition-writer-securities-form.component.html',
    styleUrls: ['./petition-writer-securities-form.component.scss']
})
export class PetitionWriterSecuritiesFormComponent implements OnInit {
    petitionForm!: FormGroup;
    isEditMode = false;
    editId: number | null = null;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        private petitionService: PetitionWriterSecuritiesService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService
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
        
        this.route.queryParams.subscribe(params => {
            if (params['id']) {
                this.isEditMode = true;
                this.editId = parseInt(params['id'], 10);
                this.loadData(this.editId);
            }
        });
    }

    initForm(): void {
        this.petitionForm = this.fb.group({
            id: [null],
            registrationNumber: ['', [Validators.required, Validators.maxLength(50)]],
            petitionWriterName: ['', [Validators.required, Validators.maxLength(200)]],
            petitionWriterFatherName: ['', [Validators.required, Validators.maxLength(200)]],
            licenseNumber: ['', [Validators.required, Validators.maxLength(50)]],
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
                if (data.distributionDateFormatted) {
                    this.petitionForm.patchValue({ distributionDate: data.distributionDateFormatted });
                }
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });
    }

    private formatDateForBackend(dateValue: any): string {
        const currentCalendar = this.calendarService.getSelectedCalendar();
        if (dateValue instanceof Date) {
            const calendarDate = this.calendarConversionService.fromGregorian(dateValue, currentCalendar);
            return `${calendarDate.year}-${String(calendarDate.month).padStart(2, '0')}-${String(calendarDate.day).padStart(2, '0')}`;
        } else if (typeof dateValue === 'object' && dateValue.year) {
            return `${dateValue.year}-${String(dateValue.month).padStart(2, '0')}-${String(dateValue.day).padStart(2, '0')}`;
        } else if (typeof dateValue === 'string') {
            return dateValue.replace(/\//g, '-');
        }
        return '';
    }

    validateSerialNumbers(): boolean {
        const start = this.petitionForm.get('serialNumberStart')?.value;
        const end = this.petitionForm.get('serialNumberEnd')?.value;
        if (!start || !end) return true;
        const startNum = parseInt(start, 10);
        const endNum = parseInt(end, 10);
        if (!isNaN(startNum) && !isNaN(endNum)) {
            if (startNum > endNum) {
                this.toastr.error('آغاز سریال نمبر باید کمتر یا مساوی ختم سریال نمبر باشد');
                return false;
            }
        } else if (start > end) {
            this.toastr.error('آغاز سریال نمبر باید کمتر یا مساوی ختم سریال نمبر باشد');
            return false;
        }
        return true;
    }

    onSubmit(): void {
        if (this.petitionForm.invalid) {
            this.toastr.error('لطفاً تمام فیلدهای الزامی را پر کنید');
            this.markFormGroupTouched();
            return;
        }
        if (!this.validateSerialNumbers()) return;

        const formValue = this.petitionForm.value;
        const data: PetitionWriterSecuritiesData = {
            ...formValue,
            distributionDate: formValue.distributionDate ? this.formatDateForBackend(formValue.distributionDate) : undefined,
            calendarType: this.calendarService.getSelectedCalendar()
        };

        if (this.isEditMode && this.editId) {
            this.updateRecord(data);
        } else {
            this.createRecord(data);
        }
    }

    private createRecord(data: PetitionWriterSecuritiesData): void {
        this.petitionService.create(data).subscribe({
            next: (res) => {
                this.toastr.success('معلومات موفقانه ثبت شد');
                this.editId = res.id;
                this.isEditMode = true;
                this.petitionService.dataChanged.next();
                this.router.navigate(['/petition-writer-securities/list']);
            },
            error: (err) => this.toastr.error(err.error?.message || 'خطا در ثبت اطلاعات')
        });
    }

    private updateRecord(data: PetitionWriterSecuritiesData): void {
        this.petitionService.update(this.editId!, data).subscribe({
            next: () => {
                this.toastr.info('معلومات موفقانه تغیر یافت');
                this.petitionService.dataChanged.next();
            },
            error: (err) => this.toastr.error(err.error?.message || 'خطا در بروزرسانی اطلاعات')
        });
    }

    markFormGroupTouched(): void {
        Object.keys(this.petitionForm.controls).forEach(key => {
            this.petitionForm.get(key)?.markAsTouched();
        });
    }

    resetForm(): void {
        this.petitionForm.reset();
        this.isEditMode = false;
        this.editId = null;
        this.router.navigate(['/petition-writer-securities']);
    }

    goToList(): void {
        this.router.navigate(['/petition-writer-securities/list']);
    }

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
