import { Component, OnInit, Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import {
    NgbDateStruct,
    NgbCalendar,
    NgbDatepickerI18n,
    NgbCalendarPersian,
} from '@ng-bootstrap/ng-bootstrap';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import {
    SecuritiesReportService,
    SecuritiesReportRequest,
    SecuritiesReportResponse,
    SecuritiesReportSummary,
    SecuritiesReportRow,
    ReportConfig
} from 'src/app/shared/securities-report.service';

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
    selector: 'app-securities-report',
    templateUrl: './securities-report.component.html',
    styleUrls: ['./securities-report.component.scss'],
    providers: [
        { provide: NgbCalendar, useClass: NgbCalendarPersian },
        { provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
    ],
})
export class SecuritiesReportComponent implements OnInit {
    // Filter form
    isFilterPanelOpen = true;
    isLoading = false;

    // Filter values
    transactionGuideName = '';
    licenseNumber = '';
    registrationNumber = '';
    documentType: number | null = null;
    registrationBookType: number | null = null;
    minAmount: number | null = null;
    maxAmount: number | null = null;

    // Report config
    config: ReportConfig | null = null;
    selectedMetrics: string[] = [];
    selectedGroupBy: string[] = [];

    // Report data
    summary: SecuritiesReportSummary | null = null;
    reportData: SecuritiesReportRow[] = [];
    byGuideData: any[] = [];
    byDocumentTypeData: any[] = [];
    byBookTypeData: any = null;
    monthlyTrendData: any[] = [];

    // Date filters
    startDate: any;
    endDate: any;

    // Active tab
    activeTab = 'summary';

    // Pagination
    page = 1;
    pageSize = 10;

    constructor(
        private toastr: ToastrService,
        private reportService: SecuritiesReportService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService
    ) { }

    ngOnInit(): void {
        this.loadConfig();
        this.loadSummary();
    }

    loadConfig(): void {
        this.reportService.getReportConfig().subscribe({
            next: (config) => {
                this.config = config;
            },
            error: (err) => {
                console.error('Error loading config:', err);
            }
        });
    }

    loadSummary(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getSummary(startDateStr, endDateStr, calendar).subscribe({
            next: (summary) => {
                this.summary = summary;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری خلاصه گزارش');
                this.isLoading = false;
            }
        });
    }

    loadByGuide(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getByGuide(startDateStr, endDateStr, calendar).subscribe({
            next: (response) => {
                this.byGuideData = response.data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری گزارش');
                this.isLoading = false;
            }
        });
    }

    loadByDocumentType(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getByDocumentType(startDateStr, endDateStr, calendar).subscribe({
            next: (data) => {
                this.byDocumentTypeData = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری گزارش');
                this.isLoading = false;
            }
        });
    }

    loadByBookType(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getByBookType(startDateStr, endDateStr, calendar).subscribe({
            next: (data) => {
                this.byBookTypeData = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری گزارش');
                this.isLoading = false;
            }
        });
    }

    loadMonthlyTrend(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        this.reportService.getMonthlyTrend(undefined, calendar).subscribe({
            next: (data) => {
                this.monthlyTrendData = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری گزارش');
                this.isLoading = false;
            }
        });
    }

    generateReport(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        const request: SecuritiesReportRequest = {
            metrics: this.selectedMetrics,
            groupBy: this.selectedGroupBy,
            startDate: this.formatDateForBackend(this.startDate),
            endDate: this.formatDateForBackend(this.endDate),
            transactionGuideName: this.transactionGuideName || undefined,
            licenseNumber: this.licenseNumber || undefined,
            registrationNumber: this.registrationNumber || undefined,
            documentType: this.documentType || undefined,
            registrationBookType: this.registrationBookType || undefined,
            minAmount: this.minAmount || undefined,
            maxAmount: this.maxAmount || undefined,
            calendarType: calendar
        };

        this.reportService.generateReport(request).subscribe({
            next: (response) => {
                this.summary = response.summary;
                this.reportData = response.data;
                this.isLoading = false;
                this.toastr.success('گزارش با موفقیت تولید شد');
            },
            error: (err) => {
                this.toastr.error('خطا در تولید گزارش');
                this.isLoading = false;
            }
        });
    }

    onTabChange(tab: string): void {
        this.activeTab = tab;
        switch (tab) {
            case 'summary':
                this.loadSummary();
                break;
            case 'byGuide':
                this.loadByGuide();
                break;
            case 'byDocumentType':
                this.loadByDocumentType();
                break;
            case 'byBookType':
                this.loadByBookType();
                break;
            case 'monthlyTrend':
                this.loadMonthlyTrend();
                break;
        }
    }

    applyFilters(): void {
        this.onTabChange(this.activeTab);
    }

    resetFilters(): void {
        this.transactionGuideName = '';
        this.licenseNumber = '';
        this.registrationNumber = '';
        this.documentType = null;
        this.registrationBookType = null;
        this.minAmount = null;
        this.maxAmount = null;
        this.startDate = null;
        this.endDate = null;
        this.selectedMetrics = [];
        this.selectedGroupBy = [];
        this.onTabChange(this.activeTab);
    }

    toggleMetric(metricId: string): void {
        const index = this.selectedMetrics.indexOf(metricId);
        if (index > -1) {
            this.selectedMetrics.splice(index, 1);
        } else {
            this.selectedMetrics.push(metricId);
        }
    }

    toggleGroupBy(groupId: string): void {
        const index = this.selectedGroupBy.indexOf(groupId);
        if (index > -1) {
            this.selectedGroupBy.splice(index, 1);
        } else {
            this.selectedGroupBy.push(groupId);
        }
    }

    isMetricSelected(metricId: string): boolean {
        return this.selectedMetrics.includes(metricId);
    }

    isGroupBySelected(groupId: string): boolean {
        return this.selectedGroupBy.includes(groupId);
    }

    exportToPdf(): void {
        window.print();
    }

    exportToExcel(): void {
        const calendar = this.calendarService.getSelectedCalendar();

        const request: SecuritiesReportRequest = {
            startDate: this.formatDateForBackend(this.startDate),
            endDate: this.formatDateForBackend(this.endDate),
            transactionGuideName: this.transactionGuideName || undefined,
            licenseNumber: this.licenseNumber || undefined,
            documentType: this.documentType || undefined,
            calendarType: calendar
        };

        this.reportService.exportReport(request).subscribe({
            next: (response) => {
                this.downloadAsExcel(response.data);
            },
            error: (err) => {
                this.toastr.error('خطا در صادرات گزارش');
            }
        });
    }

    private downloadAsExcel(data: any[]): void {
        // Convert to CSV format
        const headers = [
            'نمبر ثبت', 'نام صاحب امتیاز', 'نام رهنمای معاملات', 'نمبر جواز',
            'نوع سند', 'تعداد سته خرید و فروش جایداد', 'تعداد سته بیع وفا', 'تعداد سته کرایی',
            'تعداد سته وسایط', 'تعداد سته تبادله', 'تعداد کتاب ثبت', 'تعداد کتاب ثبت مثنی',
            'مبلغ سته‌ها', 'مبلغ کتاب ثبت', 'مبلغ مجموع', 'تاریخ توزیع'
        ];

        let csv = '\uFEFF' + headers.join(',') + '\n';
        data.forEach(row => {
            csv += [
                row.registrationNumber,
                row.licenseOwnerName,
                row.transactionGuideName,
                row.licenseNumber,
                row.documentType,
                row.propertySaleCount || 0,
                row.bayWafaCount || 0,
                row.rentCount || 0,
                row.vehicleSaleCount || 0,
                row.vehicleExchangeCount || 0,
                row.registrationBookCount || 0,
                row.duplicateBookCount || 0,
                row.totalDocumentsPrice || 0,
                row.registrationBookPrice || 0,
                row.totalSecuritiesPrice || 0,
                row.distributionDate || ''
            ].join(',') + '\n';
        });

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `securities-report-${new Date().toISOString().split('T')[0]}.csv`;
        link.click();
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

    formatNumber(value: number): string {
        return value?.toLocaleString('fa-AF') || '۰';
    }

    formatCurrency(value: number): string {
        return value?.toLocaleString('fa-AF') + ' افغانی' || '۰ افغانی';
    }

    // Helper methods for template calculations
    getByGuideTotal(field: string): number {
        if (!this.byGuideData || this.byGuideData.length === 0) return 0;
        return this.byGuideData.reduce((sum: number, row: any) => {
            switch (field) {
                case 'recordCount': return sum + (row.recordCount || 0);
                case 'propertyDocs': return sum + (row.propertySaleCount || 0) + (row.bayWafaCount || 0) + (row.rentCount || 0);
                case 'vehicleDocs': return sum + (row.vehicleSaleCount || 0) + (row.vehicleExchangeCount || 0);
                case 'books': return sum + (row.registrationBookCount || 0) + (row.duplicateBookCount || 0);
                case 'totalPrice': return sum + (row.totalSecuritiesPrice || 0);
                default: return sum;
            }
        }, 0);
    }

    getMonthlyTotal(field: string): number {
        if (!this.monthlyTrendData || this.monthlyTrendData.length === 0) return 0;
        return this.monthlyTrendData.reduce((sum: number, row: any) => {
            switch (field) {
                case 'recordCount': return sum + (row.recordCount || 0);
                case 'totalDocuments': return sum + (row.totalDocuments || 0);
                case 'totalBooks': return sum + (row.totalBooks || 0);
                case 'totalAmount': return sum + (row.totalAmount || 0);
                default: return sum;
            }
        }, 0);
    }
}