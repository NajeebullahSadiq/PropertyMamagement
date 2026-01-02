import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CalendarType } from '../models/calendar-type';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = environment.apiUrl + '/Dashboard';
  constructor(private http: HttpClient) { }

  getDashboardData() {
    return this.http.get<any>(this.baseUrl+'/GetEstateDashboardData');
  }
  getVehicleDashboardData() {
    return this.http.get<any>(this.baseUrl+'/GetVehicleDashboardData');
  }
  GetPropertyTypesByMonth() {
    return this.http.get<any>(this.baseUrl+'/GetPropertyTypesByMonth');
  }
  GetTransactionTypesByMonth() {
    return this.http.get<any>(this.baseUrl+'/GetTransactionTypesByMonth');
  }
  GetCompanyDashboardData() {
    return this.http.get<any>(this.baseUrl+'/GetCompanyDashboardData');
  }
  GetExpiredLicenseDashboardData() {
    return this.http.get<any>(this.baseUrl+'/GetExpireLicenseDashboardData');
  }
  getDashboardDataByDate(startDate: any, endDate: any, calendarType?: CalendarType): Observable<any> {
    let url = `${this.baseUrl+'/GetDashboardDataByDate'}?startDate=${startDate}&endDate=${endDate}`;
    if (calendarType) {
      url += `&calendarType=${calendarType}`;
    }
    return this.http.get(url);
  }
  getTotalUserDataByDate(startDate: any, endDate: any, calendarType?: CalendarType): Observable<any> {
    let url = `${this.baseUrl+'/GetTopUsersSummaryByDate'}?startDate=${startDate}&endDate=${endDate}`;
    if (calendarType) {
      url += `&calendarType=${calendarType}`;
    }
    return this.http.get(url);
  }
  getVehicleTotalUserDataByDate(startDate: any, endDate: any, calendarType?: CalendarType): Observable<any> {
    let url = `${this.baseUrl+'/GetVehicleTopUsersSummaryByDate'}?startDate=${startDate}&endDate=${endDate}`;
    if (calendarType) {
      url += `&calendarType=${calendarType}`;
    }
    return this.http.get(url);
  }
}
