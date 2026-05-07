import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
    LicenseApplication,
    LicenseApplicationData,
    LicenseApplicationListResponse,
    LicenseApplicationReportUser,
    LicenseApplicationGuarantor,
    LicenseApplicationGuarantorData,
    LicenseApplicationWithdrawal,
    LicenseApplicationWithdrawalData
} from '../models/LicenseApplication';

@Injectable({
    providedIn: 'root'
})
export class LicenseApplicationService {
    private baseUrl = environment.apiURL + '/LicenseApplication';
    
    // Subject to notify list component of data changes
    private dataChangedSubject = new Subject<void>();
    dataChanged$ = this.dataChangedSubject.asObservable();
    
    // Store main table ID for child components
    mainTableId: number = 0;

    constructor(private http: HttpClient) { }

    notifyDataChanged(): void {
        this.dataChangedSubject.next();
    }

    // ==================== Main Application CRUD ====================

    getAll(
        page: number = 1,
        pageSize: number = 10,
        search?: string,
        calendarType?: string
    ): Observable<LicenseApplicationListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (search) {
            params = params.set('search', search);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }

        return this.http.get<LicenseApplicationListResponse>(this.baseUrl, { params });
    }

    search(
        serialNumber?: string,
        requestDate?: string,
        applicantName?: string,
        applicantFatherName?: string,
        proposedGuideName?: string,
        electronicNumber?: string,
        shariaDeedNumber?: string,
        customaryDeedSerial?: string,
        guarantorName?: string,
        guarantorFatherName?: string,
        page: number = 1,
        pageSize: number = 10,
        calendarType?: string
    ): Observable<LicenseApplicationListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (serialNumber) {
            params = params.set('serialNumber', serialNumber);
        }
        if (requestDate) {
            params = params.set('requestDate', requestDate);
        }
        if (applicantName) {
            params = params.set('applicantName', applicantName);
        }
        if (applicantFatherName) {
            params = params.set('applicantFatherName', applicantFatherName);
        }
        if (proposedGuideName) {
            params = params.set('proposedGuideName', proposedGuideName);
        }
        if (electronicNumber) {
            params = params.set('electronicNumber', electronicNumber);
        }
        if (shariaDeedNumber) {
            params = params.set('shariaDeedNumber', shariaDeedNumber);
        }
        if (customaryDeedSerial) {
            params = params.set('customaryDeedSerial', customaryDeedSerial);
        }
        if (guarantorName) {
            params = params.set('guarantorName', guarantorName);
        }
        if (guarantorFatherName) {
            params = params.set('guarantorFatherName', guarantorFatherName);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }

        return this.http.get<LicenseApplicationListResponse>(`${this.baseUrl}/search`, { params });
    }

    getById(id: number, calendarType?: string): Observable<LicenseApplication> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<LicenseApplication>(`${this.baseUrl}/${id}`, { params });
    }

    create(data: LicenseApplicationData): Observable<{ id: number; message: string }> {
        return this.http.post<{ id: number; message: string }>(this.baseUrl, data).pipe(
            tap((result) => {
                this.mainTableId = result.id;
                this.notifyDataChanged();
            })
        );
    }

    update(id: number, data: LicenseApplicationData): Observable<{ id: number; message: string }> {
        return this.http.put<{ id: number; message: string }>(`${this.baseUrl}/${id}`, data).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    // ==================== Guarantors CRUD ====================

    getGuarantors(applicationId: number, calendarType?: string): Observable<LicenseApplicationGuarantor[]> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<LicenseApplicationGuarantor[]>(
            `${this.baseUrl}/${applicationId}/guarantors`, { params }
        );
    }

    addGuarantor(data: LicenseApplicationGuarantorData): Observable<{ id: number }> {
        return this.http.post<{ id: number }>(
            `${this.baseUrl}/${data.licenseApplicationId}/guarantors`, data
        );
    }

    updateGuarantor(data: LicenseApplicationGuarantorData): Observable<{ id: number }> {
        return this.http.put<{ id: number }>(
            `${this.baseUrl}/${data.licenseApplicationId}/guarantors/${data.id}`, data
        );
    }

    deleteGuarantor(applicationId: number, guarantorId: number): Observable<void> {
        return this.http.delete<void>(
            `${this.baseUrl}/${applicationId}/guarantors/${guarantorId}`
        );
    }

    // ==================== Withdrawal CRUD ====================

    getWithdrawal(applicationId: number, calendarType?: string): Observable<LicenseApplicationWithdrawal | null> {
        let params = new HttpParams();
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get<LicenseApplicationWithdrawal | null>(
            `${this.baseUrl}/${applicationId}/withdrawal`, { params }
        );
    }

    saveWithdrawal(data: LicenseApplicationWithdrawalData): Observable<{ id: number }> {
        return this.http.post<{ id: number }>(
            `${this.baseUrl}/${data.licenseApplicationId}/withdrawal`, data
        ).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    deleteWithdrawal(applicationId: number): Observable<void> {
        return this.http.delete<void>(
            `${this.baseUrl}/${applicationId}/withdrawal`
        ).pipe(
            tap(() => this.notifyDataChanged())
        );
    }

    // ==================== Duplicate Check ====================

    checkProposedGuideName(proposedGuideName: string, excludeId?: number): Observable<{ isDuplicate: boolean }> {
        let params = new HttpParams().set('proposedGuideName', proposedGuideName);
        if (excludeId) {
            params = params.set('excludeId', excludeId.toString());
        }
        return this.http.get<{ isDuplicate: boolean }>(`${this.baseUrl}/check-proposed-guide-name`, { params });
    }

    checkGuarantor(guarantorName: string, guarantorFatherName?: string, excludeGuarantorId?: number): Observable<{ isDuplicate: boolean }> {
        let params = new HttpParams().set('guarantorName', guarantorName);
        if (guarantorFatherName) {
            params = params.set('guarantorFatherName', guarantorFatherName);
        }
        if (excludeGuarantorId) {
            params = params.set('excludeGuarantorId', excludeGuarantorId.toString());
        }
        return this.http.get<{ isDuplicate: boolean }>(`${this.baseUrl}/check-guarantor`, { params });
    }

    checkShariaDeedNumber(shariaDeedNumber: string, excludeGuarantorId?: number): Observable<{ isDuplicate: boolean }> {
        let params = new HttpParams().set('shariaDeedNumber', shariaDeedNumber);
        if (excludeGuarantorId) {
            params = params.set('excludeGuarantorId', excludeGuarantorId.toString());
        }
        return this.http.get<{ isDuplicate: boolean }>(`${this.baseUrl}/check-sharia-deed-number`, { params });
    }

    checkCustomaryDeedSerial(customaryDeedSerialNumber: string, excludeGuarantorId?: number): Observable<{ isDuplicate: boolean }> {
        let params = new HttpParams().set('customaryDeedSerialNumber', customaryDeedSerialNumber);
        if (excludeGuarantorId) {
            params = params.set('excludeGuarantorId', excludeGuarantorId.toString());
        }
        return this.http.get<{ isDuplicate: boolean }>(`${this.baseUrl}/check-customary-deed-serial`, { params });
    }

    // ==================== Utility ====================

    resetMainTableId(): void {
        this.mainTableId = 0;
    }

    // ==================== Reports ====================

    getApplicantsCountReport(startDate?: string, endDate?: string, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (startDate) {
            params = params.set('startDate', startDate);
        }
        if (endDate) {
            params = params.set('endDate', endDate);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get(`${this.baseUrl}/reports/applicants-count`, { params });
    }

    getReportUsers(): Observable<LicenseApplicationReportUser[]> {
        return this.http.get<LicenseApplicationReportUser[]>(`${this.baseUrl}/reports/users`);
    }

    getGuarantorsByTypeReport(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (startDate) {
            params = params.set('startDate', startDate);
        }
        if (endDate) {
            params = params.set('endDate', endDate);
        }
        if (createdBy) {
            params = params.set('createdBy', createdBy);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get(`${this.baseUrl}/reports/guarantors-by-type`, { params });
    }

    getWithdrawalsCountReport(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (startDate) {
            params = params.set('startDate', startDate);
        }
        if (endDate) {
            params = params.set('endDate', endDate);
        }
        if (createdBy) {
            params = params.set('createdBy', createdBy);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get(`${this.baseUrl}/reports/withdrawals-count`, { params });
    }

    getComprehensiveReport(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (startDate) {
            params = params.set('startDate', startDate);
        }
        if (endDate) {
            params = params.set('endDate', endDate);
        }
        if (createdBy) {
            params = params.set('createdBy', createdBy);
        }
        if (calendarType) {
            params = params.set('calendarType', calendarType);
        }
        return this.http.get(`${this.baseUrl}/reports/comprehensive`, { params });
    }
}
