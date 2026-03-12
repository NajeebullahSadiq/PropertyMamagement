import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { GeolocationService } from './geolocation.service';

@Injectable({
  providedIn: 'root'
})
export class GeolocationGuard implements CanActivate {
  constructor(
    private geolocationService: GeolocationService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> {
    return this.geolocationService.isAccessFromAfghanistan().pipe(
      tap(isAllowed => {
        if (!isAllowed) {
          console.warn('[GeolocationGuard] Access denied: User is not in Afghanistan');
          this.router.navigate(['/access-denied']);
        } else {
          console.log('[GeolocationGuard] Access allowed');
        }
      }),
      catchError(error => {
        console.error('[GeolocationGuard] Error in geolocation check:', error);
        // Allow access on error
        console.warn('[GeolocationGuard] Allowing access due to error');
        return of(true);
      })
    );
  }
}
