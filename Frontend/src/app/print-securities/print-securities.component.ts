import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SecuritiesService } from '../shared/securities.service';
import { CalendarService } from '../shared/calendar.service';
import { 
    SecuritiesDistribution, 
    DocumentTypes
} from '../models/SecuritiesDistribution';

@Component({
    selector: 'app-print-securities',
    templateUrl: './print-securities.component.html',
    styleUrls: ['./print-securities.component.scss']
})
export class PrintSecuritiesComponent implements OnInit {
    item: SecuritiesDistribution | null = null;
    isLoading = true;
    currentDate = new Date();

    constructor(
        private route: ActivatedRoute,
        private securitiesService: SecuritiesService,
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
        this.securitiesService.getById(id, calendar).subscribe({
            next: (data) => {
                this.item = data;
                this.isLoading = false;
                setTimeout(() => {
                    window.print();
                }, 500);
            },
            error: (err) => {
                console.error('Error loading data:', err);
                this.isLoading = false;
            }
        });
    }

    getDocumentTypeName(type: number | undefined): string {
        if (!type) return '-';
        const found = DocumentTypes.find(x => x.id === type);
        return found ? found.name : '-';
    }

    getPricePerUnit(type: number | undefined): number {
        if (!type) return 0;
        const found = DocumentTypes.find(x => x.id === type);
        return found ? found.pricePerUnit : 0;
    }

    getTotalDocumentCount(): number {
        if (!this.item || !this.item.items) return 0;
        return this.item.items.reduce((sum, item) => sum + item.count, 0);
    }

    getTotalPrice(): number {
        if (!this.item) return 0;
        return this.item.totalSecuritiesPrice || 0;
    }
}
