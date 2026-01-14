import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PetitionWriterSecuritiesService } from 'src/app/shared/petition-writer-securities.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { PetitionWriterSecurities } from 'src/app/models/PetitionWriterSecurities';

@Component({
    selector: 'app-print-petition-writer-securities',
    templateUrl: './print-petition-writer-securities.component.html',
    styleUrls: ['./print-petition-writer-securities.component.scss']
})
export class PrintPetitionWriterSecuritiesComponent implements OnInit {
    item: PetitionWriterSecurities | null = null;
    isLoading = true;
    error: string | null = null;
    printDate: string = '';

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private petitionService: PetitionWriterSecuritiesService,
        private calendarService: CalendarService
    ) { }

    ngOnInit(): void {
        this.setPrintDate();
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.loadData(parseInt(id, 10));
        } else {
            this.error = 'شناسه سند یافت نشد';
            this.isLoading = false;
        }
    }

    setPrintDate(): void {
        const now = new Date();
        const options: Intl.DateTimeFormatOptions = { 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric',
            calendar: 'persian'
        };
        this.printDate = now.toLocaleDateString('fa-AF', options);
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.petitionService.getById(id, calendar).subscribe({
            next: (data) => {
                this.item = data;
                this.isLoading = false;
                // Auto print after data loads
                setTimeout(() => {
                    window.print();
                }, 500);
            },
            error: (err) => {
                this.error = 'خطا در بارگذاری اطلاعات';
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    goBack(): void {
        this.router.navigate(['/petition-writer-securities/list']);
    }
}
