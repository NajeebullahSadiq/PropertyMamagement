import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { SecuritiesService } from 'src/app/shared/securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { SecuritiesDistribution } from 'src/app/models/SecuritiesDistribution';

@Component({
    selector: 'app-securities-list',
    templateUrl: './securities-list.component.html',
    styleUrls: ['./securities-list.component.scss']
})
export class SecuritiesListComponent implements OnInit, OnDestroy {
    items: SecuritiesDistribution[] = [];
    filteredItems: SecuritiesDistribution[] = [];
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
        private securitiesService: SecuritiesService,
        private calendarService: CalendarService,
        private rbacService: RbacService,
        private router: Router,
        private toastr: ToastrService
    ) { }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadData();

        // Subscribe to data changes
        this.dataChangedSub = this.securitiesService.dataChanged$.subscribe(() => {
            this.loadData();
        });
    }

    ngOnDestroy(): void {
        this.dataChangedSub?.unsubscribe();
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.isViewOnly = role === UserRoles.Authority || role === UserRoles.LicenseReviewer;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar || role === UserRoles.PropertyOperator;
        this.canDelete = role === UserRoles.Admin;
    }

    loadData(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        
        this.securitiesService.getAll(1, 1000, '', calendar).subscribe({
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
                (item.registrationNumber?.toLowerCase().includes(term)) ||
                (item.transactionGuideName?.toLowerCase().includes(term)) ||
                (item.licenseNumber?.toLowerCase().includes(term)) ||
                (item.bankReceiptNumber?.toLowerCase().includes(term)) ||
                (item.licenseOwnerName?.toLowerCase().includes(term))
            );
        }
        this.totalCount = this.filteredItems.length;
        // Reset items to filtered for pagination
        this.items = this.filteredItems;
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

    viewDetails(id: number): void {
        this.router.navigate(['/securities/view', id]);
    }

    editItem(id: number): void {
        this.router.navigate(['/securities/edit', id]);
    }

    printItem(id: number): void {
        window.open(`/printSecurities/${id}`, '_blank');
    }

    deleteItem(id: number): void {
        if (confirm('آیا مطمئن هستید که می‌خواهید این رکورد را حذف کنید؟')) {
            this.securitiesService.delete(id).subscribe({
                next: (res) => {
                    this.toastr.success(res.message || 'رکورد با موفقیت حذف شد');
                    this.loadData();
                },
                error: (err) => {
                    this.toastr.error(err.error?.message || 'خطا در حذف رکورد');
                }
            });
        }
    }

    createNew(): void {
        this.router.navigate(['/securities']);
    }

    getTotalDocumentCount(item: SecuritiesDistribution): number {
        if (!item || !item.items) return 0;
        return item.items.reduce((sum, docItem) => sum + docItem.count, 0);
    }
}
