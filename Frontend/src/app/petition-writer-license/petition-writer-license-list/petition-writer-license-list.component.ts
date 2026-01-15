import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { PetitionWriterLicenseService } from 'src/app/shared/petition-writer-license.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { PetitionWriterLicense, LicenseStatusEnum, LicenseStatusTypes } from 'src/app/models/PetitionWriterLicense';

@Component({
    selector: 'app-petition-writer-license-list',
    templateUrl: './petition-writer-license-list.component.html',
    styleUrls: ['./petition-writer-license-list.component.scss']
})
export class PetitionWriterLicenseListComponent implements OnInit, OnDestroy {
    items: PetitionWriterLicense[] = [];
    filteredItems: PetitionWriterLicense[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;
    pageSizes = [5, 10, 25, 50];
    searchTerm = '';
    isLoading = false;

    // RBAC
    canEdit = false;
    canDelete = false;
    isViewOnly = false;

    private dataChangedSub?: Subscription;

    constructor(
        private licenseService: PetitionWriterLicenseService,
        private calendarService: CalendarService,
        private rbacService: RbacService,
        private router: Router,
        private toastr: ToastrService
    ) { }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadData();

        this.dataChangedSub = this.licenseService.dataChanged$.subscribe(() => {
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

        this.licenseService.getAll(1, 1000, '', calendar).subscribe({
            next: (response) => {
                this.items = response.items;
                this.totalCount = response.totalCount;
                this.applyFilter();
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
                item.licenseNumber?.toLowerCase().includes(term) ||
                item.applicantName?.toLowerCase().includes(term) ||
                item.activityLocation?.toLowerCase().includes(term)
            );
        }
        this.totalCount = this.filteredItems.length;
    }

    onSearch(): void {
        this.page = 1;
        this.applyFilter();
    }

    onPageChange(page: number): void {
        this.page = page;
    }

    onPageSizeChange(): void {
        this.page = 1;
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
            case LicenseStatusEnum.Active:
                return 'bg-green-100 text-green-800';
            case LicenseStatusEnum.Cancelled:
                return 'bg-red-100 text-red-800';
            case LicenseStatusEnum.Withdrawn:
                return 'bg-amber-100 text-amber-800';
            default:
                return 'bg-gray-100 text-gray-800';
        }
    }

    getStatusText(item: PetitionWriterLicense): string {
        const found = LicenseStatusTypes.find(s => s.id === item.licenseStatus);
        return found ? found.name : 'نامشخص';
    }
}
