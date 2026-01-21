import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ActivityMonitoringService } from 'src/app/shared/activity-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import {
    ActivityMonitoringData,
    ComplaintData,
    RealEstateViolationData,
    PetitionWriterViolationData,
    Complaint,
    RealEstateViolation,
    PetitionWriterViolation
} from 'src/app/models/ActivityMonitoring';

@Component({
    selector: 'app-activity-monitoring-form',
    templateUrl: './activity-monitoring-form.component.html',
    styleUrls: ['./activity-monitoring-form.component.scss'],
})
export class ActivityMonitoringFormComponent implements OnInit {

    // Forms
    mainForm!: FormGroup;
    complaintForm!: FormGroup;
    realEstateViolationForm!: FormGroup;
    petitionWriterViolationForm!: FormGroup;

    isEditMode = false;
    editId: number | null = null;

    // Lists
    complaintsList: Complaint[] = [];
    realEstateViolationsList: RealEstateViolation[] = [];
    petitionWriterViolationsList: PetitionWriterViolation[] = [];

    selectedComplaintId: number = 0;
    selectedRealEstateViolationId: number = 0;
    selectedPetitionWriterViolationId: number = 0;

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
        private rbacService: RbacService
    ) {
        this.initForms();
    }

    ngOnInit(): void {
        this.checkPermissions();

        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isEditMode = true;
            this.editId = parseInt(id, 10);
            this.service.mainTableId = this.editId;
            this.loadData(this.editId);
        }
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.isViewOnly = role === UserRoles.Authority || role === UserRoles.LicenseReviewer;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar;
    }

    initForms(): void {
        // Main Form (Sections 1, 2, 6)
        this.mainForm = this.fb.group({
            id: [null],
            // Section 1: Financial Clearance
            licenseHolderName: ['', [Validators.required, Validators.maxLength(200)]],
            taxClearanceStatus: [''],
            taxClearanceLetterNumber: [''],
            taxClearanceDate: ['', Validators.required],
            paidTaxAmount: [''],
            // Section 2: Annual Report
            reportRegistrationDate: [''],
            saleDeedsCount: [''],
            rentalDeedsCount: [''],
            baiUlWafaDeedsCount: [''],
            vehicleTransactionDeedsCount: [''],
            cancelledMixedTransactions: [''],
            lostDeedsCount: [''],
            annualReportRemarks: [''],
            // Section 6: Inspection
            inspectionDate: ['', Validators.required],
            inspectedRealEstateOfficesCount: [''],
            sealedOfficesCount: [''],
            inspectedPetitionWritersCount: [''],
            violatingPetitionWritersCount: [''],
        });

        // Complaint Form (Section 3)
        this.complaintForm = this.fb.group({
            id: [0],
            complaintSerialNumber: ['', Validators.required],
            complainantName: ['', Validators.required],
            complaintSubject: ['', Validators.required],
            complaintRegistrationDate: ['', Validators.required],
            accusedPartyName: ['', Validators.required],
            actionsTaken: [''],
            remarks: [''],
        });

        // Real Estate Violation Form (Section 4)
        this.realEstateViolationForm = this.fb.group({
            id: [0],
            violationSerialNumber: ['', Validators.required],
            licenseHolderName: ['', Validators.required],
            violationType: ['', Validators.required],
            violationDate: ['', Validators.required],
            actionsTaken: [''],
            remarks: [''],
        });

        // Petition Writer Violation Form (Section 5)
        this.petitionWriterViolationForm = this.fb.group({
            id: [0],
            violationSerialNumber: ['', Validators.required],
            petitionWriterName: ['', Validators.required],
            violationType: ['', Validators.required],
            violationDate: ['', Validators.required],
            actionsTaken: [''],
            remarks: [''],
        });
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.service.getById(id, calendar).subscribe({
            next: (data) => {
                this.patchMainForm(data);
                this.loadComplaints();
                this.loadRealEstateViolations();
                this.loadPetitionWriterViolations();
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });
    }

    patchMainForm(data: any): void {
        this.mainForm.patchValue({
            id: data.id,
            licenseHolderName: data.licenseHolderName,
            taxClearanceStatus: data.taxClearanceStatus,
            taxClearanceLetterNumber: data.taxClearanceLetterNumber,
            paidTaxAmount: data.paidTaxAmount,
            saleDeedsCount: data.saleDeedsCount,
            rentalDeedsCount: data.rentalDeedsCount,
            baiUlWafaDeedsCount: data.baiUlWafaDeedsCount,
            vehicleTransactionDeedsCount: data.vehicleTransactionDeedsCount,
            cancelledMixedTransactions: data.cancelledMixedTransactions,
            lostDeedsCount: data.lostDeedsCount,
            annualReportRemarks: data.annualReportRemarks,
            inspectedRealEstateOfficesCount: data.inspectedRealEstateOfficesCount,
            sealedOfficesCount: data.sealedOfficesCount,
            inspectedPetitionWritersCount: data.inspectedPetitionWritersCount,
            violatingPetitionWritersCount: data.violatingPetitionWritersCount,
        });

        // Parse dates - convert formatted dates to Date objects
        if (data.taxClearanceDateFormatted) {
            const date = this.parseDateString(data.taxClearanceDateFormatted);
            if (date) {
                this.mainForm.patchValue({ taxClearanceDate: date });
            }
        }
        if (data.reportRegistrationDateFormatted) {
            const date = this.parseDateString(data.reportRegistrationDateFormatted);
            if (date) {
                this.mainForm.patchValue({ reportRegistrationDate: date });
            }
        }
        if (data.inspectionDateFormatted) {
            const date = this.parseDateString(data.inspectionDateFormatted);
            if (date) {
                this.mainForm.patchValue({ inspectionDate: date });
            }
        }
    }

    private parseDateString(dateStr: string): Date | null {
        if (!dateStr) return null;
        const calendar = this.calendarService.getSelectedCalendar();
        return this.calendarConversionService.parseInputDate(dateStr, calendar);
    }

    loadComplaints(): void {
        if (!this.service.mainTableId) return;
        const calendar = this.calendarService.getSelectedCalendar();
        this.service.getComplaints(this.service.mainTableId, calendar).subscribe({
            next: (data) => {
                this.complaintsList = data;
            },
            error: (err) => console.error(err)
        });
    }

    loadRealEstateViolations(): void {
        if (!this.service.mainTableId) return;
        const calendar = this.calendarService.getSelectedCalendar();
        this.service.getRealEstateViolations(this.service.mainTableId, calendar).subscribe({
            next: (data) => {
                this.realEstateViolationsList = data;
            },
            error: (err) => console.error(err)
        });
    }

    loadPetitionWriterViolations(): void {
        if (!this.service.mainTableId) return;
        const calendar = this.calendarService.getSelectedCalendar();
        this.service.getPetitionWriterViolations(this.service.mainTableId, calendar).subscribe({
            next: (data) => {
                this.petitionWriterViolationsList = data;
            },
            error: (err) => console.error(err)
        });
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

    // ==================== Main Form CRUD ====================

    saveMainForm(): void {
        if (this.mainForm.invalid) {
            this.toastr.warning('لطفا تمام فیلدهای الزامی را پر کنید');
            return;
        }

        const formValue = this.mainForm.value;
        const currentCalendar = this.calendarService.getSelectedCalendar();

        const data: ActivityMonitoringData = {
            id: formValue.id,
            licenseHolderName: formValue.licenseHolderName,
            taxClearanceStatus: formValue.taxClearanceStatus,
            taxClearanceLetterNumber: formValue.taxClearanceLetterNumber,
            taxClearanceDate: this.formatDateForBackend(formValue.taxClearanceDate),
            paidTaxAmount: formValue.paidTaxAmount ? Number(formValue.paidTaxAmount) : undefined,
            reportRegistrationDate: this.formatDateForBackend(formValue.reportRegistrationDate),
            saleDeedsCount: formValue.saleDeedsCount ? Number(formValue.saleDeedsCount) : undefined,
            rentalDeedsCount: formValue.rentalDeedsCount ? Number(formValue.rentalDeedsCount) : undefined,
            baiUlWafaDeedsCount: formValue.baiUlWafaDeedsCount ? Number(formValue.baiUlWafaDeedsCount) : undefined,
            vehicleTransactionDeedsCount: formValue.vehicleTransactionDeedsCount ? Number(formValue.vehicleTransactionDeedsCount) : undefined,
            cancelledMixedTransactions: formValue.cancelledMixedTransactions ? Number(formValue.cancelledMixedTransactions) : undefined,
            lostDeedsCount: formValue.lostDeedsCount ? Number(formValue.lostDeedsCount) : undefined,
            annualReportRemarks: formValue.annualReportRemarks,
            inspectionDate: this.formatDateForBackend(formValue.inspectionDate),
            inspectedRealEstateOfficesCount: formValue.inspectedRealEstateOfficesCount ? Number(formValue.inspectedRealEstateOfficesCount) : undefined,
            sealedOfficesCount: formValue.sealedOfficesCount ? Number(formValue.sealedOfficesCount) : undefined,
            inspectedPetitionWritersCount: formValue.inspectedPetitionWritersCount ? Number(formValue.inspectedPetitionWritersCount) : undefined,
            violatingPetitionWritersCount: formValue.violatingPetitionWritersCount ? Number(formValue.violatingPetitionWritersCount) : undefined,
            calendarType: currentCalendar
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
                },
                error: (err) => {
                    this.toastr.error('خطا در ذخیره معلومات');
                    console.error(err);
                }
            });
        }
    }

    // ==================== Complaint CRUD ====================

    saveComplaint(): void {
        if (this.complaintForm.invalid) {
            this.toastr.warning('لطفا تمام فیلدهای الزامی را پر کنید');
            return;
        }

        if (!this.service.mainTableId) {
            this.toastr.error('لطفا ابتدا معلومات اصلی را ثبت کنید');
            return;
        }

        const formValue = this.complaintForm.value;
        const currentCalendar = this.calendarService.getSelectedCalendar();

        const data: ComplaintData = {
            id: formValue.id || 0,
            activityMonitoringRecordId: this.service.mainTableId,
            complaintSerialNumber: formValue.complaintSerialNumber,
            complainantName: formValue.complainantName,
            complaintSubject: formValue.complaintSubject,
            complaintRegistrationDate: this.formatDateForBackend(formValue.complaintRegistrationDate),
            accusedPartyName: formValue.accusedPartyName,
            actionsTaken: formValue.actionsTaken,
            remarks: formValue.remarks,
            calendarType: currentCalendar
        };

        if (this.selectedComplaintId && this.selectedComplaintId > 0) {
            data.id = this.selectedComplaintId;
            this.service.updateComplaint(data).subscribe({
                next: () => {
                    this.toastr.success('شکایت موفقانه تغییر یافت');
                    this.loadComplaints();
                    this.resetComplaintForm();
                },
                error: (err) => {
                    this.toastr.error('خطا در تغییر معلومات');
                    console.error(err);
                }
            });
        } else {
            this.service.addComplaint(data).subscribe({
                next: () => {
                    this.toastr.success('شکایت موفقانه ثبت شد');
                    this.loadComplaints();
                    this.resetComplaintForm();
                },
                error: (err) => {
                    this.toastr.error('خطا در ثبت معلومات');
                    console.error(err);
                }
            });
        }
    }

    selectComplaint(complaint: Complaint): void {
        this.selectedComplaintId = complaint.id || 0;
        this.complaintForm.patchValue({
            id: complaint.id,
            complaintSerialNumber: complaint.complaintSerialNumber,
            complainantName: complaint.complainantName,
            complaintSubject: complaint.complaintSubject,
            accusedPartyName: complaint.accusedPartyName,
            actionsTaken: complaint.actionsTaken,
            remarks: complaint.remarks,
        });

        if (complaint.complaintRegistrationDateFormatted) {
            const date = this.parseDateString(complaint.complaintRegistrationDateFormatted);
            if (date) {
                this.complaintForm.patchValue({ complaintRegistrationDate: date });
            }
        }
    }

    resetComplaintForm(): void {
        this.selectedComplaintId = 0;
        this.complaintForm.reset();
        this.complaintForm.patchValue({ id: 0 });
    }

    // ==================== Real Estate Violation CRUD ====================

    saveRealEstateViolation(): void {
        if (this.realEstateViolationForm.invalid) {
            this.toastr.warning('لطفا تمام فیلدهای الزامی را پر کنید');
            return;
        }

        if (!this.service.mainTableId) {
            this.toastr.error('لطفا ابتدا معلومات اصلی را ثبت کنید');
            return;
        }

        const formValue = this.realEstateViolationForm.value;
        const currentCalendar = this.calendarService.getSelectedCalendar();

        const data: RealEstateViolationData = {
            id: formValue.id || 0,
            activityMonitoringRecordId: this.service.mainTableId,
            violationSerialNumber: formValue.violationSerialNumber,
            licenseHolderName: formValue.licenseHolderName,
            violationType: formValue.violationType,
            violationDate: this.formatDateForBackend(formValue.violationDate),
            actionsTaken: formValue.actionsTaken,
            remarks: formValue.remarks,
            calendarType: currentCalendar
        };

        if (this.selectedRealEstateViolationId && this.selectedRealEstateViolationId > 0) {
            data.id = this.selectedRealEstateViolationId;
            this.service.updateRealEstateViolation(data).subscribe({
                next: () => {
                    this.toastr.success('تخلف موفقانه تغییر یافت');
                    this.loadRealEstateViolations();
                    this.resetRealEstateViolationForm();
                },
                error: (err) => {
                    this.toastr.error('خطا در تغییر معلومات');
                    console.error(err);
                }
            });
        } else {
            this.service.addRealEstateViolation(data).subscribe({
                next: () => {
                    this.toastr.success('تخلف موفقانه ثبت شد');
                    this.loadRealEstateViolations();
                    this.resetRealEstateViolationForm();
                },
                error: (err) => {
                    this.toastr.error('خطا در ثبت معلومات');
                    console.error(err);
                }
            });
        }
    }

    selectRealEstateViolation(violation: RealEstateViolation): void {
        this.selectedRealEstateViolationId = violation.id || 0;
        this.realEstateViolationForm.patchValue({
            id: violation.id,
            violationSerialNumber: violation.violationSerialNumber,
            licenseHolderName: violation.licenseHolderName,
            violationType: violation.violationType,
            actionsTaken: violation.actionsTaken,
            remarks: violation.remarks,
        });

        if (violation.violationDateFormatted) {
            const date = this.parseDateString(violation.violationDateFormatted);
            if (date) {
                this.realEstateViolationForm.patchValue({ violationDate: date });
            }
        }
    }

    resetRealEstateViolationForm(): void {
        this.selectedRealEstateViolationId = 0;
        this.realEstateViolationForm.reset();
        this.realEstateViolationForm.patchValue({ id: 0 });
    }

    // ==================== Petition Writer Violation CRUD ====================

    savePetitionWriterViolation(): void {
        if (this.petitionWriterViolationForm.invalid) {
            this.toastr.warning('لطفا تمام فیلدهای الزامی را پر کنید');
            return;
        }

        if (!this.service.mainTableId) {
            this.toastr.error('لطفا ابتدا معلومات اصلی را ثبت کنید');
            return;
        }

        const formValue = this.petitionWriterViolationForm.value;
        const currentCalendar = this.calendarService.getSelectedCalendar();

        const data: PetitionWriterViolationData = {
            id: formValue.id || 0,
            activityMonitoringRecordId: this.service.mainTableId,
            violationSerialNumber: formValue.violationSerialNumber,
            petitionWriterName: formValue.petitionWriterName,
            violationType: formValue.violationType,
            violationDate: this.formatDateForBackend(formValue.violationDate),
            actionsTaken: formValue.actionsTaken,
            remarks: formValue.remarks,
            calendarType: currentCalendar
        };

        if (this.selectedPetitionWriterViolationId && this.selectedPetitionWriterViolationId > 0) {
            data.id = this.selectedPetitionWriterViolationId;
            this.service.updatePetitionWriterViolation(data).subscribe({
                next: () => {
                    this.toastr.success('تخلف موفقانه تغییر یافت');
                    this.loadPetitionWriterViolations();
                    this.resetPetitionWriterViolationForm();
                },
                error: (err) => {
                    this.toastr.error('خطا در تغییر معلومات');
                    console.error(err);
                }
            });
        } else {
            this.service.addPetitionWriterViolation(data).subscribe({
                next: () => {
                    this.toastr.success('تخلف موفقانه ثبت شد');
                    this.loadPetitionWriterViolations();
                    this.resetPetitionWriterViolationForm();
                },
                error: (err) => {
                    this.toastr.error('خطا در ثبت معلومات');
                    console.error(err);
                }
            });
        }
    }

    selectPetitionWriterViolation(violation: PetitionWriterViolation): void {
        this.selectedPetitionWriterViolationId = violation.id || 0;
        this.petitionWriterViolationForm.patchValue({
            id: violation.id,
            violationSerialNumber: violation.violationSerialNumber,
            petitionWriterName: violation.petitionWriterName,
            violationType: violation.violationType,
            actionsTaken: violation.actionsTaken,
            remarks: violation.remarks,
        });

        if (violation.violationDateFormatted) {
            const date = this.parseDateString(violation.violationDateFormatted);
            if (date) {
                this.petitionWriterViolationForm.patchValue({ violationDate: date });
            }
        }
    }

    resetPetitionWriterViolationForm(): void {
        this.selectedPetitionWriterViolationId = 0;
        this.petitionWriterViolationForm.reset();
        this.petitionWriterViolationForm.patchValue({ id: 0 });
    }

    // ==================== Navigation ====================

    goToList(): void {
        this.router.navigate(['/activity-monitoring/list']);
    }

    resetAll(): void {
        this.service.resetMainTableId();
        this.mainForm.reset();
        this.complaintForm.reset();
        this.realEstateViolationForm.reset();
        this.petitionWriterViolationForm.reset();
        this.complaintsList = [];
        this.realEstateViolationsList = [];
        this.petitionWriterViolationsList = [];
        this.isEditMode = false;
        this.editId = null;
        this.selectedComplaintId = 0;
        this.selectedRealEstateViolationId = 0;
        this.selectedPetitionWriterViolationId = 0;
    }
}
