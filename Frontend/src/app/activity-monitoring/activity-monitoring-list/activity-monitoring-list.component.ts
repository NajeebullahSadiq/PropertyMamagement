import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ActivityMonitoringService } from 'src/app/shared/activity-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { ActivityMonitoringRecord } from 'src/app/models/ActivityMonitoring';

@Component({
    selector: 'app-activity-monitoring-list',
    templateUrl: './activity-monitoring-list.component.html',
    styleUrls: ['./activity-monitoring-list.component.scss']
})
export class ActivityMonitoringListComponent implements OnInit {
    items: ActivityMonitoringRecord[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;
    pageSizes = [5, 10, 25, 50];
    searchText = '';
    isLoading = false;

    canCreate = false;
    canEdit = false;
    canDelete = false;

    constructor(
        private router: Router,
        private service: ActivityMonitoringService,
        private toastr: ToastrService,
        private calendarService: CalendarService,
        private rbacService: RbacService
    ) { }

    ngOnInit(): void {
        this.checkPermissions();
        this.loadData();
        this.service.dataChanged$.subscribe(() => this.loadData());
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.canCreate = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar;
        this.canDelete = role === UserRoles.Admin;
    }

    loadData(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        this.service.getAll(this.page, this.pageSize, this.searchText, calendar).subscribe({
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

    onPageChange(pageNumber: number): void {
        this.page = pageNumber;
        this.loadData();
    }

    onPageSizeChange(): void {
        this.page = 1;
        this.loadData();
    }

    goToForm(): void {
        this.router.navigate(['/activity-monitoring/form']);
    }

    viewRecord(id: number): void {
        this.router.navigate(['/activity-monitoring/view', id]);
    }

    editRecord(id: number): void {
        this.router.navigate(['/activity-monitoring/form', id]);
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
}
