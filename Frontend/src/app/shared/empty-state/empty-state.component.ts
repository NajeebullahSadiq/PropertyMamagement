import { Component, Input, Output, EventEmitter } from '@angular/core';

/**
 * Reusable empty state component for lists and tables.
 * Displays a friendly message when no data is available,
 * with optional icon, description, and action button.
 */
@Component({
  selector: 'app-empty-state',
  template: `
    <div class="flex flex-col items-center justify-center py-16 px-4 text-center" [class]="containerClass">
      <!-- Icon -->
      <div class="w-20 h-20 rounded-full bg-gray-100 flex items-center justify-center mb-4">
        <mat-icon class="text-4xl text-gray-400">{{ icon }}</mat-icon>
      </div>

      <!-- Title -->
      <h3 class="text-lg font-bold text-gray-700 mb-2">{{ title }}</h3>

      <!-- Description -->
      <p class="text-sm text-gray-500 max-w-md mb-6 leading-relaxed">{{ description }}</p>

      <!-- Action button -->
      <button *ngIf="showAction" (click)="action.emit()"
              class="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl text-sm font-medium
                     bg-indigo-600 text-white hover:bg-indigo-700 active:bg-indigo-800
                     transition-all duration-200 shadow-md hover:shadow-lg">
        <mat-icon class="text-lg">{{ actionIcon }}</mat-icon>
        <span>{{ actionLabel }}</span>
      </button>
    </div>
  `,
  styles: [``]
})
export class EmptyStateComponent {
  /** Material icon name */
  @Input() icon = 'inbox';
  /** Main message */
  @Input() title = 'هیچ موردی یافت نشد';
  /** Secondary description */
  @Input() description = 'در حال حاضر اطلاعاتی برای نمایش وجود ندارد';
  /** Show action button */
  @Input() showAction = false;
  /** Action button label */
  @Input() actionLabel = 'تلاش مجدد';
  /** Action button icon */
  @Input() actionIcon = 'refresh';
  /** Additional CSS classes */
  @Input() containerClass = '';

  @Output() action = new EventEmitter<void>();
}
