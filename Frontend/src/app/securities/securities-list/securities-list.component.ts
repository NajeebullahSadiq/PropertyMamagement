import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SecuritiesService } from 'src/app/shared/securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { SecuritiesDistribution, PropertySubTypes, VehicleSubTypes } from 'src/app/models/SecuritiesDistribution';

@Component({
    selector: 'app-securities-list',
    templateUrl: './securities-list.component.html',
    styleUrls: ['./securities-list.component.scss']
})
export class SecuritiesListComponent implements OnInit {
    items: SecuritiesDistribution[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;
    totalPages = 0;
    searchTerm = '';
    isLoading = false;
    Math = Math; // Make Math available in template

    // RBAC
    canEdit = false;
    canDelete = false;
    isViewOnly = false;

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
        
        this.securitiesService.getAll(this.page, this.pageSize, this.searchTerm, calendar).subscribe({
            next: (response) => {
                this.items = response.items;
                this.totalCount = response.totalCount;
                this.totalPages = response.totalPages;
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

    onPageChange(page: number): void {
        this.page = page;
        this.loadData();
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

    getPropertySubTypeName(subType: number | undefined): string {
        if (!subType) return '-';
        const found = PropertySubTypes.find(x => x.id === subType);
        return found ? found.name : '-';
    }

    getVehicleSubTypeName(subType: number | undefined): string {
        if (!subType) return '-';
        const found = VehicleSubTypes.find(x => x.id === subType);
        return found ? found.name : '-';
    }

    getTotalDocumentCount(item: SecuritiesDistribution): number {
        let total = 0;
        if (item.propertySaleCount) total += item.propertySaleCount;
        if (item.bayWafaCount) total += item.bayWafaCount;
        if (item.rentCount) total += item.rentCount;
        if (item.vehicleSaleCount) total += item.vehicleSaleCount;
        if (item.vehicleExchangeCount) total += item.vehicleExchangeCount;
        return total;
    }
}
