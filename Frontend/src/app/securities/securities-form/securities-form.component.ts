import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SecuritiesService } from 'src/app/shared/securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { 
    SecuritiesDistributionData,
    DocumentTypes,
    PropertySubTypes,
    VehicleSubTypes,
    RegistrationBookTypes
} from 'src/app/models/SecuritiesDistribution';

@Component({
    selector: 'app-securities-form',
    templateUrl: './securities-form.component.html',
    styleUrls: ['./securities-form.component.scss'],
})
export class SecuritiesFormComponent implements OnInit {
    // Date range constraints (optional - can be removed if not needed)
    minDate = { year: 1320, month: 1, day: 1 };
    maxDate = { year: 1410, month: 12, day: 31 };

    securitiesForm!: FormGroup;
    isEditMode = false;
    editId: number | null = null;
    selectedTabIndex = 0;

    // Dropdown options
    documentTypes = DocumentTypes;
    propertySubTypes = PropertySubTypes;
    vehicleSubTypes = VehicleSubTypes;
    registrationBookTypes = RegistrationBookTypes;

    // Conditional visibility flags
    showPropertyFields = false;
    showVehicleFields = false;
    showPropertySaleFields = false;
    showBayWafaFields = false;
    showRentFields = false;
    showVehicleSaleFields = false;
    showVehicleExchangeFields = false;
    showRegistrationBookCount = false;
    showDuplicateBookCount = false;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        private securitiesService: SecuritiesService,
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
    }

    initForm(): void {
        this.securitiesForm = this.fb.group({
            id: [null],
            // Tab 1
            registrationNumber: ['', [Validators.required, Validators.maxLength(50)]],
            licenseOwnerName: ['', [Validators.required, Validators.maxLength(200)]],
            licenseOwnerFatherName: ['', [Validators.required, Validators.maxLength(200)]],
            transactionGuideName: ['', [Validators.required, Validators.maxLength(200)]],
            licenseNumber: ['', [Validators.required, Validators.maxLength(50)]],
            // Tab 2
            documentType: [null],
            propertySubType: [null],
            vehicleSubType: [null],
            propertySaleCount: [null, [Validators.min(0)]],
            propertySaleSerialStart: ['', [Validators.maxLength(100)]],
            propertySaleSerialEnd: ['', [Validators.maxLength(100)]],
            bayWafaCount: [null, [Validators.min(0)]],
            bayWafaSerialStart: ['', [Validators.maxLength(100)]],
            bayWafaSerialEnd: ['', [Validators.maxLength(100)]],
            rentCount: [null, [Validators.min(0)]],
            rentSerialStart: ['', [Validators.maxLength(100)]],
            rentSerialEnd: ['', [Validators.maxLength(100)]],
            vehicleSaleCount: [null, [Validators.min(0)]],
            vehicleSaleSerialStart: ['', [Validators.maxLength(100)]],
            vehicleSaleSerialEnd: ['', [Validators.maxLength(100)]],
            vehicleExchangeCount: [null, [Validators.min(0)]],
            vehicleExchangeSerialStart: ['', [Validators.maxLength(100)]],
            vehicleExchangeSerialEnd: ['', [Validators.maxLength(100)]],
            registrationBookType: [null],
            registrationBookCount: [null, [Validators.min(0)]],
            duplicateBookCount: [null, [Validators.min(0)]],
            // Tab 3
            pricePerDocument: [4000, [Validators.min(0)]],
            totalDocumentsPrice: [null, [Validators.min(0)]],
            registrationBookPrice: [null, [Validators.min(0)]],
            totalSecuritiesPrice: [null, [Validators.min(0)]],
            // Tab 4
            bankReceiptNumber: ['', [Validators.maxLength(100)]],
            deliveryDate: [null],
            distributionDate: [null]
        });
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.securitiesService.getById(id, calendar).subscribe({
            next: (data) => {
                this.securitiesForm.patchValue({
                    id: data.id,
                    registrationNumber: data.registrationNumber,
                    licenseOwnerName: data.licenseOwnerName,
                    licenseOwnerFatherName: data.licenseOwnerFatherName,
                    transactionGuideName: data.transactionGuideName,
                    licenseNumber: data.licenseNumber,
                    documentType: data.documentType,
                    propertySubType: data.propertySubType,
                    vehicleSubType: data.vehicleSubType,
                    propertySaleCount: data.propertySaleCount,
                    propertySaleSerialStart: data.propertySaleSerialStart,
                    propertySaleSerialEnd: data.propertySaleSerialEnd,
                    bayWafaCount: data.bayWafaCount,
                    bayWafaSerialStart: data.bayWafaSerialStart,
                    bayWafaSerialEnd: data.bayWafaSerialEnd,
                    rentCount: data.rentCount,
                    rentSerialStart: data.rentSerialStart,
                    rentSerialEnd: data.rentSerialEnd,
                    vehicleSaleCount: data.vehicleSaleCount,
                    vehicleSaleSerialStart: data.vehicleSaleSerialStart,
                    vehicleSaleSerialEnd: data.vehicleSaleSerialEnd,
                    vehicleExchangeCount: data.vehicleExchangeCount,
                    vehicleExchangeSerialStart: data.vehicleExchangeSerialStart,
                    vehicleExchangeSerialEnd: data.vehicleExchangeSerialEnd,
                    registrationBookType: data.registrationBookType,
                    registrationBookCount: data.registrationBookCount,
                    duplicateBookCount: data.duplicateBookCount,
                    pricePerDocument: data.pricePerDocument,
                    totalDocumentsPrice: data.totalDocumentsPrice,
                    registrationBookPrice: data.registrationBookPrice,
                    totalSecuritiesPrice: data.totalSecuritiesPrice,
                    bankReceiptNumber: data.bankReceiptNumber
                });

                // Parse dates
                if (data.deliveryDateFormatted) {
                    const deliveryDate = this.parseDate(data.deliveryDateFormatted);
                    if (deliveryDate) {
                        this.securitiesForm.patchValue({ deliveryDate });
                    }
                }
                if (data.distributionDateFormatted) {
                    const distributionDate = this.parseDate(data.distributionDateFormatted);
                    if (distributionDate) {
                        this.securitiesForm.patchValue({ distributionDate });
                    }
                }

                // Set visibility based on loaded data
                this.onDocumentTypeChange();
                this.onPropertySubTypeChange();
                this.onVehicleSubTypeChange();
                this.onRegistrationBookTypeChange();
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });
    }

    onDocumentTypeChange(): void {
        const docType = this.securitiesForm.get('documentType')?.value;
        this.showPropertyFields = docType === 1;
        this.showVehicleFields = docType === 2;

        if (docType === 1) {
            // Clear vehicle fields
            this.securitiesForm.patchValue({
                vehicleSubType: null,
                vehicleSaleCount: null,
                vehicleSaleSerialStart: '',
                vehicleSaleSerialEnd: '',
                vehicleExchangeCount: null,
                vehicleExchangeSerialStart: '',
                vehicleExchangeSerialEnd: ''
            });
            this.showVehicleSaleFields = false;
            this.showVehicleExchangeFields = false;
        } else if (docType === 2) {
            // Clear property fields
            this.securitiesForm.patchValue({
                propertySubType: null,
                propertySaleCount: null,
                propertySaleSerialStart: '',
                propertySaleSerialEnd: '',
                bayWafaCount: null,
                bayWafaSerialStart: '',
                bayWafaSerialEnd: '',
                rentCount: null,
                rentSerialStart: '',
                rentSerialEnd: ''
            });
            this.showPropertySaleFields = false;
            this.showBayWafaFields = false;
            this.showRentFields = false;
        }
    }

    onPropertySubTypeChange(): void {
        const subType = this.securitiesForm.get('propertySubType')?.value;
        this.showPropertySaleFields = subType === 1 || subType === 4;
        this.showBayWafaFields = subType === 2 || subType === 4;
        this.showRentFields = subType === 3 || subType === 4;

        // Clear fields that are not visible
        if (!this.showPropertySaleFields) {
            this.securitiesForm.patchValue({ propertySaleCount: null, propertySaleSerialStart: '', propertySaleSerialEnd: '' });
        }
        if (!this.showBayWafaFields) {
            this.securitiesForm.patchValue({ bayWafaCount: null, bayWafaSerialStart: '', bayWafaSerialEnd: '' });
        }
        if (!this.showRentFields) {
            this.securitiesForm.patchValue({ rentCount: null, rentSerialStart: '', rentSerialEnd: '' });
        }
    }

    onVehicleSubTypeChange(): void {
        const subType = this.securitiesForm.get('vehicleSubType')?.value;
        this.showVehicleSaleFields = subType === 1;
        this.showVehicleExchangeFields = subType === 2;

        if (!this.showVehicleSaleFields) {
            this.securitiesForm.patchValue({ vehicleSaleCount: null, vehicleSaleSerialStart: '', vehicleSaleSerialEnd: '' });
        }
        if (!this.showVehicleExchangeFields) {
            this.securitiesForm.patchValue({ vehicleExchangeCount: null, vehicleExchangeSerialStart: '', vehicleExchangeSerialEnd: '' });
        }
    }

    onRegistrationBookTypeChange(): void {
        const bookType = this.securitiesForm.get('registrationBookType')?.value;
        this.showRegistrationBookCount = bookType === 1;
        this.showDuplicateBookCount = bookType === 2;

        if (!this.showRegistrationBookCount) {
            this.securitiesForm.patchValue({ registrationBookCount: null });
        }
        if (!this.showDuplicateBookCount) {
            this.securitiesForm.patchValue({ duplicateBookCount: null });
        }

        // Recalculate registration book price when type changes
        this.calculateRegistrationBookPrice();
    }

    calculateRegistrationBookPrice(): void {
        const bookType = this.securitiesForm.get('registrationBookType')?.value;
        let bookPrice = 0;

        if (bookType === 1) {
            // کتاب ثبت: تعداد × 1000
            const count = this.securitiesForm.get('registrationBookCount')?.value || 0;
            bookPrice = count * 1000;
        } else if (bookType === 2) {
            // کتاب ثبت مثنی: تعداد × 20000
            const count = this.securitiesForm.get('duplicateBookCount')?.value || 0;
            bookPrice = count * 20000;
        }

        this.securitiesForm.patchValue({ registrationBookPrice: bookPrice });
        this.calculateTotalPrice();
    }

    calculateTotalPrice(): void {
        // قیمت فی جلد سته is always 4000
        const pricePerDoc = 4000;
        this.securitiesForm.patchValue({ pricePerDocument: pricePerDoc }, { emitEvent: false });
        
        const bookPrice = this.securitiesForm.get('registrationBookPrice')?.value || 0;
        
        // Calculate total document count
        let totalDocs = 0;
        if (this.showPropertySaleFields) {
            totalDocs += this.securitiesForm.get('propertySaleCount')?.value || 0;
        }
        if (this.showBayWafaFields) {
            totalDocs += this.securitiesForm.get('bayWafaCount')?.value || 0;
        }
        if (this.showRentFields) {
            totalDocs += this.securitiesForm.get('rentCount')?.value || 0;
        }
        if (this.showVehicleSaleFields) {
            totalDocs += this.securitiesForm.get('vehicleSaleCount')?.value || 0;
        }
        if (this.showVehicleExchangeFields) {
            totalDocs += this.securitiesForm.get('vehicleExchangeCount')?.value || 0;
        }

        // قیمت مجموعی سته‌ها = تعداد سته‌ها × قیمت فی جلد
        const totalDocsPrice = totalDocs * pricePerDoc;
        
        // قیمت مجموعی اسناد بهادار = قیمت مجموعی سته‌ها + قیمت کتاب ثبت
        const totalSecurities = totalDocsPrice + bookPrice;

        this.securitiesForm.patchValue({
            totalDocumentsPrice: totalDocsPrice,
            totalSecuritiesPrice: totalSecurities
        });
    }

    parseDate(dateStr: string): Date | null {
        if (!dateStr) return null;
        const date = new Date(dateStr);
        return isNaN(date.getTime()) ? null : date;
    }

    formatDateForBackend(dateValue: any): string | undefined {
        if (!dateValue) return undefined;
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
        return undefined;
    }

    onSubmit(): void {
        if (this.securitiesForm.invalid) {
            this.toastr.error('لطفاً تمام فیلدهای الزامی را پر کنید');
            this.markFormGroupTouched();
            return;
        }

        const formValue = this.securitiesForm.value;
        const data: SecuritiesDistributionData = {
            ...formValue,
            deliveryDate: this.formatDateForBackend(formValue.deliveryDate),
            distributionDate: this.formatDateForBackend(formValue.distributionDate)
        };

        if (this.isEditMode && this.editId) {
            this.securitiesService.update(this.editId, data).subscribe({
                next: (res) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت بروزرسانی شد');
                    // Stay on page for edit mode - just show success message
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در بروزرسانی اطلاعات');
                }
            });
        } else {
            this.securitiesService.create(data).subscribe({
                next: (res) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت ثبت شد');
                    // Redirect to list for create mode
                    this.router.navigate(['/securities/list']);
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در ثبت اطلاعات');
                }
            });
        }
    }

    markFormGroupTouched(): void {
        Object.keys(this.securitiesForm.controls).forEach(key => {
            this.securitiesForm.get(key)?.markAsTouched();
        });
    }

    resetForm(): void {
        this.securitiesForm.reset();
        this.showPropertyFields = false;
        this.showVehicleFields = false;
        this.showPropertySaleFields = false;
        this.showBayWafaFields = false;
        this.showRentFields = false;
        this.showVehicleSaleFields = false;
        this.showVehicleExchangeFields = false;
        this.showRegistrationBookCount = false;
        this.showDuplicateBookCount = false;
        
        // If in edit mode, navigate to create page
        if (this.isEditMode) {
            this.isEditMode = false;
            this.editId = null;
            this.router.navigate(['/securities']);
        }
    }

    goToList(): void {
        this.router.navigate(['/securities/list']);
    }

    nextTab(): void {
        if (this.selectedTabIndex < 3) {
            this.selectedTabIndex++;
        }
    }

    prevTab(): void {
        if (this.selectedTabIndex > 0) {
            this.selectedTabIndex--;
        }
    }

    // Form control getters
    get registrationNumber() { return this.securitiesForm.get('registrationNumber'); }
    get licenseOwnerName() { return this.securitiesForm.get('licenseOwnerName'); }
    get licenseOwnerFatherName() { return this.securitiesForm.get('licenseOwnerFatherName'); }
    get transactionGuideName() { return this.securitiesForm.get('transactionGuideName'); }
    get licenseNumber() { return this.securitiesForm.get('licenseNumber'); }
}
