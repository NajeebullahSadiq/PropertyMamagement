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
    PetitionWriterReportService,
    PetitionWriterReportRequest,
    PetitionWriterReportResponse,
    PetitionWriterReportSummary,
    PetitionWriterReportRow,
    PetitionWriterReportConfig,
    MonthlyTrendData,
    YearlyTrendData
} from 'src/app/shared/petition-writer-report.service';

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
    selector: 'app-petition-writer-report',
    templateUrl: './petition-writer-report.component.html',
    styleUrls: ['./petition-writer-report.component.scss'],
    providers: [
        { provide: NgbCalendar, useClass: NgbCalendarPersian },
        { provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
    ],
})
export class PetitionWriterReportComponent implements OnInit {
    // Math reference for template
    Math = Math;
    
    // Filter panel state
    isFilterPanelOpen = true;
    isLoading = false;

    // Filter values
    petitionWriterName = '';
    licenseNumber = '';
    registrationNumber = '';
    minAmount: number | null = null;
    maxAmount: number | null = null;
    minCount: number | null = null;
    maxCount: number | null = null;

    // Report config
    config: PetitionWriterReportConfig | null = null;
    selectedMetrics: string[] = [];
    selectedGroupBy: string[] = [];

    // Report data
    summary: PetitionWriterReportSummary | null = null;
    reportData: PetitionWriterReportRow[] = [];
    byWriterData: PetitionWriterReportRow[] = [];
    byLicenseData: PetitionWriterReportRow[] = [];
    monthlyTrendData: MonthlyTrendData[] = [];
    yearlyTrendData: YearlyTrendData[] = [];
    serialTrackingData: any[] = [];
    detailedListData: any[] = [];

    // Date filters
    startDate: any;
    endDate: any;

    // Active tab
    activeTab = 'summary';

    // Pagination for detailed list
    page = 1;
    pageSize = 20;
    totalCount = 0;
    totalPages = 0;

    constructor(
        private toastr: ToastrService,
        private reportService: PetitionWriterReportService,
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

    loadByWriter(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getByWriter(startDateStr, endDateStr, calendar).subscribe({
            next: (response) => {
                this.byWriterData = response.data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری گزارش');
                this.isLoading = false;
            }
        });
    }

    loadByLicense(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getByLicense(startDateStr, endDateStr, calendar).subscribe({
            next: (response) => {
                this.byLicenseData = response.data;
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

    loadYearlyTrend(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        this.reportService.getYearlyTrend(calendar).subscribe({
            next: (data) => {
                this.yearlyTrendData = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری گزارش');
                this.isLoading = false;
            }
        });
    }

    loadSerialTracking(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        this.reportService.getSerialTracking(
            this.licenseNumber || undefined,
            this.petitionWriterName || undefined,
            calendar
        ).subscribe({
            next: (data) => {
                this.serialTrackingData = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری گزارش');
                this.isLoading = false;
            }
        });
    }

    loadDetailedList(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getDetailedList(
            startDateStr,
            endDateStr,
            this.petitionWriterName || undefined,
            this.licenseNumber || undefined,
            this.registrationNumber || undefined,
            this.page,
            this.pageSize,
            calendar
        ).subscribe({
            next: (response) => {
                this.detailedListData = response.data;
                this.totalCount = response.totalCount;
                this.totalPages = response.totalPages;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری لیست');
                this.isLoading = false;
            }
        });
    }

    generateReport(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        const request: PetitionWriterReportRequest = {
            metrics: this.selectedMetrics,
            groupBy: this.selectedGroupBy,
            startDate: this.formatDateForBackend(this.startDate),
            endDate: this.formatDateForBackend(this.endDate),
            petitionWriterName: this.petitionWriterName || undefined,
            licenseNumber: this.licenseNumber || undefined,
            registrationNumber: this.registrationNumber || undefined,
            minAmount: this.minAmount || undefined,
            maxAmount: this.maxAmount || undefined,
            minCount: this.minCount || undefined,
            maxCount: this.maxCount || undefined,
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
            case 'byWriter':
                this.loadByWriter();
                break;
            case 'byLicense':
                this.loadByLicense();
                break;
            case 'monthlyTrend':
                this.loadMonthlyTrend();
                break;
            case 'yearlyTrend':
                this.loadYearlyTrend();
                break;
            case 'serialTracking':
                this.loadSerialTracking();
                break;
            case 'detailedList':
                this.loadDetailedList();
                break;
        }
    }

    applyFilters(): void {
        this.page = 1;
        this.onTabChange(this.activeTab);
    }

    resetFilters(): void {
        this.petitionWriterName = '';
        this.licenseNumber = '';
        this.registrationNumber = '';
        this.minAmount = null;
        this.maxAmount = null;
        this.minCount = null;
        this.maxCount = null;
        this.startDate = null;
        this.endDate = null;
        this.selectedMetrics = [];
        this.selectedGroupBy = [];
        this.page = 1;
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

    onPageChange(newPage: number): void {
        this.page = newPage;
        this.loadDetailedList();
    }

    exportToPdf(): void {
        window.print();
    }

    exportToExcel(): void {
        const calendar = this.calendarService.getSelectedCalendar();

        const request: PetitionWriterReportRequest = {
            startDate: this.formatDateForBackend(this.startDate),
            endDate: this.formatDateForBackend(this.endDate),
            petitionWriterName: this.petitionWriterName || undefined,
            licenseNumber: this.licenseNumber || undefined,
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
        // Convert to CSV format with proper headers
        const headers = [
            'نمبر ثبت تعرفه',
            'اسم عریضه‌نویس',
            'ولد',
            'نمبر جواز',
            'تعداد عریضه',
            'مبلغ پول',
            'آویز نمبر بانکی',
            'آغاز سریال نمبر',
            'ختم سریال نمبر',
            'تاریخ توزیع'
        ];

        let csv = '\uFEFF' + headers.join(',') + '\n';
        data.forEach(row => {
            csv += [
                row.registrationNumber || '',
                row.petitionWriterName || '',
                row.petitionWriterFatherName || '',
                row.licenseNumber || '',
                row.petitionCount || 0,
                row.amount || 0,
                row.bankReceiptNumber || '',
                row.serialNumberStart || '',
                row.serialNumberEnd || '',
                row.distributionDate || ''
            ].join(',') + '\n';
        });

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `petition-writer-report-${new Date().toISOString().split('T')[0]}.csv`;
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
    getByWriterTotal(field: string): number {
        if (!this.byWriterData || this.byWriterData.length === 0) return 0;
        return this.byWriterData.reduce((sum: number, row: any) => {
            switch (field) {
                case 'recordCount': return sum + (row.recordCount || 0);
                case 'petitionCount': return sum + (row.totalPetitionCount || 0);
                case 'totalAmount': return sum + (row.totalAmount || 0);
                default: return sum;
            }
        }, 0);
    }

    getByLicenseTotal(field: string): number {
        if (!this.byLicenseData || this.byLicenseData.length === 0) return 0;
        return this.byLicenseData.reduce((sum: number, row: any) => {
            switch (field) {
                case 'recordCount': return sum + (row.recordCount || 0);
                case 'petitionCount': return sum + (row.totalPetitionCount || 0);
                case 'totalAmount': return sum + (row.totalAmount || 0);
                default: return sum;
            }
        }, 0);
    }

    getMonthlyTotal(field: string): number {
        if (!this.monthlyTrendData || this.monthlyTrendData.length === 0) return 0;
        return this.monthlyTrendData.reduce((sum: number, row: any) => {
            switch (field) {
                case 'recordCount': return sum + (row.recordCount || 0);
                case 'totalPetitionCount': return sum + (row.totalPetitionCount || 0);
                case 'totalAmount': return sum + (row.totalAmount || 0);
                default: return sum;
            }
        }, 0);
    }

    getYearlyTotal(field: string): number {
        if (!this.yearlyTrendData || this.yearlyTrendData.length === 0) return 0;
        return this.yearlyTrendData.reduce((sum: number, row: any) => {
            switch (field) {
                case 'recordCount': return sum + (row.recordCount || 0);
                case 'totalPetitionCount': return sum + (row.totalPetitionCount || 0);
                case 'totalAmount': return sum + (row.totalAmount || 0);
                case 'uniqueWriters': return sum + (row.uniqueWriters || 0);
                case 'uniqueLicenses': return sum + (row.uniqueLicenses || 0);
                default: return sum;
            }
        }, 0);
    }
}
