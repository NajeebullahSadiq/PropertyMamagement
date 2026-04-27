import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { LicenseApplicationService } from 'src/app/shared/license-application.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { LicenseApplication } from 'src/app/models/LicenseApplication';

@Component({
    selector: 'app-license-application-list',
    templateUrl: './license-application-list.component.html',
    styleUrls: ['./license-application-list.component.scss']
})
export class LicenseApplicationListComponent extends BaseComponent implements OnInit, OnDestroy {
    items: LicenseApplication[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 100;
    pageSizes = [10, 25, 50, 100, 200];
    searchTerm = '';
    isLoading = false;

    // Math for template
    Math = Math;

    // Advanced search fields
    showAdvancedSearch = false;
    searchSerialNumber = '';
    searchRequestDate = '';
    searchApplicantName = '';
    searchApplicantFatherName = '';
    searchProposedGuideName = '';
    searchElectronicNumber = '';
    searchShariaDeedNumber = '';
    searchCustomaryDeedSerial = '';
    searchGuarantorName = '';
    searchGuarantorFatherName = '';

    // Report fields
    showReports = false;
    reportStartDate: any = '';
    reportEndDate: any = '';
    reportData: any = null;
    isLoadingReport = false;

    // RBAC
    canEdit = false;
    canDelete = false;
    isViewOnly = false;

    private searchSubject = new Subject<string>();

    constructor(
        private licenseAppService: LicenseApplicationService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private rbacService: RbacService,
        private router: Router,
        private toastr: ToastrService
    ) {
        super();
        // Setup debounced search
        this.searchSubject.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            takeUntil(this.destroy$)
        ).subscribe(searchTerm => {
            this.searchTerm = searchTerm;
            this.page = 1;
            this.loadData();
        });
    }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadData();

        this.licenseAppService.dataChanged$.pipe(takeUntil(this.destroy$)).subscribe(() => {
            this.loadData();
        });
    }

    override ngOnDestroy(): void {
        this.searchSubject.complete();
        super.ngOnDestroy();
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.isViewOnly = role === UserRoles.Authority || 
                          role === UserRoles.LicenseReviewer || 
                          role === UserRoles.CompanyRegistrar;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.LicenseApplicationManager;
        this.canDelete = role === UserRoles.Admin || role === UserRoles.LicenseApplicationManager;
    }

    loadData(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        this.licenseAppService.getAll(this.page, this.pageSize, this.searchTerm, calendar).subscribe({
            next: (response) => {
                this.items = response.items;
                this.totalCount = response.totalCount;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    onSearch(): void {
        this.searchSubject.next(this.searchTerm);
    }

    toggleAdvancedSearch(): void {
        this.showAdvancedSearch = !this.showAdvancedSearch;
        if (!this.showAdvancedSearch) {
            this.clearAdvancedSearch();
        }
    }

    performAdvancedSearch(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        // If no advanced search criteria, just load normal data
        if (!this.hasAdvancedSearchCriteria()) {
            this.loadData();
            return;
        }

        this.licenseAppService.search(
            this.searchSerialNumber || undefined,
            this.searchRequestDate || undefined,
            this.searchApplicantName || undefined,
            this.searchApplicantFatherName || undefined,
            this.searchProposedGuideName || undefined,
            this.searchElectronicNumber || undefined,
            this.searchShariaDeedNumber || undefined,
            this.searchCustomaryDeedSerial || undefined,
            this.searchGuarantorName || undefined,
            this.searchGuarantorFatherName || undefined,
            this.page,
            this.pageSize,
            calendar
        ).subscribe({
            next: (response) => {
                this.items = response.items;
                this.totalCount = response.totalCount;
                this.isLoading = false;
                
                if (this.totalCount === 0) {
                    this.toastr.info('هیچ نتیجه‌ای یافت نشد');
                }
            },
            error: (err) => {
                this.toastr.error('خطا در جستجو');
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    clearAdvancedSearch(): void {
        this.searchSerialNumber = '';
        this.searchRequestDate = '';
        this.searchApplicantName = '';
        this.searchApplicantFatherName = '';
        this.searchProposedGuideName = '';
        this.searchElectronicNumber = '';
        this.searchShariaDeedNumber = '';
        this.searchCustomaryDeedSerial = '';
        this.searchGuarantorName = '';
        this.searchGuarantorFatherName = '';
        this.searchTerm = '';
        this.page = 1;
        this.loadData();
    }

    onPageChange(page: number): void {
        this.page = page;
        if (this.showAdvancedSearch && this.hasAdvancedSearchCriteria()) {
            this.performAdvancedSearch();
        } else {
            this.loadData();
        }
    }

    onPageSizeChange(): void {
        this.page = 1;
        if (this.showAdvancedSearch && this.hasAdvancedSearchCriteria()) {
            this.performAdvancedSearch();
        } else {
            this.loadData();
        }
    }

    hasAdvancedSearchCriteria(): boolean {
        return !!(
            this.searchSerialNumber ||
            this.searchRequestDate ||
            this.searchApplicantName ||
            this.searchApplicantFatherName ||
            this.searchProposedGuideName ||
            this.searchElectronicNumber ||
            this.searchShariaDeedNumber ||
            this.searchCustomaryDeedSerial ||
            this.searchGuarantorName ||
            this.searchGuarantorFatherName
        );
    }

    totalPages(): number {
        return Math.ceil(this.totalCount / this.pageSize);
    }

    isLastPage(): boolean {
        return this.page >= this.totalPages();
    }

    getGuarantorNames(item: LicenseApplication): string {
        if (!item.guarantors || item.guarantors.length == 0) return '-';
        return item.guarantors.map(g => g.guarantorName).join('، ');
    }

    getGuarantorFatherNames(item: LicenseApplication): string {
        if (!item.guarantors || item.guarantors.length == 0) return '-';
        return item.guarantors.map(g => g.guarantorFatherName || '-').join('، ');
    }

    getGuarantorTypeNames(item: LicenseApplication): string {
        if (!item.guarantors || item.guarantors.length == 0) return '-';
        return item.guarantors.map(g => g.guaranteeTypeName || '-').join('، ');
    }

    getGuarantorLocations(item: LicenseApplication): string {
        if (!item.guarantors || item.guarantors.length == 0) return '-';
        const locations = item.guarantors
            .map(g => g.guaranteeLocation || '')
            .filter(l => l.length > 0);
        return locations.length > 0 ? locations.join('، ') : '-';
    }

    getStatusText(item: LicenseApplication): string {
        return item.isWithdrawn ? 'منصرف‌شده' : 'فعال';
    }

    getStatusClass(item: LicenseApplication): string {
        return item.isWithdrawn
            ? 'bg-red-100 text-red-800'
            : 'bg-green-100 text-green-800';
    }

    viewDetails(id: number): void {
        this.router.navigate(['/license-applications/view', id]);
    }

    editItem(id: number): void {
        this.router.navigate(['/license-applications/edit', id]);
    }

    deleteItem(id: number): void {
        if (!confirm('آیا مطمئن هستید که می‌خواهید این درخواست را حذف کنید؟')) {
            return;
        }

        this.licenseAppService.delete(id).subscribe({
            next: () => {
                this.toastr.success('درخواست موفقانه حذف شد');
                this.loadData();
            },
            error: (err) => {
                this.toastr.error('خطا در حذف درخواست');
                console.error(err);
            }
        });
    }

    createNew(): void {
        this.licenseAppService.resetMainTableId();
        this.router.navigate(['/license-applications']);
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
            this.toastr.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
            return;
        }

        this.isLoadingReport = true;

        // Format dates in Hijri Shamsi
        const calendar = this.calendarService.getSelectedCalendar();
        const startDate = this.formatDateForBackend(this.reportStartDate);
        const endDate = this.formatDateForBackend(this.reportEndDate);

        this.licenseAppService.getComprehensiveReport(
            startDate,
            endDate,
            calendar,
        ).subscribe({
            next: (data) => {
                this.reportData = data;
                this.isLoadingReport = false;
            },
            error: (err) => {
                this.toastr.error('خطا در تولید گزارش');
                this.isLoadingReport = false;
                console.error(err);
            }
        });
    }

    exportToExcel(): void {
        if (!this.reportData) {
            this.toastr.warning('ابتدا گزارش را تولید کنید');
            return;
        }

        // Create Excel data
        const excelData: any[] = [];
        
        // Header
        excelData.push(['گزارش درخواست‌های جواز رهنمای معاملات']);
        excelData.push([`از تاریخ: ${this.reportData.startDate} تا تاریخ: ${this.reportData.endDate}`]);
        excelData.push([]);
        
        // Applicants count
        excelData.push(['تعداد متقاضیان', this.reportData.totalApplicants]);
        excelData.push([]);
        
        // Guarantors by type
        excelData.push(['تعداد تضمین‌کنندگان بر اساس نوع']);
        excelData.push(['نوع تضمین', 'تعداد']);
        this.reportData.guarantorsByType.forEach((item: any) => {
            excelData.push([item.guaranteeTypeName, item.count]);
        });
        excelData.push(['مجموع تضمین‌کنندگان', this.reportData.totalGuarantors]);
        excelData.push([]);
        
        // Withdrawals
        excelData.push(['تعداد انصراف‌ها', this.reportData.totalWithdrawals]);
        
        // Convert to CSV
        const csv = excelData.map(row => row.join(',')).join('\n');
        const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        
        link.setAttribute('href', url);
        link.setAttribute('download', `license-applications-report-${Date.now()}.csv`);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        this.toastr.success('گزارش با موفقیت صادر شد');
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
