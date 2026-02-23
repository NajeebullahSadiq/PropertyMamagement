import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { LicenseApplicationService } from 'src/app/shared/license-application.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { LicenseApplication } from 'src/app/models/LicenseApplication';

@Component({
    selector: 'app-license-application-list',
    templateUrl: './license-application-list.component.html',
    styleUrls: ['./license-application-list.component.scss']
})
export class LicenseApplicationListComponent implements OnInit, OnDestroy {
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
    searchProposedGuideName = '';
    searchElectronicNumber = '';
    searchShariaDeedNumber = '';
    searchCustomaryDeedSerial = '';
    searchGuarantorName = '';

    // RBAC
    canEdit = false;
    canDelete = false;
    isViewOnly = false;

    private dataChangedSub?: Subscription;

    constructor(
        private licenseAppService: LicenseApplicationService,
        private calendarService: CalendarService,
        private rbacService: RbacService,
        private router: Router,
        private toastr: ToastrService
    ) { }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadData();

        this.dataChangedSub = this.licenseAppService.dataChanged$.subscribe(() => {
            this.loadData();
        });
    }

    ngOnDestroy(): void {
        this.dataChangedSub?.unsubscribe();
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.isViewOnly = role === UserRoles.Authority || role === UserRoles.LicenseReviewer;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar;
        this.canDelete = role === UserRoles.Admin;
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
        this.page = 1;
        this.loadData();
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
            this.searchProposedGuideName || undefined,
            this.searchElectronicNumber || undefined,
            this.searchShariaDeedNumber || undefined,
            this.searchCustomaryDeedSerial || undefined,
            this.searchGuarantorName || undefined,
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
        this.searchProposedGuideName = '';
        this.searchElectronicNumber = '';
        this.searchShariaDeedNumber = '';
        this.searchCustomaryDeedSerial = '';
        this.searchGuarantorName = '';
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
            this.searchProposedGuideName ||
            this.searchElectronicNumber ||
            this.searchShariaDeedNumber ||
            this.searchCustomaryDeedSerial ||
            this.searchGuarantorName
        );
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
}
