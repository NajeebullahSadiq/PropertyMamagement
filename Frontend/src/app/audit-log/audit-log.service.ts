import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AuditLog {
  id: number;
  userId: string;
  userName: string;
  userRole: string;
  actionType: string;
  module: string;
  entityType: string;
  entityId: string;
  description: string;
  descriptionDari: string;
  ipAddress: string;
  requestUrl: string;
  httpMethod: string;
  status: string;
  errorMessage: string;
  userProvince: string;
  timestamp: string;
  durationMs: number;
}

export interface AuditLogDetail extends AuditLog {
  oldValues: string;
  newValues: string;
  userAgent: string;
  metadata: string;
}

export interface AuditStatistics {
  dateRange: {
    startDate: string;
    endDate: string;
  };
  summary: {
    totalActions: number;
    successfulActions: number;
    failedActions: number;
    successRate: number;
  };
  actionsByModule: { module: string; count: number }[];
  actionsByType: { actionType: string; count: number }[];
  topUsers: { userId: string; userName: string; count: number }[];
  dailyActivity: { date: string; count: number }[];
  recentErrors: any[];
}

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private baseUrl = '/api/auditlog';

  constructor(private http: HttpClient) {}

  getAuditLogs(params: {
    page?: number;
    pageSize?: number;
    userId?: string;
    module?: string;
    actionType?: string;
    status?: string;
    startDate?: string;
    endDate?: string;
    searchTerm?: string;
  }): Observable<any> {
    let httpParams = new HttpParams();
    
    if (params.page) httpParams = httpParams.set('page', params.page.toString());
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params.userId) httpParams = httpParams.set('userId', params.userId);
    if (params.module) httpParams = httpParams.set('module', params.module);
    if (params.actionType) httpParams = httpParams.set('actionType', params.actionType);
    if (params.status) httpParams = httpParams.set('status', params.status);
    if (params.startDate) httpParams = httpParams.set('startDate', params.startDate);
    if (params.endDate) httpParams = httpParams.set('endDate', params.endDate);
    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);

    return this.http.get(this.baseUrl, { params: httpParams });
  }

  getAuditLogById(id: number): Observable<AuditLogDetail> {
    return this.http.get<AuditLogDetail>(`${this.baseUrl}/${id}`);
  }

  getStatistics(startDate?: string, endDate?: string): Observable<AuditStatistics> {
    let httpParams = new HttpParams();
    if (startDate) httpParams = httpParams.set('startDate', startDate);
    if (endDate) httpParams = httpParams.set('endDate', endDate);
    
    return this.http.get<AuditStatistics>(`${this.baseUrl}/statistics`, { params: httpParams });
  }

  getModules(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/modules`);
  }

  getActionTypes(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/action-types`);
  }

  getEntityAuditHistory(entityType: string, entityId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/entity/${entityType}/${entityId}`);
  }

  getUserActivity(userId: string, page?: number, pageSize?: number): Observable<any> {
    let httpParams = new HttpParams();
    if (page) httpParams = httpParams.set('page', page.toString());
    if (pageSize) httpParams = httpParams.set('pageSize', pageSize.toString());
    
    return this.http.get(`${this.baseUrl}/user/${userId}`, { params: httpParams });
  }

  exportAuditLogs(params: {
    module?: string;
    actionType?: string;
    startDate?: string;
    endDate?: string;
  }): Observable<Blob> {
    let httpParams = new HttpParams();
    if (params.module) httpParams = httpParams.set('module', params.module);
    if (params.actionType) httpParams = httpParams.set('actionType', params.actionType);
    if (params.startDate) httpParams = httpParams.set('startDate', params.startDate);
    if (params.endDate) httpParams = httpParams.set('endDate', params.endDate);

    return this.http.get(`${this.baseUrl}/export`, { 
      params: httpParams, 
      responseType: 'blob' 
    });
  }
}
