import { Component, Injectable, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PetitionWriterLicenseService } from 'src/app/shared/petition-writer-license.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarType } from 'src/app/models/calendar-type';
import { SellerService } from 'src/app/shared/seller.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import {
    PetitionWriterLicenseData,
    PetitionWriterRelocationData,
    PetitionWriterRelocation,
    LicenseStatusEnum,
    LicenseStatusTypes,
    LicenseTypes,
    CompetencyTypes
} from 'src/app/models/PetitionWriterLicense';

@Component({
    selector: 'app-petition-writer-license-form',
    templateUrl: './petition-writer-license-form.component.html',
    styleUrls: ['./petition-writer-license-form.component.scss'],
})
export class PetitionWriterLicenseFormComponent implements OnInit {

    // Forms
    licenseForm!: FormGroup;
    financialForm!: FormGroup;
    cancellationForm!: FormGroup;
    relocationForm!: FormGroup;

    isEditMode = false;
    editId: number | null = null;

    // Dropdown data
    provinces: any[] = [];
    permanentDistricts: any[] = [];
    currentDistricts: any[] = [];
    licenseStatusTypes = LicenseStatusTypes;
    licenseTypes = LicenseTypes;
    competencyTypes = CompetencyTypes;

    // Relocation list
    relocationsList: PetitionWriterRelocation[] = [];
    selectedRelocationId: number = 0;

    // Picture upload
    imageName: string = '';

    // RBAC
    canEdit = false;
    isViewOnly = false;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        public licenseService: PetitionWriterLicenseService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private sellerService: SellerService,
        private rbacService: RbacService
    ) { }

    ngOnInit(): void {
        this.checkPermissions();
        this.initForms();
        this.loadProvinces();

        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isEditMode = true;
            this.editId = +id;
            this.licenseService.mainTableId = this.editId;
            this.loadLicenseData(this.editId);
        }
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.isViewOnly = role === UserRoles.Authority || role === UserRoles.LicenseReviewer;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar;
    }

    initForms(): void {
        // Tab 1: مشخصات عریضه‌نویس
        this.licenseForm = this.fb.group({
            provinceId: [null, Validators.required],
            licenseNumber: [''],
            applicantName: ['', Validators.required],
            applicantFatherName: [''],
            applicantGrandFatherName: [''],
            mobileNumber: [''],
            competency: [null],
            electronicNationalIdNumber: ['', Validators.required],
            permanentProvinceId: [null],
            permanentDistrictId: [null],
            permanentVillage: [''],
            currentProvinceId: [null],
            currentDistrictId: [null],
            currentVillage: [''],
            detailedAddress: ['']
        });

        // Tab 2: ثبت مالیه و مشخصات جواز
        this.financialForm = this.fb.group({
            bankReceiptNumber: [''],
            bankReceiptDate: [null],
            district: [''],
            licenseType: [null],
            licensePrice: [{ value: null, disabled: true }],
            licenseIssueDate: [null],
            licenseExpiryDate: [null]
        });

        // Tab 3: لغو / انصراف
        this.cancellationForm = this.fb.group({
            licenseStatus: [LicenseStatusEnum.Active],
            cancellationDate: [null]
        });

        // Relocation form
        this.relocationForm = this.fb.group({
            newActivityLocation: ['', Validators.required],
            relocationDate: [null],
            remarks: ['']
        });

        // Subscribe to license type changes
        this.financialForm.get('licenseType')?.valueChanges.subscribe(() => {
            this.onLicenseTypeChange();
        });

        // Subscribe to license issue date changes
        this.financialForm.get('licenseIssueDate')?.valueChanges.subscribe(() => {
            this.onLicenseIssueDateChange();
        });
    }

    loadProvinces(): void {
        this.licenseService.getProvinces().subscribe({
            next: (data: any) => {
                this.provinces = data;
            },
            error: (err: any) => console.error('Error loading provinces', err)
        });
    }

    onPermanentProvinceChange(event: any): void {
        if (event?.id) {
            this.sellerService.getdistrict(event.id).subscribe({
                next: (data: any) => {
                    this.permanentDistricts = data;
                    this.licenseForm.patchValue({ permanentDistrictId: null });
                }
            });
        } else {
            this.permanentDistricts = [];
        }
    }

    onCurrentProvinceChange(event: any): void {
        if (event?.id) {
            this.sellerService.getdistrict(event.id).subscribe({
                next: (data: any) => {
                    this.currentDistricts = data;
                    this.licenseForm.patchValue({ currentDistrictId: null });
                }
            });
        } else {
            this.currentDistricts = [];
        }
    }

    loadLicenseData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.licenseService.getById(id, calendar).subscribe({
            next: (data) => {
                // Patch license form
                this.licenseForm.patchValue({
                    provinceId: data.provinceId,
                    licenseNumber: data.licenseNumber,
                    applicantName: data.applicantName,
                    applicantFatherName: data.applicantFatherName,
                    applicantGrandFatherName: data.applicantGrandFatherName,
                    mobileNumber: data.mobileNumber,
                    competency: data.competency,
                    electronicNationalIdNumber: data.electronicNationalIdNumber,
                    permanentProvinceId: data.permanentProvinceId,
                    permanentDistrictId: data.permanentDistrictId,
                    permanentVillage: data.permanentVillage,
                    currentProvinceId: data.currentProvinceId,
                    currentDistrictId: data.currentDistrictId,
                    currentVillage: data.currentVillage,
                    detailedAddress: data.detailedAddress
                });

                // Set imageName from existing picturePath
                this.imageName = data.picturePath || '';

                // Load districts
                if (data.permanentProvinceId) {
                    this.sellerService.getdistrict(data.permanentProvinceId).subscribe((d: any) => {
                        this.permanentDistricts = d;
                        this.licenseForm.patchValue({ permanentDistrictId: data.permanentDistrictId });
                    });
                }
                if (data.currentProvinceId) {
                    this.sellerService.getdistrict(data.currentProvinceId).subscribe((d: any) => {
                        this.currentDistricts = d;
                        this.licenseForm.patchValue({ currentDistrictId: data.currentDistrictId });
                    });
                }

                // Patch financial form - use the same pattern as license details
                this.financialForm.patchValue({
                    bankReceiptNumber: data.bankReceiptNumber,
                    bankReceiptDate: data.bankReceiptDate,
                    district: data.district,
                    licenseType: data.licenseType,
                    licensePrice: data.licensePrice,
                    licenseIssueDate: data.licenseIssueDate,
                    licenseExpiryDate: data.licenseExpiryDate
                });

                // Patch cancellation form
                this.cancellationForm.patchValue({
                    licenseStatus: data.licenseStatus,
                    cancellationDate: data.cancellationDate
                });

                // Load relocations
                this.loadRelocations();
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });
    }

    // Form getters
    get licenseNumber() { return this.licenseForm.get('licenseNumber'); }
    get applicantName() { return this.licenseForm.get('applicantName'); }

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

    saveLicense(): void {
        if (this.licenseForm.invalid) {
            this.toastr.warning('لطفا فیلدهای الزامی را تکمیل نمایید');
            return;
        }

        const calendar = this.calendarService.getSelectedCalendar();
        const formData = this.licenseForm.value;
        const financialData = this.financialForm.value;

        // Get date values
        const bankReceiptDateValue = this.financialForm.get('bankReceiptDate')?.value;
        const licenseIssueDateValue = this.financialForm.get('licenseIssueDate')?.value;
        const licenseExpiryDateValue = this.financialForm.get('licenseExpiryDate')?.value;
        const cancellationDateValue = this.cancellationForm.get('cancellationDate')?.value;

        // Format dates for backend
        const bankReceiptDate = bankReceiptDateValue ? this.formatDateForBackend(bankReceiptDateValue) : undefined;
        const licenseIssueDate = licenseIssueDateValue ? this.formatDateForBackend(licenseIssueDateValue) : undefined;
        const licenseExpiryDate = licenseExpiryDateValue ? this.formatDateForBackend(licenseExpiryDateValue) : undefined;
        const cancellationDate = cancellationDateValue ? this.formatDateForBackend(cancellationDateValue) : undefined;

        const data: PetitionWriterLicenseData = {
            ...formData,
            provinceId: formData.provinceId,
            licenseNumber: formData.licenseNumber || undefined,
            mobileNumber: formData.mobileNumber,
            competency: formData.competency,
            detailedAddress: formData.detailedAddress,
            bankReceiptNumber: financialData.bankReceiptNumber,
            bankReceiptDate,
            district: financialData.district,
            licenseType: financialData.licenseType,
            licensePrice: financialData.licensePrice,
            licenseIssueDate,
            licenseExpiryDate,
            licenseStatus: this.cancellationForm.value.licenseStatus,
            cancellationDate,
            picturePath: this.imageName,
            calendarType: calendar
        };

        if (this.isEditMode && this.editId) {
            this.licenseService.update(this.editId, data).subscribe({
                next: () => {
                    this.toastr.success('معلومات با موفقیت تغییر یافت');
                },
                error: (err) => {
                    this.toastr.error('خطا در تغییر معلومات');
                    console.error(err);
                }
            });
        } else {
            this.licenseService.create(data).subscribe({
                next: (result) => {
                    this.toastr.success('معلومات با موفقیت ثبت شد');
                    this.isEditMode = true;
                    this.editId = result.id!;
                    this.licenseService.mainTableId = this.editId;
                },
                error: (err) => {
                    this.toastr.error('خطا در ثبت معلومات');
                    console.error(err);
                }
            });
        }
    }

    // ==================== Relocation Methods ====================

    loadRelocations(): void {
        if (!this.licenseService.mainTableId) return;
        const calendar = this.calendarService.getSelectedCalendar();
        this.licenseService.getRelocations(this.licenseService.mainTableId, calendar).subscribe({
            next: (data) => {
                this.relocationsList = data;
            },
            error: (err) => console.error('Error loading relocations', err)
        });
    }

    saveRelocation(): void {
        if (this.relocationForm.invalid) {
            this.toastr.warning('لطفا محل فعالیت جدید را وارد نمایید');
            return;
        }

        const calendar = this.calendarService.getSelectedCalendar();
        const formData = this.relocationForm.value;
        const relocationDateValue = this.relocationForm.get('relocationDate')?.value;

        const data: PetitionWriterRelocationData = {
            newActivityLocation: formData.newActivityLocation,
            relocationDate: relocationDateValue ? this.formatDateForBackend(relocationDateValue) : undefined,
            remarks: formData.remarks,
            calendarType: calendar
        };

        if (this.selectedRelocationId) {
            this.licenseService.updateRelocation(this.licenseService.mainTableId, this.selectedRelocationId, data).subscribe({
                next: () => {
                    this.toastr.success('نقل مکان با موفقیت تغییر یافت');
                    this.resetRelocationForm();
                    this.loadRelocations();
                },
                error: (err) => {
                    this.toastr.error('خطا در تغییر نقل مکان');
                    console.error(err);
                }
            });
        } else {
            this.licenseService.createRelocation(this.licenseService.mainTableId, data).subscribe({
                next: () => {
                    this.toastr.success('نقل مکان با موفقیت ثبت شد');
                    this.resetRelocationForm();
                    this.loadRelocations();
                },
                error: (err) => {
                    this.toastr.error('خطا در ثبت نقل مکان');
                    console.error(err);
                }
            });
        }
    }

    selectRelocation(r: PetitionWriterRelocation): void {
        this.selectedRelocationId = r.id!;
        this.relocationForm.patchValue({
            newActivityLocation: r.newActivityLocation,
            relocationDate: r.relocationDate,
            remarks: r.remarks
        });
    }

    resetRelocationForm(): void {
        this.selectedRelocationId = 0;
        this.relocationForm.reset();
    }

    deleteRelocation(id: number): void {
        if (!confirm('آیا مطمئن هستید؟')) return;
        this.licenseService.deleteRelocation(this.licenseService.mainTableId, id).subscribe({
            next: () => {
                this.toastr.success('نقل مکان حذف شد');
                this.loadRelocations();
            },
            error: (err) => {
                this.toastr.error('خطا در حذف');
                console.error(err);
            }
        });
    }

    // ==================== Status Update ====================

    updateStatus(): void {
        if (!this.licenseService.mainTableId) return;

        const calendar = this.calendarService.getSelectedCalendar();
        const status = this.cancellationForm.value.licenseStatus;
        const cancellationDateValue = this.cancellationForm.get('cancellationDate')?.value;
        const cancellationDate = cancellationDateValue ? this.formatDateForBackend(cancellationDateValue) : undefined;

        this.licenseService.updateStatus(this.licenseService.mainTableId, status, cancellationDate, calendar).subscribe({
            next: () => {
                this.toastr.success('وضعیت با موفقیت تغییر یافت');
            },
            error: (err) => {
                this.toastr.error('خطا در تغییر وضعیت');
                console.error(err);
            }
        });
    }

    // ==================== Navigation ====================

    resetAll(): void {
        this.isEditMode = false;
        this.editId = null;
        this.licenseService.mainTableId = 0;
        this.licenseForm.reset();
        this.financialForm.reset();
        this.cancellationForm.reset();
        this.cancellationForm.patchValue({ licenseStatus: LicenseStatusEnum.Active });
        this.relocationForm.reset();
        this.relocationsList = [];
        this.selectedRelocationId = 0;
        this.router.navigate(['/petition-writer-license']);
    }

    goToList(): void {
        this.router.navigate(['/petition-writer-license/list']);
    }

    getStatusName(status: number): string {
        const found = this.licenseStatusTypes.find(s => s.id === status);
        return found ? found.name : '-';
    }

    onProfileImageUploaded(imagePath: string): void {
        this.imageName = imagePath;
        this.toastr.success('عکس پروفایل با موفقیت بارگذاری شد');
    }

    onProvinceChange(): void {
        const provinceId = this.licenseForm.get('provinceId')?.value;
        if (provinceId) {
            this.licenseForm.get('licenseNumber')?.disable();
            this.licenseForm.patchValue({ licenseNumber: '' });
        } else {
            this.licenseForm.get('licenseNumber')?.enable();
        }
    }

    onLicenseTypeChange(): void {
        const licenseType = this.financialForm.get('licenseType')?.value;
        const licenseIssueDate = this.financialForm.get('licenseIssueDate')?.value;
        
        // Find the selected license type
        const selectedType = this.licenseTypes.find(t => t.id === licenseType);
        
        // Set price based on license type
        if (selectedType) {
            this.financialForm.patchValue({ licensePrice: selectedType.price });
        }
        
        // Handle expiry date
        if (licenseType === 'new' || licenseType === 'renewal') {
            // Auto-calculate expiry date (1 year)
            if (licenseIssueDate) {
                const expiryDate = this.addYearsToDate(licenseIssueDate, 1);
                this.financialForm.patchValue({ licenseExpiryDate: expiryDate });
            }
            this.financialForm.get('licenseExpiryDate')?.disable();
        } else if (licenseType === 'duplicate') {
            // Allow manual entry for مثنی
            this.financialForm.get('licenseExpiryDate')?.enable();
        }
    }

    onLicenseIssueDateChange(): void {
        const licenseType = this.financialForm.get('licenseType')?.value;
        const licenseIssueDate = this.financialForm.get('licenseIssueDate')?.value;
        
        // Auto-calculate expiry date for جدید and تمدید (1 year)
        if ((licenseType === 'new' || licenseType === 'renewal') && licenseIssueDate) {
            const expiryDate = this.addYearsToDate(licenseIssueDate, 1);
            this.financialForm.patchValue({ licenseExpiryDate: expiryDate });
        }
    }

    addYearsToDate(dateValue: any, years: number): Date | null {
        if (!dateValue) return null;
        
        const currentCalendar = this.calendarService.getSelectedCalendar();
        
        try {
            // Use the same pattern as license details component
            const issueDate = this.tryResolveGregorianDate(dateValue, currentCalendar);
            if (!issueDate) return null;
            
            // Add years to the Gregorian date
            const expiryDate = new Date(issueDate);
            expiryDate.setFullYear(expiryDate.getFullYear() + years);
            
            return expiryDate;
        } catch (error) {
            console.error('Error adding years to date:', error, dateValue);
        }
        
        return null;
    }

    private tryResolveGregorianDate(value: any, calendar: CalendarType): Date | null {
        if (!value) return null;

        // If it's already a valid Date object
        if (value instanceof Date && !isNaN(value.getTime())) {
            return value;
        }

        // If it's a calendar object with year, month, day
        if (typeof value === 'object' && value.year && value.month && value.day) {
            try {
                return this.calendarConversionService.toGregorian({
                    year: value.year,
                    month: value.month,
                    day: value.day,
                    calendarType: calendar
                });
            } catch {
                return null;
            }
        }

        // If it's a string, parse it
        if (typeof value === 'string') {
            const normalized = value.trim();
            if (!normalized) return null;

            const normalizedForParser = calendar === CalendarType.GREGORIAN
                ? normalized
                : normalized.replace(/-/g, '/');

            return this.calendarConversionService.parseInputDate(normalizedForParser, calendar);
        }

        return null;
    }
}
