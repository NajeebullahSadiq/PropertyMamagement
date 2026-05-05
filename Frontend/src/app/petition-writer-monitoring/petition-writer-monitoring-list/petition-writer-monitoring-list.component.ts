import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { PetitionWriterMonitoringService } from 'src/app/shared/petition-writer-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
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

    constructor(
        private router: Router,
        private service: PetitionWriterMonitoringService,
        private toastr: ToastrService,
        private calendarService: CalendarService,
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
}
