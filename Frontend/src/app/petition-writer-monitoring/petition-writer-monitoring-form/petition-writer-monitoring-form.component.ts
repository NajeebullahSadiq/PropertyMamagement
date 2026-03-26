import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PetitionWriterMonitoringService } from 'src/app/shared/petition-writer-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { AuthService } from 'src/app/shared/auth.service';
import {
    PetitionWriterMonitoringData,
    PetitionWriterMonitoringSectionTypes,
    AfghanMonths
} from 'src/app/models/PetitionWriterMonitoring';

@Component({
    selector: 'app-petition-writer-monitoring-form',
    templateUrl: './petition-writer-monitoring-form.component.html',
    styleUrls: ['./petition-writer-monitoring-form.component.scss'],
})
export class PetitionWriterMonitoringFormComponent implements OnInit {

    mainForm!: FormGroup;
    isEditMode = false;
    editId: number | null = null;

    // Section visibility
    showComplaintsSection = false;
    showViolationsSection = false;
    showMonitoringSection = false;

    // Dropdown data
    sectionTypes = PetitionWriterMonitoringSectionTypes;
    afghanMonths = AfghanMonths;

    // Years for monitoring (current year ± 5 years)
    monitoringYears: string[] = [];
    
    // RBAC
    canEdit = false;
    isViewOnly = false;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        public service: PetitionWriterMonitoringService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private rbacService: RbacService,
        private authService: AuthService
    ) {
        this.initForm();
        this.generateYears();
    }

    ngOnInit(): void {
        this.checkPermissions();

        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isEditMode = true;
            this.editId = parseInt(id, 10);
            this.service.mainTableId = this.editId;
            this.loadData(this.editId);
        } else {
            this.loadNextSerialNumber();
        }
    }

    checkPermissions(): void {
        this.isViewOnly = !this.rbacService.hasPermission('petitionwritermonitoring.create');
        this.canEdit = this.rbacService.hasPermission('petitionwritermonitoring.edit');
    }

    generateYears(): void {
        const currentYear = new Date().getFullYear();
        for (let i = currentYear - 5; i <= currentYear + 5; i++) {
            this.monitoringYears.push(i.toString());
        }
    }

    initForm(): void {
        this.mainForm = this.fb.group({
            id: [null],
            serialNumber: [{ value: '', disabled: true }],
            sectionType: ['', Validators.required],
            registrationDate: [''],
            
            // Complaints fields
            complainantName: [''],
            complaintSubject: [''],
            complaintActionsTaken: [''],
            complaintRemarks: [''],
            
            // Violations fields
            petitionWriterName: [''],
            petitionWriterLicenseNumber: [''],
            petitionWriterDistrict: [''],
            violationType: [''],
            violationActionsTaken: [''],
            violationRemarks: [''],
            
            // Monitoring fields
            monitoringYear: [''],
            monitoringMonth: [''],
            monitoringCount: [''],
            monitoringRemarks: [''],
        });
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.service.getById(id, calendar).subscribe({
            next: (data) => {
                this.patchForm(data);
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });
    }

    loadNextSerialNumber(): void {
        this.service.getNextSerialNumber().subscribe({
            next: (result) => {
                this.mainForm.patchValue({ serialNumber: result.serialNumber });
            },
            error: (err) => {
                console.error('Error loading serial number:', err);
                this.mainForm.patchValue({ serialNumber: '1' });
            }
        });
    }

    patchForm(data: any): void {
        this.mainForm.patchValue({
            id: data.id,
            serialNumber: data.serialNumber,
            sectionType: data.sectionType,
            complainantName: data.complainantName,
            complaintSubject: data.complaintSubject,
            complaintActionsTaken: data.complaintActionsTaken,
            complaintRemarks: data.complaintRemarks,
            petitionWriterName: data.petitionWriterName,
            petitionWriterLicenseNumber: data.petitionWriterLicenseNumber,
            petitionWriterDistrict: data.petitionWriterDistrict,
            violationType: data.violationType,
            violationActionsTaken: data.violationActionsTaken,
            violationRemarks: data.violationRemarks,
            monitoringYear: data.monitoringYear,
            monitoringMonth: data.monitoringMonth,
            monitoringCount: data.monitoringCount,
            monitoringRemarks: data.monitoringRemarks,
        });

        // Parse dates
        if (data.registrationDateFormatted) {
            const date = this.parseDateString(data.registrationDateFormatted);
            if (date) {
                this.mainForm.patchValue({ registrationDate: date });
            }
        }

        // Trigger section visibility
        this.onSectionTypeChange();
    }

    parseDateString(dateStr: string): Date | null {
        try {
            const parts = dateStr.split('/');
            if (parts.length === 3) {
                return new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
            }
            return new Date(dateStr);
        } catch {
            return null;
        }
    }

    onSectionTypeChange(): void {
        const sectionType = this.mainForm.get('sectionType')?.value;
        
        this.showComplaintsSection = sectionType === 'complaints';
        this.showViolationsSection = sectionType === 'violations';
        this.showMonitoringSection = sectionType === 'monitoring';

        // Update validators based on section type
        this.updateValidators();
    }

    updateValidators(): void {
        // Reset all validators
        this.mainForm.get('complainantName')?.clearValidators();
        this.mainForm.get('complaintSubject')?.clearValidators();
        this.mainForm.get('petitionWriterName')?.clearValidators();
        this.mainForm.get('petitionWriterLicenseNumber')?.clearValidators();
        this.mainForm.get('petitionWriterDistrict')?.clearValidators();
        this.mainForm.get('violationType')?.clearValidators();
        this.mainForm.get('monitoringYear')?.clearValidators();
        this.mainForm.get('monitoringMonth')?.clearValidators();
        this.mainForm.get('monitoringCount')?.clearValidators();

        // Set validators based on section type
        if (this.showComplaintsSection) {
            this.mainForm.get('complainantName')?.setValidators([Validators.required]);
            this.mainForm.get('complaintSubject')?.setValidators([Validators.required]);
        } else if (this.showViolationsSection) {
            this.mainForm.get('petitionWriterName')?.setValidators([Validators.required]);
            this.mainForm.get('petitionWriterLicenseNumber')?.setValidators([Validators.required]);
            this.mainForm.get('petitionWriterDistrict')?.setValidators([Validators.required]);
            this.mainForm.get('violationType')?.setValidators([Validators.required]);
        } else if (this.showMonitoringSection) {
            this.mainForm.get('monitoringYear')?.setValidators([Validators.required]);
            this.mainForm.get('monitoringMonth')?.setValidators([Validators.required]);
            this.mainForm.get('monitoringCount')?.setValidators([Validators.required]);
        }

        // Update validity
        this.mainForm.get('complainantName')?.updateValueAndValidity();
        this.mainForm.get('complaintSubject')?.updateValueAndValidity();
        this.mainForm.get('petitionWriterName')?.updateValueAndValidity();
        this.mainForm.get('petitionWriterLicenseNumber')?.updateValueAndValidity();
        this.mainForm.get('petitionWriterDistrict')?.updateValueAndValidity();
        this.mainForm.get('violationType')?.updateValueAndValidity();
        this.mainForm.get('monitoringYear')?.updateValueAndValidity();
        this.mainForm.get('monitoringMonth')?.updateValueAndValidity();
        this.mainForm.get('monitoringCount')?.updateValueAndValidity();
    }

    saveForm(): void {
        if (this.mainForm.invalid) {
            this.toastr.error('لطفا فیلدهای الزامی را تکمیل کنید');
            this.markFormGroupTouched(this.mainForm);
            return;
        }

        const formValue = this.mainForm.getRawValue();
        const calendar = this.calendarService.getSelectedCalendar();

        const data: PetitionWriterMonitoringData = {
            id: formValue.id,
            serialNumber: formValue.serialNumber,
            sectionType: formValue.sectionType,
            registrationDate: formValue.registrationDate ? this.formatDate(formValue.registrationDate) : undefined,
            calendarType: calendar,
            
            // Complaints
            complainantName: formValue.complainantName,
            complaintSubject: formValue.complaintSubject,
            complaintActionsTaken: formValue.complaintActionsTaken,
            complaintRemarks: formValue.complaintRemarks,
            
            // Violations
            petitionWriterName: formValue.petitionWriterName,
            petitionWriterLicenseNumber: formValue.petitionWriterLicenseNumber,
            petitionWriterDistrict: formValue.petitionWriterDistrict,
            violationType: formValue.violationType,
            violationActionsTaken: formValue.violationActionsTaken,
            violationRemarks: formValue.violationRemarks,
            
            // Monitoring
            monitoringYear: formValue.monitoringYear || undefined,
            monitoringMonth: formValue.monitoringMonth || undefined,
            monitoringCount: formValue.monitoringCount ? parseInt(formValue.monitoringCount, 10) : undefined,
            monitoringRemarks: formValue.monitoringRemarks || undefined,
        };

        if (this.isEditMode && this.editId) {
            this.service.update(this.editId, data).subscribe({
                next: () => {
                    this.toastr.success('معلومات موفقانه تغییر یافت');
                    this.goToList();
                },
                error: (err) => {
                    this.toastr.error('خطا در ذخیره معلومات');
                    console.error(err);
                }
            });
        } else {
            this.service.create(data).subscribe({
                next: () => {
                    this.toastr.success('معلومات موفقانه ثبت شد');
                    this.goToList();
                },
                error: (err) => {
                    this.toastr.error('خطا در ذخیره معلومات');
                    console.error(err);
                }
            });
        }
    }

    formatDate(date: Date): string {
        const d = new Date(date);
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    markFormGroupTouched(formGroup: FormGroup): void {
        Object.values(formGroup.controls).forEach(control => {
            control.markAsTouched();
            if ((control as any).controls) {
                this.markFormGroupTouched(control as FormGroup);
            }
        });
    }

    goToList(): void {
        this.router.navigate(['/petition-writer-monitoring/list']);
    }

    resetForm(): void {
        this.mainForm.reset();
        this.showComplaintsSection = false;
        this.showViolationsSection = false;
        this.showMonitoringSection = false;
        if (!this.isEditMode) {
            this.loadNextSerialNumber();
        }
    }
}
