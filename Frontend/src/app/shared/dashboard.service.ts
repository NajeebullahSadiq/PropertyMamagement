import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CalendarType } from '../models/calendar-type';
import {
  EstateDashboardData,
  VehicleDashboardData,
  CompanyDashboardData,
  ExpiredLicenseDashboardData,
  PropertyTypeByMonthData,
  VehicleReportData,
  ModuleCountResponse
} from '../models/dashboard.models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = environment.apiUrl + '/Dashboard';
  private apiBaseUrl = environment.apiURL;

  // Cached observables - shareReplay(1) prevents duplicate API calls
  private dashboardData$: Observable<EstateDashboardData> | null = null;
  private vehicleDashboardData$: Observable<VehicleDashboardData> | null = null;
  private companyDashboardData$: Observable<CompanyDashboardData> | null = null;
  private expiredLicenseData$: Observable<ExpiredLicenseDashboardData> | null = null;
  private propertyTypesByMonth$: Observable<PropertyTypeByMonthData[]> | null = null;
  private transactionTypesByMonth$: Observable<PropertyTypeByMonthData[]> | null = null;
  private vehicleReport$: Observable<VehicleReportData[]> | null = null;

  // Module count caches
  private securitiesCount$: Observable<number> | null = null;
  private petitionWriterSecuritiesCount$: Observable<number> | null = null;
  private petitionWriterLicenseCount$: Observable<number> | null = null;
  private activityMonitoringCount$: Observable<number> | null = null;
  private petitionWriterMonitoringCount$: Observable<number> | null = null;
  private licenseApplicationCount$: Observable<number> | null = null;
  private userCount$: Observable<number> | null = null;

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

  // ==================== Module Count Methods ====================
  // Efficiently fetch totalCount from paginated list APIs using pageSize=1

  getSecuritiesCount(): Observable<number> {
    if (!this.securitiesCount$) {
      const params = new HttpParams().set('page', '1').set('pageSize', '1');
      this.securitiesCount$ = this.http.get<ModuleCountResponse>(this.apiBaseUrl + '/SecuritiesDistribution', { params })
        .pipe(map(res => res.totalCount || 0), shareReplay(1));
    }
    return this.securitiesCount$;
  }

  getPetitionWriterSecuritiesCount(): Observable<number> {
    if (!this.petitionWriterSecuritiesCount$) {
      const params = new HttpParams().set('page', '1').set('pageSize', '1');
      this.petitionWriterSecuritiesCount$ = this.http.get<ModuleCountResponse>(this.apiBaseUrl + '/PetitionWriterSecurities', { params })
        .pipe(map(res => res.totalCount || 0), shareReplay(1));
    }
    return this.petitionWriterSecuritiesCount$;
  }

  getPetitionWriterLicenseCount(): Observable<number> {
    if (!this.petitionWriterLicenseCount$) {
      const params = new HttpParams().set('page', '1').set('pageSize', '1');
      this.petitionWriterLicenseCount$ = this.http.get<ModuleCountResponse>(this.apiBaseUrl + '/PetitionWriterLicense', { params })
        .pipe(map(res => res.totalCount || 0), shareReplay(1));
    }
    return this.petitionWriterLicenseCount$;
  }

  getActivityMonitoringCount(): Observable<number> {
    if (!this.activityMonitoringCount$) {
      const params = new HttpParams().set('page', '1').set('pageSize', '1');
      this.activityMonitoringCount$ = this.http.get<ModuleCountResponse>(this.apiBaseUrl + '/ActivityMonitoring', { params })
        .pipe(map(res => res.totalCount || 0), shareReplay(1));
    }
    return this.activityMonitoringCount$;
  }

  getPetitionWriterMonitoringCount(): Observable<number> {
    if (!this.petitionWriterMonitoringCount$) {
      const params = new HttpParams().set('page', '1').set('pageSize', '1');
      this.petitionWriterMonitoringCount$ = this.http.get<ModuleCountResponse>(this.apiBaseUrl + '/PetitionWriterMonitoring', { params })
        .pipe(map(res => res.totalCount || 0), shareReplay(1));
    }
    return this.petitionWriterMonitoringCount$;
  }

  getLicenseApplicationCount(): Observable<number> {
    if (!this.licenseApplicationCount$) {
      const params = new HttpParams().set('page', '1').set('pageSize', '1');
      this.licenseApplicationCount$ = this.http.get<ModuleCountResponse>(this.apiBaseUrl + '/LicenseApplication', { params })
        .pipe(map(res => res.totalCount || 0), shareReplay(1));
    }
    return this.licenseApplicationCount$;
  }

  getUserCount(): Observable<number> {
    if (!this.userCount$) {
      this.userCount$ = this.http.get<ModuleCountResponse>(this.apiBaseUrl + '/ApplicationUser/GetAllUsers')
        .pipe(map((res: any) => {
          if (Array.isArray(res)) return res.length;
          return res?.totalCount || res?.length || 0;
        }), shareReplay(1));
    }
    return this.userCount$;
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
    this.securitiesCount$ = null;
    this.petitionWriterSecuritiesCount$ = null;
    this.petitionWriterLicenseCount$ = null;
    this.activityMonitoringCount$ = null;
    this.petitionWriterMonitoringCount$ = null;
    this.licenseApplicationCount$ = null;
    this.userCount$ = null;
  }
}
