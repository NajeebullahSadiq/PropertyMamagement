import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { PetitionWriterSecuritiesService } from 'src/app/shared/petition-writer-securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { PetitionWriterSecurities } from 'src/app/models/PetitionWriterSecurities';

@Component({
    selector: 'app-petition-writer-securities-list',
    templateUrl: './petition-writer-securities-list.component.html',
    styleUrls: ['./petition-writer-securities-list.component.scss']
})
export class PetitionWriterSecuritiesListComponent implements OnInit, OnDestroy {
    @Input() embeddedMode = false;
    
    items: PetitionWriterSecurities[] = [];
    filteredItems: PetitionWriterSecurities[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;
    tableSizes: number[] = [10, 50, 100];
    searchTerm = '';
    isLoading = false;

    // RBAC flags
    isViewOnly = false;
    canEdit = false;
    canDelete = false;
    currentUserId = '';

    private dataChangedSubscription?: Subscription;

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
        
        // Subscribe to data changes from service
        this.dataChangedSubscription = this.petitionService.dataChanged.subscribe(() => {
            this.loadData();
        });
    }

    ngOnDestroy(): void {
        this.dataChangedSubscription?.unsubscribe();
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.isViewOnly = role === UserRoles.Authority || role === UserRoles.LicenseReviewer;
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar || role === UserRoles.PropertyOperator;
        this.canDelete = role === UserRoles.Admin;
        this.currentUserId = this.rbacService.getCurrentUserId();
    }

    loadData(): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        
        this.petitionService.getAll(this.page, this.pageSize, this.searchTerm, calendar).subscribe({
            next: (response) => {
                this.items = response.items;
                this.filteredItems = this.filterItems(response.items, this.searchTerm);
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

    filterItems(items: PetitionWriterSecurities[], searchTerm: string): PetitionWriterSecurities[] {
        const term = (searchTerm ?? '').toString().trim().toLowerCase();
        if (!term) {
            return items;
        }

        const toText = (value: unknown) => (value ?? '').toString().toLowerCase();

        return (items || []).filter(item => {
            const regMatch = toText(item.registrationNumber).includes(term);
            const nameMatch = toText(item.petitionWriterName).includes(term);
            const fatherMatch = toText(item.petitionWriterFatherName).includes(term);
            const licenseMatch = toText(item.licenseNumber).includes(term);
            const bankMatch = toText(item.bankReceiptNumber).includes(term);
            const serialStartMatch = toText(item.serialNumberStart).includes(term);
            const serialEndMatch = toText(item.serialNumberEnd).includes(term);

            return regMatch || nameMatch || fatherMatch || licenseMatch || bankMatch || serialStartMatch || serialEndMatch;
        });
    }

    onSearch(): void {
        this.filteredItems = this.filterItems(this.items, this.searchTerm);
        this.totalCount = this.filteredItems.length;
        this.page = 1;
    }

    onTableDataChange(event: any): void {
        this.page = event;
        this.loadData();
    }

    onTableSizeChange(event: any): void {
        this.pageSize = event.target.value;
        this.page = 1;
        this.loadData();
    }

    onView(id: number): void {
        this.router.navigate(['/petition-writer-securities/view', id]);
    }

    onEdit(id: number): void {
        this.router.navigate(['/petition-writer-securities/edit', id]);
    }

    onPrint(id: number): void {
        const tree = this.router.createUrlTree(['/printPetitionWriterSecurities', id]);
        const url = tree.toString();
        const absoluteUrl = `${window.location.origin}${url.startsWith('/') ? url : `/${url}`}`;
        const newWindow = window.open(absoluteUrl, '_blank', 'noopener,noreferrer');
        if (newWindow) {
            newWindow.opener = null;
        } else {
            this.router.navigateByUrl(tree);
        }
    }

    createNew(): void {
        this.router.navigate(['/petition-writer-securities']);
    }
}
