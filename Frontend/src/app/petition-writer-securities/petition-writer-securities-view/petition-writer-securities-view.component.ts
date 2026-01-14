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
    error: string | null = null;
    canEdit = false;

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
        } else {
            this.error = 'شناسه سند یافت نشد';
            this.isLoading = false;
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
                this.error = 'خطا در بارگذاری اطلاعات';
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    editItem(): void {
        if (this.item?.id) {
            this.router.navigate(['/petition-writer-securities/edit', this.item.id]);
        }
    }

    printItem(): void {
        if (this.item?.id) {
            const tree = this.router.createUrlTree(['/printPetitionWriterSecurities', this.item.id]);
            const url = tree.toString();
            const absoluteUrl = `${window.location.origin}${url.startsWith('/') ? url : `/${url}`}`;
            const newWindow = window.open(absoluteUrl, '_blank', 'noopener,noreferrer');
            if (newWindow) {
                newWindow.opener = null;
            } else {
                this.router.navigateByUrl(tree);
            }
        }
    }

    goBack(): void {
        this.router.navigate(['/petition-writer-securities/list']);
    }
}
