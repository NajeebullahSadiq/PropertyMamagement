import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { PetitionWriterMonitoringService } from 'src/app/shared/petition-writer-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { PetitionWriterMonitoringRecord, PetitionWriterMonitoringSectionTypes } from 'src/app/models/PetitionWriterMonitoring';

@Component({
    selector: 'app-petition-writer-monitoring-view',
    templateUrl: './petition-writer-monitoring-view.component.html',
    styleUrls: ['./petition-writer-monitoring-view.component.scss']
})
export class PetitionWriterMonitoringViewComponent extends BaseComponent implements OnInit {
    record: PetitionWriterMonitoringRecord | null = null;
    isLoading = true;
    sectionTypes = PetitionWriterMonitoringSectionTypes;

    canEdit = false;

    constructor(
        private route: ActivatedRoute,
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
        this.loadRecord();
    }

    checkPermissions(): void {
        const role = this.rbacService.getCurrentRole();
        this.canEdit = role === UserRoles.Admin || role === UserRoles.CompanyRegistrar || role === UserRoles.ActivityMonitoringManager;
    }

    loadRecord(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (!id) {
            this.toastr.error('شناسه رکورد یافت نشد');
            this.goToList();
            return;
        }

        const calendar = this.calendarService.getSelectedCalendar();
        this.service.getById(parseInt(id, 10), calendar).subscribe({
            next: (data) => {
                this.record = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
                this.isLoading = false;
                this.goToList();
            }
        });
    }

    getSectionTypeLabel(sectionType: string): string {
        const found = this.sectionTypes.find(s => s.value === sectionType);
        return found ? found.label : '-';
    }

    editRecord(): void {
        if (this.record?.id) {
            this.router.navigate(['/petition-writer-monitoring/form', this.record.id]);
        }
    }

    goToList(): void {
        this.router.navigate(['/petition-writer-monitoring/list']);
    }

    printRecord(): void {
        window.print();
    }
}
