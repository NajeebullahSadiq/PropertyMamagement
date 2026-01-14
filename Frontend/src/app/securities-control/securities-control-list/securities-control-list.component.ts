import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { SecuritiesControlService } from 'src/app/shared/securities-control.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { SecuritiesControl } from 'src/app/models/SecuritiesControl';

@Component({
    selector: 'app-securities-control-list',
    templateUrl: './securities-control-list.component.html',
    styleUrls: ['./securities-control-list.component.scss']
})
export class SecuritiesControlListComponent implements OnInit, OnDestroy {
    items: SecuritiesControl[] = [];
    filteredItems: SecuritiesControl[] = [];
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
        private securitiesControlService: SecuritiesControlService,
        private calendarService: CalendarService,
        private rbacService: RbacService,
        private router: Router,
        private toastr: ToastrService
    ) { }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadData();

        // Subscribe to data changes
        this.dataChangedSub = this.securitiesControlService.dataChanged$.subscribe(() => {
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
        
        this.securitiesControlService.getAll(1, 1000, '', calendar).subscribe({
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
                (item.serialNumber?.toLowerCase().includes(term)) ||
                (item.proposalNumber?.toLowerCase().includes(term)) ||
                (item.distributionTicketNumber?.toLowerCase().includes(term)) ||
                (item.distributionStartNumber?.toLowerCase().includes(term))
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
        this.router.navigate(['/securities-control/view', id]);
    }

    editItem(id: number): void {
        this.router.navigate(['/securities-control/edit', id]);
    }

    printItem(id: number): void {
        // Assuming there's a print endpoint for securities control
        window.open(`/printSecuritiesControl/${id}`, '_blank');
    }

    deleteItem(id: number): void {
        if (confirm('آیا مطمئن هستید که می‌خواهید این رکورد را حذف کنید؟')) {
            this.securitiesControlService.delete(id).subscribe({
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
        this.router.navigate(['/securities-control']);
    }

    getTotalDocumentCount(item: SecuritiesControl): number {
        let total = 0;
        if (item.propertySaleCount) total += item.propertySaleCount;
        if (item.bayWafaCount) total += item.bayWafaCount;
        if (item.rentCount) total += item.rentCount;
        if (item.vehicleSaleCount) total += item.vehicleSaleCount;
        if (item.exchangeCount) total += item.exchangeCount;
        if (item.registrationBookCount) total += item.registrationBookCount;
        if (item.printedPetitionCount) total += item.printedPetitionCount;
        return total;
    }
}