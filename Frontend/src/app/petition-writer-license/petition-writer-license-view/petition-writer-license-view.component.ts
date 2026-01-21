import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PetitionWriterLicenseService } from 'src/app/shared/petition-writer-license.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import {
    PetitionWriterLicense,
    PetitionWriterRelocation,
    LicenseStatusEnum,
    LicenseStatusTypes
} from 'src/app/models/PetitionWriterLicense';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-petition-writer-license-view',
    templateUrl: './petition-writer-license-view.component.html',
    styleUrls: ['./petition-writer-license-view.component.scss']
})
export class PetitionWriterLicenseViewComponent implements OnInit {
    item: PetitionWriterLicense | null = null;
    relocations: PetitionWriterRelocation[] = [];
    isLoading = false;

    // Collapsible sections
    sections = {
        personal: true,
        financial: true,
        relocations: true,
        cancellation: true
    };

    // RBAC
    canEdit = false;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastrService,
        private licenseService: PetitionWriterLicenseService,
        private calendarService: CalendarService,
        private rbacService: RbacService
    ) { }

    ngOnInit(): void {
        this.checkPermissions();
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.loadData(+id);
        }
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar;
    }

    loadData(id: number): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();

        this.licenseService.getById(id, calendar).subscribe({
            next: (data) => {
                this.item = data;
                this.loadRelocations(id);
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    loadRelocations(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.licenseService.getRelocations(id, calendar).subscribe({
            next: (data) => {
                this.relocations = data;
            },
            error: (err) => console.error('Error loading relocations', err)
        });
    }

    toggleSection(section: keyof typeof this.sections): void {
        this.sections[section] = !this.sections[section];
    }

    getStatusClass(): string {
        if (!this.item) return 'bg-gray-100 text-gray-800';
        switch (this.item.licenseStatus) {
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

    getStatusText(): string {
        if (!this.item) return 'نامشخص';
        const found = LicenseStatusTypes.find(s => s.id === this.item!.licenseStatus);
        return found ? found.name : 'نامشخص';
    }

    goBack(): void {
        this.router.navigate(['/petition-writer-license/list']);
    }

    editItem(): void {
        if (this.item?.id) {
            this.router.navigate(['/petition-writer-license/edit', this.item.id]);
        }
    }

    printItem(): void {
        if (this.item?.id) {
            this.router.navigate(['/printPetitionWriterLicense', this.item.id]);
        }
    }

    getImageUrl(path: string): string {
        if (!path) return 'assets/img/avatar.png'; // Default fallback image
        // If path already starts with http/https, return as is
        if (path.startsWith('http://') || path.startsWith('https://')) {
            return path;
        }
        // Otherwise, construct the full URL using the environment API URL and Upload controller's view endpoint
        const baseUrl = environment.apiURL.replace('/api', ''); // Remove /api suffix if present
        return `${baseUrl}/api/Upload/view/${path}`;
    }

    onImageError(event: any): void {
        // Prevent infinite loop by setting to a valid fallback image only once
        if (event.target.src !== 'assets/img/avatar.png') {
            event.target.src = 'assets/img/avatar.png';
        }
    }
}
