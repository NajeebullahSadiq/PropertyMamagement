import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SecuritiesService } from '../shared/securities.service';
import { CalendarService } from '../shared/calendar.service';
import { 
    SecuritiesDistribution, 
    DocumentTypes, 
    PropertySubTypes, 
    VehicleSubTypes,
    RegistrationBookTypes 
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

    getTotalDocumentCount(): number {
        if (!this.item) return 0;
        let total = 0;
        if (this.item.propertySaleCount) total += this.item.propertySaleCount;
        if (this.item.bayWafaCount) total += this.item.bayWafaCount;
        if (this.item.rentCount) total += this.item.rentCount;
        if (this.item.vehicleSaleCount) total += this.item.vehicleSaleCount;
        if (this.item.vehicleExchangeCount) total += this.item.vehicleExchangeCount;
        return total;
    }
}
