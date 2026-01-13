import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PetitionWriterSecuritiesService } from 'src/app/shared/petition-writer-securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { PetitionWriterSecurities } from 'src/app/models/PetitionWriterSecurities';

@Component({
    selector: 'app-petition-writer-securities-view',
    templateUrl: './petition-writer-securities-view.component.html',
    styleUrls: ['./petition-writer-securities-view.component.scss']
})
export class PetitionWriterSecuritiesViewComponent implements OnInit {
    item: PetitionWriterSecurities | null = null;
    isLoading = true;
    canEdit = false;

    // Collapsible sections
    sections = {
        tab1: true,
        tab2: true
    };

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private petitionService: PetitionWriterSecuritiesService,
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
        this.petitionService.getById(id, calendar).subscribe({
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

    editItem(): void {
        if (this.item?.id) {
            this.router.navigate(['/petition-writer-securities/edit', this.item.id]);
        }
    }

    printItem(): void {
        if (this.item?.id) {
            window.open(`/printPetitionWriterSecurities/${this.item.id}`, '_blank');
        }
    }

    goBack(): void {
        this.router.navigate(['/petition-writer-securities/list']);
    }
}
