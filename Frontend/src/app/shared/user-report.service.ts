import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface UserReportSummary {
    totalUsers: number;
    activeUsers: number;
    inactiveUsers: number;
    inactiveByLock: number;
    inactiveByExpiredLicense: number;
    inactiveByCancellation: number;
    systemUsers: number;
    companyUsers: number;
    byRole: UserRoleBreakdown[];
    byLicenseType: LicenseTypeBreakdown[];
}

export interface UserRoleBreakdown {
    role: string;
    roleDari: string;
    count: number;
    active: number;
    inactive: number;
}

export interface LicenseTypeBreakdown {
    licenseType: string;
    licenseTypeDari: string;
    count: number;
    active: number;
    inactive: number;
}

export interface ProvinceUserReport {
    provinceId: number;
    provinceName: string;
    provinceDari: string;
    totalUsers: number;
    activeUsers: number;
    inactiveUsers: number;
}

export interface DistrictUserReport {
    provinceId: number;
    totalUsers: number;
    activeUsers: number;
    inactiveUsers: number;
    districts: { districtId: number; districtName: string; districtDari: string }[];
}

export interface InactiveUserDetail {
    id: string;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    companyId: number;
    companyName: string;
    licenseType: string;
    licenseTypeDari: string;
    isLocked: boolean;
    createdAt: string;
    province: { id: number; name: string; dari: string } | null;
    role: string;
    roleDari: string;
    reasons: string[];
    details: any[];
}

export interface RegistrationTrendItem {
    month: string;
    monthLabel: string;
    totalRegistrations: number;
    systemUsers: number;
    companyUsers: number;
}

export interface ExpiringLicenseItem {
    id: number;
    licenseNumber: string;
    licenseType: string;
    licenseTypeDari: string;
    expireDate: string;
    daysRemaining: number;
    company: { id: number; title: string } | null;
    province: { id: number; name: string; dari: string } | null;
    users: { id: string; userName: string; firstName: string; lastName: string }[];
}

@Injectable({
    providedIn: 'root'
})
export class UserReportService {
    private baseUrl = environment.apiUrl + '/UserReport';

    constructor(private http: HttpClient) { }

    getSummary(startDate?: string, endDate?: string, calendarType?: string, provinceId?: number, districtId?: number): Observable<UserReportSummary> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        if (provinceId) params = params.set('provinceId', provinceId.toString());
        if (districtId) params = params.set('districtId', districtId.toString());
        return this.http.get<UserReportSummary>(`${this.baseUrl}/Summary`, { params });
    }

    getByProvince(startDate?: string, endDate?: string, calendarType?: string): Observable<ProvinceUserReport[]> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<ProvinceUserReport[]>(`${this.baseUrl}/ByProvince`, { params });
    }

    getByDistrict(provinceId: number, startDate?: string, endDate?: string, calendarType?: string): Observable<DistrictUserReport> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<DistrictUserReport>(`${this.baseUrl}/ByDistrict/${provinceId}`, { params });
    }

    getInactiveUsers(reason?: string, startDate?: string, endDate?: string, calendarType?: string, provinceId?: number, page?: number, pageSize?: number): Observable<{ total: number; page: number; pageSize: number; users: InactiveUserDetail[] }> {
        let params = new HttpParams();
        if (reason) params = params.set('reason', reason);
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        if (provinceId) params = params.set('provinceId', provinceId.toString());
        if (page) params = params.set('page', page.toString());
        if (pageSize) params = params.set('pageSize', pageSize.toString());
        return this.http.get<any>(`${this.baseUrl}/InactiveUsers`, { params });
    }

    getRegistrationTrend(startDate?: string, endDate?: string, calendarType?: string): Observable<RegistrationTrendItem[]> {
        let params = new HttpParams();
        if (startDate) params = params.set('startDate', startDate);
        if (endDate) params = params.set('endDate', endDate);
        if (calendarType) params = params.set('calendarType', calendarType);
        return this.http.get<RegistrationTrendItem[]>(`${this.baseUrl}/RegistrationTrend`, { params });
    }

    getExpiringLicenses(daysAhead?: number, provinceId?: number, page?: number, pageSize?: number): Observable<{ total: number; page: number; pageSize: number; daysAhead: number; data: ExpiringLicenseItem[] }> {
        let params = new HttpParams();
        if (daysAhead) params = params.set('daysAhead', daysAhead.toString());
        if (provinceId) params = params.set('provinceId', provinceId.toString());
        if (page) params = params.set('page', page.toString());
        if (pageSize) params = params.set('pageSize', pageSize.toString());
        return this.http.get<any>(`${this.baseUrl}/ExpiringLicenses`, { params });
    }

    autoDeactivate(): Observable<any> {
        return this.http.post(`${this.baseUrl}/AutoDeactivate`, {});
    }
}
