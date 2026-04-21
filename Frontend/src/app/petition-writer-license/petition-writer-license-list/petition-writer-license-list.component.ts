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

    private searchSubject = new Subject<string>();

    constructor(
        private licenseService: PetitionWriterLicenseService,
        private calendarService: CalendarService,
        private rbacService: RbacService,
        private router: Router,
        private toastr: ToastrService
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

        // Use server-side search via API
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

    applyFilter(): void {
        // No longer needed - using server-side search
        this.loadData();
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

    printLicense(id: number): void {
        window.open(`/printPetitionWriterLicense/${id}`, '_blank');
    }
}
