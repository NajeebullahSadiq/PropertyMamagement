import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
export class SecuritiesAddReceiptComponent implements OnInit {
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

    c