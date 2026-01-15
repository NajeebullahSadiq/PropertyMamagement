import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LicenseApplicationService } from 'src/app/shared/license-application.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import {
    LicenseApplication,
    LicenseApplicationGuarantor,
    LicenseApplicationWithdrawal,
    LicenseGuaranteeTypes
} from 'src/app/models/LicenseApplication';

@Component({
    selector: 'app-license-application-view',
    templateUrl: './license-application-view.component.html',
    styleUrls: ['./license-application-view.component.scss']
})
export class LicenseApplicationViewComponent implements OnInit {
    item: LicenseApplication | null = null;
    guarantors: LicenseApplicationGuarantor[] = [];
    withdrawal: LicenseApplicationWithdrawal | null = null;
    isLoading = true;
    canEdit = false;

    // Collapsible sections
    sections = {
        applicant: true,
        guarantors: true,
        withdrawal: true
    };

    guaranteeTypes = LicenseGuaranteeTypes;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private licenseAppService: LicenseApplicationService,
        private calendarService: CalendarService,
        private rbacService: RbacService,
        private toastr: ToastrService
    ) { }

    ngOnInit(): void {
        this.checkPermissions();
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.loadData(parseInt(id, 10));
        }
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar;
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();

        this.licenseAppService.getById(id, calendar).subscribe({
            next: (data) => {
                this.item = data;
                this.loadGuarantors(id);
                this.loadWithdrawal(id);
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    loadGuarantors(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.licenseAppService.getGuarantors(id, calendar).subscribe({
            next: (data) => {
                this.guarantors = data;
            },
            error: (err) => console.error(err)
        });
    }

    loadWithdrawal(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.licenseAppService.getWithdrawal(id, calendar).subscribe({
            next: (data) => {
                this.withdrawal = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    toggleSection(section: keyof typeof this.sections): void {
        this.sections[section] = !this.sections[section];
    }

    getGuaranteeTypeName(typeId: number | undefined): string {
        if (!typeId) return '-';
        const found = this.guaranteeTypes.find(t => t.id === typeId);
        return found ? found.name : '-';
    }

    getStatusText(): string {
        return this.item?.isWithdrawn ? 'منصرف‌شده' : 'فعال';
    }

    getStatusClass(): string {
        return this.item?.isWithdrawn
            ? 'bg-red-100 text-red-800'
            : 'bg-green-100 text-green-800';
    }

    editItem(): void {
        if (this.item?.id) {
            this.router.navigate(['/license-applications/edit', this.item.id]);
        }
    }

    printItem(): void {
        window.print();
    }

    goBack(): void {
        this.router.navigate(['/license-applications/list']);
    }
}
