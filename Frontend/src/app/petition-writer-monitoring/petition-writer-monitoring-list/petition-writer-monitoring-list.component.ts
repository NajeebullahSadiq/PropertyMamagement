import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { PetitionWriterMonitoringService } from 'src/app/shared/petition-writer-monitoring.service';
import { PetitionWriterMonitoringReportService, PetitionWriterMonitoringReportSummary, ComplaintDetail, ViolationDetail, MonitoringDetail, ReportUser } from 'src/app/shared/petition-writer-monitoring-report.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { PetitionWriterMonitoringRecord, PetitionWriterMonitoringSectionTypes, ActivityStatusOptions } from 'src/app/models/PetitionWriterMonitoring';

@Component({
    selector: 'app-petition-writer-monitoring-list',
    templateUrl: './petition-writer-monitoring-list.component.html',
    styleUrls: ['./petition-writer-monitoring-list.component.scss']
})
export class PetitionWriterMonitoringListComponent extends BaseComponent implements OnInit {
    items: PetitionWriterMonitoringRecord[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;
    pageSizes = [5, 10, 25, 50];
    searchText = '';
    selectedSectionType = '';
    sectionTypes = PetitionWriterMonitoringSectionTypes;
    activityStatusOptions = ActivityStatusOptions;
    isLoading = false;

    canCreate = false;
    canEdit = false;
    canDelete = false;

    // Report mode
    showReport = false;
    reportLoading = false;
    reportSummary: PetitionWriterMonitoringReportSummary | null = null;
    reportUsers: ReportUser[] = [];

    // Report filters
    reportStartDate: any;
    reportEndDate: any;
    reportCreatedBy = '';
    reportActiveTab = 'overall';

    // Report detail data
    complaintDetails: ComplaintDetail[] = [];
    violationDetails: ViolationDetail[] = [];
    monitoringDetails: MonitoringDetail[] = [];

    constructor(
        private router: Router,
        private service: PetitionWriterMonitoringService,
        private reportService: PetitionWriterMonitoringReportService,
        private toastr: ToastrService,
        private calendarService: CalendarService,
        private calendarConversionService: CalendarConversionService,
        private rbacService: RbacService
    ) {
        super();
    }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadData();
        this.service.dataChanged$.pipe(takeUntil(this.destroy$)).subscribe(() => this.loadData());
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.canCreate = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar || role === UserRoles.ActivityMonitoringManager;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar || role === UserRoles.ActivityMonitoringManager;
        this.canDelete = role === UserRoles.Admin;
    }

    loadData(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        this.service.getAll(this.page, this.pageSize, this.searchText, this.selectedSectionType, calendar).subscribe({
            next: (response) => {
                this.items = response.items;
                this.totalCount = response.totalCount;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
                this.isLoading = false;
            }
        });
    }

    onSearch(): void {
        this.page = 1;
        this.loadData();
    }

    onSectionTypeChange(): void {
        this.page = 1;
        this.loadData();
    }

    onPageChange(pageNumber: number): void {
        this.page = pageNumber;
        this.loadData();
    }

    onPageSizeChange(): void {
        this.page = 1;
        this.loadData();
    }

    goToForm(): void {
        this.router.navigate(['/petition-writer-monitoring/form']);
    }

    viewRecord(id: number): void {
        this.router.navigate(['/petition-writer-monitoring/view', id]);
    }

    editRecord(id: number): void {
        this.router.navigate(['/petition-writer-monitoring/form', id]);
    }

    deleteRecord(id: number): void {
        if (confirm('آیا مطمئن هستید که می‌خواهید این رکورد را حذف کنید؟')) {
            this.service.delete(id).subscribe({
                next: () => {
                    this.toastr.success('رکورد موفقانه حذف شد');
                    this.loadData();
                },
                error: (err) => {
                    this.toastr.error('خطا در حذف رکورد');
                    console.error(err);
                }
            });
        }
    }

    getSectionTypeLabel(sectionType: string): string {
        const found = this.sectionTypes.find(s => s.value === sectionType);
        return found ? found.label : '-';
    }

    getActivityStatusLabel(activityStatus: string): string {
        const found = this.activityStatusOptions.find(s => s.value === activityStatus);
        return found ? found.label : '-';
    }

    getActivityStatusClass(activityStatus: string): string {
        switch (activityStatus) {
            case 'activity_prevention':
                return 'bg-red-100 text-red-700';
            case 'activity_permission':
                return 'bg-green-100 text-green-700';
            default:
                return 'bg-gray-100 text-gray-700';
        }
    }

    getSectionTypeClass(sectionType: string): string {
        switch (sectionType) {
            case 'complaints':
                return 'bg-red-100 text-red-700';
            case 'violations':
                return 'bg-orange-100 text-orange-700';
            case 'monitoring':
                return 'bg-green-100 text-green-700';
            default:
                return 'bg-gray-100 text-gray-700';
        }
    }

    getColspan(): number {
        switch (this.selectedSectionType) {
            case 'complaints':
                return 9;
            case 'violations':
                return 7;
            case 'monitoring':
                return 5;
            default:
                return 9;
        }
    }

    // ============ Report Methods ============

    toggleReport(): void {
        this.showReport = !this.showReport;
        if (this.showReport) {
            this.loadReportUsers();
            this.loadReportData();
        }
    }

    loadReportUsers(): void {
        this.reportService.getUsers().subscribe({
            next: (users) => { this.reportUsers = users; },
            error: (err) => console.error('Error loading report users:', err)
        });
    }

    loadReportData(): void {
        this.reportLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.reportStartDate);
        const endDateStr = this.formatDateForBackend(this.reportEndDate);

        this.reportService.getSummary(startDateStr, endDateStr, this.reportCreatedBy || undefined, calendar).subscribe({
            next: (summary) => {
                this.reportSummary = summary;
                this.reportLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری گزارش');
                console.error(err);
                this.reportLoading = false;
            }
        });

        this.loadReportTabData();
    }

    loadReportTabData(): void {
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.reportStartDate);
        const endDateStr = this.formatDateForBackend(this.reportEndDate);
        const createdBy = this.reportCreatedBy || undefined;

        switch (this.reportActiveTab) {
            case 'complaints':
                this.reportService.getComplaints(startDateStr, endDateStr, createdBy, calendar).subscribe({
                    next: (res) => { this.complaintDetails = res.details; },
                    error: (err) => console.error(err)
                });
                break;
            case 'violations':
                this.reportService.getViolations(startDateStr, endDateStr, createdBy, calendar).subscribe({
                    next: (res) => { this.violationDetails = res.details; },
                    error: (err) => console.error(err)
                });
                break;
            case 'monitoring':
                this.reportService.getMonitoring(startDateStr, endDateStr, createdBy, calendar).subscribe({
                    next: (res) => { this.monitoringDetails = res.details; },
                    error: (err) => console.error(err)
                });
                break;
        }
    }

    onReportTabChange(tab: string): void {
        this.reportActiveTab = tab;
        this.loadReportTabData();
    }

    applyReportFilters(): void {
        this.loadReportData();
    }

    resetReportFilters(): void {
        this.reportStartDate = null;
        this.reportEndDate = null;
        this.reportCreatedBy = '';
        this.loadReportData();
    }

    exportToExcel(): void {
        const calendar = this.calendarService.getSelectedCalendar();
        const startDateStr = this.formatDateForBackend(this.reportStartDate);
        const endDateStr = this.formatDateForBackend(this.reportEndDate);
        const sectionType = this.reportActiveTab !== 'overall' ? this.reportActiveTab : undefined;

        this.reportService.getExport(startDateStr, endDateStr, this.reportCreatedBy || undefined, sectionType, calendar).subscribe({
            next: (data) => { this.downloadAsExcel(data); },
            error: (err) => {
                this.toastr.error('خطا در صادرات گزارش');
                console.error(err);
            }
        });
    }

    private downloadAsExcel(data: any[]): void {
        let csv = '\uFEFF';

        if (this.reportActiveTab === 'complaints' || this.reportActiveTab === 'overall') {
            const complaintData = this.reportActiveTab === 'complaints' ? data : data.filter(r => r.sectionType === 'complaints');
            if (complaintData.length > 0) {
                csv += 'ثبت شکایات\n';
                csv += 'نمبر مسلسل,نمبر جواز,ناحیه,شهرت عریضه نویس,شهرت عارض,موضوع شکایت,اجراآت,تاریخ ثبت,ثبت کننده\n';
                complaintData.forEach(row => {
                    csv += `${row.serialNumber || '-'},${row.petitionWriterLicenseNumber || '-'},${row.petitionWriterDistrict || '-'},${row.petitionWriterName || '-'},${row.complainantName || '-'},${row.complaintSubject || '-'},${row.complaintActionsTaken || '-'},${row.registrationDate || '-'},${row.createdBy || '-'}\n`;
                });
                csv += '\n';
            }
        }

        if (this.reportActiveTab === 'violations' || this.reportActiveTab === 'overall') {
            const violationData = this.reportActiveTab === 'violations' ? data : data.filter(r => r.sectionType === 'violations');
            if (violationData.length > 0) {
                csv += 'تخلفات عریضه نویسان\n';
                csv += 'نمبر مسلسل,شهرت عریضه نویس,نمبر جواز,ناحیه,نوعیت تخلف,وضعیت فعالیت,علت اجازه فعالیت,اجراآت,تاریخ ثبت,ثبت کننده\n';
                violationData.forEach(row => {
                    csv += `${row.serialNumber || '-'},${row.petitionWriterName || '-'},${row.petitionWriterLicenseNumber || '-'},${row.petitionWriterDistrict || '-'},${row.violationType || '-'},${this.getActivityStatusLabel(row.activityStatus)},${row.activityPermissionReason || '-'},${row.violationActionsTaken || '-'},${row.registrationDate || '-'},${row.createdBy || '-'}\n`;
                });
                csv += '\n';
            }
        }

        if (this.reportActiveTab === 'monitoring' || this.reportActiveTab === 'overall') {
            const monitoringData = this.reportActiveTab === 'monitoring' ? data : data.filter(r => r.sectionType === 'monitoring');
            if (monitoringData.length > 0) {
                csv += 'نظارت فعالیت عریضه نویسان\n';
                csv += 'سال,ماه,تعداد نظارت,ملاحظات,تاریخ ثبت,ثبت کننده\n';
                monitoringData.forEach(row => {
                    csv += `${row.monitoringYear || '-'},${row.monitoringMonth || '-'},${row.monitoringCount || 0},${row.monitoringRemarks || '-'},${row.registrationDate || '-'},${row.createdBy || '-'}\n`;
                });
                csv += '\n';
            }
        }

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `petition-writer-monitoring-report-${new Date().toISOString().split('T')[0]}.csv`;
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
}
