import { Component, Injectable, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MatTabGroup } from '@angular/material/tabs';
import { LicenseApplicationService } from 'src/app/shared/license-application.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { SellerService } from 'src/app/shared/seller.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import {
    LicenseApplicationData,
    LicenseApplicationGuarantorData,
    LicenseApplicationWithdrawalData,
    LicenseApplicationGuarantor,
    LicenseApplicationWithdrawal,
    LicenseGuaranteeTypeEnum,
    LicenseGuaranteeTypes
} from 'src/app/models/LicenseApplication';

@Component({
    selector: 'app-license-application-form',
    templateUrl: './license-application-form.component.html',
    styleUrls: ['./license-application-form.component.scss'],
})
export class LicenseApplicationFormComponent implements OnInit {

    @ViewChild('tabGroup') tabGroup!: MatTabGroup;

    // Forms
    applicationForm!: FormGroup;
    guarantorForm!: FormGroup;
    withdrawalForm!: FormGroup;

    isEditMode = false;
    editId: number | null = null;
    selectedTabIndex = 0;

    // Dropdown data
    provinces: any[] = [];
    permanentDistricts: any[] = [];
    currentDistricts: any[] = [];
    guarantorPermanentDistricts: any[] = [];
    guarantorCurrentDistricts: any[] = [];
    guaranteeTypes = LicenseGuaranteeTypes;

    // Guarantor list
    guarantorsList: LicenseApplicationGuarantor[] = [];
    selectedGuarantorId: number = 0;

    // Withdrawal data
    withdrawalData: LicenseApplicationWithdrawal | null = null;

    // Conditional visibility flags for guarantor
    showCashFields = false;
    showShariaDeedFields = false;
    showCustomaryDeedFields = false;

    // RBAC
    canEdit = false;
    isViewOnly = false;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        public licenseAppService: LicenseApplicationService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private sellerService: SellerService,
        private rbacService: RbacService
    ) {
        this.initForms();
    }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadDropdowns();

        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isEditMode = true;
            this.editId = parseInt(id, 10);
            this.licenseAppService.mainTableId = this.editId;
            this.loadData(this.editId);
        }
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.isViewOnly = role === UserRoles.Authority || role === UserRoles.LicenseReviewer;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar;
    }

    initForms(): void {
        // Tab 1: Application Form
        this.applicationForm = this.fb.group({
            id: [null],
            requestDate: ['', Validators.required],
            requestSerialNumber: ['', [Validators.required, Validators.maxLength(50)]],
            applicantName: ['', [Validators.required, Validators.maxLength(200)]],
            applicantFatherName: ['', Validators.maxLength(200)],
            applicantGrandfatherName: ['', Validators.maxLength(200)],
            applicantElectronicNumber: ['', Validators.maxLength(50)],
            proposedGuideName: ['', [Validators.required, Validators.maxLength(200)]],
            permanentProvinceId: ['', Validators.required],
            permanentDistrictId: ['', Validators.required],
            permanentVillage: ['', Validators.required],
            currentProvinceId: ['', Validators.required],
            currentDistrictId: ['', Validators.required],
            currentVillage: ['', Validators.required],
        });

        // Tab 2: Guarantor Form
        this.guarantorForm = this.fb.group({
            id: [0],
            guarantorName: ['', Validators.required],
            guarantorFatherName: [''],
            guaranteeTypeId: ['', Validators.required],
            cashAmount: [''],
            shariaDeedNumber: [''],
            shariaDeedDate: [''],
            customaryDeedSerialNumber: [''],
            permanentProvinceId: ['', Validators.required],
            permanentDistrictId: ['', Validators.required],
            permanentVillage: ['', Validators.required],
            currentProvinceId: ['', Validators.required],
            currentDistrictId: ['', Validators.required],
            currentVillage: ['', Validators.required],
        });

        // Tab 3: Withdrawal Form
        this.withdrawalForm = this.fb.group({
            id: [0],
            withdrawalReason: ['', Validators.required],
            withdrawalDate: ['', Validators.required],
        });
    }

    loadDropdowns(): void {
        this.sellerService.getprovince().subscribe((res: any) => {
            this.provinces = res as any[];
        });
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.licenseAppService.getById(id, calendar).subscribe({
            next: (data) => {
                this.patchApplicationForm(data);
                this.loadGuarantors();
                this.loadWithdrawal();
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });
    }

    patchApplicationForm(data: any): void {
        // Load districts first
        if (data.permanentProvinceId) {
            this.sellerService.getdistrict(data.permanentProvinceId).subscribe((res: any) => {
                this.permanentDistricts = res as any[];
            });
        }
        if (data.currentProvinceId) {
            this.sellerService.getdistrict(data.currentProvinceId).subscribe((res: any) => {
                this.currentDistricts = res as any[];
            });
        }

        this.applicationForm.patchValue({
            id: data.id,
            requestSerialNumber: data.requestSerialNumber,
            applicantName: data.applicantName,
            applicantFatherName: data.applicantFatherName,
            applicantGrandfatherName: data.applicantGrandfatherName,
            applicantElectronicNumber: data.applicantElectronicNumber,
            proposedGuideName: data.proposedGuideName,
            permanentProvinceId: data.permanentProvinceId,
            permanentDistrictId: data.permanentDistrictId,
            permanentVillage: data.permanentVillage,
            currentProvinceId: data.currentProvinceId,
            currentDistrictId: data.currentDistrictId,
            currentVillage: data.currentVillage,
        });

        // Parse date
        if (data.requestDateFormatted) {
            const requestDate = this.parseDate(data.requestDateFormatted);
            if (requestDate) {
                this.applicationForm.patchValue({ requestDate });
            }
        }
    }

    parseDate(dateStr: string): Date | null {
        if (!dateStr) return null;
        const date = new Date(dateStr);
        return isNaN(date.getTime()) ? null : date;
    }

    loadGuarantors(): void {
        if (!this.licenseAppService.mainTableId) return;
        const calendar = this.calendarService.getSelectedCalendar();
        this.licenseAppService.getGuarantors(this.licenseAppService.mainTableId, calendar).subscribe({
            next: (data) => {
                this.guarantorsList = data;
            },
            error: (err) => console.error(err)
        });
    }

    loadWithdrawal(): void {
        if (!this.licenseAppService.mainTableId) return;
        const calendar = this.calendarService.getSelectedCalendar();
        this.licenseAppService.getWithdrawal(this.licenseAppService.mainTableId, calendar).subscribe({
            next: (data) => {
                this.withdrawalData = data;
                if (data) {
                    this.patchWithdrawalForm(data);
                }
            },
            error: (err) => console.error(err)
        });
    }

    patchWithdrawalForm(data: LicenseApplicationWithdrawal): void {
        this.withdrawalForm.patchValue({
            id: data.id,
            withdrawalReason: data.withdrawalReason,
        });

        if (data.withdrawalDateFormatted) {
            const withdrawalDate = this.parseDate(data.withdrawalDateFormatted);
            if (withdrawalDate) {
                this.withdrawalForm.patchValue({ withdrawalDate });
            }
        }
    }

    // ==================== Province/District Handlers ====================

    onPermanentProvinceChange(event: any): void {
        if (event?.id) {
            this.sellerService.getdistrict(event.id).subscribe((res: any) => {
                this.permanentDistricts = res as any[];
                this.applicationForm.patchValue({ permanentDistrictId: '' });
            });
        }
    }

    onCurrentProvinceChange(event: any): void {
        if (event?.id) {
            this.sellerService.getdistrict(event.id).subscribe((res: any) => {
                this.currentDistricts = res as any[];
                this.applicationForm.patchValue({ currentDistrictId: '' });
            });
        }
    }

    onGuarantorPermanentProvinceChange(event: any): void {
        if (event?.id) {
            this.sellerService.getdistrict(event.id).subscribe((res: any) => {
                this.guarantorPermanentDistricts = res as any[];
                this.guarantorForm.patchValue({ permanentDistrictId: '' });
            });
        }
    }

    onGuarantorCurrentProvinceChange(event: any): void {
        if (event?.id) {
            this.sellerService.getdistrict(event.id).subscribe((res: any) => {
                this.guarantorCurrentDistricts = res as any[];
                this.guarantorForm.patchValue({ currentDistrictId: '' });
            });
        }
    }

    // ==================== Guarantee Type Handler ====================

    onGuaranteeTypeChange(): void {
        const typeId = Number(this.guarantorForm.get('guaranteeTypeId')?.value);

        this.showCashFields = false;
        this.showShariaDeedFields = false;
        this.showCustomaryDeedFields = false;

        this.clearGuarantorConditionalValidators();
        this.clearGuarantorConditionalValues();

        switch (typeId) {
            case LicenseGuaranteeTypeEnum.Cash:
                this.showCashFields = true;
                break;
            case LicenseGuaranteeTypeEnum.ShariaDeed:
                this.showShariaDeedFields = true;
                break;
            case LicenseGuaranteeTypeEnum.CustomaryDeed:
                this.showCustomaryDeedFields = true;
                break;
        }

        this.updateGuarantorConditionalValidity();
    }

    private clearGuarantorConditionalValidators(): void {
        this.guarantorForm.get('cashAmount')?.clearValidators();
        this.guarantorForm.get('shariaDeedNumber')?.clearValidators();
        this.guarantorForm.get('shariaDeedDate')?.clearValidators();
        this.guarantorForm.get('customaryDeedSerialNumber')?.clearValidators();
    }

    private clearGuarantorConditionalValues(): void {
        this.guarantorForm.patchValue({
            cashAmount: '',
            shariaDeedNumber: '',
            shariaDeedDate: '',
            customaryDeedSerialNumber: '',
        });
    }

    private updateGuarantorConditionalValidity(): void {
        this.guarantorForm.get('cashAmount')?.updateValueAndValidity();
        this.guarantorForm.get('shariaDeedNumber')?.updateValueAndValidity();
        this.guarantorForm.get('shariaDeedDate')?.updateValueAndValidity();
        this.guarantorForm.get('customaryDeedSerialNumber')?.updateValueAndValidity();
    }

    private setGuaranteeTypeVisibility(typeId: number | undefined): void {
        this.showCashFields = false;
        this.showShariaDeedFields = false;
        this.showCustomaryDeedFields = false;

        switch (typeId) {
            case LicenseGuaranteeTypeEnum.Cash:
                this.showCashFields = true;
                break;
            case LicenseGuaranteeTypeEnum.ShariaDeed:
                this.showShariaDeedFields = true;
                break;
            case LicenseGuaranteeTypeEnum.CustomaryDeed:
                this.showCustomaryDeedFields = true;
                break;
        }
    }

    // ==================== Date Formatting ====================

    private formatDateForBackend(dateValue: any): string {
        const currentCalendar = this.calendarService.getSelectedCalendar();

        if (dateValue instanceof Date) {
            const calendarDate = this.calendarConversionService.fromGregorian(dateValue, currentCalendar);
            const year = calendarDate.year;
            const month = String(calendarDate.month).padStart(2, '0');
            const day = String(calendarDate.day).padStart(2, '0');
            return `${year}-${month}-${day}`;
        } else if (typeof dateValue === 'object' && dateValue?.year) {
            const year = dateValue.year;
            const month = String(dateValue.month).padStart(2, '0');
            const day = String(dateValue.day).padStart(2, '0');
            return `${year}-${month}-${day}`;
        } else if (typeof dateValue === 'string') {
            return dateValue.replace(/\//g, '-');
        }
        return '';
    }

    // ==================== Tab 1: Application CRUD ====================

    saveApplication(): void {
        if (this.applicationForm.invalid) {
            this.toastr.warning('لطفا تمام فیلدهای الزامی را پر کنید');
            return;
        }

        const formValue = this.applicationForm.value;
        const currentCalendar = this.calendarService.getSelectedCalendar();

        const data: LicenseApplicationData = {
            id: formValue.id,
            requestDate: this.formatDateForBackend(formValue.requestDate),
            requestSerialNumber: formValue.requestSerialNumber,
            applicantName: formValue.applicantName,
            applicantFatherName: formValue.applicantFatherName,
            applicantGrandfatherName: formValue.applicantGrandfatherName,
            applicantElectronicNumber: formValue.applicantElectronicNumber,
            proposedGuideName: formValue.proposedGuideName,
            permanentProvinceId: formValue.permanentProvinceId,
            permanentDistrictId: formValue.permanentDistrictId,
            permanentVillage: formValue.permanentVillage,
            currentProvinceId: formValue.currentProvinceId,
            currentDistrictId: formValue.currentDistrictId,
            currentVillage: formValue.currentVillage,
            calendarType: currentCalendar
        };

        if (this.isEditMode && this.editId) {
            this.licenseAppService.update(this.editId, data).subscribe({
                next: () => {
                    this.toastr.success('معلومات موفقانه تغییر یافت');
                },
                error: (err) => {
                    this.toastr.error('خطا در ذخیره معلومات');
                    console.error(err);
                }
            });
        } else {
            this.licenseAppService.create(data).subscribe({
                next: (result) => {
                    this.toastr.success('معلومات موفقانه ثبت شد');
                    this.editId = result.id;
                    this.isEditMode = true;
                    this.applicationForm.patchValue({ id: result.id });
                    this.nextTab();
                },
                error: (err) => {
                    this.toastr.error('خطا در ذخیره معلومات');
                    console.error(err);
                }
            });
        }
    }

    // ==================== Tab 2: Guarantor CRUD ====================

    saveGuarantor(): void {
        if (this.guarantorForm.invalid) {
            this.toastr.warning('لطفا تمام فیلدهای الزامی را پر کنید');
            return;
        }

        if (!this.licenseAppService.mainTableId) {
            this.toastr.error('لطفا ابتدا معلومات درخواست را ثبت کنید');
            return;
        }

        const formValue = this.guarantorForm.value;
        const currentCalendar = this.calendarService.getSelectedCalendar();

        const data: LicenseApplicationGuarantorData = {
            id: formValue.id || 0,
            licenseApplicationId: this.licenseAppService.mainTableId,
            guarantorName: formValue.guarantorName,
            guarantorFatherName: formValue.guarantorFatherName,
            guaranteeTypeId: Number(formValue.guaranteeTypeId),
            cashAmount: formValue.cashAmount ? Number(formValue.cashAmount) : undefined,
            shariaDeedNumber: formValue.shariaDeedNumber,
            shariaDeedDate: this.formatDateForBackend(formValue.shariaDeedDate),
            customaryDeedSerialNumber: formValue.customaryDeedSerialNumber,
            permanentProvinceId: formValue.permanentProvinceId,
            permanentDistrictId: formValue.permanentDistrictId,
            permanentVillage: formValue.permanentVillage,
            currentProvinceId: formValue.currentProvinceId,
            currentDistrictId: formValue.currentDistrictId,
            currentVillage: formValue.currentVillage,
            calendarType: currentCalendar
        };

        if (this.selectedGuarantorId && this.selectedGuarantorId > 0) {
            data.id = this.selectedGuarantorId;
            this.licenseAppService.updateGuarantor(data).subscribe({
                next: () => {
                    this.toastr.success('معلومات تضمین‌کننده تغییر یافت');
                    this.loadGuarantors();
                    this.resetGuarantorForm();
                },
                error: (err) => {
                    this.toastr.error('خطا در تغییر معلومات');
                    console.error(err);
                }
            });
        } else {
            this.licenseAppService.addGuarantor(data).subscribe({
                next: () => {
                    this.toastr.success('تضمین‌کننده موفقانه ثبت شد');
                    this.loadGuarantors();
                    this.resetGuarantorForm();
                },
                error: (err) => {
                    if (err.status === 400) {
                        this.toastr.error('شما نمی‌توانید بیشتر از دو تضمین‌کننده ثبت کنید');
                    } else {
                        this.toastr.error('خطا در ثبت معلومات');
                    }
                    console.error(err);
                }
            });
        }
    }

    selectGuarantor(guarantor: LicenseApplicationGuarantor): void {
        this.selectedGuarantorId = guarantor.id || 0;

        // Load districts
        if (guarantor.permanentProvinceId) {
            this.sellerService.getdistrict(guarantor.permanentProvinceId).subscribe((res: any) => {
                this.guarantorPermanentDistricts = res as any[];
            });
        }
        if (guarantor.currentProvinceId) {
            this.sellerService.getdistrict(guarantor.currentProvinceId).subscribe((res: any) => {
                this.guarantorCurrentDistricts = res as any[];
            });
        }

        this.guarantorForm.patchValue({
            id: guarantor.id,
            guarantorName: guarantor.guarantorName,
            guarantorFatherName: guarantor.guarantorFatherName,
            guaranteeTypeId: guarantor.guaranteeTypeId,
            cashAmount: guarantor.cashAmount,
            shariaDeedNumber: guarantor.shariaDeedNumber,
            customaryDeedSerialNumber: guarantor.customaryDeedSerialNumber,
            permanentProvinceId: guarantor.permanentProvinceId,
            permanentDistrictId: guarantor.permanentDistrictId,
            permanentVillage: guarantor.permanentVillage,
            currentProvinceId: guarantor.currentProvinceId,
            currentDistrictId: guarantor.currentDistrictId,
            currentVillage: guarantor.currentVillage,
        });

        // Parse sharia deed date
        if (guarantor.shariaDeedDateFormatted) {
            const shariaDeedDate = this.parseDate(guarantor.shariaDeedDateFormatted);
            if (shariaDeedDate) {
                this.guarantorForm.patchValue({ shariaDeedDate });
            }
        }

        this.setGuaranteeTypeVisibility(guarantor.guaranteeTypeId);
    }

    resetGuarantorForm(): void {
        this.selectedGuarantorId = 0;
        this.guarantorForm.reset();
        this.guarantorForm.patchValue({ id: 0 });
        this.showCashFields = false;
        this.showShariaDeedFields = false;
        this.showCustomaryDeedFields = false;
        this.guarantorPermanentDistricts = [];
        this.guarantorCurrentDistricts = [];
    }

    getGuaranteeTypeName(typeId: number | undefined): string {
        if (!typeId) return '-';
        const found = this.guaranteeTypes.find(t => t.id === typeId);
        return found ? found.name : '-';
    }

    // ==================== Tab 3: Withdrawal CRUD ====================

    saveWithdrawal(): void {
        if (this.withdrawalForm.invalid) {
            this.toastr.warning('لطفا تمام فیلدهای الزامی را پر کنید');
            return;
        }

        if (!this.licenseAppService.mainTableId) {
            this.toastr.error('لطفا ابتدا معلومات درخواست را ثبت کنید');
            return;
        }

        const formValue = this.withdrawalForm.value;
        const currentCalendar = this.calendarService.getSelectedCalendar();

        const data: LicenseApplicationWithdrawalData = {
            id: formValue.id || 0,
            licenseApplicationId: this.licenseAppService.mainTableId,
            withdrawalReason: formValue.withdrawalReason,
            withdrawalDate: this.formatDateForBackend(formValue.withdrawalDate),
            calendarType: currentCalendar
        };

        this.licenseAppService.saveWithdrawal(data).subscribe({
            next: () => {
                this.toastr.success('معلومات انصراف موفقانه ثبت شد');
                this.loadWithdrawal();
            },
            error: (err) => {
                this.toastr.error('خطا در ثبت معلومات');
                console.error(err);
            }
        });
    }

    deleteWithdrawal(): void {
        if (!this.licenseAppService.mainTableId) return;

        this.licenseAppService.deleteWithdrawal(this.licenseAppService.mainTableId).subscribe({
            next: () => {
                this.toastr.success('معلومات انصراف حذف شد');
                this.withdrawalData = null;
                this.withdrawalForm.reset();
            },
            error: (err) => {
                this.toastr.error('خطا در حذف معلومات');
                console.error(err);
            }
        });
    }

    // ==================== Navigation ====================

    nextTab(): void {
        if (this.tabGroup) {
            const nextIndex = (this.tabGroup.selectedIndex ?? 0) + 1;
            const tabCount = this.tabGroup._tabs.length;
            this.tabGroup.selectedIndex = nextIndex % tabCount;
        }
    }

    goToList(): void {
        this.router.navigate(['/license-applications/list']);
    }

    resetAll(): void {
        this.licenseAppService.resetMainTableId();
        this.applicationForm.reset();
        this.guarantorForm.reset();
        this.withdrawalForm.reset();
        this.guarantorsList = [];
        this.withdrawalData = null;
        this.isEditMode = false;
        this.editId = null;
        this.selectedGuarantorId = 0;
        this.showCashFields = false;
        this.showShariaDeedFields = false;
        this.showCustomaryDeedFields = false;
    }

    // Form getters for validation
    get requestDate() { return this.applicationForm.get('requestDate'); }
    get requestSerialNumber() { return this.applicationForm.get('requestSerialNumber'); }
    get applicantName() { return this.applicationForm.get('applicantName'); }
    get applicantFatherName() { return this.applicationForm.get('applicantFatherName'); }
    get applicantGrandfatherName() { return this.applicationForm.get('applicantGrandfatherName'); }
    get applicantElectronicNumber() { return this.applicationForm.get('applicantElectronicNumber'); }
    get proposedGuideName() { return this.applicationForm.get('proposedGuideName'); }
}
