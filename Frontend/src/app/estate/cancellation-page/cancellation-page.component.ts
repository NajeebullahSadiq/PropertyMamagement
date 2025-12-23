import { Component, OnInit } from '@angular/core';
import { PropertyCancellationService } from 'src/app/shared/property-cancellation.service';
import { LocalizationService } from 'src/app/shared/localization.service';

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
  searchText: string = '';
  filterTransactionType: string = '';
  transactionTypes: any[] = [];

  constructor(
    private cancellationService: PropertyCancellationService,
    private localizationService: LocalizationService
  ) { }

  ngOnInit(): void {
    this.loadTransactionTypes();
    this.loadActiveTransactions();
  }

  loadTransactionTypes(): void {
    this.transactionTypes = this.localizationService.transactionTypes;
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
    this.showCancellationModal = true;
  }

  closeCancellationModal(): void {
    this.showCancellationModal = false;
    this.selectedTransaction = null;
    this.cancellationReason = '';
  }

  confirmCancellation(): void {
    if (!this.selectedTransaction) return;

    this.loading = true;
    this.cancellationService.cancelTransaction(
      this.selectedTransaction.id,
      this.cancellationReason
    ).subscribe(
      (response) => {
        this.closeCancellationModal();
        this.loadActiveTransactions();
        this.loading = false;
      },
      (error) => {
        console.error('Error cancelling transaction:', error);
        this.loading = false;
      }
    );
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
