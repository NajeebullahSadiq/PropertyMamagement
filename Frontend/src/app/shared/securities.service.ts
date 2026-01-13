import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { 
    SecuritiesDistribution, 
    SecuritiesDistributionData, 
    SecuritiesDistributionListResponse 
} from '../models/SecuritiesDistribution';

@Injectable({
    providedIn: 'root'
})
export class SecuritiesService {
    private baseUrl = environment.apiURL + '/SecuritiesDistribution';

    constructor(private http: HttpClient) { }

    /**
     * Get all securities distributions with pagination and search
     */
    getAll(
        page: number = 1, 
        pageSize: number = 10, 
        search?: string,
        calendarType?: string
    ): Observable<SecuritiesDistributionListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }

        return this.http.get<SecuritiesDistributionListResponse>(this.baseUrl, { params });
    }

    /**
     * Get securities distribution by ID
     */
    getById(id: number, calendarType?: string): Observable<SecuritiesDistribution> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<SecuritiesDistribution>(`${this.baseUrl}/${id}`, { params });
    }

    /**
     * Create new securities distribution
     */
    create(data: SecuritiesDistributionData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(this.baseUrl, data);
    }

    /**
     * Update securities distribution
     */
    update(id: number, data: SecuritiesDistributionData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(`${this.baseUrl}/${id}`, data);
    }

    /**
     * Delete securities distribution
     */
    delete(id: number): Observable<{ message: string }> {
        return this.http.delete<{ message: string }>(`${this.baseUrl}/${id}`);
    }
}
