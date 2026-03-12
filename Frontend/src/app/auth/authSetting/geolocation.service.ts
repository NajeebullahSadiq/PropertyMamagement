import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, timer } from 'rxjs';
import { catchError, map, timeout, retryWhen, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class GeolocationService {
  private readonly AFGHANISTAN_COUNTRY_CODE = 'AF';
  private readonly AFGHANISTAN_COUNTRY_NAME = 'Afghanistan';
  // IMPORTANT: Set to true to enable geolocation check
  // Set to false to disable (for development/testing)
  private readonly ENABLE_GEOLOCATION_CHECK = true;
  private readonly API_TIMEOUT = 5000; // 5 seconds timeout

  constructor(private http: HttpClient) {}

  /**
   * Check if user is accessing from Afghanistan
   * Uses IP geolocation API to determine country
   */
  isAccessFromAfghanistan(): Observable<boolean> {
    // If geolocation check is disabled, allow access
    if (!this.ENABLE_GEOLOCATION_CHECK) {
      console.log('[Geolocation] Check disabled - allowing access');
      return of(true);
    }

    return this.http.get<any>('https://ipapi.co/json/', {
      responseType: 'json'
    }).pipe(
      timeout(this.API_TIMEOUT),
      map(response => {
        const countryCode = response?.country_code?.toUpperCase();
        const countryName = response?.country_name?.toLowerCase();
        
        const isAfghanistan = 
          countryCode === this.AFGHANISTAN_COUNTRY_CODE || 
          countryName === this.AFGHANISTAN_COUNTRY_NAME.toLowerCase();
        
        console.log(`[Geolocation] Country: ${response?.country_name} (${countryCode}), Access Allowed: ${isAfghanistan}`);
        return isAfghanistan;
      }),
      catchError(error => {
        console.error('[Geolocation] Error checking location:', error);
        console.warn('[Geolocation] Allowing access due to API error');
        return of(true); // Allow access on error to prevent blocking legitimate users
      })
    );
  }

  /**
   * Get current user's country information
   */
  getUserCountry(): Observable<any> {
    return this.http.get<any>('https://ipapi.co/json/', {
      responseType: 'json'
    }).pipe(
      timeout(this.API_TIMEOUT),
      catchError(error => {
        console.error('[Geolocation] Error fetching country info:', error);
        return of(null);
      })
    );
  }
}
