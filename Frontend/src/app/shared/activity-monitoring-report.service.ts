import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

// ============ Report Response Interfaces ============

export interface AnnualReportSummary {
    totalRecords: number;
    saleDeedsCount: number;
    rentalDeedsCount: number;
    baiUlWafaDeedsCount: number;
    vehicleTransactionDeedsCount: number;
    totalDeedsCount: number;
    totalTaxAmount: number;
}

export interface ComplaintsSummary {
    totalRecords: number;
}

export interface ViolationsSummary {
    totalRecords: number;
    blockedCount: number;
    normalCount: number;
    sealRemovedCount: number;
}

export interface InspectionSummary {
    totalRecords: number;
    totalMonitoringCount: number;
}

export interface ActivityMonitoringReportSummary {
    totalRecords: number;
    annualReport: AnnualReportSummary;
    complaints: ComplaintsSummary;
    violations: ViolationsSummary;
    inspection: InspectionSummary;
}

export interface AnnualReportDetail {
    id: number;
    serialNumber: string;
    licenseNumber: string;
    licenseHolderName: string;
    companyTitle: string;
    district: string;
    reportRegistrationDate: string;
    saleDeedsCount: number;
    rentalDeedsCount: number;
    baiUlWafaDeedsCount: number;
    vehicleTransactionDeedsCount: number;
    totalDeedsCount: number;
    taxAmount: number;
    createdBy: string;
}

export interface ComplaintDetail {
    id: number;
    serialNumber: string;
    licenseNumber: string;
    licenseHolderName: string;
    companyTitle: string;
    district: string;
    complainantName: string;
    complaintSubject: string;
    complaintActionsTaken: string;
    reportRegistrationDate: string;
    createdBy: string;
}

export interface ViolationDetail {
    id: number;
    serialNumber: string;
    licenseNumber: string;
    licenseHolderName: string;
    companyTitle: string;
    district: string;
    violationStatus: string;
    violationType: string;
    closureReason: string;
    sealRemovalReason: string;
    violationActionsTaken: string;
    reportRegistrationDate: string;
    createdBy: string;
}

export interface InspectionDetail {
    id: number;
    year: string;
    month: string;
    monitoringCount: number;
    monitoringRemarks: string;
    reportRegistrationDate: string;
    createdBy: string;
}

export interface ReportUser {
    id: string;
    name: string;
}

export interface ExportRow {
    id: number;
    serialNumber: string;
    licenseNumber: string;
    licenseHolderName: string;
    companyTitle: string;
    district: string;
    sectionType: string;
    reportRegistrationDate: string;
    saleDeedsCount: number;
    rentalDeedsCount: number;
    baiUlWafaDeedsCount: number;
    vehicleTransactionDeedsCount: number;
    taxAmount: number;
    complainantName: string;
    complaintSubject: string;
    complaintActionsTaken: string;
    violationStatus: string;
    violationType: string;
    closureReason: string;
    sealRemovalReason: string;
    violationActionsTaken: string;
    year: string;
    month: string;
    monitoringCount: number;
    monitoringRemarks: string;
    createdBy: string;
}

@Injectable({
    providedIn: 'root'
})
export class ActivityMonitoringReportService {
    private baseUrl = environment.apiURL + '/ActivityMonitoringReport';

    constructor(private http: HttpClient) { }

    getUsers(): Observable<ReportUser[]> {
        return this.http.get<ReportUser[]>(`${this.baseUrl}/users`);
    }

    getSummary(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<ActivityMonitoringReportSummary> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (createdBy) params = params.set('createdBy', createdBy);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<ActivityMonitoringReportSummary>(`${this.baseUrl}/summary`, { params });
    }

    getAnnualReport(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<{ summary: AnnualReportSummary; details: AnnualReportDetail[] }> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (createdBy) params = params.set('createdBy', createdBy);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/annual-report`, { params });
    }

    getComplaints(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<{ summary: ComplaintsSummary; details: ComplaintDetail[] }> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (createdBy) params = params.set('createdBy', createdBy);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/complaints`, { params });
    }

    getViolations(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<{ summary: ViolationsSummary; details: ViolationDetail[] }> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (createdBy) params = params.set('createdBy', createdBy);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/violations`, { params });
    }

    getInspection(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<{ summary: InspectionSummary; details: InspectionDetail[] }> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (createdBy) params = params.set('createdBy', createdBy);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/inspection`, { params });
    }

    getExport(startDate?: string, endDate?: string, createdBy?: string, sectionType?: string, calendarType?: string): Observable<ExportRow[]> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (createdBy) params = params.set('createdBy', createdBy);
        if (sectionType) params = params.set('sectionType', sectionType);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<ExportRow[]>(`${this.baseUrl}/export`, { params });
    }
}
