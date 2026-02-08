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
    filteredItems: LicenseApplication[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;
    pageSizes = [5, 10, 25, 50];
    searchTerm = '';
    isLoading = false;

    // Advanced search fields
    showAdvancedSearch = false;
    searchSerialNumber = '';
    searchRequestDate = '';
    searchApplicantName = '';
    searchProposedGuideName = '';
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

        this.licenseAppService.getAll(this.page, this.pageSize, '', calendar).subscribe({
            next: (response) => {
                this.items = response.items;
                this.totalCount = response.totalCount;
                this.filteredItems = [...this.items];
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    applyFilter(): void {
        if (!this.searchTerm.trim()) {
            this.filteredItems = [...this.items];
        } else {
            const term = this.searchTerm.toLowerCase().trim();
            this.filteredItems = this.items.filter(item =>
                item.requestSerialNumber?.toLowerCase().includes(term) ||
                item.applicantName?.toLowerCase().includes(term) ||
                item.proposedGuideName?.toLowerCase().includes(term)
            );
        }
        this.totalCount = this.filteredItems.length;
    }

    onSearch(): void {
        this.page = 1;
        this.applyFilter();
    }

    toggleAdvancedSearch(): void {
        this.showAdvancedSearch = !this.showAdvancedSearch;
        if (!this.showAdvancedSearch) {
            this.clearAdvancedSearch();
        }
    }

    performAdvancedSearch(): void {
        this.isLoading = true;
        this.page = 1;
        const calendar = this.calendarService.getSelectedCalendar();

        this.licenseAppService.search(
            this.searchSerialNumber || undefined,
            this.searchRequestDate || undefined,
            this.searchApplicantName || undefined,
            this.searchProposedGuideName || undefined,
            this.searchShariaDeedNumber || undefined,
            this.searchCustomaryDeedSerial || undefined,
            this.searchGuarantorName || undefined,
            this.page,
            this.pageSize,
            calendar
        ).subscribe({
            next: (response) => {
                this.items = response.items;
                this.filteredItems = [...this.items];
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
        this.searchShariaDeedNumber = '';
        this.searchCustomaryDeedSerial = '';
        this.searchGuarantorName = '';
        this.loadData();
    }

    onPageChange(page: number): void {
        this.page = page;
    }

    onPageSizeChange(): void {
        this.page = 1;
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
