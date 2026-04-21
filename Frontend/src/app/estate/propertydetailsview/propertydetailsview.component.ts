import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { MatDialog } from '@angular/material/dialog';
import { LocalizationService } from 'src/app/shared/localization.service';
import { PropertyService } from 'src/app/shared/property.service';
import { FileService } from 'src/app/shared/file.service';
import { DocumentViewerComponent } from 'src/app/shared/document-viewer/document-viewer.component';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarType } from 'src/app/models/calendar-type';

@Component({
  selector: 'app-propertydetailsview',
  templateUrl: './propertydetailsview.component.html',
  styleUrls: ['./propertydetailsview.component.scss']
})
export class PropertydetailsviewComponent extends BaseComponent {
  isLoading = true;
  error: string | null = null;
  viewData: any = null;

  constructor(
    private route: ActivatedRoute,
    private propertyService: PropertyService,
    private localizationService: LocalizationService,
    private dialog: MatDialog,
    private fileService: FileService,
    private calendarConversionService: CalendarConversionService
  ) {
    super();
  }

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const idParam = params.get('id');
      const id = idParam ? Number(idParam) : 0;
      if (!id) {
        this.isLoading = false;
        this.error = 'شناسهٔ ملکیت معتبر نیست';
        return;
      }

      this.loadView(id);
    });
  }

  private loadView(id: number): void {
    this.isLoading = true;
    this.error = null;
    this.viewData = null;

    this.propertyService.getPropertyViewById(id).subscribe({
      next: (data) => {
        console.log('Property View Data:', data);
        console.log('Buyers:', data.buyers);
        this.viewData = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Property details view load failed', err);
        this.error = 'خطا در دریافت معلومات ملکیت';
        this.isLoading = false;
      }
    });
  }

  getDariPropertyTypeLabel(propertyTypeValue: any): string {
    const value = (propertyTypeValue ?? '').toString();
    const match = this.localizationService.propertyTypes.find(pt => pt.value === value);
    return match?.label || (this.viewData?.propertyTypeText ?? 'سایر');
  }

  getDariUnitTypeLabel(unitTypeValue: any): string {
    const value = (unitTypeValue ?? '').toString();
    const match = this.localizationService.propertyUnitTypes.find(ut => ut.value === value);
    return match?.label || (value || '---');
  }

  getDariTransactionTypeLabel(transactionTypeValue: any): string {
    const value = (transactionTypeValue ?? '').toString().trim();
    if (!value) {
      return '---';
    }
    // Convert to lowercase for case-insensitive matching
    const lowerValue = value.toLowerCase();
    switch (lowerValue) {
      case 'sale':
      case 'purchase':
        return 'خرید و فروش';
      case 'rent':
        return 'کرایه';
      case 'lease':
        return 'اجاره';
      case 'mortgage':
        return 'رهن';
      case 'exchange':
        return 'تبادله';
      case 'gift':
        return 'هبه';
      case 'inheritance':
        return 'ارث';
      default:
        // If it's already in Dari, return as is, otherwise return the value
        return value;
    }
  }

  getRoleLabel(roleValue: any): string {
    const value = (roleValue ?? '').toString();
    const all = Object.values(this.localizationService.roleTypes) as Array<{ value: string; label: string }>;
    const match = all.find(r => r?.value === value);
    return match?.label || (value || '---');
  }

  safeValue(value: any): string {
    if (value === null || value === undefined || value === '') {
      return '---';
    }
    return String(value);
  }

  getTransactionTypeDisplay(): string {
    const propValue = this.viewData?.transactionTypeName;
    if (propValue) {
      return this.getDariTransactionTypeLabel(propValue);
    }
    const buyerType = this.viewData?.buyers?.[0]?.transactionType;
    if (buyerType) {
      return this.getDariTransactionTypeLabel(buyerType);
    }
    return '---';
  }

  getPriceDisplay(): string {
    const propPrice = this.viewData?.priceText || this.viewData?.price;
    if (propPrice) {
      return this.safeValue(propPrice);
    }
    const buyerPrice = this.viewData?.buyers?.[0]?.priceText || this.viewData?.buyers?.[0]?.price;
    if (buyerPrice) {
      return this.safeValue(buyerPrice);
    }
    return '---';
  }

  getRoyaltyAmountDisplay(): string {
    const propRoyalty = this.viewData?.royaltyAmount;
    if (propRoyalty && propRoyalty !== '0' && propRoyalty !== 0) {
      return this.safeValue(propRoyalty);
    }
    const buyerRoyalty = this.viewData?.buyers?.[0]?.royaltyAmount;
    if (buyerRoyalty && buyerRoyalty !== '0' && buyerRoyalty !== 0) {
      return this.safeValue(buyerRoyalty);
    }
    return '---';
  }

  /**
   * Get the appropriate label for issuance number based on document type
   */
  getIssuanceNumberLabel(): string {
    const docType = this.viewData?.documentType;
    if (docType === 'سند ملکیت') {
      return 'نمبر سند ملکیت';
    } else if (docType === 'قباله شرعی') {
      return 'نمبر قباله';
    }
    return 'نمبر سند';
  }

  /**
   * Get the document type display value (shows custom type if "سایر" is selected)
   */
  getDocumentTypeDisplay(): string {
    const docType = this.viewData?.documentType;
    if (docType === 'سایر' && this.viewData?.customDocumentType) {
      return this.viewData.customDocumentType;
    }
    return this.safeValue(docType);
  }

  formatDate(value: any): string {
    if (!value) {
      return '---';
    }
    const d = new Date(value);
    if (Number.isNaN(d.getTime())) {
      return '---';
    }
    // Convert to Hijri Shamsi
    return this.calendarConversionService.formatDate(d, CalendarType.HIJRI_SHAMSI);
  }

  getFileName(filePath: string): string {
    if (!filePath) {
      return '';
    }
    const parts = filePath.split(/[\\/]/);
    return parts[parts.length - 1] || filePath;
  }

  private normalizeFilePath(filePath: string): string {
    let p = (filePath ?? '').toString().trim();
    if (!p) {
      return '';
    }

    try {
      p = decodeURIComponent(p);
    } catch {
    }

    p = p.replace(/\\/g, '/');
    p = p.replace(/\/{2,}/g, '/');
    p = p.replace(/^\//, '');

    const lower = p.toLowerCase();
    const lastResourcesIdx = lower.lastIndexOf('resources/');
    if (lastResourcesIdx >= 0) {
      p = p.substring(lastResourcesIdx);
    }

    return p;
  }

  openDocument(filePath: string, fileName?: string): void {
    const normalizedPath = this.normalizeFilePath(filePath);
    if (!normalizedPath) {
      return;
    }

    this.dialog.open(DocumentViewerComponent, {
      width: '90%',
      maxWidth: '1200px',
      height: '90vh',
      data: {
        filePath: normalizedPath,
        fileName: fileName || this.getFileName(normalizedPath)
      }
    });
  }

  downloadDocument(filePath: string): void {
    const normalizedPath = this.normalizeFilePath(filePath);
    if (!normalizedPath) {
      return;
    }
    const link = document.createElement('a');
    link.href = this.fileService.getDownloadUrl(normalizedPath);
    link.download = this.getFileName(normalizedPath);
    link.click();
  }

  buildDocItems(docMap: { label: string; path?: string }[]): { label: string; path: string; fileName: string }[] {
    return (docMap || [])
      .filter(x => !!x?.path)
      .map(x => ({
        label: x.label,
        path: x.path as string,
        fileName: this.getFileName(x.path as string)
      }));
  }
}
