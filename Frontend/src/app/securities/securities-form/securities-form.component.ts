import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SecuritiesService } from 'src/app/shared/securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { AuthService } from 'src/app/shared/auth.service';
import { 
    SecuritiesDistributionData,
    SecuritiesDistributionItem,
    DocumentTypes,
    DocumentTypeInfo
} from 'src/app/models/SecuritiesDistribution';

@Component({
    selector: 'app-securities-form',
    templateUrl: './securities-form.component.html',
    styleUrls: ['./securities-form.component.scss'],
})
export class SecuritiesFormComponent implements OnInit {
    securitiesForm!: FormGroup;
    isEditMode = false;
    editId: number | null = null;
    
    // Company search states
    companySearching = false;
    companyFound = false;
    companyNotFound = false;
    selectedCompanyId: number | null = null;

    // Document types and items
    documentTypes = DocumentTypes;
    items: SecuritiesDistributionItem[] = [];
    selectedDocumentType: number | null = null;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        private securitiesService: SecuritiesService,
        private calendarService: CalendarService,
        private companyService: CompnaydetailService,
        private authService: AuthService
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
            // Tab 3
            pricePerDocument: [4000, [Validators.min(0)]],
            totalDocumentsPrice: [null, [Validators.min(0)]],
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
            next: (data: any) => {
                this.securitiesForm.patchValue({
                    id: data.id,
                    registrationNumber: data.registrationNumber,
                    licenseOwnerName: data.licenseOwnerName,
                    licenseOwnerFatherName: data.licenseOwnerFatherName,
                    transactionGuideName: data.transactionGuideName,
                    licenseNumber: data.licenseNumber,
                    pricePerDocument: data.pricePerDocument,
                    totalDocumentsPrice: data.totalDocumentsPrice,
                    totalSecuritiesPrice: data.totalSecuritiesPrice,
                    bankReceiptNumber: data.bankReceiptNumber,
                    deliveryDate: data.deliveryDate,
                    distributionDate: data.distributionDate
                });

                // Load items
                if (data.items && data.items.length > 0) {
                    this.items = data.items.map((item: any) => ({
                        id: item.id,
                        documentType: item.documentType,
                        serialStart: item.serialStart,
                        serialEnd: item.serialEnd,
                        count: item.count,
                        price: item.price
                    }));
                    this.calculateTotals();
                }
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });
    }

    addItem(): void {
        if (!this.selectedDocumentType) {
            this.toastr.warning('لطفاً نوعیت اسناد بهادار را انتخاب کنید');
            return;
        }

        const docType = this.documentTypes.find(d => d.id === this.selectedDocumentType);
        if (!docType) return;

        const newItem: SecuritiesDistributionItem = {
            documentType: this.selectedDocumentType,
            serialStart: docType.hasSerial ? '' : undefined,
            serialEnd: docType.hasSerial ? '' : undefined,
            count: 0,
            price: 0
        };

        this.items.push(newItem);
        this.selectedDocumentType = null;
        this.toastr.success('سند اضافه شد');
    }

    removeItem(index: number): void {
        this.items.splice(index, 1);
        this.calculateTotals();
        this.toastr.info('سند حذف شد');
    }

    calculateItemCount(item: SecuritiesDistributionItem): void {
        // Serial numbers are just reference numbers for tracking
        // They should NOT affect the count or price
        // Only the manually entered count should be used
        this.calculateItemPrice(item);
    }

    onCountChange(item: SecuritiesDistributionItem): void {
        // When count is manually changed, recalculate price
        this.calculateItemPrice(item);
    }

    calculateItemPrice(item: SecuritiesDistributionItem): void {
        const docType = this.documentTypes.find(d => d.id === item.documentType);
        if (docType && item.count > 0) {
            item.price = item.count * docType.pricePerUnit;
        } else {
            item.price = 0;
        }
        this.calculateTotals();
    }

    calculateTotals(): void {
        const totalDocs = this.items.reduce((sum, item) => sum + item.count, 0);
        const totalPrice = this.items.reduce((sum, item) => sum + item.price, 0);

        this.securitiesForm.patchValue({
            totalDocumentsPrice: totalPrice,
            totalSecuritiesPrice: totalPrice
        }, { emitEvent: false });
    }

    getDocumentTypeName(documentType: number): string {
        const docType = this.documentTypes.find(d => d.id === documentType);
        return docType ? docType.name : '';
    }

    hasSerialNumbers(documentType: number): boolean {
        const docType = this.documentTypes.find(d => d.id === documentType);
        return docType ? docType.hasSerial : false;
    }

    onSubmit(): void {
        if (this.securitiesForm.invalid) {
            this.toastr.error('لطفاً تمام فیلدهای الزامی را پر کنید');
            this.markFormGroupTouched();
            return;
        }

        if (this.items.length === 0) {
            this.toastr.error('لطفاً حداقل یک سند اضافه کنید');
            return;
        }

        // Validate items
        for (const item of this.items) {
            const docType = this.documentTypes.find(d => d.id === item.documentType);
            if (docType && docType.hasSerial) {
                if (!item.serialStart || !item.serialEnd) {
                    this.toastr.error('لطفاً سریال نمبر آغاز و ختم را برای تمام اسناد وارد کنید');
                    return;
                }
            }
            if (item.count <= 0) {
                this.toastr.error('تعداد باید بیشتر از صفر باشد');
                return;
            }
        }

        const formValue = this.securitiesForm.value;
        
        // Convert dates to DateOnly format (YYYY-MM-DD) if they exist
        const deliveryDate = formValue.deliveryDate 
            ? new Date(formValue.deliveryDate).toISOString().split('T')[0]
            : null;
        const distributionDate = formValue.distributionDate 
            ? new Date(formValue.distributionDate).toISOString().split('T')[0]
            : null;

        const data: SecuritiesDistributionData = {
            ...formValue,
            deliveryDate,
            distributionDate,
            items: this.items
        };

        if (this.isEditMode && this.editId) {
            this.securitiesService.update(this.editId, data).subscribe({
                next: (res: any) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت بروزرسانی شد');
                    this.router.navigate(['/securities/list']);
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در بروزرسانی اطلاعات');
                    console.error(err);
                }
            });
        } else {
            this.securitiesService.create(data).subscribe({
                next: (res: any) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت ثبت شد');
                    this.router.navigate(['/securities/list']);
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در ثبت اطلاعات');
                    console.error(err);
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
        this.items = [];
        this.selectedDocumentType = null;
        this.companyFound = false;
        this.companyNotFound = false;
        this.selectedCompanyId = null;
        
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

    searchCompanyByLicense(): void {
        const licenseNumber = this.securitiesForm.get('licenseNumber')?.value;
        
        if (!licenseNumber || !licenseNumber.trim()) {
            return;
        }

        // Reset states
        this.companySearching = true;
        this.companyFound = false;
        this.companyNotFound = false;
        this.selectedCompanyId = null;

        // Get user's province if available
        const provinceId = this.authService.getUserProvinceId();

        this.companyService.searchCompanyByLicense(licenseNumber.trim(), provinceId || undefined).subscribe({
            next: (companies) => {
                this.companySearching = false;
                
                if (companies && companies.length > 0) {
                    const company = companies[0]; // Take first match
                    this.companyFound = true;
                    this.companyNotFound = false;
                    this.selectedCompanyId = company.companyId || company.id;

                    // Auto-populate fields (handle both companyTitle and companyName)
                    this.securitiesForm.patchValue({
                        transactionGuideName: company.companyName || company.companyTitle || '',
                        licenseOwnerName: company.ownerName || '',
                        licenseOwnerFatherName: company.ownerFatherName || '',
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
    get registrationNumber() { return this.securitiesForm.get('registrationNumber'); }
    get licenseOwnerName() { return this.securitiesForm.get('licenseOwnerName'); }
    get licenseOwnerFatherName() { return this.securitiesForm.get('licenseOwnerFatherName'); }
    get transactionGuideName() { return this.securitiesForm.get('transactionGuideName'); }
    get licenseNumber() { return this.securitiesForm.get('licenseNumber'); }
}
