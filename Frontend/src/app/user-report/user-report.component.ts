import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { AuthService } from 'src/app/shared/auth.service';
import {
    UserReportService,
    UserReportSummary,
    ProvinceUserReport,
    InactiveUserDetail,
    RegistrationTrendItem,
    ExpiringLicenseItem
} from 'src/app/shared/user-report.service';
import { ConfirmationDialogComponent } from 'src/app/shared/confirmation-dialog/confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
    selector: 'app-user-report',
    templateUrl: './user-report.component.html',
    styleUrls: ['./user-report.component.scss']
})
export class UserReportComponent extends BaseComponent implements OnInit {
    // Loading state
    isLoading = false;

    // Active tab
    activeTab = 'summary';

    // Filter values
    startDate: any;
    endDate: any;
    provinceFilter: number | null = null;
    inactiveReasonFilter = '';

    // Provinces for dropdown
    provinces: any[] = [];

    // Summary data
    summary: UserReportSummary | null = null;

    // By province data
    byProvinceData: ProvinceUserReport[] = [];

    // Inactive users data
    inactiveUsers: InactiveUserDetail[] = [];
    inactiveTotal = 0;
    inactivePage = 1;
    inactivePageSize = 15;

    // Registration trend data
    registrationTrend: RegistrationTrendItem[] = [];

    // Expiring licenses data
    expiringLicenses: ExpiringLicenseItem[] = [];
    expiringTotal = 0;
    expiringPage = 1;
    expiringPageSize = 15;
    expiringDaysAhead = 30;

    // Auto-deactivation
    isAutoDeactivating = false;

    constructor(
        private toastr: ToastrService,
        private reportService: UserReportService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private authService: AuthService,
        private dialog: MatDialog
    ) {
        super();
    }

    ngOnInit(): void {
        this.loadProvinces();
        this.loadSummary();
    }

    loadProvinces(): void {
        this.authService.getProvinces().subscribe({
            next: (res: any) => { this.provinces = res || []; },
            error: () => { this.provinces = []; }
        });
    }

    // --- Tab switching ---
    onTabChange(tab: string): void {
        this.activeTab = tab;
        switch (tab) {
            case 'summary': this.loadSummary(); break;
            case 'byProvince': this.loadByProvince(); break;
            case 'inactive': this.loadInactiveUsers(); break;
            case 'trend': this.loadRegistrationTrend(); break;
            case 'expiring': this.loadExpiringLicenses(); break;
        }
    }

    // --- Date formatting ---
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
        const calendar = this.getCalendarType();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getSummary(
            startDateStr || undefined,
            endDateStr || undefined,
            calendar,
            this.provinceFilter || undefined
        ).subscribe({
            next: (data) => {
                this.summary = data;
                this.isLoading = false;
            },
            error: () => {
                this.toastr.error('خطا در بارگذاری خلاصه گزارش');
                this.isLoading = false;
            }
        });
    }

    loadByProvince(): void {
        this.isLoading = true;
        const calendar = this.getCalendarType();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getByProvince(
            startDateStr || undefined,
            endDateStr || undefined,
            calendar
        ).subscribe({
            next: (data) => {
                this.byProvinceData = data;
                this.isLoading = false;
            },
            error: () => {
                this.toastr.error('خطا در بارگذاری گزارش ولایات');
                this.isLoading = false;
            }
        });
    }

    loadInactiveUsers(): void {
        this.isLoading = true;
        const calendar = this.getCalendarType();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getInactiveUsers(
            this.inactiveReasonFilter || undefined,
            startDateStr || undefined,
            endDateStr || undefined,
            calendar,
            this.provinceFilter || undefined,
            this.inactivePage,
            this.inactivePageSize
        ).subscribe({
            next: (res: any) => {
                this.inactiveUsers = res.users;
                this.inactiveTotal = res.total;
                this.isLoading = false;
            },
            error: () => {
                this.toastr.error('خطا در بارگذاری کاربران غیرفعال');
                this.isLoading = false;
            }
        });
    }

    loadRegistrationTrend(): void {
        this.isLoading = true;
        const calendar = this.getCalendarType();
        const startDateStr = this.formatDateForBackend(this.startDate);
        const endDateStr = this.formatDateForBackend(this.endDate);

        this.reportService.getRegistrationTrend(
            startDateStr || undefined,
            endDateStr || undefined,
            calendar
        ).subscribe({
            next: (data) => {
                this.registrationTrend = data;
                this.isLoading = false;
            },
            error: () => {
                this.toastr.error('خطا در بارگذاری روند ثبت‌نام');
                this.isLoading = false;
            }
        });
    }

    loadExpiringLicenses(): void {
        this.isLoading = true;
        this.reportService.getExpiringLicenses(
            this.expiringDaysAhead,
            this.provinceFilter || undefined,
            this.expiringPage,
            this.expiringPageSize
        ).subscribe({
            next: (res: any) => {
                this.expiringLicenses = res.data;
                this.expiringTotal = res.total;
                this.isLoading = false;
            },
            error: () => {
                this.toastr.error('خطا در بارگذاری جوازهای در حال ختم');
                this.isLoading = false;
            }
        });
    }

    // --- Actions ---
    applyFilters(): void {
        this.onTabChange(this.activeTab);
    }

    resetFilters(): void {
        this.startDate = null;
        this.endDate = null;
        this.provinceFilter = null;
        this.inactiveReasonFilter = '';
        this.onTabChange(this.activeTab);
    }

    onInactivePageChange(page: number): void {
        this.inactivePage = page;
        this.loadInactiveUsers();
    }

    onExpiringPageChange(page: number): void {
        this.expiringPage = page;
        this.loadExpiringLicenses();
    }

    autoDeactivate(): void {
        const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
            data: {
                title: 'غیرفعال‌سازی خودکار کاربران',
                message: 'آیا مطمئن هستید که می‌خواهید کاربرانی که جواز آنها ختم شده یا فسخ/لغوه شده را غیرفعال کنید؟',
                confirmText: 'بله، غیرفعال شود',
                icon: 'fa-user-lock',
                confirmButtonClass: 'btn-confirm-warning'
            },
            disableClose: true
        });
        dialogRef.afterClosed().subscribe(confirmed => {
            if (confirmed) {
                this.isAutoDeactivating = true;
                this.reportService.autoDeactivate().subscribe({
                    next: (res: any) => {
                        this.toastr.success(res.message);
                        this.isAutoDeactivating = false;
                        this.onTabChange(this.activeTab);
                    },
                    error: () => {
                        this.toastr.error('خطا در غیرفعال‌سازی خودکار');
                        this.isAutoDeactivating = false;
                    }
                });
            }
        });
    }

    // --- Helpers ---
    formatNumber(value: number): string {
        return value?.toLocaleString('fa-AF') || '۰';
    }

    getInactiveTotalPages(): number {
        return Math.ceil(this.inactiveTotal / this.inactivePageSize);
    }

    getExpiringTotalPages(): number {
        return Math.ceil(this.expiringTotal / this.inactivePageSize);
    }

    getReasonBadgeClass(reason: string): string {
        switch (reason) {
            case 'locked': return 'bg-gray-100 text-gray-700';
            case 'expired': return 'bg-red-100 text-red-700';
            case 'cancelled': return 'bg-orange-100 text-orange-700';
            default: return 'bg-gray-100 text-gray-700';
        }
    }

    getReasonLabel(reason: string): string {
        switch (reason) {
            case 'locked': return 'قفل شده';
            case 'expired': return 'ختم جواز';
            case 'cancelled': return 'فسخ/لغوه';
            default: return reason;
        }
    }

    getDaysRemainingClass(days: number): string {
        if (days <= 7) return 'text-red-600 font-bold';
        if (days <= 15) return 'text-orange-600 font-semibold';
        return 'text-yellow-600';
    }

    exportToPdf(): void {
        window.print();
    }

    exportToExcel(): void {
        const headers = this.getExportHeaders();
        let csv = '\uFEFF' + headers.join(',') + '\n';

        if (this.activeTab === 'inactive') {
            this.inactiveUsers.forEach(u => {
                csv += [
                    u.userName, u.firstName, u.lastName, u.email,
                    u.companyName, u.licenseTypeDari, u.roleDari,
                    u.reasons.map(r => this.getReasonLabel(r)).join(' + '),
                    u.province?.dari || ''
                ].join(',') + '\n';
            });
        } else if (this.activeTab === 'expiring') {
            this.expiringLicenses.forEach(l => {
                csv += [
                    l.licenseNumber, l.licenseTypeDari, l.expireDate,
                    l.daysRemaining, l.company?.title || '',
                    l.province?.dari || '',
                    l.users.map(u => `${u.firstName} ${u.lastName}`).join('; ')
                ].join(',') + '\n';
            });
        } else if (this.activeTab === 'byProvince') {
            this.byProvinceData.forEach(p => {
                csv += [p.provinceDari, p.totalUsers, p.activeUsers, p.inactiveUsers].join(',') + '\n';
            });
        }

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `user-report-${new Date().toISOString().split('T')[0]}.csv`;
        link.click();
    }

    private getExportHeaders(): string[] {
        switch (this.activeTab) {
            case 'inactive':
                return ['نام کاربری', 'نام', 'نام خانوادگی', 'ایمیل', 'شرکت', 'نوع جواز', 'نقش', 'دلیل غیرفعالی', 'ولایت'];
            case 'expiring':
                return ['نمبر جواز', 'نوع جواز', 'تاریخ ختم', 'روز باقیمانده', 'شرکت', 'ولایت', 'کاربران'];
            case 'byProvince':
                return ['ولایت', 'مجموع کاربران', 'فعال', 'غیرفعال'];
            default:
                return ['گزارش کاربران'];
        }
    }
}
