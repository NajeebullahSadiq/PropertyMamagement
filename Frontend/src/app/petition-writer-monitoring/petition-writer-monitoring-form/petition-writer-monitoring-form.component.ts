import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { PetitionWriterMonitoringService } from 'src/app/shared/petition-writer-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { AuthService } from 'src/app/shared/auth.service';
import {
    PetitionWriterMonitoringData,
    PetitionWriterMonitoringSectionTypes,
    ShamsiMonths,
    QamariMonths,
    GregorianMonths
} from 'src/app/models/PetitionWriterMonitoring';
import { CalendarType } from 'src/app/models/calendar-type';

@Component({
    selector: 'app-petition-writer-monitoring-form',
    templateUrl: './petition-writer-monitoring-form.component.html',
    styleUrls: ['./petition-writer-monitoring-form.component.scss'],
})
export class PetitionWriterMonitoringFormComponent extends BaseComponent implements OnInit {

    mainForm!: FormGroup;
    isEditMode = false;
    editId: number | null = null;

    // Section visibility
    showComplaintsSection = false;
    showViolationsSection = false;
    showMonitoringSection = false;

    // Dropdown data
    sectionTypes = PetitionWriterMonitoringSectionTypes;
    monitoringMonths: { value: string; label: string }[] = [];  // Dynamic months based on calendar type
    monitoringYears: string[] = [];  // Dynamic years
    
    // RBAC
    canEdit = false;
    isViewOnly = false;
    isSearching = false;
    licenseFound = false;

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
        super();
        this.initForm();
    }

    ngOnInit(): void {
        this.checkPermissions();
        this.updateYearsAndMonths();

        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isEditMode = true;
            this.editId = parseInt(id, 10);
            this.service.mainTableId = this.editId;
            this.loadData(this.editId);
        } else {
            this.loadNextSerialNumber();
        }

        // Subscribe to calendar changes
        this.calendarService.selectedCalendar$.subscribe(() => {
            this.updateYearsAndMonths();
        });
    }

    checkPermissions(): void {
        this.isViewOnly = !this.rbacService.hasPermission('petitionwritermonitoring.create');
        this.canEdit = this.rbacService.hasPermission('petitionwritermonitoring.edit');
    }

    updateYearsAndMonths(): void {
        const calendar = this.calendarService.getSelectedCalendar();
        
        // Update months based on calendar type
        switch (calendar) {
            case CalendarType.HIJRI_SHAMSI:
                this.monitoringMonths = ShamsiMonths;
                break;
            case CalendarType.HIJRI_QAMARI:
                this.monitoringMonths = QamariMonths;
                break;
            case CalendarType.GREGORIAN:
                this.monitoringMonths = GregorianMonths;
                break;
            default:
                this.monitoringMonths = ShamsiMonths;
        }

        // Generate years dynamically based on calendar type
        this.generateYears(calendar);
    }

    generateYears(calendarType: CalendarType): void {
        this.monitoringYears = [];
        const now = new Date();
        
        let currentYear: number;
        let startYear: number;
        
        switch (calendarType) {
            case CalendarType.HIJRI_SHAMSI:
                // Shamsi year is approximately 621 years behind Gregorian
                currentYear = now.getFullYear() - 621;
                // Generate 10 years back and unlimited future
                startYear = currentYear - 10;
                break;
            case CalendarType.HIJRI_QAMARI:
                // Qamari year is approximately 579 years behind Gregorian
                currentYear = Math.floor((now.getFullYear() - 622) * 1.0307);
                startYear = currentYear - 10;
                break;
            case CalendarType.GREGORIAN:
            default:
                currentYear = now.getFullYear();
                startYear = currentYear - 10;
                break;
        }

        // Generate years from startYear to currentYear + 50 (unlimited future)
        for (let year = startYear; year <= currentYear + 50; year++) {
            this.monitoringYears.push(year.toString());
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

    loadNextSerialNumber(sectionType?: string): void {
        this.service.getNextSerialNumber(sectionType).subscribe({
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

    parseDateString(dateStr: string): Date | string | null {
        if (!dateStr) return null;
        
        // If already in Hijri Shamsi format (YYYY/MM/DD), return as-is
        if (/^\d{4}\/\d{2}\/\d{2}$/.test(dateStr)) {
            return dateStr;
        }
        
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

        // Reload serial number based on selected section type (only for new records)
        if (!this.isEditMode && sectionType) {
            this.loadNextSerialNumber(sectionType);
        }
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
            this.mainForm.get('petitionWriterLicenseNumber')?.setValidators([Validators.required]);
            this.mainForm.get('petitionWriterName')?.setValidators([Validators.required]);
            this.mainForm.get('petitionWriterDistrict')?.setValidators([Validators.required]);
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
        this.isEditMode = false;
        this.editId = null;
        // Reload serial number after reset
        this.loadNextSerialNumber();
    }

    searchLicense(): void {
        const licenseNumber = this.mainForm.get('petitionWriterLicenseNumber')?.value;
        if (!licenseNumber) {
            this.toastr.warning('لطفا نمبر جواز را وارد کنید');
            return;
        }

        this.isSearching = true;
        this.licenseFound = false;
        this.service.searchLicenseByNumber(licenseNumber).subscribe({
            next: (result) => {
                this.isSearching = false;
                this.licenseFound = true;
                // Auto-populate the fields
                this.mainForm.patchValue({
                    petitionWriterName: result.petitionWriterName,
                    petitionWriterDistrict: result.petitionWriterDistrict
                });
                this.toastr.success('معلومات عریضه نویس یافت شد');
            },
            error: (err) => {
                this.isSearching = false;
                this.licenseFound = false;
                if (err.status === 404) {
                    this.toastr.error('جواز با این نمبر یافت نشد');
                } else {
                    this.toastr.error('خطا در جستجو');
                }
                console.error(err);
            }
        });
    }
}
