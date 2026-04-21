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

  getActiveTransactions(page: number = 1, pageSize: number = 20): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetActiveTransactions?page=${page}&pageSize=${pageSize}`);
  }

  getCancelledTransactions(page: number = 1, pageSize: number = 20): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GetCancelledTransactions?page=${page}&pageSize=${pageSize}`);
  }

  cancelTransaction(
    propertyDetailsId: number,
    cancellationReason: string,
    documents: Array<{ filePath: string; originalFileName?: string }> = []
  ): Observable<any> {
    return this.http.post(`${this.baseUrl}/CancelTransaction`, {
      propertyDetailsId,
      cancellationReason,
      documents: documents.map(d => ({
        filePath: d.filePath,
        originalFileName: d.originalFileName
      }))
    });
  }

  isCancelled(propertyDetailsId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/IsCancelled/${propertyDetailsId}`);
  }
}
