import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { ActivityMonitoringService } from 'src/app/shared/activity-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { AuthService } from 'src/app/shared/auth.service';
import { ConfirmationDialogComponent, ConfirmationDialogData } from 'src/app/shared/confirmation-dialog/confirmation-dialog.component';
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
export class ActivityMonitoringFormComponent extends BaseComponent implements OnInit {

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
    showAnnualReportSection = false;
    showComplaintsSection = false;
    showViolationsSection = false;
    showInspectionSection = false;
    selectedCompanyId: number | null = null;

    // Monitoring years and months for inspection section
    monitoringYears: string[] = [];
    monitoringMonths = [
        { value: 'حمل', label: 'حمل' },
        { value: 'ثور', label: 'ثور' },
        { value: 'جوزا', label: 'جوزا' },
        { value: 'سرطان', label: 'سرطان' },
        { value: 'اسد', label: 'اسد' },
        { value: 'سنبله', label: 'سنبله' },
        { value: 'میزان', label: 'میزان' },
        { value: 'عقرب', label: 'عقرب' },
        { value: 'قوس', label: 'قوس' },
        { value: 'جدی', label: 'جدی' },
        { value: 'دلو', label: 'دلو' },
        { value: 'حوت', label: 'حوت' }
    ];

    // RBAC
    canEdit = false;
    isViewOnly = false;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private dialog: MatDialog,
        private toastr: ToastrService,
        public service: ActivityMonitoringService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private rbacService: RbacService,
        private companyService: CompnaydetailService,
        private authService: AuthService
    ) {
        super();
        this.initForm();
    }

    ngOnInit(): void {
        this.checkPermissions();
        this.initMonitoringYears();

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

    initMonitoringYears(): void {
        const currentYear = new Date().getFullYear();
        const persianYear = currentYear - 621; // Approximate Hijri Shamsi year
        for (let i = 0; i < 10; i++) {
            this.monitoringYears.push((persianYear - i).toString());
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
            companyTitle: [''],
            district: [''],
            reportRegistrationDate: [''],
            sectionType: ['', Validators.required],
            saleDeedsCount: [''],
            rentalDeedsCount: [''],
            baiUlWafaDeedsCount: [''],
            vehicleTransactionDeedsCount: [''],
            
            // Section 2: Complaints fields (conditionally required)
            complaintSubject: [''],
            complainantName: [''],
            complaintActionsTaken: [''],
            complaintRemarks: [''],
            
            // Section 3: Violations fields (conditionally required)
            violationStatus: [''],
            violationType: [''],
            closureReason: [''],
            sealRemovalReason: [''],
            violationActionsTaken: [''],
            violationRemarks: [''],
            
            // Section 4: Inspection fields (conditionally required)
            year: [''],
            month: [''],
            monitoringCount: [''],
            monitoringRemarks: [''],
            
            // Tax Amount (for annualReport)
            taxAmount: [''],
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
        // Patch common fields
        this.mainForm.patchValue({
            id: data.id,
            serialNumber: data.serialNumber,
            licenseNumber: data.licenseNumber,
            licenseHolderName: data.licenseHolderName,
            companyTitle: data.companyTitle,
            district: data.district,
            sectionType: data.sectionType,
            saleDeedsCount: data.saleDeedsCount,
            rentalDeedsCount: data.rentalDeedsCount,
            baiUlWafaDeedsCount: data.baiUlWafaDeedsCount,
            vehicleTransactionDeedsCount: data.vehicleTransactionDeedsCount,
        });

        // Set company found state if license number exists
        if (data.licenseNumber) {
            this.companyFound = true;
        }

        // Parse dates - raw dates are already Hijri Shamsi strings from backend
        if (data.reportRegistrationDate) {
            const date = this.parseDateString(data.reportRegistrationDate);
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
                count: item.count,
                remarks: item.remarks
            }));
        }

        // Load section-specific data based on sectionType
        if (data.sectionType === 'annualReport') {
            // Annual report data is already loaded in deed items above
            this.mainForm.patchValue({
                taxAmount: data.taxAmount,
            });
        } else if (data.sectionType === 'complaints' && data.complaints && data.complaints.length > 0) {
            const complaint = data.complaints[0];
            this.mainForm.patchValue({
                complaintSubject: complaint.complaintSubject,
                complainantName: complaint.complainantName,
                complaintActionsTaken: complaint.actionsTaken,
                complaintRemarks: complaint.remarks,
            });
        } else if (data.sectionType === 'violations' && data.realEstateViolations && data.realEstateViolations.length > 0) {
            const violation = data.realEstateViolations[0];
            this.mainForm.patchValue({
                violationStatus: violation.violationStatus,
                violationType: violation.violationType,
                closureReason: violation.closureReason,
                sealRemovalReason: violation.sealRemovalReason,
                violationActionsTaken: violation.actionsTaken,
                violationRemarks: violation.remarks,
            });
            this.onViolationStatusChange();
        } else if (data.sectionType === 'inspection') {
            this.mainForm.patchValue({
                year: data.year,
                month: data.month,
                monitoringCount: data.monitoringCount,
                monitoringRemarks: data.monitoringRemarks,
            });
        }

        // Trigger section visibility based on sectionType
        this.onSectionTypeChange();
    }

    private parseDateString(dateStr: string): Date | string | null {
        if (!dateStr) return null;
        
        // If already in Hijri Shamsi format (YYYY/MM/DD), return as-is
        if (/^\d{4}\/\d{2}\/\d{2}$/.test(dateStr)) {
            return dateStr;
        }
        
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
        console.log('Section type changed to:', sectionType); // Debug log
        
        // Reset all section visibility
        this.showAnnualReportSection = false;
        this.showComplaintsSection = false;
        this.showViolationsSection = false;
        this.showInspectionSection = false;
        
        // Clear all conditional validators first
        this.clearAllConditionalValidators();
        
        // Show the selected section and set validators
        if (sectionType === 'annualReport') {
            this.showAnnualReportSection = true;
            console.log('Showing annual report section'); // Debug log
            // No additional validators needed for annual report
        } else if (sectionType === 'complaints') {
            this.showComplaintsSection = true;
            this.setComplaintValidators();
        } else if (sectionType === 'violations') {
            this.showViolationsSection = true;
            this.setViolationValidators();
        } else if (sectionType === 'inspection') {
            this.showInspectionSection = true;
            this.setInspectionValidators();
        }
        
        // Update validators for license fields based on section type
        this.updateLicenseFieldValidators();

        // Reload serial number based on selected section type (only for new records)
        if (!this.isEditMode && sectionType) {
            this.loadNextSerialNumber(sectionType);
        }
    }
    
    updateLicenseFieldValidators(): void {
        const sectionType = this.mainForm.get('sectionType')?.value;
        
        if (sectionType === 'inspection') {
            // Remove required validators for license fields when inspection is selected
            this.mainForm.get('licenseNumber')?.clearValidators();
            this.mainForm.get('licenseHolderName')?.clearValidators();
        } else {
            // Set required validators for other section types
            this.mainForm.get('licenseNumber')?.setValidators([Validators.required, Validators.maxLength(50)]);
            this.mainForm.get('licenseHolderName')?.setValidators([Validators.required, Validators.maxLength(200)]);
        }
        
        this.mainForm.get('licenseNumber')?.updateValueAndValidity();
        this.mainForm.get('licenseHolderName')?.updateValueAndValidity();
    }

    clearAllConditionalValidators(): void {
        // Complaint fields
        this.mainForm.get('complaintSubject')?.clearValidators();
        this.mainForm.get('complainantName')?.clearValidators();
        
        // Violation fields
        this.mainForm.get('violationStatus')?.clearValidators();
        this.mainForm.get('violationType')?.clearValidators();
        this.mainForm.get('closureReason')?.clearValidators();
        
        // Inspection fields
        this.mainForm.get('year')?.clearValidators();
        this.mainForm.get('month')?.clearValidators();
        this.mainForm.get('monitoringCount')?.clearValidators();
        
        // Update validity
        this.mainForm.get('complaintSubject')?.updateValueAndValidity();
        this.mainForm.get('complainantName')?.updateValueAndValidity();
        this.mainForm.get('violationStatus')?.updateValueAndValidity();
        this.mainForm.get('violationType')?.updateValueAndValidity();
        this.mainForm.get('closureReason')?.updateValueAndValidity();
        this.mainForm.get('year')?.updateValueAndValidity();
        this.mainForm.get('month')?.updateValueAndValidity();
        this.mainForm.get('monitoringCount')?.updateValueAndValidity();
    }

    setComplaintValidators(): void {
        this.mainForm.get('complaintSubject')?.setValidators([Validators.required, Validators.maxLength(500)]);
        this.mainForm.get('complainantName')?.setValidators([Validators.required, Validators.maxLength(200)]);
        
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
        this.mainForm.get('year')?.setValidators([Validators.required]);
        this.mainForm.get('month')?.setValidators([Validators.required]);
        this.mainForm.get('monitoringCount')?.setValidators([Validators.required, Validators.min(1)]);
        
        this.mainForm.get('year')?.updateValueAndValidity();
        this.mainForm.get('month')?.updateValueAndValidity();
        this.mainForm.get('monitoringCount')?.updateValueAndValidity();
    }

    onViolationStatusChange(): void {
        const status = this.mainForm.get('violationStatus')?.value;
        
        if (status === 'منجر به مسدودی') {
            // Leading to Closure - require closure fields
            this.mainForm.get('closureReason')?.setValidators([Validators.required, Validators.maxLength(500)]);
            this.mainForm.get('violationType')?.clearValidators();
        } else if (status === 'عادی') {
            // Normal - require violation fields
            this.mainForm.get('violationType')?.setValidators([Validators.required, Validators.maxLength(500)]);
            this.mainForm.get('closureReason')?.clearValidators();
        } else if (status === 'رفع مهرلاک') {
            // Lifting Seal - require seal removal reason fields
            this.mainForm.get('sealRemovalReason')?.setValidators([Validators.required, Validators.maxLength(500)]);
            this.mainForm.get('violationType')?.clearValidators();
            this.mainForm.get('closureReason')?.clearValidators();
        } else {
            this.mainForm.get('violationType')?.clearValidators();
            this.mainForm.get('closureReason')?.clearValidators();
        }

        this.mainForm.get('violationType')?.updateValueAndValidity();
        this.mainForm.get('closureReason')?.updateValueAndValidity();
        this.mainForm.get('sealRemovalReason')?.updateValueAndValidity();
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
        const excludeId = this.isEditMode ? this.editId : undefined;

        // Check for duplicate license number + status in violations section
        if (sectionType === 'violations' && formValue.licenseNumber && formValue.violationStatus) {
            this.service.checkDuplicateLicense(formValue.licenseNumber, excludeId ?? undefined, 'violations', formValue.violationStatus).subscribe({
                next: (licenseResult) => {
                    if (licenseResult.count > 0) {
                        const dialogData: ConfirmationDialogData = {
                            title: 'هشدار',
                            message: `نمبر جواز "${formValue.licenseNumber}" با وضعیت "${formValue.violationStatus}" قبلاً ${licenseResult.count} بار ثبت شده است. آیا می‌خواهید ادامه دهید؟`,
                            icon: 'fa-exclamation-triangle',
                            confirmText: 'بله، ادامه دهید',
                            cancelText: 'انصراف'
                        };
                        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
                            data: dialogData,
                            disableClose: true
                        });
                        dialogRef.afterClosed().subscribe(confirmed => {
                            if (confirmed) {
                                this.proceedSave(formValue, currentCalendar, sectionType);
                            }
                        });
                    } else {
                        this.proceedSave(formValue, currentCalendar, sectionType);
                    }
                },
                error: () => {
                    this.proceedSave(formValue, currentCalendar, sectionType);
                }
            });
            return;
        }

        // Check for duplicate license number (complaints section only)
        if (sectionType === 'complaints' && formValue.licenseNumber) {
            this.service.checkDuplicateLicense(formValue.licenseNumber, excludeId ?? undefined).subscribe({
                next: (licenseResult) => {
                    if (licenseResult.count > 0) {
                        const dialogData: ConfirmationDialogData = {
                            title: 'هشدار',
                            message: `نمبر جواز "${formValue.licenseNumber}" قبلاً ${licenseResult.count} بار ثبت شده است. آیا می‌خواهید ادامه دهید؟`,
                            icon: 'fa-exclamation-triangle',
                            confirmText: 'بله، ادامه دهید',
                            cancelText: 'انصراف'
                        };
                        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
                            data: dialogData,
                            disableClose: true
                        });
                        dialogRef.afterClosed().subscribe(confirmed => {
                            if (confirmed) {
                                this.checkComplainantDuplicate(formValue, currentCalendar, sectionType, excludeId);
                            }
                        });
                    } else {
                        this.checkComplainantDuplicate(formValue, currentCalendar, sectionType, excludeId);
                    }
                },
                error: () => {
                    this.checkComplainantDuplicate(formValue, currentCalendar, sectionType, excludeId);
                }
            });
            return;
        }

        this.checkComplainantDuplicate(formValue, currentCalendar, sectionType, excludeId);
    }

    private checkComplainantDuplicate(formValue: any, currentCalendar: string, sectionType: string, excludeId: number | null | undefined): void {
        // Check for duplicate complainant name in complaints section
        if (sectionType === 'complaints' && formValue.complainantName) {
            this.service.checkDuplicateComplainant(formValue.complainantName, excludeId ?? undefined).subscribe({
                next: (result) => {
                    if (result.count > 0) {
                        const dialogData: ConfirmationDialogData = {
                            title: 'هشدار',
                            message: `شهرت عارض "${formValue.complainantName}" قبلاً ${result.count} بار ثبت شده است. آیا می‌خواهید ادامه دهید؟`,
                            icon: 'fa-exclamation-triangle',
                            confirmText: 'بله، ادامه دهید',
                            cancelText: 'انصراف'
                        };
                        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
                            data: dialogData,
                            disableClose: true
                        });
                        dialogRef.afterClosed().subscribe(confirmed => {
                            if (confirmed) {
                                this.proceedSave(formValue, currentCalendar, sectionType);
                            }
                        });
                    } else {
                        this.proceedSave(formValue, currentCalendar, sectionType);
                    }
                },
                error: () => {
                    this.proceedSave(formValue, currentCalendar, sectionType);
                }
            });
            return;
        }

        this.proceedSave(formValue, currentCalendar, sectionType);
    }

    private proceedSave(formValue: any, currentCalendar: string, sectionType: string): void {
        // Build data for single table design - all fields in one object
        const data: ActivityMonitoringData = {
            id: formValue.id,
            serialNumber: this.mainForm.getRawValue().serialNumber,
            sectionType: sectionType,
            licenseNumber: formValue.licenseNumber,
            licenseHolderName: formValue.licenseHolderName,
            companyTitle: formValue.companyTitle,
            district: formValue.district,
            reportRegistrationDate: this.formatDateForBackend(formValue.reportRegistrationDate),
            saleDeedsCount: formValue.saleDeedsCount ? Number(formValue.saleDeedsCount) : undefined,
            rentalDeedsCount: formValue.rentalDeedsCount ? Number(formValue.rentalDeedsCount) : undefined,
            baiUlWafaDeedsCount: formValue.baiUlWafaDeedsCount ? Number(formValue.baiUlWafaDeedsCount) : undefined,
            vehicleTransactionDeedsCount: formValue.vehicleTransactionDeedsCount ? Number(formValue.vehicleTransactionDeedsCount) : undefined,
            deedItems: this.deedItems,
            calendarType: currentCalendar,
            
            // Complaints fields (directly on data)
            complaintSubject: formValue.complaintSubject,
            complainantName: formValue.complainantName,
            complaintActionsTaken: formValue.complaintActionsTaken,
            complaintRemarks: formValue.complaintRemarks,
            
            // Violations fields (directly on data)
            violationStatus: formValue.violationStatus,
            violationType: formValue.violationType,
            closureReason: formValue.closureReason,
            sealRemovalReason: formValue.sealRemovalReason,
            violationActionsTaken: formValue.violationActionsTaken,
            violationRemarks: formValue.violationRemarks,
            
            // Inspections fields (directly on data)
            year: formValue.year,
            month: formValue.month,
            monitoringCount: formValue.monitoringCount ? Number(formValue.monitoringCount) : undefined,
            monitoringRemarks: formValue.monitoringRemarks,
            
            // Tax Amount (for annualReport)
            taxAmount: formValue.taxAmount ? Number(formValue.taxAmount) : undefined,
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
        this.showAnnualReportSection = false;
        this.showComplaintsSection = false;
        this.showViolationsSection = false;
        this.showInspectionSection = false;
        // Reload serial number after reset
        this.loadNextSerialNumber();
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
            count: 0,
            remarks: ''
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
                        licenseNumber: company.licenseNumber || licenseNumber,
                        companyTitle: company.companyTitle || company.companyName || ''
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
