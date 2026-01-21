import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ActivityMonitoringService } from 'src/app/shared/activity-monitoring.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import {
    ActivityMonitoringRecord,
    Complaint,
    RealEstateViolation,
    PetitionWriterViolation
} from 'src/app/models/ActivityMonitoring';

@Component({
    selector: 'app-activity-monitoring-view',
    templateUrl: './activity-monitoring-view.component.html',
    styleUrls: ['./activity-monitoring-view.component.scss']
})
export class ActivityMonitoringViewComponent implements OnInit {
    record: ActivityMonitoringRecord | null = null;
    complaints: Complaint[] = [];
    realEstateViolations: RealEstateViolation[] = [];
    petitionWriterViolations: PetitionWriterViolation[] = [];
    recordId: number = 0;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private service: ActivityMonitoringService,
        private toastr: ToastrService,
        private calendarService: CalendarService
    ) { }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.recordId = parseInt(id, 10);
            this.loadData();
        }
    }

    loadData(): void {
        const calendar = this.calendarService.getSelectedCalendar();
        
        this.service.getById(this.recordId, calendar).subscribe({
            next: (data) => {
                this.record = data;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                console.error(err);
            }
        });

        this.service.getComplaints(this.recordId, calendar).subscribe({
            next: (data) => {
                this.complaints = data;
            },
            error: (err) => console.error(err)
        });

        this.service.getRealEstateViolations(this.recordId, calendar).subscribe({
            next: (data) => {
                this.realEstateViolations = data;
            },
            error: (err) => console.error(err)
        });

        this.service.getPetitionWriterViolations(this.recordId, calendar).subscribe({
            next: (data) => {
                this.petitionWriterViolations = data;
            },
            error: (err) => console.error(err)
        });
    }

    goToList(): void {
        this.router.navigate(['/activity-monitoring/list']);
    }

    editRecord(): void {
        this.router.navigate(['/activity-monitoring/form', this.recordId]);
    }

    print(): void {
        window.print();
    }
}
