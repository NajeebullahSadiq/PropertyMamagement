import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ActivityMonitoringService } from 'src/app/shared/activity-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { AuthService } from 'src/app/shared/auth.service';
import {
    ActivityMonitoringData,
    DeedDocumentTypes,
    DeedItem
} from 'src/app/models/ActivityMonitoring';

@Component({
    selector: 'app-activity-monitoring-form',
    templateUrl: './activity-monitoring-form.component.html',
    styleUrls: ['./activity-monitoring-form.component.scss'],
})
export class ActivityMonitoringFormComponent implements OnInit {

    // Single Form
    mainForm!: FormGroup;

    isEditMode = false;
    editId: number | null = null;

    // Deed Items (for Annual Report)
    deedTypes = DeedDocumentTypes;
    deedItems: DeedItem[] = [];
    selectedDeedType: number | null = null;

    // Company search states
    companySearching = false;
    companyFound = false;
    companyNotFound = false;

    // Section visibility based on sectionType selection
    showComplaintsSection = false;
    showViolationsSection = false;
    showInspectionSection = false;
    selectedCompanyId: number | null = null;

    // RBAC
    canEdit = false;
    isViewOnly = false;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        public service: ActivityMonitoringService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private rbacService: RbacService,
        private companyService: CompnaydetailService,
        private authService: AuthService
    ) {
        this.initForm();
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
            // New record - fetch next serial number
            this.loadNextSerialNumber();
        }
    }

    checkPermissions(): void {
        this.isViewOnly = !this.rbacService.hasPermission('activitymonitoring.create');
        this.canEdit = this.rbacService.hasPermission('activitymonitoring.edit');
    }

    initForm(): void {
        // Single Form with all fields
        this.mainForm = this.fb.group({
            id: [null],
            // Section 1: Annual Report (Common fields)
            serialNumber: [{ value: '', disabled: true }], // Auto-generated, readonly
            licenseNumber: ['', [Validators.required, Validators.maxLength(50)]],
            licenseHolderName: ['', [Validators.required, Validators.maxLength(200)]],
            district: [''],
            reportRegistrationDate: [''],
            sectionType: ['', Validators.required],
            saleDeedsCount: [''],
            rentalDeedsCount: [''],
            baiUlWafaDeedsCount: [''],
            vehicleTransactionDeedsCount: [''],
            annualReportRemarks: [''],
            
            // Section 2: Complaints fields (conditionally required)
            complaintRegistrationDate: [''],
            complaintSubject: [''],
            complainantName: [''],
            complaintActionsTaken: [''],
            complaintRemarks: [''],
            
            // Section 3: Violations fields (conditionally required)
            violationStatus: [''],
            violationType: [''],
            violationDate: [''],
            closureDate: [''],
            closureReason: [''],
            violationActionsTaken: [''],
            violationRemarks: [''],
            
            // Section 4: Inspection fields (conditionally required)
            monitoringType: [''],
            month: [''],
            monitoringCount: [''],
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
        // Patch common fields
        this.mainForm.patchValue({
            id: data.id,
            serialNumber: data.serialNumber,
            licenseNumber: data.licenseNumber,
            licenseHolderName: data.licenseHolderName,
            district: data.district,
            sectionType: data.sectionType,
            saleDeedsCount: data.saleDeedsCount,
            rentalDeedsCount: data.rentalDeedsCount,
            baiUlWafaDeedsCount: data.baiUlWafaDeedsCount,
            vehicleTransactionDeedsCount: data.vehicleTransactionDeedsCount,
            annualReportRemarks: data.annualReportRemarks,
        });

        // Set company found state if license number exists
        if (data.licenseNumber) {
            this.companyFound = true;
        }

        // Parse dates - convert formatted dates to Date objects
        if (data.reportRegistrationDateFormatted) {
            const date = this.parseDateString(data.reportRegistrationDateFormatted);
            if (date) {
                this.mainForm.patchValue({ reportRegistrationDate: date });
            }
        }

        // Load deed items if available
        if (data.deedItems && data.deedItems.length > 0) {
            this.deedItems = data.deedItems.map((item: any) => ({
                id: item.id,
                deedType: item.deedType,
                serialStart: item.serialStart,
                serialEnd: item.serialEnd,
                count: item.count
            }));
        }

        // Load section-specific data based on sectionType
        if (data.sectionType === 'complaints' && data.complaints && data.complaints.length > 0) {
            const complaint = data.complaints[0];
            this.mainForm.patchValue({
                complaintSubject: complaint.complaintSubject,
                complainantName: complaint.complainantName,
                complaintActionsTaken: complaint.actionsTaken,
                complaintRemarks: complaint.remarks,
            });
            if (complaint.complaintRegistrationDateFormatted) {
                const date = this.parseDateString(complaint.complaintRegistrationDateFormatted);
                if (date) {
                    this.mainForm.patchValue({ complaintRegistrationDate: date });
                }
            }
        } else if (data.sectionType === 'violations' && data.realEstateViolations && data.realEstateViolations.length > 0) {
            const violation = data.realEstateViolations[0];
            this.mainForm.patchValue({
                violationStatus: violation.violationStatus,
                violationType: violation.violationType,
                closureReason: violation.closureReason,
                violationActionsTaken: violation.actionsTaken,
                violationRemarks: violation.remarks,
            });
            if (violation.violationDateFormatted) {
                const date = this.parseDateString(violation.violationDateFormatted);
                if (date) {
                    this.mainForm.patchValue({ violationDate: date });
                }
            }
            if (violation.closureDateFormatted) {
                const date = this.parseDateString(violation.closureDateFormatted);
                if (date) {
                    this.mainForm.patchValue({ closureDate: date });
                }
            }
            this.onViolationStatusChange();
        } else if (data.sectionType === 'inspection' && data.inspections && data.inspections.length > 0) {
            const inspection = data.inspections[0];
            this.mainForm.patchValue({
                monitoringType: inspection.monitoringType,
                month: inspection.month,
                monitoringCount: inspection.monitoringCount,
            });
            this.onMonitoringTypeChange();
        }

        // Trigger section visibility based on sectionType
        this.onSectionTypeChange();
    }

    private parseDateString(dateStr: string): Date | null {
        if (!dateStr) return null;
        const calendar = this.calendarService.getSelectedCalendar();
        return this.calendarConversionService.parseInputDate(dateStr, calendar);
    }

    private formatDateForBackend(dateValue: any): string {
        if (!dateValue) return '';
        
        const currentCalendar = this.calendarService.getSelectedCalendar();

        if (dateValue instanceof Date) {
            return this.calendarConversionService.formatDate(dateValue, currentCalendar).replace(/\//g, '-');
        } else if (typeof dateValue === 'string') {
            return dateValue.replace(/\//g, '-');
        }
        return '';
    }

    // ==================== Section Type Change ====================

    onSectionTypeChange(): void {
        const sectionType = this.mainForm.get('sectionType')?.value;
        
        // Reset all section visibility
        this.showComplaintsSection = false;
        this.showViolationsSection = false;
        this.showInspectionSection = false;
        
        // Clear all conditional validators first
        this.clearAllConditionalValidators();
        
        // Show the selected section and set validators
        if (sectionType === 'complaints') {
            this.showComplaintsSection = true;
            this.setComplaintValidators();
        } else if (sectionType === 'violations') {
            this.showViolationsSection = true;
            this.setViolationValidators();
        } else if (sectionType === 'inspection') {
            this.showInspectionSection = true;
            this.setInspectionValidators();
        }
    }

    clearAllConditionalValidators(): void {
        // Complaint fields
        this.mainForm.get('complaintRegistrationDate')?.clearValidators();
        this.mainForm.get('complaintSubject')?.clearValidators();
        this.mainForm.get('complainantName')?.clearValidators();
        
        // Violation fields
        this.mainForm.get('violationStatus')?.clearValidators();
        this.mainForm.get('violationType')?.clearValidators();
        this.mainForm.get('violationDate')?.clearValidators();
        this.mainForm.get('closureDate')?.clearValidators();
        this.mainForm.get('closureReason')?.clearValidators();
        
        // Inspection fields
        this.mainForm.get('monitoringType')?.clearValidators();
        this.mainForm.get('month')?.clearValidators();
        this.mainForm.get('monitoringCount')?.clearValidators();
        
        // Update validity
        this.mainForm.get('complaintRegistrationDate')?.updateValueAndValidity();
        this.mainForm.get('complaintSubject')?.updateValueAndValidity();
        this.mainForm.get('complainantName')?.updateValueAndValidity();
        this.mainForm.get('violationStatus')?.updateValueAndValidity();
        this.mainForm.get('violationType')?.updateValueAndValidity();
        this.mainForm.get('violationDate')?.updateValueAndValidity();
        this.mainForm.get('closureDate')?.updateValueAndValidity();
        this.mainForm.get('closureReason')?.updateValueAndValidity();
        this.mainForm.get('monitoringType')?.updateValueAndValidity();
        this.mainForm.get('month')?.updateValueAndValidity();
        this.mainForm.get('monitoringCount')?.updateValueAndValidity();
    }

    setComplaintValidators(): void {
        this.mainForm.get('complaintRegistrationDate')?.setValidators([Validators.required]);
        this.mainForm.get('complaintSubject')?.setValidators([Validators.required, Validators.maxLength(500)]);
        this.mainForm.get('complainantName')?.setValidators([Validators.required, Validators.maxLength(200)]);
        
        this.mainForm.get('complaintRegistrationDate')?.updateValueAndValidity();
        this.mainForm.get('complaintSubject')?.updateValueAndValidity();
        this.mainForm.get('complainantName')?.updateValueAndValidity();
    }

    setViolationValidators(): void {
        this.mainForm.get('violationStatus')?.setValidators([Validators.required]);
        
        this.mainForm.get('violationStatus')?.updateValueAndValidity();
        
        // Additional validators based on violation status
        this.onViolationStatusChange();
    }

    setInspectionValidators(): void {
        this.mainForm.get('monitoringType')?.setValidators([Validators.required]);
        
        this.mainForm.get('monitoringType')?.updateValueAndValidity();
        
        // Additional validators based on monitoring type
        this.onMonitoringTypeChange();
    }

    onViolationStatusChange(): void {
        const status = this.mainForm.get('violationStatus')?.value;
        
        if (status === 'منجر به مسدودی') {
            // Leading to Closure - require closure fields
            this.mainForm.get('closureDate')?.setValidators([Validators.required]);
            this.mainForm.get('closureReason')?.setValidators([Validators.required, Validators.maxLength(500)]);
            this.mainForm.get('violationType')?.clearValidators();
            this.mainForm.get('violationDate')?.clearValidators();
        } else if (status === 'عادی') {
            // Normal - require violation fields
            this.mainForm.get('violationType')?.setValidators([Validators.required, Validators.maxLength(500)]);
            this.mainForm.get('violationDate')?.setValidators([Validators.required]);
            this.mainForm.get('closureDate')?.clearValidators();
            this.mainForm.get('closureReason')?.clearValidators();
        } else {
            this.mainForm.get('violationType')?.clearValidators();
            this.mainForm.get('violationDate')?.clearValidators();
            this.mainForm.get('closureDate')?.clearValidators();
            this.mainForm.get('closureReason')?.clearValidators();
        }

        this.mainForm.get('violationType')?.updateValueAndValidity();
        this.mainForm.get('violationDate')?.updateValueAndValidity();
        this.mainForm.get('closureDate')?.updateValueAndValidity();
        this.mainForm.get('closureReason')?.updateValueAndValidity();
    }

    onMonitoringTypeChange(): void {
        const monitoringType = this.mainForm.get('monitoringType')?.value;
        
        if (monitoringType) {
            this.mainForm.get('month')?.setValidators([Validators.required]);
            this.mainForm.get('monitoringCount')?.setValidators([Validators.required, Validators.min(1)]);
        } else {
            this.mainForm.get('month')?.clearValidators();
            this.mainForm.get('monitoringCount')?.clearValidators();
        }

        this.mainForm.get('month')?.updateValueAndValidity();
        this.mainForm.get('monitoringCount')?.updateValueAndValidity();
    }

    // ==================== Single Save Function ====================

    saveForm(): void {
        if (this.mainForm.invalid) {
            this.markAllAsTouched();
            this.toastr.warning('لطفا تمام فیلدهای الزامی را پر کنید');
            return;
        }

        const formValue = this.mainForm.value;
        const currentCalendar = this.calendarService.getSelectedCalendar();
        const sectionType = formValue.sectionType;

        // Build data for single table design - all fields in one object
        const data: ActivityMonitoringData = {
            id: formValue.id,
            sectionType: sectionType,
            licenseNumber: formValue.licenseNumber,
            licenseHolderName: formValue.licenseHolderName,
            district: formValue.district,
            reportRegistrationDate: this.formatDateForBackend(formValue.reportRegistrationDate),
            saleDeedsCount: formValue.saleDeedsCount ? Number(formValue.saleDeedsCount) : undefined,
            rentalDeedsCount: formValue.rentalDeedsCount ? Number(formValue.rentalDeedsCount) : undefined,
            baiUlWafaDeedsCount: formValue.baiUlWafaDeedsCount ? Number(formValue.baiUlWafaDeedsCount) : undefined,
            vehicleTransactionDeedsCount: formValue.vehicleTransactionDeedsCount ? Number(formValue.vehicleTransactionDeedsCount) : undefined,
            annualReportRemarks: formValue.annualReportRemarks,
            deedItems: this.deedItems,
            calendarType: currentCalendar,
            
            // Complaints fields (directly on data)
            complaintRegistrationDate: this.formatDateForBackend(formValue.complaintRegistrationDate),
            complaintSubject: formValue.complaintSubject,
            complainantName: formValue.complainantName,
            complaintActionsTaken: formValue.complaintActionsTaken,
            complaintRemarks: formValue.complaintRemarks,
            
            // Violations fields (directly on data)
            violationStatus: formValue.violationStatus,
            violationType: formValue.violationType,
            violationDate: this.formatDateForBackend(formValue.violationDate),
            closureDate: this.formatDateForBackend(formValue.closureDate),
            closureReason: formValue.closureReason,
            violationActionsTaken: formValue.violationActionsTaken,
            violationRemarks: formValue.violationRemarks,
            
            // Inspections fields (directly on data)
            monitoringType: formValue.monitoringType,
            month: formValue.month,
            monitoringCount: formValue.monitoringCount ? Number(formValue.monitoringCount) : undefined
        };

        if (this.isEditMode && this.editId) {
            this.service.update(this.editId, data).subscribe({
                next: () => {
                    this.toastr.success('معلومات موفقانه تغییر یافت');
                },
                error: (err) => {
                    this.toastr.error('خطا در ذخیره معلومات');
                    console.error(err);
                }
            });
        } else {
            this.service.create(data).subscribe({
                next: (result) => {
                    this.toastr.success('معلومات موفقانه ثبت شد');
                    this.editId = result.id;
                    this.isEditMode = true;
                    this.mainForm.patchValue({ id: result.id });
                    this.service.mainTableId = result.id;
                },
                error: (err) => {
                    this.toastr.error('خطا در ذخیره معلومات');
                    console.error(err);
                }
            });
        }
    }

    markAllAsTouched(): void {
        Object.keys(this.mainForm.controls).forEach(key => {
            this.mainForm.get(key)?.markAsTouched();
        });
    }

    // ==================== Navigation ====================

    goToList(): void {
        this.router.navigate(['/activity-monitoring/list']);
    }

    resetForm(): void {
        this.service.resetMainTableId();
        this.mainForm.reset();
        this.deedItems = [];
        this.selectedDeedType = null;
        this.companySearching = false;
        this.companyFound = false;
        this.companyNotFound = false;
        this.selectedCompanyId = null;
        this.isEditMode = false;
        this.editId = null;
        this.showComplaintsSection = false;
        this.showViolationsSection = false;
        this.showInspectionSection = false;
    }

    // ==================== Deed Items CRUD ====================

    addDeedItem(): void {
        if (!this.selectedDeedType) {
            this.toastr.warning('لطفاً نوعیت سته را انتخاب کنید');
            return;
        }

        const docType = this.deedTypes.find(d => d.id === this.selectedDeedType);
        if (!docType) return;

        const newItem: DeedItem = {
            deedType: this.selectedDeedType,
            serialStart: docType.hasSerial ? '' : undefined,
            serialEnd: docType.hasSerial ? '' : undefined,
            count: 0
        };

        this.deedItems.push(newItem);
        this.selectedDeedType = null;
        this.toastr.success('سته اضافه شد');
    }

    removeDeedItem(index: number): void {
        this.deedItems.splice(index, 1);
        this.updateDeedCounts();
        this.toastr.info('سته حذف شد');
    }

    getDeedTypeName(deedType: number): string {
        const docType = this.deedTypes.find(d => d.id === deedType);
        return docType ? docType.name : '';
    }

    hasSerialNumbers(deedType: number): boolean {
        const docType = this.deedTypes.find(d => d.id === deedType);
        return docType ? docType.hasSerial : false;
    }

    updateDeedCounts(): void {
        const saleItems = this.deedItems.filter(d => d.deedType === 3);
        const rentalItems = this.deedItems.filter(d => d.deedType === 2);
        const baiUlWafaItems = this.deedItems.filter(d => d.deedType === 4);
        const vehicleItems = this.deedItems.filter(d => d.deedType === 1);

        this.mainForm.patchValue({
            saleDeedsCount: saleItems.reduce((sum, item) => sum + item.count, 0),
            rentalDeedsCount: rentalItems.reduce((sum, item) => sum + item.count, 0),
            baiUlWafaDeedsCount: baiUlWafaItems.reduce((sum, item) => sum + item.count, 0),
            vehicleTransactionDeedsCount: vehicleItems.reduce((sum, item) => sum + item.count, 0)
        }, { emitEvent: false });
    }

    onDeedCountChange(item: DeedItem): void {
        this.updateDeedCounts();
    }

    // ==================== Company Search ====================

    searchCompanyByLicense(): void {
        const licenseNumber = this.mainForm.get('licenseNumber')?.value;
        
        if (!licenseNumber || !licenseNumber.trim()) {
            return;
        }

        this.companySearching = true;
        this.companyFound = false;
        this.companyNotFound = false;
        this.selectedCompanyId = null;

        const provinceId = this.authService.getUserProvinceId();

        this.companyService.searchCompanyByLicense(licenseNumber.trim(), provinceId || undefined).subscribe({
            next: (companies) => {
                this.companySearching = false;
                
                if (companies && companies.length > 0) {
                    const company = companies[0];
                    this.companyFound = true;
                    this.companyNotFound = false;
                    this.selectedCompanyId = company.companyId || company.id;

                    this.mainForm.patchValue({
                        licenseHolderName: company.ownerName || '',
                        district: company.activityLocation || company.district || company.area || '',
                        licenseNumber: company.licenseNumber || licenseNumber
                    });

                    this.toastr.success('معلومات شرکت با موفقیت بارگذاری شد');
                } else {
                    this.companyFound = false;
                    this.companyNotFound = true;
                    this.toastr.warning('شرکت با این نمبر جواز یافت نشد');
                }
            },
            error: (err) => {
                this.companySearching = false;
                this.companyFound = false;
                this.companyNotFound = true;
                console.error('Error searching company:', err);
                this.toastr.error('خطا در جستجوی شرکت');
            }
        });
    }

    // Form control getters
    get licenseNumber() { return this.mainForm.get('licenseNumber'); }
    get licenseHolderName() { return this.mainForm.get('licenseHolderName'); }
    get district() { return this.mainForm.get('district'); }
}
