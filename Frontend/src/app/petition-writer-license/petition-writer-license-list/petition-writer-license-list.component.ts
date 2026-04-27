import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { PetitionWriterLicenseService } from 'src/app/shared/petition-writer-license.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { PetitionWriterLicense, LicenseStatusEnum, LicenseStatusTypes } from 'src/app/models/PetitionWriterLicense';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarType } from 'src/app/models/calendar-type';

@Component({
    selector: 'app-petition-writer-license-list',
    templateUrl: './petition-writer-license-list.component.html',
    styleUrls: ['./petition-writer-license-list.component.scss']
})
export class PetitionWriterLicenseListComponent extends BaseComponent implements OnInit, OnDestroy {
    items: PetitionWriterLicense[] = [];
    filteredItems: PetitionWriterLicense[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 100;
    pageSizes = [10, 25, 50, 100];
    searchTerm = '';
    isLoading = false;

    // RBAC
    canEdit = false;
    canDelete = false;
    isViewOnly = false;

    // Report
    showReport = false;
    reportStartDate: any = '';
    reportEndDate: any = '';
    reportActivityLocation = '';
    reportProvinceId = 0;
    reportDistrictId = 0;
    reportData: any = null;
    isLoadingReport = false;
    activityLocations: string[] = [];
    reportProvinces: any[] = [];
    reportDistricts: any[] = [];

    readonly hijriShamsi = CalendarType.HIJRI_SHAMSI;

    private searchSubject = new Subject<string>();

    constructor(
        private licenseService: PetitionWriterLicenseService,
        private calendarService: CalendarService,
        private rbacService: RbacService,
        private router: Router,
        private toastr: ToastrService,
        private calendarConversion: CalendarConversionService
    ) {
        super();
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
        this.loadActivityLocations();

        this.licenseService.dataChanged$.pipe(takeUntil(this.destroy$)).subscribe(() => {
            this.loadData();
        });
    }

    override ngOnDestroy(): void {
        this.searchSubject.complete();
        super.ngOnDestroy();
    }

    checkPermissions(): void {
        this.isViewOnly = !this.rbacService.hasPermission('petitionwriterlicense.create');
        this.canEdit = this.rbacService.hasPermission('petitionwriterlicense.edit');
        this.canDelete = this.rbacService.isAdmin();
    }

    loadData(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        this.licenseService.getAll(this.page, this.pageSize, this.searchTerm.trim(), calendar).subscribe({
            next: (response) => {
                this.items = response.items;
                this.filteredItems = response.items;
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

    loadActivityLocations(): void {
        // Load from report endpoint (overall) to populate dropdowns
        this.licenseService.getReport().subscribe({
            next: (data) => {
                this.activityLocations = data?.activityLocations || [];
                this.reportProvinces = data?.provinces || [];
            },
            error: () => {}
        });
    }

    onReportProvinceChange(): void {
        this.reportDistrictId = 0;
        this.reportDistricts = [];
        if (this.reportProvinceId > 0) {
            this.licenseService.getReportDistricts(this.reportProvinceId).subscribe({
                next: (data) => { this.reportDistricts = data || []; },
                error: () => {}
            });
        }
    }

    onSearch(): void {
        this.searchSubject.next(this.searchTerm);
    }

    onPageChange(page: number): void {
        this.page = page;
        this.loadData();
    }

    onPageSizeChange(): void {
        this.page = 1;
        this.loadData();
    }

    createNew(): void {
        this.licenseService.mainTableId = 0;
        this.router.navigate(['/petition-writer-license']);
    }

    viewDetails(id: number): void {
        this.router.navigate(['/petition-writer-license/view', id]);
    }

    editItem(id: number): void {
        this.router.navigate(['/petition-writer-license/edit', id]);
    }

    deleteItem(id: number): void {
        if (!confirm('آیا مطمئن هستید که می‌خواهید این جواز را حذف کنید؟')) return;

        this.licenseService.delete(id).subscribe({
            next: () => {
                this.toastr.success('جواز با موفقیت حذف شد');
                this.loadData();
            },
            error: (err) => {
                this.toastr.error('خطا در حذف جواز');
                console.error(err);
            }
        });
    }

    getStatusClass(item: PetitionWriterLicense): string {
        switch (item.licenseStatus) {
            case LicenseStatusEnum.Active:    return 'bg-green-100 text-green-800';
            case LicenseStatusEnum.Cancelled: return 'bg-red-100 text-red-800';
            case LicenseStatusEnum.Withdrawn: return 'bg-amber-100 text-amber-800';
            default:                          return 'bg-gray-100 text-gray-800';
        }
    }

    getStatusText(item: PetitionWriterLicense): string {
        const found = LicenseStatusTypes.find(s => s.id === item.licenseStatus);
        return found ? found.name : 'نامشخص';
    }

    printLicense(id: number): void {
        window.open(`/print/petition-writer-license/${id}`, '_blank');
    }

    // ==================== Report ====================

    toggleReport(): void {
        this.showReport = !this.showReport;
        if (!this.showReport) {
            this.reportData = null;
        }
    }

    private convertToHijriString(date: Date): string {
        const h = this.calendarConversion.fromGregorian(date, CalendarType.HIJRI_SHAMSI);
        return `${h.year}/${String(h.month).padStart(2, '0')}/${String(h.day).padStart(2, '0')}`;
    }

    generateReport(): void {
        if (!this.reportStartDate || !this.reportEndDate) {
            this.toastr.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
            return;
        }

        this.isLoadingReport = true;

        const startDate = this.reportStartDate instanceof Date
            ? this.convertToHijriString(this.reportStartDate)
            : String(this.reportStartDate);

        const endDate = this.reportEndDate instanceof Date
            ? this.convertToHijriString(this.reportEndDate)
            : String(this.reportEndDate);

        this.licenseService.getReport(
            startDate,
            endDate,
            'hijriShamsi',
            this.reportActivityLocation || undefined,
            this.reportProvinceId > 0 ? this.reportProvinceId : undefined,
            this.reportDistrictId > 0 ? this.reportDistrictId : undefined
        ).subscribe({
            next: (data) => {
                this.reportData = data;
                this.activityLocations = data?.activityLocations || this.activityLocations;
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

        const rows: any[][] = [];
        const f = this.reportData.filtered;
        const o = this.reportData.overall;

        rows.push(['گزارش جوازهای عریضه‌نویسان']);
        rows.push([`از تاریخ: ${this.reportData.startDate || 'همه'}  تا تاریخ: ${this.reportData.endDate || 'همه'}`]);
        rows.push([`تاریخ تولید گزارش: ${this.reportData.reportGeneratedAt}`]);
        rows.push([]);

        // Filtered section
        rows.push(['--- آمار بازه زمانی انتخاب شده ---']);
        rows.push(['مجموع جوازها', f.totalLicenses]);
        rows.push(['فعال', f.activeCount]);
        rows.push(['لغو', f.cancelledCount]);
        rows.push(['انصراف', f.withdrawnCount]);
        rows.push(['نقل مکان', f.relocationCount]);
        rows.push([]);

        rows.push(['نوعیت جواز', 'تعداد', 'مجموع عواید (افغانی)']);
        (f.byLicenseType || []).forEach((x: any) => rows.push([x.licenseType, x.count, x.totalCost]));
        rows.push(['مجموع کل', f.totalLicenses, f.totalCost]);
        rows.push([]);

        rows.push(['محل فعالیت', 'تعداد']);
        (f.byActivityLocation || []).forEach((x: any) => rows.push([x.activityLocation, x.count]));
        rows.push([]);

        rows.push(['ولایت', 'تعداد']);
        (f.byProvince || []).forEach((x: any) => rows.push([x.province, x.count]));
        rows.push([]);

        rows.push(['ولسوالی', 'تعداد']);
        (f.byDistrict || []).forEach((x: any) => rows.push([x.district, x.count]));
        rows.push([]);

        // Overall section
        rows.push(['--- آمار کلی (همه زمان‌ها) ---']);
        rows.push(['مجموع جوازها', o.totalLicenses]);
        rows.push(['فعال', o.activeCount]);
        rows.push(['لغو', o.cancelledCount]);
        rows.push(['انصراف', o.withdrawnCount]);
        rows.push(['نقل مکان', o.relocationCount]);
        rows.push([]);

        rows.push(['نوعیت جواز', 'تعداد', 'مجموع عواید (افغانی)']);
        (o.byLicenseType || []).forEach((x: any) => rows.push([x.licenseType, x.count, x.totalCost]));
        rows.push(['مجموع کل', o.totalLicenses, o.totalCost]);
        rows.push([]);

        rows.push(['محل فعالیت', 'تعداد']);
        (o.byActivityLocation || []).forEach((x: any) => rows.push([x.activityLocation, x.count]));
        rows.push([]);

        rows.push(['ولایت', 'تعداد']);
        (o.byProvince || []).forEach((x: any) => rows.push([x.province, x.count]));
        rows.push([]);

        rows.push(['ولسوالی', 'تعداد']);
        (o.byDistrict || []).forEach((x: any) => rows.push([x.district, x.count]));

        const csv = rows.map(r => r.map(c => `"${c ?? ''}"`).join(',')).join('\n');
        const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `petition-writer-report-${Date.now()}.csv`;
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        this.toastr.success('گزارش با موفقیت صادر شد');
    }
}
