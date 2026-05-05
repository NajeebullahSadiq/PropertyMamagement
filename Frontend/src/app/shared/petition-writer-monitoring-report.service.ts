import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

// ============ Report Response Interfaces ============

export interface ComplaintsSummary {
    totalRecords: number;
}

export interface ViolationsSummary {
    totalRecords: number;
    activityPreventionCount: number;
    activityPermissionCount: number;
}

export interface MonitoringSummary {
    totalRecords: number;
    totalMonitoringCount: number;
}

export interface PetitionWriterMonitoringReportSummary {
    totalRecords: number;
    complaints: ComplaintsSummary;
    violations: ViolationsSummary;
    monitoring: MonitoringSummary;
}

export interface ComplaintDetail {
    id: number;
    serialNumber: string;
    petitionWriterLicenseNumber: string;
    petitionWriterDistrict: string;
    petitionWriterName: string;
    complainantName: string;
    complaintSubject: string;
    complaintActionsTaken: string;
    registrationDate: string;
    createdBy: string;
}

export interface ViolationDetail {
    id: number;
    serialNumber: string;
    petitionWriterName: string;
    petitionWriterLicenseNumber: string;
    petitionWriterDistrict: string;
    violationType: string;
    activityStatus: string;
    activityPermissionReason: string;
    violationActionsTaken: string;
    registrationDate: string;
    createdBy: string;
}

export interface MonitoringDetail {
    id: number;
    monitoringYear: string;
    monitoringMonth: string;
    monitoringCount: number;
    monitoringRemarks: string;
    registrationDate: string;
    createdBy: string;
}

export interface ReportUser {
    id: string;
    name: string;
}

export interface ExportRow {
    id: number;
    serialNumber: string;
    sectionType: string;
    registrationDate: string;
    petitionWriterName: string;
    petitionWriterLicenseNumber: string;
    petitionWriterDistrict: string;
    complainantName: string;
    complaintSubject: string;
    complaintActionsTaken: string;
    violationType: string;
    activityStatus: string;
    activityPermissionReason: string;
    violationActionsTaken: string;
    monitoringYear: string;
    monitoringMonth: string;
    monitoringCount: number;
    monitoringRemarks: string;
    createdBy: string;
}

@Injectable({
    providedIn: 'root'
})
export class PetitionWriterMonitoringReportService {
    private baseUrl = environment.apiURL + '/PetitionWriterMonitoringReport';

    constructor(private http: HttpClient) { }

    getUsers(): Observable<ReportUser[]> {
        return this.http.get<ReportUser[]>(`${this.baseUrl}/users`);
    }

    getSummary(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<PetitionWriterMonitoringReportSummary> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (createdBy) params = params.set('createdBy', createdBy);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<PetitionWriterMonitoringReportSummary>(`${this.baseUrl}/summary`, { params });
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

    getMonitoring(startDate?: string, endDate?: string, createdBy?: string, calendarType?: string): Observable<{ summary: MonitoringSummary; details: MonitoringDetail[] }> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (createdBy) params = params.set('createdBy', createdBy);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/monitoring`, { params });
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
