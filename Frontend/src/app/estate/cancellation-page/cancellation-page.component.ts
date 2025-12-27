import { HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PropertyCancellationService } from 'src/app/shared/property-cancellation.service';
import { LocalizationService } from 'src/app/shared/localization.service';
import { DocumentViewerComponent } from 'src/app/shared/document-viewer/document-viewer.component';
import { FileService } from 'src/app/shared/file.service';

@Component({
  selector: 'app-cancellation-page',
  templateUrl: './cancellation-page.component.html',
  styleUrls: ['./cancellation-page.component.scss']
})
export class CancellationPageComponent implements OnInit {
  activeTransactions: any[] = [];
  cancelledTransactions: any[] = [];
  selectedTab: string = 'active';
  loading: boolean = false;
  showCancellationModal: boolean = false;
  selectedTransaction: any = null;
  cancellationReason: string = '';
  submitted: boolean = false;
  cancellationReasons: any[] = [];
  cancellationDocuments: Array<{
    filePath: string;
    originalFileName: string;
    progress: number;
    uploading: boolean;
    error?: string;
  }> = [];
  searchText: string = '';
  filterTransactionType: string = '';
  transactionTypes: any[] = [];

  constructor(
    private cancellationService: PropertyCancellationService,
    private localizationService: LocalizationService,
    private fileService: FileService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadTransactionTypes();
    this.loadCancellationReasons();
    this.loadActiveTransactions();
  }

  loadTransactionTypes(): void {
    this.transactionTypes = this.localizationService.transactionTypes;
  }

  loadCancellationReasons(): void {
    this.cancellationReasons = (this.localizationService as any).cancellationReasons || [];
  }

  loadActiveTransactions(): void {
    this.loading = true;
    this.cancellationService.getActiveTransactions().subscribe(
      (data) => {
        this.activeTransactions = data;
        this.loading = false;
      },
      (error) => {
        console.error('Error loading active transactions:', error);
        this.loading = false;
      }
    );
  }

  loadCancelledTransactions(): void {
    this.loading = true;
    this.cancellationService.getCancelledTransactions().subscribe(
      (data) => {
        this.cancelledTransactions = data;
        this.loading = false;
      },
      (error) => {
        console.error('Error loading cancelled transactions:', error);
        this.loading = false;
      }
    );
  }

  switchTab(tab: string): void {
    this.selectedTab = tab;
    if (tab === 'cancelled' && this.cancelledTransactions.length === 0) {
      this.loadCancelledTransactions();
    }
  }

  openCancellationModal(transaction: any): void {
    this.selectedTransaction = transaction;
    this.cancellationReason = '';
    this.submitted = false;
    this.cancellationDocuments = [];
    this.showCancellationModal = true;
  }

  closeCancellationModal(): void {
    this.showCancellationModal = false;
    this.selectedTransaction = null;
    this.cancellationReason = '';
    this.submitted = false;
    this.cancellationDocuments = [];
  }

  onCancellationFilesSelected(event: any): void {
    const files: FileList | null = event?.target?.files;
    if (!files || files.length === 0) return;

    Array.from(files).forEach(file => {
      const doc = {
        filePath: '',
        originalFileName: file.name,
        progress: 0,
        uploading: true as boolean,
        error: undefined as string | undefined
      };

      this.cancellationDocuments.push(doc);

      this.fileService.uploadFile(file, 'cancellation').subscribe({
        next: (uploadEvent) => {
          if (uploadEvent.type === HttpEventType.UploadProgress) {
            const total = uploadEvent.total || 1;
            doc.progress = Math.round(100 * (uploadEvent.loaded || 0) / total);
          }

          if (uploadEvent.type === HttpEventType.Response) {
            const response: any = uploadEvent.body;
            doc.filePath = response?.dbPath || '';
            doc.originalFileName = response?.originalFileName || doc.originalFileName;
            doc.progress = 100;
            doc.uploading = false;
            doc.error = undefined;
          }
        },
        error: () => {
          doc.uploading = false;
          doc.progress = 0;
          doc.error = 'خطا در آپلود فایل';
        }
      });
    });

    event.target.value = '';
  }

  viewDocument(filePath: string, fileName: string): void {
    if (!filePath) return;
    this.dialog.open(DocumentViewerComponent, {
      width: '90%',
      maxWidth: '1200px',
      height: '90vh',
      data: {
        filePath,
        fileName: fileName || this.extractFileName(filePath)
      }
    });
  }

  downloadDocument(filePath: string): void {
    if (!filePath) return;
    const link = document.createElement('a');
    link.href = this.fileService.getDownloadUrl(filePath);
    link.click();
  }

  private extractFileName(filePath: string): string {
    if (!filePath) return '';
    const parts = filePath.split(/[\\/]/);
    return parts[parts.length - 1] || '';
  }

  getUploadedCancellationDocumentsCount(): number {
    return this.cancellationDocuments.filter(d => !!d.filePath).length;
  }

  canConfirmCancellation(): boolean {
    const hasReason = !!this.cancellationReason && this.cancellationReason.trim().length > 0;
    const uploadedDocs = this.cancellationDocuments.filter(d => !!d.filePath);
    const isUploading = this.cancellationDocuments.some(d => d.uploading);
    return hasReason && uploadedDocs.length > 0 && !isUploading;
  }

  confirmCancellation(): void {
    if (!this.selectedTransaction) return;

    this.submitted = true;

    if (!this.canConfirmCancellation()) {
      return;
    }

    const documents = this.cancellationDocuments
      .filter(d => !!d.filePath)
      .map(d => ({ filePath: d.filePath, originalFileName: d.originalFileName }));

    this.loading = true;
    this.cancellationService.cancelTransaction(
      this.selectedTransaction.id,
      this.cancellationReason,
      documents
    ).subscribe(
      (response) => {
        this.closeCancellationModal();
        this.loadActiveTransactions();
        if (this.selectedTab === 'cancelled') {
          this.loadCancelledTransactions();
        }
        this.loading = false;
      },
      (error) => {
        console.error('Error cancelling transaction:', error);
        this.loading = false;
      }
    );
  }

  getCancellationReasonLabel(value: string): string {
    const reason = this.cancellationReasons.find(r => r.value === value);
    return reason ? reason.label : value;
  }

  getFilteredActiveTransactions(): any[] {
    return this.activeTransactions.filter(transaction => {
      const matchesSearch = !this.searchText || 
        transaction.sellerName.toLowerCase().includes(this.searchText.toLowerCase()) ||
        transaction.buyerName.toLowerCase().includes(this.searchText.toLowerCase());

      const matchesType = !this.filterTransactionType || 
        transaction.transactionTypeName === this.filterTransactionType;

      return matchesSearch && matchesType;
    });
  }

  getFilteredCancelledTransactions(): any[] {
    return this.cancelledTransactions.filter(transaction => {
      const matchesSearch = !this.searchText || 
        transaction.sellerName.toLowerCase().includes(this.searchText.toLowerCase()) ||
        transaction.buyerName.toLowerCase().includes(this.searchText.toLowerCase()) ||
        transaction.propertyNumber.toString().includes(this.searchText);

      const matchesType = !this.filterTransactionType || 
        transaction.transactionTypeName === this.filterTransactionType;

      return matchesSearch && matchesType;
    });
  }

  getTransactionTypeLabel(typeName: string): string {
    const type = this.transactionTypes.find(t => t.value === typeName);
    return type ? type.label : typeName;
  }

  formatDate(date: any): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('en-US');
  }
}
