import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { 
    PetitionWriterSecurities, 
    PetitionWriterSecuritiesData, 
    PetitionWriterSecuritiesListResponse 
} from '../models/PetitionWriterSecurities';

@Injectable({
    providedIn: 'root'
})
export class PetitionWriterSecuritiesService {
    private baseUrl = environment.apiURL + '/PetitionWriterSecurities';
    
    // Subject to notify when data changes (like PropertyService.propertyAdded)
    dataChanged = new Subject<void>();

    constructor(private http: HttpClient) { }

    /**
     * Get all petition writer securities with pagination and search
     */
    getAll(
        page: number = 1, 
        pageSize: number = 10, 
        search?: string,
        calendarType?: string
    ): Observable<PetitionWriterSecuritiesListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }

        return this.http.get<PetitionWriterSecuritiesListResponse>(this.baseUrl, { params });
    }

    /**
     * Get petition writer securities by ID
     */
    getById(id: number, calendarType?: string): Observable<PetitionWriterSecurities> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<PetitionWriterSecurities>(`${this.baseUrl}/${id}`, { params });
    }

    /**
     * Create new petition writer securities
     */
    create(data: PetitionWriterSecuritiesData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(this.baseUrl, data);
    }

    /**
     * Update petition writer securities
     */
    update(id: number, data: PetitionWriterSecuritiesData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(`${this.baseUrl}/${id}`, data);
    }

    /**
     * Delete petition writer securities
     */
    delete(id: number): Observable<{ message: string }> {
        return this.http.delete<{ message: string }>(`${this.baseUrl}/${id}`);
    }
}
