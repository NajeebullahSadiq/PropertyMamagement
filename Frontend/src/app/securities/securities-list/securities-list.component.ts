import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { SecuritiesService } from 'src/app/shared/securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { NotificationService } from 'src/app/shared/notification.service';
import { SecuritiesDistribution } from 'src/app/models/SecuritiesDistribution';

@Component({
    selector: 'app-securities-list',
    templateUrl: './securities-list.component.html',
    styleUrls: ['./securities-list.component.scss']
})
export class SecuritiesListComponent extends BaseComponent implements OnInit, OnDestroy {
    items: SecuritiesDistribution[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;
    pageSizes = [5, 10, 25, 50];
    searchTerm = '';
    isLoading = false;

    // RBAC
    canEdit = false;
    canDelete = false;
    canPrint = false;
    isViewOnly = false;

    // Report fields
    showReports = false;
    reportStartDate: any = '';
    reportEndDate: any = '';
    reportType: string = 'all'; // 'all' or 'single'
    reportLicenseNumber: string = '';
    reportData: any = null;
    isLoadingReport = false;

    errorMessage: string | null = null;

    constructor(
        private securitiesService: SecuritiesService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private rbacService: RbacService,
        private router: Router,
        private notification: NotificationService
    ) {
        super();
    }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadData();

        // Subscribe to data changes
        this.securitiesService.dataChanged$.pipe(takeUntil(this.destroy$)).subscribe(() => {
            this.loadData();
        });
    }

    override ngOnDestroy(): void {
        super.ngOnDestroy();
    }

    checkPermissions(): void {
        this.isViewOnly = this.rbacService.isViewOnly() || !this.rbacService.hasPermission('securities.create');
        this.canEdit = this.rbacService.canEditSecurities();
        this.canPrint = this.rbacService.hasPermission('securities.view');
        this.canDelete = this.rbacService.isAdmin();
    }

    loadData(): void {
        this.isLoading = true;
        this.errorMessage = null;
        const calendar = this.calendarService.getSelectedCalendar();

        // Call API with search parameter - backend will handle serial number range search
        this.securitiesService.getAll(this.page, this.pageSize, this.searchTerm, calendar).subscribe({
            next: (response) => {
                this.items = response.items;
                this.totalCount = response.totalCount;
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                this.errorMessage = 'خطا در بارگذاری اطلاعات اسناد بهادار. لطفاً دوباره تلاش کنید';
                this.notification.showHttpError(err);
            }
        });
    }

    onSearch(): void {
        this.page = 1;
        this.loadData(); // Call API with search term
    }

    onPageChange(page: number): void {
        this.page = page;
        this.loadData(); // Reload data when page changes
    }

    onPageSizeChange(): void {
        this.page = 1;
        this.loadData(); // Reload data when page size changes
    }

    viewDetails(id: number): void {
        this.router.navigate(['/securities/view', id]);
    }

    editItem(id: number): void {
        this.router.navigate(['/securities/edit', id]);
    }

    printItem(id: number): void {
        window.open(`/print/securities/${id}`, '_blank');
    }

    deleteItem(id: number): void {
        if (confirm('آیا مطمئن هستید که می‌خواهید این رکورد را حذف کنید؟')) {
            this.securitiesService.delete(id).subscribe({
                next: (res) => {
                    this.notification.showDeleteSuccess();
                    this.loadData();
                },
                error: (err) => {
                    this.notification.showHttpError(err);
                }
            });
        }
    }

    createNew(): void {
        this.router.navigate(['/securities']);
    }

    getTotalDocumentCount(item: SecuritiesDistribution): number {
        if (!item || !item.items) return 0;
        return item.items.reduce((sum, docItem) => sum + docItem.count, 0);
    }

    getDocumentTypesList(item: SecuritiesDistribution): string {
        if (!item || !item.items || item.items.length === 0) return '-';
        
        // Import DocumentTypes from the model
        const DocumentTypes = [
            { id: 1, name: 'سټه یی خرید و فروش' },
            { id: 2, name: 'سټه یی بیع وفا' },
            { id: 3, name: 'سټه یی کرایی' },
            { id: 4, name: 'سټه وسایط نقلیه' },
            { id: 5, name: 'کتاب ثبت' },
            { id: 6, name: 'کتاب ثبت مثنی' }
        ];
        
        // Get unique document types
        const uniqueTypes = [...new Set(item.items.map(i => i.documentType))];
        
        // Map to names
        const typeNames = uniqueTypes
            .map(typeId => {
                const docType = DocumentTypes.find(dt => dt.id === typeId);
                return docType ? docType.name : `نوع ${typeId}`;
            })
            .filter(name => name);
        
        return typeNames.join('، ') || '-';
    }

    // ==================== Reports ====================

    toggleReports(): void {
        this.showReports = !this.showReports;
        if (!this.showReports) {
            this.reportData = null;
        }
    }

    generateReport(): void {
        if (!this.reportStartDate || !this.reportEndDate) {
            this.notification.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
            return;
        }

        if (this.reportStartDate === '' || this.reportEndDate === '') {
            this.notification.warning('لطفاً تاریخ شروع و پایان را انتخاب کنید');
            return;
        }

        if (this.reportType === 'single' && !this.reportLicenseNumber) {
            this.notification.warning('لطفاً نمبر جواز را وارد کنید');
            return;
        }

        this.isLoadingReport = true;

        // Format dates in Hijri Shamsi
        const calendar = this.calendarService.getSelectedCalendar();
        const startDate = this.formatDateForBackend(this.reportStartDate);
        const endDate = this.formatDateForBackend(this.reportEndDate);

        const licenseNumber = this.reportType === 'single' ? this.reportLicenseNumber : undefined;

        this.securitiesService.getComprehensiveReport(
            startDate,
            endDate,
            calendar,
            licenseNumber
        ).subscribe({
            next: (data) => {
                this.reportData = data;
                this.isLoadingReport = false;
            },
            error: (err) => {
                this.notification.error('خطا در تولید گزارش');
                this.isLoadingReport = false;
                console.error('Report error:', err);
            }
        });
    }

    exportToExcel(): void {
        if (!this.reportData) {
            this.notification.warning('ابتدا گزارش را تولید کنید');
            return;
        }

        const excelData: any[] = [];
        
        // Header
        excelData.push(['گزارش اسناد بهادار رهنمای معاملات']);
        excelData.push([`از تاریخ: ${this.reportData.startDate} تا تاریخ: ${this.reportData.endDate}`]);
        excelData.push([`دفتر: ${this.reportData.licenseNumber}`]);
        excelData.push([]);
        
        // Document counts
        excelData.push(['تعداد اسناد']);
        excelData.push(['سټه خرید و فروش جایداد', this.reportData.propertyBuySellCount]);
        excelData.push(['سټه بیع وفا', this.reportData.bayeWafaCount]);
        excelData.push(['سټه کرایی', this.reportData.rentalCount]);
        excelData.push(['سټه وسایط نقلیه', this.reportData.vehicleCount]);
        excelData.push(['مجموع سته های 4 گانه', this.reportData.fourTypesTotal]);
        excelData.push(['کتاب ثبت معاملات', this.reportData.registrationBookCount]);
        excelData.push(['کتاب ثبت معاملات مثنی', this.reportData.registrationBookDuplicateCount]);
        excelData.push([]);
        
        // Revenue
        excelData.push(['مبلغ پول (افغانی)']);
        excelData.push(['سټه خرید و فروش جایداد', this.reportData.propertyBuySellRevenue]);
        excelData.push(['سټه بیع وفا', this.reportData.bayeWafaRevenue]);
        excelData.push(['سټه کرایی', this.reportData.rentalRevenue]);
        excelData.push(['سټه وسایط نقلیه', this.reportData.vehicleRevenue]);
        excelData.push(['مجموع سته های 4 گانه', this.reportData.fourTypesRevenue]);
        excelData.push(['کتاب ثبت معاملات', this.reportData.registrationBookRevenue]);
        excelData.push(['کتاب ثبت مثنی', this.reportData.registrationBookDuplicateRevenue]);
        excelData.push(['مجموع اسناد بهادار', this.reportData.totalRevenue]);
        
        // Convert to CSV
        const csv = excelData.map(row => row.join(',')).join('\n');
        const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        
        link.setAttribute('href', url);
        link.setAttribute('download', `securities-report-${Date.now()}.csv`);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        this.notification.success('گزارش با موفقیت صادر شد');
    }

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
}
