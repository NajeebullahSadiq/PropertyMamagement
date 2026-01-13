import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PetitionWriterSecuritiesService } from 'src/app/shared/petition-writer-securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { PetitionWriterSecurities } from 'src/app/models/PetitionWriterSecurities';

@Component({
    selector: 'app-petition-writer-securities-list',
    templateUrl: './petition-writer-securities-list.component.html',
    styleUrls: ['./petition-writer-securities-list.component.scss']
})
export class PetitionWriterSecuritiesListComponent implements OnInit {
    items: PetitionWriterSecurities[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;
    totalPages = 0;
    searchTerm = '';
    isLoading = false;
    Math = Math;

    // RBAC
    canEdit = false;
    canDelete = false;
    isViewOnly = false;

    constructor(
        private petitionService: PetitionWriterSecuritiesService,
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
        
        this.petitionService.getAll(this.page, this.pageSize, this.searchTerm, calendar).subscribe({
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
        this.router.navigate(['/petition-writer-securities/view', id]);
    }

    editItem(id: number): void {
        this.router.navigate(['/petition-writer-securities/edit', id]);
    }

    printItem(id: number): void {
        window.open(`/printPetitionWriterSecurities/${id}`, '_blank');
    }

    deleteItem(id: number): void {
        if (confirm('آیا مطمئن هستید که می‌خواهید این رکورد را حذف کنید؟')) {
            this.petitionService.delete(id).subscribe({
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
        this.router.navigate(['/petition-writer-securities']);
    }
}
