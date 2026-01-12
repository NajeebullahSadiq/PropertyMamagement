import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { FileService } from 'src/app/shared/file.service';
import { DocumentViewerComponent } from 'src/app/shared/document-viewer/document-viewer.component';

@Component({
  selector: 'app-companydetailsview',
  templateUrl: './companydetailsview.component.html',
  styleUrls: ['./companydetailsview.component.scss']
})
export class CompanydetailsviewComponent {
  isLoading = true;
  error: string | null = null;
  viewData: any = null;
  private companyId: number = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private companyService: CompnaydetailService,
    private dialog: MatDialog,
    private fileService: FileService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      const id = idParam ? Number(idParam) : 0;
      if (!id) {
        this.isLoading = false;
        this.error = 'شناسهٔ شرکت معتبر نیست';
        return;
      }
      this.companyId = id;
      this.loadView(id);
    });
  }

  printLicense(): void {
    if (this.companyId) {
      const url = this.router.createUrlTree(['/printLicense', this.companyId]).toString();
      window.open(url, '_blank', 'noopener,noreferrer');
    }
  }

  private loadView(id: number): void {
    this.isLoading = true;
    this.error = null;
    this.viewData = null;

    this.companyService.getCompanyViewById(id).subscribe({
      next: (data) => {
        this.viewData = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Company details view load failed', err);
        this.error = 'خطا در دریافت معلومات شرکت';
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

  getGuaranteeTypeLabel(typeId: number | null): string {
    if (!typeId) return '---';
    switch (typeId) {
      case 1: return 'قباله شرعی';
      case 2: return 'قباله عرفی';
      case 3: return 'پول نقد';
      default: return '---';
    }
  }

  getLicenseCategoryLabel(category: string | null): string {
    if (!category) return '---';
    switch (category) {
      case 'New': return 'جدید';
      case 'Renewal': return 'تجدید';
      case 'Duplicate': return 'مثنی';
      default: return category;
    }
  }
}
