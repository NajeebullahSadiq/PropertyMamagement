import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PetitionWriterSecuritiesService } from '../shared/petition-writer-securities.service';
import { CalendarService } from '../shared/calendar.service';
import { PetitionWriterSecurities } from '../models/PetitionWriterSecurities';

@Component({
    selector: 'app-print-petition-writer-securities',
    templateUrl: './print-petition-writer-securities.component.html',
    styleUrls: ['./print-petition-writer-securities.component.scss']
})
export class PrintPetitionWriterSecuritiesComponent implements OnInit {
    item: PetitionWriterSecurities | null = null;
    isLoading = true;

    constructor(
        private route: ActivatedRoute,
        private petitionService: PetitionWriterSecuritiesService,
        private calendarService: CalendarService
    ) { }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.loadData(parseInt(id, 10));
        }
    }

    loadData(id: number): void {
        const calendar = this.calendarService.getSelectedCalendar();
        this.petitionService.getById(id, calendar).subscribe({
            next: (data) => {
                this.item = data;
                this.isLoading = false;
                // Auto print after loading
                setTimeout(() => {
                    window.print();
                }, 500);
            },
            error: (err) => {
                this.isLoading = false;
                console.error(err);
            }
        });
    }
}
