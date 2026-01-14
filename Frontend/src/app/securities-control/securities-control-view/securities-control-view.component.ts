import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SecuritiesControlService } from 'src/app/shared/securities-control.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { SecuritiesControl } from 'src/app/models/SecuritiesControl';

@Component({
    selector: 'app-securities-control-view',
    templateUrl: './securities-control-view.component.html',
    styleUrls: ['./securities-control-view.component.scss']
})
export class SecuritiesControlViewComponent implements OnInit {
    item: SecuritiesControl | null = null;
    isLoading = true;
    error: string | null = null;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private securitiesControlService: SecuritiesControlService,
        private calendarService: CalendarService,
        private toastr: ToastrService
    ) { }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.loadItem(parseInt(id, 10));
        } else {
            this.error = 'ID is required';
            this.isLoading = false;
        }
    }

    loadItem(id: number): void {
        this.isLoading = true;
        const calendar = this.calendarService.getSelectedCalendar();
        
        this.securitiesControlService.getById(id, calendar).subscribe({
            next: (data) => {
                this.item = data;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('خطا در بارگذاری اطلاعات');
                this.error = err.message || 'خطا در بارگذاری اطلاعات';
                this.isLoading = false;
                console.error(err);
            }
        });
    }

    editItem(): void {
        if (this.item?.id) {
            this.router.navigate(['/securities-control/edit', this.item.id]);
        }
    }

    goToList(): void {
        this.router.navigate(['/securities-control/list']);
    }

    printItem(): void {
        // Assuming there's a print endpoint for securities control
        if (this.item?.id) {
            window.open(`/printSecuritiesControl/${this.item.id}`, '_blank');
        }
    }

    getSecurityDocumentTypeName(type: number): string {
        switch(type) {
            case 1: return 'ستههای رهنمای معاملات';
            case 2: return 'کتاب ثبت معاملات';
            case 3: return 'عرایض مطبوع';
            default: return 'نامشخص';
        }
    }

    getSecuritiesTypeName(type: number | undefined): string {
        if (!type) return '-';
        switch(type) {
            case 1: return 'ستههای خرید و فروش جایداد';
            case 2: return 'ستههای بیع وفا';
            case 3: return 'ستههای کرایی';
            case 4: return 'ستههای خرید و فروش وسایط نقلیه';
            case 5: return 'ستههای تبادله';
            case 6: return 'کتاب ثبت معاملات';
            case 7: return 'عرایض مطبوع';
            case 8: return 'ستههای خرید و فروش جایداد و بیع وفا';
            case 9: return 'ستههای خرید و فروش جایداد و کرایی';
            case 10: return 'ستههای بیع وفا و کرایی';
            case 11: return 'تمام انواع ستههای جایداد';
            default: return 'نامشخص';
        }
    }

    getTotalDocumentCount(): number {
        if (!this.item) return 0;
        let total = 0;
        if (this.item.propertySaleCount) total += this.item.propertySaleCount;
        if (this.item.bayWafaCount) total += this.item.bayWafaCount;
        if (this.item.rentCount) total += this.item.rentCount;
        if (this.item.vehicleSaleCount) total += this.item.vehicleSaleCount;
        if (this.item.exchangeCount) total += this.item.exchangeCount;
        if (this.item.registrationBookCount) total += this.item.registrationBookCount;
        if (this.item.printedPetitionCount) total += this.item.printedPetitionCount;
        return total;
    }
}