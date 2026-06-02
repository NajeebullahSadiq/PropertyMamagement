import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import {
    EstateReportService,
    EstateReportSummary,
    TransactionTypeSummary,
    CompanyReportItem,
    ProvinceReportItem,
    MonthlyTrendItem,
    TransactionTypeDetailReport
} from 'src/app/shared/estate-report.service';

@Component({
    selector: 'app-estate-report',
    templateUrl: './estate-report.component.html',
    styleUrls: ['./estate-report.component.scss']
})
export class EstateReportComponent extends BaseComponent implements OnInit {
    isLoading = false;
    activeTab = 'summary';

    // Filters
    startDate: any;
    endDate: any;
    provinceFilter: number | null = null;
    districtFilter: number | null = null;
    companyFilter: number | null = null;

    // Dropdown data
    provinces: any[] = [];
    districts: any[] = [];
    companies: any[] = [];

    // Report data
    summary: EstateReportSummary | null = null;
    byCompanyData: CompanyReportItem[] = [];
    byProvinceData: ProvinceReportItem[] = [];
    monthlyTrendData: MonthlyTrendItem[] = [];
    transactionDetail: TransactionTypeDetailReport | null = null;
    selectedTransactionTypeId: number | null = null;
    detailPage = 1;
    detailPageSize = 20;

    // Expanded company in byCompany view
    expandedCompanyId: number | null = null;

    constructor(
        private toastr: ToastrService,
        private reportService: EstateReportService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService
    ) {
        super();
    }

    ngOnInit(): void {
        this.loadDropdowns();
        this.loadSummary();
    }

    loadDropdowns(): void {
        this.reportService.getProvinces().subscribe({ next: (res: any) => { this.provinces = res || []; }, error: () => {} });
        this.reportService.getCompanies().subscribe({ next: (res: any) => { this.companies = res || []; }, error: () => {} });
    }

    onProvinceChange(): void {
        this.districtFilter = null;
        this.districts = [];
        if (this.provinceFilter) {
            this.reportService.getDistricts(this.provinceFilter).subscribe({
                next: (res: any) => { this.districts = res || []; },
                error: () => { this.districts = []; }
            });
        }
    }

    onTabChange(tab: string): void {
        this.activeTab = tab;
        switch (tab) {
            case 'summary': this.loadSummary(); break;
            case 'byCompany': this.loadByCompany(); break;
            case 'byProvince': this.loadByProvince(); break;
            case 'trend': this.loadMonthlyTrend(); break;
        }
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

    private getCalendarType(): string {
        return this.calendarService.getSelectedCalendar();
    }

    // --- Load data ---
    loadSummary(): void {
        this.isLoading = true;
        this.reportService.getSummary(
            this.formatDateForBackend(this.startDate) || undefined,
            this.formatDateForBackend(this.endDate) || undefined,
            this.getCalendarType(),
            this.provinceFilter || undefined,
            this.districtFilter || undefined,
            this.companyFilter || undefined
        ).subscribe({
            next: (data) => { this.summary = data; this.isLoading = false; },
            error: () => { this.toastr.error('خطا در بارگذاری خلاصه گزارش'); this.isLoading = false; }
        });
    }

    loadByCompany(): void {
        this.isLoading = true;
        this.reportService.getByCompany(
            this.formatDateForBackend(this.startDate) || undefined,
            this.formatDateForBackend(this.endDate) || undefined,
            this.getCalendarType(),
            this.provinceFilter || undefined,
            this.districtFilter || undefined
        ).subscribe({
            next: (data) => { this.byCompanyData = data; this.isLoading = false; },
            error: () => { this.toastr.error('خطا در بارگذاری گزارش رهنماها'); this.isLoading = false; }
        });
    }

    loadByProvince(): void {
        this.isLoading = true;
        this.reportService.getByProvince(
            this.formatDateForBackend(this.startDate) || undefined,
            this.formatDateForBackend(this.endDate) || undefined,
            this.getCalendarType()
        ).subscribe({
            next: (data) => { this.byProvinceData = data; this.isLoading = false; },
            error: () => { this.toastr.error('خطا در بارگذاری گزارش ولایات'); this.isLoading = false; }
        });
    }

    loadMonthlyTrend(): void {
        this.isLoading = true;
        this.reportService.getMonthlyTrend(
            this.formatDateForBackend(this.startDate) || undefined,
            this.formatDateForBackend(this.endDate) || undefined,
            this.getCalendarType(),
            this.companyFilter || undefined
        ).subscribe({
            next: (data) => { this.monthlyTrendData = data; this.isLoading = false; },
            error: () => { this.toastr.error('خطا در بارگذاری روند ماهانه'); this.isLoading = false; }
        });
    }

    loadTransactionDetail(transactionTypeId: number): void {
        this.selectedTransactionTypeId = transactionTypeId;
        this.activeTab = 'detail';
        this.isLoading = true;
        this.reportService.getByTransactionType(
            transactionTypeId,
            this.formatDateForBackend(this.startDate) || undefined,
            this.formatDateForBackend(this.endDate) || undefined,
            this.getCalendarType(),
            this.provinceFilter || undefined,
            this.districtFilter || undefined,
            this.companyFilter || undefined,
            this.detailPage,
            this.detailPageSize
        ).subscribe({
            next: (data) => { this.transactionDetail = data; this.isLoading = false; },
            error: () => { this.toastr.error('خطا در بارگذاری جزئیات'); this.isLoading = false; }
        });
    }

    onDetailPageChange(page: number): void {
        this.detailPage = page;
        if (this.selectedTransactionTypeId) this.loadTransactionDetail(this.selectedTransactionTypeId);
    }

    // --- Actions ---
    applyFilters(): void {
        this.onTabChange(this.activeTab === 'detail' ? 'summary' : this.activeTab);
    }

    resetFilters(): void {
        this.startDate = null;
        this.endDate = null;
        this.provinceFilter = null;
        this.districtFilter = null;
        this.companyFilter = null;
        this.districts = [];
        this.onTabChange(this.activeTab === 'detail' ? 'summary' : this.activeTab);
    }

    toggleCompanyExpand(companyId: number): void {
        this.expandedCompanyId = this.expandedCompanyId === companyId ? null : companyId;
    }

    // --- Helpers ---
    formatNumber(value: number): string {
        return value?.toLocaleString('fa-AF') || '۰';
    }

    formatCurrency(value: number): string {
        if (!value) return '۰ افغانی';
        return value.toLocaleString('fa-AF') + ' افغانی';
    }

    getDetailTotalPages(): number {
        if (!this.transactionDetail) return 0;
        return Math.ceil(this.transactionDetail.total / this.detailPageSize);
    }

    exportToPdf(): void {
        window.print();
    }

    exportToExcel(): void {
        const headers = this.getExportHeaders();
        let csv = '\uFEFF' + headers.join(',') + '\n';

        if (this.activeTab === 'summary' && this.summary) {
            this.summary.byTransactionType.forEach(t => {
                csv += [t.transactionTypeDari, t.count, this.formatNumber(t.totalPrice), this.formatNumber(t.totalRoyaltyAmount)].join(',') + '\n';
            });
        } else if (this.activeTab === 'byCompany') {
            this.byCompanyData.forEach(c => {
                csv += [c.companyTitle, c.totalRecords, this.formatNumber(c.totalPrice), this.formatNumber(c.totalRoyaltyAmount)].join(',') + '\n';
                c.byTransactionType.forEach(t => {
                    csv += ['  ' + t.transactionTypeDari, t.count, this.formatNumber(t.totalPrice), this.formatNumber(t.totalRoyaltyAmount)].join(',') + '\n';
                });
            });
        } else if (this.activeTab === 'byProvince') {
            this.byProvinceData.forEach(p => {
                csv += [p.provinceDari, p.totalRecords, this.formatNumber(p.totalPrice), this.formatNumber(p.totalRoyaltyAmount)].join(',') + '\n';
            });
        } else if (this.activeTab === 'trend') {
            this.monthlyTrendData.forEach(t => {
                csv += [t.monthLabel, t.totalRecords, this.formatNumber(t.totalPrice), this.formatNumber(t.totalRoyaltyAmount)].join(',') + '\n';
            });
        } else if (this.activeTab === 'detail' && this.transactionDetail) {
            this.transactionDetail.records.forEach(r => {
                csv += [r.pnumber, r.transactionTypeDari, r.price, r.royaltyAmount, r.companyTitle || '—', r.sellerName || '—', r.buyerName || '—', r.createdBy || '—'].join(',') + '\n';
            });
        }

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `estate-report-${new Date().toISOString().split('T')[0]}.csv`;
        link.click();
    }

    private getExportHeaders(): string[] {
        switch (this.activeTab) {
            case 'summary': return ['نوعیت معامله', 'تعداد', 'مجموع قیمت', 'مجموع حق‌العمل'];
            case 'byCompany': return ['نام رهنما', 'تعداد', 'مجموع قیمت', 'مجموع حق‌العمل'];
            case 'byProvince': return ['ولایت', 'تعداد', 'مجموع قیمت', 'مجموع حق‌العمل'];
            case 'trend': return ['ماه', 'تعداد', 'مجموع قیمت', 'مجموع حق‌العمل'];
            case 'detail': return ['شماره مسلسل', 'نوعیت معامله', 'قیمت', 'حق‌العمل', 'رهنما', 'فروشنده', 'مشتری', 'ثبت / ویرایش کننده'];
            default: return ['گزارش املاک'];
        }
    }
}
