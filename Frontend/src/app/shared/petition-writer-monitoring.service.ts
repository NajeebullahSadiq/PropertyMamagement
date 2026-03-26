import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
    PetitionWriterMonitoringRecord,
    PetitionWriterMonitoringData,
    PetitionWriterMonitoringListResponse
} from '../models/PetitionWriterMonitoring';

@Injectable({
    providedIn: 'root'
})
export class PetitionWriterMonitoringService {
    private baseUrl = environment.apiURL + '/PetitionWriterMonitoring';
    
    private dataChangedSubject = new Subject<void>();
    dataChanged$ = this.dataChangedSubject.asObservable();
    
    mainTableId: number = 0;

    constructor(private http: HttpClient) { }

    notifyDataChanged(): void {
        this.dataChangedSubject.next();
    }

    // ==================== Main Record CRUD ====================

    getAll(
        page: number = 1,
        pageSize: number = 10,
        search?: string,
        sectionType?: string,
        calendarType?: string
    ): Observable<PetitionWriterMonitoringListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }
        if (sectionType) {
            params = params.set('sectionType', sectionType);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }

        return this.http.get<PetitionWriterMonitoringListResponse>(this.baseUrl, { params });
    }

    getById(id: number, calendarType?: string): Observable<PetitionWriterMonitoringRecord> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<PetitionWriterMonitoringRecord>(`${this.baseUrl}/${id}`, { params });
    }

    create(data: PetitionWriterMonitoringData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(this.baseUrl, data).pipe(
            tap((result) => {
                this.mainTableId = result.id;
                this.notifyDataChanged();
            })
        );
    }

    update(id: number, data: PetitionWriterMonitoringData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(`${this.baseUrl}/${id}`, data).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    // ==================== Utility ====================

    resetMainTableId(): void {
        this.mainTableId = 0;
    }

    getNextSerialNumber(): Observable<{ serialNumber: string }> {
        return this.http.get<{ serialNumber: string }>(`${this.baseUrl}/next-serial-number`);
    }

    // Search petition writer license by license number for violations section
    searchLicenseByNumber(licenseNumber: string): Observable<{
        id: number;
        licenseNumber: string;
        petitionWriterName: string;
        petitionWriterDistrict: string;
    }> {
        return this.http.get<{
            id: number;
            licenseNumber: string;
            petitionWriterName: string;
            petitionWriterDistrict: string;
        }>(`${this.baseUrl}/search-license/${encodeURIComponent(licenseNumber)}`);
    }
}
