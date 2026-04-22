import { Component, Input } from '@angular/core';

/**
 * Reusable loading skeleton component for list/table views.
 * Displays animated placeholder rows while data is loading.
 */
@Component({
  selector: 'app-loading-skeleton',
  template: `
    <div class="animate-pulse" [class]="containerClass">
      <!-- Header skeleton -->
      <div *ngIf="showHeader" class="flex items-center justify-between mb-4">
        <div class="h-6 bg-gray-200 rounded w-1/4"></div>
        <div class="h-8 bg-gray-200 rounded w-24"></div>
      </div>

      <!-- Table skeleton -->
      <div *ngIf="type === 'table'" class="space-y-3">
        <div class="flex gap-4 pb-3 border-b border-gray-100">
          <div *ngFor="let col of columns; let i = index" class="h-4 bg-gray-200 rounded"
               [style.width.%]="columnWidths[i] || 100 / columns.length"></div>
        </div>
        <div *ngFor="let row of rowArray" class="flex gap-4 py-3 border-b border-gray-50">
          <div *ngFor="let col of columns; let i = index" class="h-4 bg-gray-200 rounded"
               [style.width.%]="columnWidths[i] || 100 / columns.length"></div>
        </div>
      </div>

      <!-- Card skeleton -->
      <div *ngIf="type === 'card'" class="grid gap-4" [class]="gridClass">
        <div *ngFor="let row of rowArray" class="bg-white rounded-xl border border-gray-100 p-4 space-y-3">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-gray-200 rounded-full"></div>
            <div class="flex-1 space-y-2">
              <div class="h-4 bg-gray-200 rounded w-3/4"></div>
              <div class="h-3 bg-gray-200 rounded w-1/2"></div>
            </div>
          </div>
          <div class="h-3 bg-gray-200 rounded w-full"></div>
          <div class="h-3 bg-gray-200 rounded w-5/6"></div>
        </div>
      </div>

      <!-- List skeleton -->
      <div *ngIf="type === 'list'" class="space-y-3">
        <div *ngFor="let row of rowArray" class="flex items-center gap-4 py-3 border-b border-gray-50">
          <div class="w-10 h-10 bg-gray-200 rounded-full flex-shrink-0"></div>
          <div class="flex-1 space-y-2">
            <div class="h-4 bg-gray-200 rounded w-3/4"></div>
            <div class="h-3 bg-gray-200 rounded w-1/2"></div>
          </div>
          <div class="h-8 bg-gray-200 rounded w-20"></div>
        </div>
      </div>

      <!-- Form skeleton -->
      <div *ngIf="type === 'form'" class="space-y-4">
        <div class="grid gap-4" [class]="gridClass">
          <div *ngFor="let field of columns" class="space-y-2">
            <div class="h-4 bg-gray-200 rounded w-24"></div>
            <div class="h-10 bg-gray-200 rounded w-full"></div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    @keyframes shimmer {
      0% { background-position: -200% 0; }
      100% { background-position: 200% 0; }
    }
    .animate-pulse > div > div {
      background: linear-gradient(90deg, #e5e7eb 25%, #f3f4f6 50%, #e5e7eb 75%);
      background-size: 200% 100%;
      animation: shimmer 1.5s infinite;
    }
  `]
})
export class LoadingSkeletonComponent {
  /** Skeleton type: 'table' | 'card' | 'list' | 'form' */
  @Input() type: 'table' | 'card' | 'list' | 'form' = 'table';
  /** Number of rows to show */
  @Input() rows = 5;
  /** Number of columns (for table) or fields (for form) */
  @Input() columns: string[] = ['col1', 'col2', 'col3', 'col4', 'col5'];
  /** Relative column widths for table type */
  @Input() columnWidths: number[] = [];
  /** Show header skeleton */
  @Input() showHeader = true;
  /** Additional container CSS classes */
  @Input() containerClass = '';
  /** Grid columns class for card/form types */
  @Input() gridClass = 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3';

  get rowArray(): number[] {
    return Array(this.rows).fill(0).map((_, i) => i);
  }
}
