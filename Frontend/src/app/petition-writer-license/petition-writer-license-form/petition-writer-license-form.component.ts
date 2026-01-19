import { Component, Injectable, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import {
    NgbDateStruct,
    NgbCalendar,
    NgbDatepickerI18n,
    NgbCalendarPersian,
    NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import { PetitionWriterLicenseService } from 'src/app/shared/petition-writer-license.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { SellerService } from 'src/app/shared/seller.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import {
    PetitionWriterLicenseData,
    PetitionWriterRelocationData,
    PetitionWriterRelocation,
    LicenseStatusEnum,
    LicenseStatusTypes,
    LicenseTypes
} from 'src/app/models/PetitionWriterLicense';

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
    selector: 'app-petition-writer-license-form',
    templateUrl: './petition-writer-license-form.component.html',
    styleUrls: ['./petition-writer-license-form.component.scss'],
    providers: [
        { provide: NgbCalendar, useClass: NgbCalendarPersian },
        { provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
    ],
})
export class PetitionWriterLicenseFormComponent implements OnInit {
    maxDate = { year: 1410, month: 12, day: 31 };
    minDate = { year: 1320, month: 12, day: 31 };

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

    // Relocation list
    relocationsList: PetitionWriterRelocation[] = [];
    selectedRelocationId: number = 0;

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
        private rbacService: RbacService,
        private ngbDateParserFormatter: NgbDateParserFormatter
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
            licenseNumber: ['', Validators.required],
            applicantName: ['', Validators.required],
            applicantFatherName: [''],
            applicantGrandFatherName: [''],
            electronicIdNumber: ['', Validators.required],
            permanentProvinceId: [null],
            permanentDistrictId: [null],
            permanentVillage: [''],
            currentProvinceId: [null],
            currentDistrictId: [null],
            currentVillage: [''],
            activityLocation: ['']
        });

        // Tab 2: ثبت مالیه و مشخصات جواز
        this.financialForm = this.fb.group({
            bankReceiptNumber: [''],
            bankReceiptDate: [null],
            licenseType: [null],
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
    }

    loadProvinces(): void {
        this.sellerService.getprovince().subscribe({
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
                    licenseNumber: data.licenseNumber,
                    applicantName: data.applicantName,
                    applicantFatherName: data.applicantFatherName,
                    applicantGrandFatherName: data.applicantGrandFatherName,
                    electronicIdNumber: data.electronicIdNumber,
                    permanentProvinceId: data.permanentProvinceId,
                    permanentDistrictId: data.permanentDistrictId,
                    permanentVillage: data.permanentVillage,
                    currentProvinceId: data.currentProvinceId,
                    currentDistrictId: data.currentDistrictId,
                    currentVillage: data.currentVillage,
                    activityLocation: data.activityLocation
                });

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

                // Patch financial form
                this.financialForm.patchValue({
                    bankReceiptNumber: data.bankReceiptNumber,
                    bankReceiptDate: this.parseDate(data.bankReceiptDateFormatted),
                    licenseType: data.licenseType,
                    licenseIssueDate: this.parseDate(data.licenseIssueDateFormatted),
                    licenseExpiryDate: this.parseDate(data.licenseExpiryDateFormatted)
                });

                // Patch cancellation form
                this.cancellationForm.patchValue({
                    licenseStatus: data.licenseStatus,
                    cancellationDate: this.parseDate(data.cancellationDateFormatted)
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

    parseDate(dateStr: string | undefined): NgbDateStruct | null {
        if (!dateStr) return null;
        const parts = dateStr.split('/');
        if (parts.length === 3) {
            return {
                year: parseInt(parts[0], 10),
                month: parseInt(parts[1], 10),
                day: parseInt(parts[2], 10)
            };
        }
        return null;
    }

    formatDate(date: NgbDateStruct | Date | any | null | undefined): string {
        if (!date) return '';

        const year = typeof date?.year === 'number' ? date.year : (typeof date?.getFullYear === 'function' ? date.getFullYear() : undefined);
        const month = typeof date?.month === 'number' ? date.month : (typeof date?.getMonth === 'function' ? date.getMonth() + 1 : undefined);
        const day = typeof date?.day === 'number' ? date.day : (typeof date?.getDate === 'function' ? date.getDate() : undefined);

        if (!year || !month || !day) return '';

        return `${year}/${String(month).padStart(2, '0')}/${String(day).padStart(2, '0')}`;
    }

    // Form getters
    get licenseNumber() { return this.licenseForm.get('licenseNumber'); }
    get applicantName() { return this.licenseForm.get('applicantName'); }

    saveLicense(): void {
        if (this.licenseForm.invalid) {
            this.toastr.warning('لطفا فیلدهای الزامی را تکمیل نمایید');
            return;
        }

        const calendar = this.calendarService.getSelectedCalendar();
        const formData = this.licenseForm.value;
        const financialData = this.financialForm.value;

        const bankReceiptDate = this.formatDate(financialData.bankReceiptDate) || undefined;
        const licenseIssueDate = this.formatDate(financialData.licenseIssueDate) || undefined;
        const licenseExpiryDate = this.formatDate(financialData.licenseExpiryDate) || undefined;
        const cancellationDate = this.formatDate(this.cancellationForm.value.cancellationDate) || undefined;

        const data: PetitionWriterLicenseData = {
            ...formData,
            bankReceiptNumber: financialData.bankReceiptNumber,
            bankReceiptDate,
            licenseType: financialData.licenseType,
            licenseIssueDate,
            licenseExpiryDate,
            licenseStatus: this.cancellationForm.value.licenseStatus,
            cancellationDate,
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

        const data: PetitionWriterRelocationData = {
            newActivityLocation: formData.newActivityLocation,
            relocationDate: this.formatDate(formData.relocationDate) || undefined,
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
            relocationDate: this.parseDate(r.relocationDateFormatted),
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
        const cancellationDate = this.formatDate(this.cancellationForm.value.cancellationDate) || undefined;

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
}
