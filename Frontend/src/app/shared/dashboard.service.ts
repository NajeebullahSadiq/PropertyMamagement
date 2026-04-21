import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { shareReplay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CalendarType } from '../models/calendar-type';
import {
  EstateDashboardData,
  VehicleDashboardData,
  CompanyDashboardData,
  ExpiredLicenseDashboardData,
  PropertyTypeByMonthData,
  VehicleReportData
} from '../models/dashboard.models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = environment.apiUrl + '/Dashboard';

  // Cached observables - shareReplay(1) prevents duplicate API calls
  private dashboardData$: Observable<EstateDashboardData> | null = null;
  private vehicleDashboardData$: Observable<VehicleDashboardData> | null = null;
  private companyDashboardData$: Observable<CompanyDashboardData> | null = null;
  private expiredLicenseData$: Observable<ExpiredLicenseDashboardData> | null = null;
  private propertyTypesByMonth$: Observable<PropertyTypeByMonthData[]> | null = null;
  private transactionTypesByMonth$: Observable<PropertyTypeByMonthData[]> | null = null;
  private vehicleReport$: Observable<VehicleReportData[]> | null = null;

  constructor(private http: HttpClient) { }

  getDashboardData(): Observable<EstateDashboardData> {
    if (!this.dashboardData$) {
      this.dashboardData$ = this.http.get<EstateDashboardData>(this.baseUrl + '/GetEstateDashboardData')
        .pipe(shareReplay(1));
    }
    return this.dashboardData$;
  }

  getVehicleDashboardData(): Observable<VehicleDashboardData> {
    if (!this.vehicleDashboardData$) {
      this.vehicleDashboardData$ = this.http.get<VehicleDashboardData>(this.baseUrl + '/GetVehicleDashboardData')
        .pipe(shareReplay(1));
    }
    return this.vehicleDashboardData$;
  }

  GetPropertyTypesByMonth(): Observable<PropertyTypeByMonthData[]> {
    if (!this.propertyTypesByMonth$) {
      this.propertyTypesByMonth$ = this.http.get<PropertyTypeByMonthData[]>(this.baseUrl + '/GetPropertyTypesByMonth')
        .pipe(shareReplay(1));
    }
    return this.propertyTypesByMonth$;
  }

  GetTransactionTypesByMonth(): Observable<PropertyTypeByMonthData[]> {
    if (!this.transactionTypesByMonth$) {
      this.transactionTypesByMonth$ = this.http.get<PropertyTypeByMonthData[]>(this.baseUrl + '/GetTransactionTypesByMonth')
        .pipe(shareReplay(1));
    }
    return this.transactionTypesByMonth$;
  }

  GetCompanyDashboardData(): Observable<CompanyDashboardData> {
    if (!this.companyDashboardData$) {
      this.companyDashboardData$ = this.http.get<CompanyDashboardData>(this.baseUrl + '/GetCompanyDashboardData')
        .pipe(shareReplay(1));
    }
    return this.companyDashboardData$;
  }

  GetExpiredLicenseDashboardData(): Observable<ExpiredLicenseDashboardData> {
    if (!this.expiredLicenseData$) {
      this.expiredLicenseData$ = this.http.get<ExpiredLicenseDashboardData>(this.baseUrl + '/GetExpireLicenseDashboardData')
        .pipe(shareReplay(1));
    }
    return this.expiredLicenseData$;
  }

  getVehicleReportByMonth(): Observable<VehicleReportData[]> {
    if (!this.vehicleReport$) {
      this.vehicleReport$ = this.http.get<VehicleReportData[]>(this.baseUrl + '/GetVehicleReportByMonth')
        .pipe(shareReplay(1));
    }
    return this.vehicleReport$;
  }

  getDashboardDataByDate(startDate: any, endDate: any, calendarType?: CalendarType): Observable<any> {
    let url = `${this.baseUrl + '/GetDashboardDataByDate'}?startDate=${startDate}&endDate=${endDate}`;
    if (calendarType) {
      url += `&calendarType=${calendarType}`;
    }
    return this.http.get(url);
  }

  getTotalUserDataByDate(startDate: any, endDate: any, calendarType?: CalendarType): Observable<any> {
    let url = `${this.baseUrl + '/GetTopUsersSummaryByDate'}?startDate=${startDate}&endDate=${endDate}`;
    if (calendarType) {
      url += `&calendarType=${calendarType}`;
    }
    return this.http.get(url);
  }

  getVehicleTotalUserDataByDate(startDate: any, endDate: any, calendarType?: CalendarType): Observable<any> {
    let url = `${this.baseUrl + '/GetVehicleTopUsersSummaryByDate'}?startDate=${startDate}&endDate=${endDate}`;
    if (calendarType) {
      url += `&calendarType=${calendarType}`;
    }
    return this.http.get(url);
  }

  // Clear cached data (call on logout or when data needs refresh)
  clearCache(): void {
    this.dashboardData$ = null;
    this.vehicleDashboardData$ = null;
    this.companyDashboardData$ = null;
    this.expiredLicenseData$ = null;
    this.propertyTypesByMonth$ = null;
    this.transactionTypesByMonth$ = null;
    this.vehicleReport$ = null;
  }
}
