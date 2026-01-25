import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
    PetitionWriterLicense,
    PetitionWriterLicenseData,
    PetitionWriterLicenseListResponse,
    PetitionWriterRelocation,
    PetitionWriterRelocationData
} from '../models/PetitionWriterLicense';

@Injectable({
    providedIn: 'root'
})
export class PetitionWriterLicenseService {
    private baseUrl = environment.apiURL + '/PetitionWriterLicense';
    
    private dataChangedSubject = new Subject<void>();
    dataChanged$ = this.dataChangedSubject.asObservable();
    
    mainTableId: number = 0;

    constructor(private http: HttpClient) { }

    notifyDataChanged(): void {
        this.dataChangedSubject.next();
    }

    // ==================== Main License CRUD ====================

    getAll(
        page: number = 1,
        pageSize: number = 10,
        search?: string,
        calendarType?: string
    ): Observable<PetitionWriterLicenseListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }

        return this.http.get<PetitionWriterLicenseListResponse>(this.baseUrl, { params });
    }

    getById(id: number, calendarType?: string): Observable<PetitionWriterLicense> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<PetitionWriterLicense>(`${this.baseUrl}/${id}`, { params });
    }

    create(data: PetitionWriterLicenseData): Observable<PetitionWriterLicense> {
        return this.http.post<PetitionWriterLicense>(this.baseUrl, data).pipe(
            tap((result) => {
                if (result.id) {
                    this.mainTableId = result.id;
                }
                this.notifyDataChanged();
            })
        );
    }

    update(id: number, data: PetitionWriterLicenseData): Observable<PetitionWriterLicense> {
        return this.http.put<PetitionWriterLicense>(`${this.baseUrl}/${id}`, data).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    // ==================== Relocation CRUD ====================

    getRelocations(licenseId: number, calendarType?: string): Observable<PetitionWriterRelocation[]> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<PetitionWriterRelocation[]>(`${this.baseUrl}/${licenseId}/relocations`, { params });
    }

    createRelocation(licenseId: number, data: PetitionWriterRelocationData): Observable<PetitionWriterRelocation> {
        return this.http.post<PetitionWriterRelocation>(`${this.baseUrl}/${licenseId}/relocations`, data).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    updateRelocation(licenseId: number, relocationId: number, data: PetitionWriterRelocationData): Observable<PetitionWriterRelocation> {
        return this.http.put<PetitionWriterRelocation>(`${this.baseUrl}/${licenseId}/relocations/${relocationId}`, data).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    deleteRelocation(licenseId: number, relocationId: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${licenseId}/relocations/${relocationId}`).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    // ==================== Status Update ====================

    updateStatus(id: number, status: number, cancellationDate?: string, calendarType?: string): Observable<void> {
        return this.http.patch<void>(`${this.baseUrl}/${id}/status`, {
            licenseStatus: status,
            cancellationDate,
            calendarType
        }).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    // ==================== Provinces ====================

    getProvinces(): Observable<any> {
        return this.http.get(`${this.baseUrl}/provinces`);
    }
}
