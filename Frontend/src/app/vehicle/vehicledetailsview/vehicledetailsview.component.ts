import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { FileService } from 'src/app/shared/file.service';
import { DocumentViewerComponent } from 'src/app/shared/document-viewer/document-viewer.component';
import { LocalizationService } from 'src/app/shared/localization.service';

@Component({
  selector: 'app-vehicledetailsview',
  templateUrl: './vehicledetailsview.component.html',
  styleUrls: ['./vehicledetailsview.component.scss']
})
export class VehicledetailsviewComponent implements OnInit {
  isLoading = true;
  error: string | null = null;
  viewData: any = null;

  constructor(
    private route: ActivatedRoute,
    private vehicleService: VehicleService,
    private dialog: MatDialog,
    private fileService: FileService,
    private localizationService: LocalizationService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      const id = idParam ? Number(idParam) : 0;
      if (!id) {
        this.isLoading = false;
        this.error = 'شناسهٔ وسیله نقلیه معتبر نیست';
        return;
      }
      this.loadView(id);
    });
  }

  private loadView(id: number): void {
    this.isLoading = true;
    this.error = null;
    this.viewData = null;

    this.vehicleService.getVehicleViewById(id).subscribe({
      next: (data) => {
        this.viewData = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Vehicle details view load failed', err);
        this.error = 'خطا در دریافت معلومات وسیله نقلیه';
        this.isLoading = false;
      }
    });
  }

  safeValue(value: any): string {
    if (value === null || value === undefined || value === '') {
      return '---';
    }
    return String(value);
  }

  formatDate(value: any): string {
    if (!value) return '---';
    const d = new Date(value);
    if (Number.isNaN(d.getTime())) return '---';
    const y = d.getFullYear();
    const m = `${d.getMonth() + 1}`.padStart(2, '0');
    const day = `${d.getDate()}`.padStart(2, '0');
    return `${y}-${m}-${day}`;
  }

  formatCurrency(value: any): string {
    if (value === null || value === undefined) return '---';
    return Number(value).toLocaleString('fa-AF') + ' افغانی';
  }

  getFileName(filePath: string): string {
    if (!filePath) return '';
    const parts = filePath.split(/[\\/]/);
    return parts[parts.length - 1] || filePath;
  }

  private normalizeFilePath(filePath: string): string {
    let p = (filePath ?? '').toString().trim();
    if (!p) return '';
    try { p = decodeURIComponent(p); } catch {}
    p = p.replace(/\\/g, '/').replace(/\/{2,}/g, '/').replace(/^\//, '');
    const lower = p.toLowerCase();
    const lastResourcesIdx = lower.lastIndexOf('resources/');
    if (lastResourcesIdx >= 0) p = p.substring(lastResourcesIdx);
    return p;
  }

  openDocument(filePath: string, fileName?: string): void {
    const normalizedPath = this.normalizeFilePath(filePath);
    if (!normalizedPath) return;
    this.dialog.open(DocumentViewerComponent, {
      width: '90%',
      maxWidth: '1200px',
      height: '90vh',
      data: { filePath: normalizedPath, fileName: fileName || this.getFileName(normalizedPath) }
    });
  }

  downloadDocument(filePath: string): void {
    const normalizedPath = this.normalizeFilePath(filePath);
    if (!normalizedPath) return;
    const link = document.createElement('a');
    link.href = this.fileService.getDownloadUrl(normalizedPath);
    link.download = this.getFileName(normalizedPath);
    link.click();
  }

  getTransactionTypeLabel(typeName: string | null): string {
    if (!typeName) return '---';
    switch (typeName) {
      case 'Sale': return 'فروش';
      case 'Rent': return 'کرایه';
      case 'Revocable Sale': return 'بیع جایزی';
      default: return typeName;
    }
  }

  getVehicleHandLabel(hand: string | null): string {
    if (!hand) return '---';
    const option = this.localizationService.vehicleHandOptions.find((o: any) => o.value === hand);
    return option ? option.label : hand;
  }

  getRoleLabel(roleType: string | null): string {
    if (!roleType) return '---';
    switch (roleType) {
      case 'Seller': return 'فروشنده';
      case 'Sales Agent': return 'وکیل فروشنده';
      case 'Lease Agent': return 'وکیل کرایه دهنده';
      case 'Agent for a revocable sale': return 'وکیل بیع جایزی';
      case 'Heirs': return 'ورثه';
      case 'Buyer': return 'خریدار';
      case 'Buyer Agent': return 'وکیل خریدار';
      case 'Renter': return 'کرایه گیرنده';
      case 'Renter Agent': return 'وکیل کرایه گیرنده';
      default: return roleType;
    }
  }

  printPage(): void {
    window.print();
  }
}
