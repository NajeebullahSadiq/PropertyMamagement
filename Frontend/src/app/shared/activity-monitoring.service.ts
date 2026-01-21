import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
    ActivityMonitoringRecord,
    ActivityMonitoringData,
    ActivityMonitoringListResponse,
    Complaint,
    ComplaintData,
    RealEstateViolation,
    RealEstateViolationData,
    PetitionWriterViolation,
    PetitionWriterViolationData
} from '../models/ActivityMonitoring';

@Injectable({
    providedIn: 'root'
})
export class ActivityMonitoringService {
    private baseUrl = environment.apiURL + '/ActivityMonitoring';
    
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
        calendarType?: string
    ): Observable<ActivityMonitoringListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }

        return this.http.get<ActivityMonitoringListResponse>(this.baseUrl, { params });
    }

    getById(id: number, calendarType?: string): Observable<ActivityMonitoringRecord> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<ActivityMonitoringRecord>(`${this.baseUrl}/${id}`, { params });
    }

    create(data: ActivityMonitoringData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(this.baseUrl, data).pipe(
            tap((result) => {
                this.mainTableId = result.id;
                this.notifyDataChanged();
            })
        );
    }

    update(id: number, data: ActivityMonitoringData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(`${this.baseUrl}/${id}`, data).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    // ==================== Complaints CRUD ====================

    getComplaints(recordId: number, calendarType?: string): Observable<Complaint[]> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<Complaint[]>(`${this.baseUrl}/${recordId}/complaints`, { params });
    }

    addComplaint(data: ComplaintData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(
            `${this.baseUrl}/${data.activityMonitoringRecordId}/complaints`, data
        );
    }

    updateComplaint(data: ComplaintData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(
            `${this.baseUrl}/${data.activityMonitoringRecordId}/complaints/${data.id}`, data
        );
    }

    deleteComplaint(recordId: number, complaintId: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${recordId}/complaints/${complaintId}`);
    }

    // ==================== Real Estate Violations CRUD ====================

    getRealEstateViolations(recordId: number, calendarType?: string): Observable<RealEstateViolation[]> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<RealEstateViolation[]>(
            `${this.baseUrl}/${recordId}/realestate-violations`, { params }
        );
    }

    addRealEstateViolation(data: RealEstateViolationData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(
            `${this.baseUrl}/${data.activityMonitoringRecordId}/realestate-violations`, data
        );
    }

    updateRealEstateViolation(data: RealEstateViolationData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(
            `${this.baseUrl}/${data.activityMonitoringRecordId}/realestate-violations/${data.id}`, data
        );
    }

    deleteRealEstateViolation(recordId: number, violationId: number): Observable<void> {
        return this.http.delete<void>(
            `${this.baseUrl}/${recordId}/realestate-violations/${violationId}`
        );
    }

    // ==================== Petition Writer Violations CRUD ====================

    getPetitionWriterViolations(recordId: number, calendarType?: string): Observable<PetitionWriterViolation[]> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<PetitionWriterViolation[]>(
            `${this.baseUrl}/${recordId}/petitionwriter-violations`, { params }
        );
    }

    addPetitionWriterViolation(data: PetitionWriterViolationData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(
            `${this.baseUrl}/${data.activityMonitoringRecordId}/petitionwriter-violations`, data
        );
    }

    updatePetitionWriterViolation(data: PetitionWriterViolationData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(
            `${this.baseUrl}/${data.activityMonitoringRecordId}/petitionwriter-violations/${data.id}`, data
        );
    }

    deletePetitionWriterViolation(recordId: number, violationId: number): Observable<void> {
        return this.http.delete<void>(
            `${this.baseUrl}/${recordId}/petitionwriter-violations/${violationId}`
        );
    }

    // ==================== Utility ====================

    resetMainTableId(): void {
        this.mainTableId = 0;
    }
}
