import { Directive, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

/**
 * Base component class that provides automatic unsubscription via destroy$ Subject.
 * Extend this class in components that have subscriptions, then use:
 *   this.someObservable.pipe(takeUntil(this.destroy$)).subscribe(...)
 * 
 * This ensures all subscriptions are cleaned up when the component is destroyed,
 * preventing memory leaks.
 */
@Directive()
export class BaseComponent implements OnDestroy {
  protected destroy$ = new Subject<void>();

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
