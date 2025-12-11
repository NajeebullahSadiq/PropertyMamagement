import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PropertyCancellationService {
  private baseUrl = environment.apiUrl + '/PropertyCancellation';

  constructor(private http: HttpClient) { }

  getActiveTransactions(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/GetActiveTransactions`);
  }

  getCancelledTransactions(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/GetCancelledTransactions`);
  }

  cancelTransaction(propertyDetailsId: number, cancellationReason: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/CancelTransaction`, {
      propertyDetailsId,
      cancellationReason
    });
  }

  isCancelled(propertyDetailsId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/IsCancelled/${propertyDetailsId}`);
  }
}
