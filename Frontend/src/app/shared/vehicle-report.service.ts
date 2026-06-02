import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface TransactionTypeSummary {
    transactionTypeId: number;
    transactionTypeName: string;
    transactionTypeDari: string;
    count: number;
    completeCount: number;
    totalPrice: number;
    totalRoyaltyAmount: number;
}

export interface VehicleTypeSummary {
    vehicleType: string;
    count: number;
    totalPrice: number;
    totalRoyaltyAmount: number;
}

export interface VehicleReportSummary {
    totalRecords: number;
    completeRecords: number;
    grandTotalPrice: number;
    grandTotalRoyaltyAmount: number;
    byTransactionType: TransactionTypeSummary[];
    byVehicleType: VehicleTypeSummary[];
}

export interface CompanyReportItem {
    companyId: number;
    companyTitle: string;
    totalRecords: number;
    totalPrice: number;
    totalRoyaltyAmount: number;
    byTransactionType: TransactionTypeSummary[];
}

export interface ProvinceReportItem {
    provinceId: number;
    provinceName: string;
    provinceDari: string;
    totalRecords: number;
    totalPrice: number;
    totalRoyaltyAmount: number;
}

export interface MonthlyTrendItem {
    month: string;
    monthLabel: string;
    totalRecords: number;
    totalPrice: number;
    totalRoyaltyAmount: number;
    byTransactionType: { transactionType: string; transactionTypeDari: string; count: number }[];
}

export interface TransactionTypeDetailRecord {
    id: number;
    permitNo: string | null;
    pilateNo: string | null;
    typeOfVehicle: string | null;
    model: string | null;
    transactionType: string | null;
    transactionTypeDari: string;
    price: string | null;
    royaltyAmount: string | null;
    companyTitle: string | null;
    companyId: number | null;
    createdAt: string | null;
    sellerName: string | null;
    buyerName: string | null;
    createdBy: string | null;
    iscomplete: boolean | null;
}

export interface TransactionTypeDetailReport {
    transactionTypeId: number;
    transactionTypeName: string;
    transactionTypeDari: string;
    total: number;
    page: number;
    pageSize: number;
    totalPrice: number;
    totalRoyaltyAmount: number;
    records: TransactionTypeDetailRecord[];
}

@Injectable({
    providedIn: 'root'
})
export class VehicleReportService {
    private baseUrl = environment.apiUrl + '/VehicleReport';

    constructor(private http: HttpClient) { }

    getSummary(startDate?: string, endDate?: string, calendarType?: string, provinceId?: number, districtId?: number, companyId?: number): Observable<VehicleReportSummary> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        if (provinceId) params = params.set('provinceId', provinceId.toString());
        if (districtId) params = params.set('districtId', districtId.toString());
        if (companyId) params = params.set('companyId', companyId.toString());
        return this.http.get<VehicleReportSummary>(`${this.baseUrl}/Summary`, { params });
    }

    getByCompany(startDate?: string, endDate?: string, calendarType?: string, provinceId?: number, districtId?: number): Observable<CompanyReportItem[]> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        if (provinceId) params = params.set('provinceId', provinceId.toString());
        if (districtId) params = params.set('districtId', districtId.toString());
        return this.http.get<CompanyReportItem[]>(`${this.baseUrl}/ByCompany`, { params });
    }

    getByProvince(startDate?: string, endDate?: string, calendarType?: string): Observable<ProvinceReportItem[]> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<ProvinceReportItem[]>(`${this.baseUrl}/ByProvince`, { params });
    }

    getMonthlyTrend(startDate?: string, endDate?: string, calendarType?: string, companyId?: number): Observable<MonthlyTrendItem[]> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        if (companyId) params = params.set('companyId', companyId.toString());
        return this.http.get<MonthlyTrendItem[]>(`${this.baseUrl}/MonthlyTrend`, { params });
    }

    getByTransactionType(transactionTypeId: number, startDate?: string, endDate?: string, calendarType?: string, provinceId?: number, districtId?: number, companyId?: number, page?: number, pageSize?: number): Observable<TransactionTypeDetailReport> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        if (provinceId) params = params.set('provinceId', provinceId.toString());
        if (districtId) params = params.set('districtId', districtId.toString());
        if (companyId) params = params.set('companyId', companyId.toString());
        if (page) params = params.set('page', page.toString());
        if (pageSize) params = params.set('pageSize', pageSize.toString());
        return this.http.get<TransactionTypeDetailReport>(`${this.baseUrl}/ByTransactionType/${transactionTypeId}`, { params });
    }

    getCompanies(): Observable<any> {
        return this.http.get(environment.apiUrl + '/CompanyDetails/getCompanies');
    }

    getProvinces(): Observable<any> {
        return this.http.get(environment.apiUrl + '/DistrictManagement/provinces');
    }

    getDistricts(provinceId: number): Observable<any> {
        return this.http.get(environment.apiUrl + '/DistrictManagement/province/' + provinceId);
    }
}
