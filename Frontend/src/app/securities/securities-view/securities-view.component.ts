import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SecuritiesService } from 'src/app/shared/securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { 
    SecuritiesDistribution, 
    DocumentTypes, 
    PropertySubTypes, 
    VehicleSubTypes,
    RegistrationBookTypes 
} from 'src/app/models/SecuritiesDistribution';

@Component({
    selector: 'app-securities-view',
    templateUrl: './securities-view.component.html',
    styleUrls: ['./securities-view.component.scss']
})
export class SecuritiesViewComponent implements OnInit {
    item: SecuritiesDistribution | null = null;
    isLoading = true;
    canEdit = false;

    // Collapsible sections
    sections = {
        tab1: true,
        tab2: true,
        tab3: true,
        tab4: true
    };

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private securitiesService: SecuritiesService,
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
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar || role === UserRoles.PropertyOperator;
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.securitiesService.getById(id, calendar).subscribe({
            next: (data) => {
                this.item = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    toggleSection(section: keyof typeof this.sections): void {
        this.sections[section] = !this.sections[section];
    }

    getDocumentTypeName(type: number | undefined): string {
        if (!type) return '-';
        const found = DocumentTypes.find(x => x.id === type);
        return found ? found.name : '-';
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

    getRegistrationBookTypeName(type: number | undefined): string {
        if (!type) return '-';
        const found = RegistrationBookTypes.find(x => x.id === type);
        return found ? found.name : '-';
    }

    editItem(): void {
        if (this.item?.id) {
            this.router.navigate(['/securities/edit', this.item.id]);
        }
    }

    printItem(): void {
        if (this.item?.id) {
            window.open(`/printSecurities/${this.item.id}`, '_blank');
        }
    }

    goBack(): void {
        this.router.navigate(['/securities/list']);
    }
}
