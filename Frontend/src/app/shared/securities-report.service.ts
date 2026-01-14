import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface SecuritiesReportRequest {
    metrics?: string[];
    groupBy?: string[];
    startDate?: string;
    endDate?: string;
    year?: number;
    month?: number;
    transactionGuideName?: string;
    licenseNumber?: string;
    registrationNumber?: string;
    documentType?: number;
    propertySubType?: number;
    vehicleSubType?: number;
    registrationBookType?: number;
    minAmount?: number;
    maxAmount?: number;
    minCount?: number;
    maxCount?: number;
    serialNumberStart?: string;
    serialNumberEnd?: string;
    calendarType?: string;
}

export interface SecuritiesReportSummary {
    totalDocuments: number;
    totalPropertyDocuments: number;
    totalVehicleDocuments: number;
    propertySaleCount: number;
    bayWafaCount: number;
    rentCount: number;
    vehicleSaleCount: number;
    vehicleExchangeCount: number;
    registrationBookCount: number;
    duplicateBookCount: number;
    totalBookCount: number;
    totalDocumentsPrice: number;
    totalRegistrationBookPrice: number;
    totalSecuritiesPrice: number;
    totalRecords: number;
}

export interface SecuritiesReportRow {
    groupKey?: string;
    transactionGuideName?: string;
    licenseNumber?: string;
    registrationNumber?: string;
    dateGroup?: string;
    documentType?: number;
    documentTypeName?: string;
    registrationBookType?: number;
    registrationBookTypeName?: string;
    recordCount: number;
    propertySaleCount: number;
    bayWafaCount: number;
    rentCount: number;
    vehicleSaleCount: number;
    vehicleExchangeCount: number;
    registrationBookCount: number;
    duplicateBookCount: number;
    totalDocuments: number;
    totalBooks: number;
    totalDocumentsPrice: number;
    registrationBookPrice: number;
    totalSecuritiesPrice: number;
    propertySaleSerialRange?: string;
    bayWafaSerialRange?: string;
    rentSerialRange?: string;
    vehicleSaleSerialRange?: string;
    vehicleExchangeSerialRange?: string;
    distributionDate?: string;
    deliveryDate?: string;
}

export interface SecuritiesReportMetadata {
    generatedAt: Date;
    reportTitle: string;
    appliedFilters: string[];
    groupedBy: string[];
    includedMetrics: string[];
}

export interface SecuritiesReportResponse {
    summary: SecuritiesReportSummary;
    data: SecuritiesReportRow[];
    metadata: SecuritiesReportMetadata;
}

export interface ReportConfig {
    metrics: { id: string; name: string; category: string }[];
    groupByOptions: { id: string; name: string }[];
    documentTypes: { id: number; name: string }[];
    propertySubTypes: { id: number; name: string }[];
    vehicleSubTypes: { id: number; name: string }[];
    bookTypes: { id: number; name: string }[];
}

@Injectable({
    providedIn: 'root'
})
export class SecuritiesReportService {
    private baseUrl = environment.apiURL + '/SecuritiesReport';

    constructor(private http: HttpClient) { }

    getReportConfig(): Observable<ReportConfig> {
        return this.http.get<ReportConfig>(`${this.baseUrl}/config`);
    }

    generateReport(request: SecuritiesReportRequest): Observable<SecuritiesReportResponse> {
        return this.http.post<SecuritiesReportResponse>(`${this.baseUrl}/generate`, request);
    }

    getSummary(startDate?: string, endDate?: string, calendarType?: string): Observable<SecuritiesReportSummary> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<SecuritiesReportSummary>(`${this.baseUrl}/summary`, { params });
    }

    getByGuide(startDate?: string, endDate?: string, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/by-guide`, { params });
    }

    getByDocumentType(startDate?: string, endDate?: string, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/by-document-type`, { params });
    }

    getByBookType(startDate?: string, endDate?: string, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/by-book-type`, { params });
    }

    getMonthlyTrend(year?: number, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (year) params = params.set('year', year.toString());
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/monthly-trend`, { params });
    }

    getSerialTracking(licenseNumber?: string, transactionGuideName?: string, documentType?: number, calendarType?: string): Observable<any> {
        let params = new HttpParams();
        if (licenseNumber) params = params.set('licenseNumber', licenseNumber);
        if (transactionGuideName) params = params.set('transactionGuideName', transactionGuideName);
        if (documentType) params = params.set('documentType', documentType.toString());
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<any>(`${this.baseUrl}/serial-tracking`, { params });
    }

    exportReport(request: SecuritiesReportRequest): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/export`, request);
    }
}
