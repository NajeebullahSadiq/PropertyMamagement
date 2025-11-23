import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResetService {

  private resetFormSubject = new Subject<void>();

  resetForm$ = this.resetFormSubject.asObservable();

  triggerFormReset(): void {
    this.resetFormSubject.next();
  }
}
