import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { SecuritiesReceiptService } from 'src/app/shared/securities-receipt.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { 
    SecuritiesDistributionReceipt, 
    SecuritiesDistributionItem,
    DocumentTypes,
    DocumentTypeInfo
} from 'src/app/models/SecuritiesDistribution';

@Component({
    selector: 'app-securities-add-receipt',
    templateUrl: './securities-add-receipt.component.html',
    styleUrls: ['./securities-add-receipt.component.scss']
})
export class SecuritiesAddReceiptComponent extends BaseComponent implements OnInit {
    distributionId!: number;
    receipt: SecuritiesDistributionReceipt = {
        securitiesDistributionId: 0,
        bankReceiptNumber: '',
        deliveryDate: '',
        distributionDate: '',
        totalPrice: 0,
        notes: '',
        items: []
    };
    
    documentTypes = DocumentTypes;
    isSubmitting = false;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastrService,
        private receiptService: SecuritiesReceiptService,
        private calendarService: CalendarService
    ) {
        super();
    }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.distributionId = parseInt(id, 10);
            this.receipt.securitiesDistributionId = this.distributionId;
        }
    }

    onSubmit(): void {
        if (this.isSubmitting) return;
        this.isSubmitting = true;

        this.receiptService.addReceipt(this.receipt).pipe(takeUntil(this.destroy$)).subscribe({
            next: (result: any) => {
                this.isSubmitting = false;
                this.toastr.success('رسید با موفقیت ثبت شد');
                this.router.navigate(['/securities/view', this.distributionId]);
            },
            error: (err: any) => {
                this.isSubmitting = false;
                this.toastr.error('خطا در ثبت رسید');
                console.error(err);
            }
        });
    }

    addItem(): void {
        this.receipt.items.push({
            documentType: 0,
            serialStart: '',
            serialEnd: '',
            count: 0,
            price: 0,
            pricePerDocument: 0,
            totalPrice: 0
        } as unknown as SecuritiesDistributionItem);
    }

    removeItem(index: number): void {
        this.receipt.items.splice(index, 1);
    }

    goBack(): void {
        this.router.navigate(['/securities/view', this.distributionId]);
    }
}