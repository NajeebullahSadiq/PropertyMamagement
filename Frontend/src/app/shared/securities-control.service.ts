import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { 
    SecuritiesControl, 
    SecuritiesControlData, 
    SecuritiesControlListResponse 
} from '../models/SecuritiesControl';

@Injectable({
    providedIn: 'root'
})
export class SecuritiesControlService {
    private baseUrl = environment.apiURL + '/SecuritiesControl';
    
    // Subject to notify list component of data changes
    private dataChangedSubject = new Subject<void>();
    dataChanged$ = this.dataChangedSubject.asObservable();

    constructor(private http: HttpClient) { }

    notifyDataChanged(): void {
        this.dataChangedSubject.next();
    }

    /**
     * Get all securities controls with pagination and search
     */
    getAll(
        page: number = 1, 
        pageSize: number = 10, 
        search?: string,
        calendarType?: string
    ): Observable<SecuritiesControlListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }

        return this.http.get<SecuritiesControlListResponse>(this.baseUrl, { params });
    }

    /**
     * Get securities control by ID
     */
    getById(id: number, calendarType?: string): Observable<SecuritiesControl> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<SecuritiesControl>(`${this.baseUrl}/${id}`, { params });
    }

    /**
     * Create new securities control
     */
    create(data: SecuritiesControlData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(this.baseUrl, data).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    /**
     * Update securities control
     */
    update(id: number, data: SecuritiesControlData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(`${this.baseUrl}/${id}`, data).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    /**
     * Delete securities control
     */
    delete(id: number): Observable<{ message: string }> {
        return this.http.delete<{ message: string }>(`${this.baseUrl}/${id}`).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    /**
     * Get next serial number
     */
    getNextSerialNumber(): Observable<{ serialNumber: string }> {
        return this.http.get<{ serialNumber: string }>(`${this.baseUrl}/next-serial`);
    }
}