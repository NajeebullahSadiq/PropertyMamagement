import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

/**
 * Report configuration options
 */
export interface PetitionWriterReportConfig {
    metrics: { id: string; name: string; category: string }[];
    groupByOptions: { id: string; name: string }[];
}

/**
 * Request model for dynamic report generation
 */
export interface PetitionWriterReportRequest {
    metrics?: string[];
    groupBy?: string[];
    startDate?: string;
    endDate?: string;
    year?: number;
    month?: number;
    petitionWriterName?: string;
    licenseNumber?: string;
    registrationNumber?: string;
    minAmount?: number;
    maxAmount?: number;
    minCount?: number;
    maxCount?: number;
    serialNumberStart?: string;
    serialNumberEnd?: string;
    calendarType?: string;
}

/**
 * Summary totals for the report
 */
export interface PetitionWriterReportSummary {
    totalPetitionCount: number;
    totalRecords: number;
    uniquePetitionWriters: number;
    uniqueLicenses: number;
    totalAmount: number;
    averageAmountPerDistribution: number;
    averagePetitionsPerDistribution: number;
    minSerialNumber?: string;
    maxSerialNumber?: string;
}

/**
 * Individual row in the report
 */
export interface PetitionWriterReportRow {
    groupKey?: string;
    petitionWriterName?: string;
    petitionWriterFatherName?: string;
    licenseNumber?: string;
    registrationNumber?: string;
    dateGroup?: string;
    yearGroup?: number;
    monthGroup?: number;
    monthName?: string;
    recordCount: number;
    totalPetitionCount: number;
    totalAmount: number;
    averageAmount: number;
    serialNumberStart?: string;
    serialNumberEnd?: string;
    serialRange?: string;
    bankReceiptNumber?: string;
    distributionDate?: string;
    createdAt?: string;
}

/**
 * Report metadata
 */
export interface PetitionWriterReportMetadata {
    generatedAt: Date;
    reportTitle: string;
    appliedFilters: string[];
    groupedBy: string[];
    includedMetrics: string[];
}

/**
 * Response model for the report
 */
export interface PetitionWriterReportResponse {
    summary: PetitionWriterReportSummary;
    data: PetitionWriterReportRow[];
    metadata: PetitionWriterReportMetadata;
}

/**
 * Monthly trend data
 */
export interface MonthlyTrendData {
    year: number;
    month: number;
    monthName: string;
    recordCount: number;
    totalPetitionCount: number;
    totalAmount: number;
    averageAmount: number;
}

/**
 * Yearly trend data
 */
export interface YearlyTrendData {
    year: number;
    recordCount: number;
    totalPetitionCount: number;
    totalAmount: number;
    uniqueWriters: number;
    uniqueLicenses: number;
}

/**
 * Detailed list response
 */
export interface DetailedListResponse {
    data: any[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

@Injectable({
    providedIn: 'root'
})
export class PetitionWriterReportService {
    private baseUrl = environment.apiURL + '/PetitionWriterReport';

    constructor(private http: HttpClient) { }

    /**
     * Get report configuration options
     */
    getReportConfig(): Observable<PetitionWriterReportConfig> {
        return this.http.get<PetitionWriterReportConfig>(`${this.baseUrl}/config`);
    }

    /**
     * Generate dynamic report based on request parameters
     */
    generateReport(request: PetitionWriterReportRequest): Observable<PetitionWriterReportResponse> {
        return this.http.post<PetitionWriterReportResponse>(`${this.baseUrl}/generate`, request);
    }

    /**
     * Get summary statistics
     */
    getSummary(startDate?: string, endDate?: string, calendarType?: string): Observable<PetitionWriterReportSummary> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        
        return this.http.get<PetitionWriterReportSummary>(`${this.baseUrl}/summary`, { params });
    }

    /**
     * Get report grouped by petition writer
     */
    getByWriter(startDate?: string, endDate?: string, calendarType?: string): Observable<{ data: PetitionWriterReportRow[]; summary: PetitionWriterReportSummary }> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        
        return this.http.get<{ data: PetitionWriterReportRow[]; summary: PetitionWriterReportSummary }>(`${this.baseUrl}/by-writer`, { params });
    }

    /**
     * Get report grouped by license number
     */
    getByLicense(startDate?: string, endDate?: string, calendarType?: string): Observable<{ data: PetitionWriterReportRow[]; summary: PetitionWriterReportSummary }> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        
        return this.http.get<{ data: PetitionWriterReportRow[]; summary: PetitionWriterReportSummary }>(`${this.baseUrl}/by-license`, { params });
    }

    /**
     * Get monthly trend report
     */
    getMonthlyTrend(year?: number, calendarType?: string): Observable<MonthlyTrendData[]> {
        let params = new HttpParams();
        if (year) params = params.set('year', year.toString());
        if (calendarType) params = params.set('calendarType', calendarType);
        
        return this.http.get<MonthlyTrendData[]>(`${this.baseUrl}/monthly-trend`, { params });
    }

    /**
     * Get yearly trend report
     */
    getYearlyTrend(calendarType?: string): Observable<YearlyTrendData[]> {
        let params = new HttpParams();
        if (calendarType) params = params.set('calendarType', calendarType);
        
        return this.http.get<YearlyTrendData[]>(`${this.baseUrl}/yearly-trend`, { params });
    }

    /**
     * Get serial number tracking report
     */
    getSerialTracking(licenseNumber?: string, petitionWriterName?: string, calendarType?: string): Observable<any[]> {
        let params = new HttpParams();
        if (licenseNumber) params = params.set('licenseNumber', licenseNumber);
        if (petitionWriterName) params = params.set('petitionWriterName', petitionWriterName);
        if (calendarType) params = params.set('calendarType', calendarType);
        
        return this.http.get<any[]>(`${this.baseUrl}/serial-tracking`, { params });
    }

    /**
     * Get detailed list with pagination
     */
    getDetailedList(
        startDate?: string,
        endDate?: string,
        petitionWriterName?: string,
        licenseNumber?: string,
        registrationNumber?: string,
        page: number = 1,
        pageSize: number = 20,
        calendarType?: string
    ): Observable<DetailedListResponse> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());
        
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (petitionWriterName) params = params.set('petitionWriterName', petitionWriterName);
        if (licenseNumber) params = params.set('licenseNumber', licenseNumber);
        if (registrationNumber) params = params.set('registrationNumber', registrationNumber);
        if (calendarType) params = params.set('calendarType', calendarType);
        
        return this.http.get<DetailedListResponse>(`${this.baseUrl}/detailed-list`, { params });
    }

    /**
     * Export report data for PDF/Excel generation
     */
    exportReport(request: PetitionWriterReportRequest): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/export`, request);
    }
}
