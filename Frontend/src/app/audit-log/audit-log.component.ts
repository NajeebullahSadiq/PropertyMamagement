import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { NotificationService } from 'src/app/shared/notification.service';
import { AuditLogService, AuditLog, AuditStatistics, AuditLogDetail } from './audit-log.service';

@Component({
  selector: 'app-audit-log',
  templateUrl: './audit-log.component.html',
  styleUrls: ['./audit-log.component.scss']
})
export class AuditLogComponent extends BaseComponent implements OnInit {
  auditLogs: AuditLog[] = [];
  statistics: AuditStatistics | null = null;
  modules: string[] = [];
  actionTypes: string[] = [];
  
  // Pagination
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 0;

  // Filters
  filterForm: FormGroup;
  showFilters = false;

  // Loading states
  isLoading = false;
  isLoadingStats = false;

  // Error state
  errorMessage: string | null = null;

  // View modes
  viewMode: 'list' | 'statistics' = 'list';

  // Selected log for detail view
  selectedLog: AuditLogDetail | null = null;
  showDetailPanel = false;

  // Action type translations
  actionTypeTranslations: { [key: string]: string } = {
    'Create': 'ثبت',
    'Update': 'تغییر',
    'Delete': 'حذف',
    'Login': 'ورود',
    'Logout': 'خروج',
    'View': 'مشاهده',
    'Export': 'صادر کردن',
    'Print': 'چاپ',
    'Verify': 'تصدیق',
    'Approve': 'تایید',
    'Reject': 'رد',
    'Assign': 'تعیین',
    'Revoke': 'لغو',
    'PasswordChange': 'تغییر پسورد',
    'PasswordReset': 'بازنشانی پسورد',
    'UserLock': 'قفل کاربر',
    'UserUnlock': 'باز کردن کاربر',
    'FailedLogin': 'ورود ناموفق',
    'Unauthorized': 'غیرمجاز',
    'Error': 'خطا'
  };

  // Module translations
  moduleTranslations: { [key: string]: string } = {
    'Authentication': 'احراز هویت',
    'UserManagement': 'مدیریت کاربران',
    'RoleManagement': 'مدیریت نقش‌ها',
    'PermissionManagement': 'مدیریت صلاحیت‌ها',
    'Property': 'ملکیت',
    'Vehicle': 'وسایط نقلیه',
    'Company': 'شرکت',
    'License': 'جواز',
    'Securities': 'اسناد بهادار',
    'PetitionWriter': 'عریضه‌نویس',
    'ActivityMonitoring': 'نظارت فعالیت',
    'DistrictManagement': 'مدیریت ولسوالی',
    'Report': 'گزارش',
    'Verification': 'تصدیق',
    'System': 'سیستم'
  };

  // Status translations
  statusTranslations: { [key: string]: string } = {
    'Success': 'موفق',
    'Failed': 'ناموفق',
    'Error': 'خطا'
  };

  constructor(
    private auditLogService: AuditLogService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private notification: NotificationService
  ) {
    super();
    this.filterForm = this.fb.group({
      module: [''],
      actionType: [''],
      status: [''],
      startDate: [''],
      endDate: [''],
      searchTerm: ['']
    });
  }

  ngOnInit(): void {
    this.loadModules();
    this.loadActionTypes();
    this.loadAuditLogs();
  }

  loadModules(): void {
    this.auditLogService.getModules().pipe(takeUntil(this.destroy$)).subscribe({
      next: (modules: any) => {
        console.log('Modules response:', modules);
        this.modules = modules || [];
      },
      error: (err) => {
        console.error('Failed to load modules', err);
        this.notification.showHttpError(err);
      }
    });
  }

  loadActionTypes(): void {
    this.auditLogService.getActionTypes().pipe(takeUntil(this.destroy$)).subscribe({
      next: (types: any) => {
        console.log('Action types response:', types);
        this.actionTypes = types || [];
      },
      error: (err) => {
        console.error('Failed to load action types', err);
        this.notification.showHttpError(err);
      }
    });
  }

  loadAuditLogs(): void {
    this.isLoading = true;
    const filters = this.filterForm.value;
    
    this.auditLogService.getAuditLogs({
      page: this.currentPage,
      pageSize: this.pageSize,
      ...filters,
      startDate: filters.startDate ? new Date(filters.startDate).toISOString() : undefined,
      endDate: filters.endDate ? new Date(filters.endDate).toISOString() : undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (response: any) => {
        console.log('Audit logs response:', response);
        this.errorMessage = null;
        // Backend returns PascalCase, map to camelCase
        this.auditLogs = (response.data || response.Data || []).map((log: any) => ({
          id: log.id || log.Id,
          userId: log.userId || log.UserId,
          userName: log.userName || log.UserName,
          userRole: log.userRole || log.UserRole,
          actionType: log.actionType || log.ActionType,
          module: log.module || log.Module,
          entityType: log.entityType || log.EntityType,
          entityId: log.entityId || log.EntityId,
          description: log.description || log.Description,
          descriptionDari: log.descriptionDari || log.DescriptionDari,
          ipAddress: log.ipAddress || log.IpAddress,
          requestUrl: log.requestUrl || log.RequestUrl,
          httpMethod: log.httpMethod || log.HttpMethod,
          status: log.status || log.Status,
          errorMessage: log.errorMessage || log.ErrorMessage,
          userProvince: log.userProvince || log.UserProvince,
          timestamp: log.timestamp || log.Timestamp,
          durationMs: log.durationMs || log.DurationMs
        }));
        
        const pagination = response.pagination || response.Pagination || {};
        this.totalCount = pagination.totalCount || pagination.TotalCount || 0;
        this.totalPages = pagination.totalPages || pagination.TotalPages || 0;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load audit logs', err);
        this.isLoading = false;
        this.notification.showHttpError(err);
        if (err.status === 403) {
          this.errorMessage = 'شما صلاحیت مشاهده لاگ فعالیت‌ها را ندارید';
        } else {
          this.errorMessage = 'خطا در بارگذاری لاگ فعالیت‌ها. لطفاً دوباره تلاش کنید';
        }
      }
    });
  }

  loadStatistics(): void {
    this.isLoadingStats = true;
    const filters = this.filterForm.value;
    
    this.auditLogService.getStatistics(
      filters.startDate ? new Date(filters.startDate).toISOString() : undefined,
      filters.endDate ? new Date(filters.endDate).toISOString() : undefined
    ).pipe(takeUntil(this.destroy$)).subscribe({
      next: (stats: any) => {
        console.log('Statistics response:', stats);
        // Map PascalCase to camelCase
        this.statistics = {
          dateRange: {
            startDate: stats.dateRange?.startDate || stats.DateRange?.StartDate,
            endDate: stats.dateRange?.endDate || stats.DateRange?.EndDate
          },
          summary: {
            totalActions: stats.summary?.totalActions || stats.Summary?.TotalActions || 0,
            successfulActions: stats.summary?.successfulActions || stats.Summary?.SuccessfulActions || 0,
            failedActions: stats.summary?.failedActions || stats.Summary?.FailedActions || 0,
            successRate: stats.summary?.successRate || stats.Summary?.SuccessRate || 0
          },
          actionsByModule: (stats.actionsByModule || stats.ActionsByModule || []).map((item: any) => ({
            module: item.module || item.Module,
            count: item.count || item.Count
          })),
          actionsByType: (stats.actionsByType || stats.ActionsByType || []).map((item: any) => ({
            actionType: item.actionType || item.ActionType,
            count: item.count || item.Count
          })),
          topUsers: (stats.topUsers || stats.TopUsers || []).map((item: any) => ({
            userId: item.userId || item.UserId,
            userName: item.userName || item.UserName,
            count: item.count || item.Count
          })),
          dailyActivity: (stats.dailyActivity || stats.DailyActivity || []).map((item: any) => ({
            date: item.date || item.Date,
            count: item.count || item.Count
          })),
          recentErrors: stats.recentErrors || stats.RecentErrors || []
        };
        this.isLoadingStats = false;
      },
      error: (err) => {
        console.error('Failed to load statistics', err);
        this.isLoadingStats = false;
      }
    });
  }

  onFilter(): void {
    this.currentPage = 1;
    if (this.viewMode === 'statistics') {
      this.loadStatistics();
    }
    this.loadAuditLogs();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.currentPage = 1;
    this.loadAuditLogs();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadAuditLogs();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadAuditLogs();
  }

  toggleViewMode(): void {
    this.viewMode = this.viewMode === 'list' ? 'statistics' : 'list';
    if (this.viewMode === 'statistics') {
      this.loadStatistics();
    }
  }

  viewLogDetail(log: AuditLog): void {
    this.auditLogService.getAuditLogById(log.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: (detail: any) => {
        console.log('Log detail response:', detail);
        // Map PascalCase to camelCase
        this.selectedLog = {
          id: detail.id || detail.Id,
          userId: detail.userId || detail.UserId,
          userName: detail.userName || detail.UserName,
          userRole: detail.userRole || detail.UserRole,
          actionType: detail.actionType || detail.ActionType,
          module: detail.module || detail.Module,
          entityType: detail.entityType || detail.EntityType,
          entityId: detail.entityId || detail.EntityId,
          description: detail.description || detail.Description,
          descriptionDari: detail.descriptionDari || detail.DescriptionDari,
          ipAddress: detail.ipAddress || detail.IpAddress,
          requestUrl: detail.requestUrl || detail.RequestUrl,
          httpMethod: detail.httpMethod || detail.HttpMethod,
          status: detail.status || detail.Status,
          errorMessage: detail.errorMessage || detail.ErrorMessage,
          userProvince: detail.userProvince || detail.UserProvince,
          timestamp: detail.timestamp || detail.Timestamp,
          durationMs: detail.durationMs || detail.DurationMs,
          oldValues: detail.oldValues || detail.OldValues,
          newValues: detail.newValues || detail.NewValues,
          userAgent: detail.userAgent || detail.UserAgent,
          metadata: detail.metadata || detail.Metadata
        };
        this.showDetailPanel = true;
      },
      error: (err) => console.error('Failed to load log detail', err)
    });
  }

  closeDetailPanel(): void {
    this.showDetailPanel = false;
    this.selectedLog = null;
  }

  exportLogs(): void {
    const filters = this.filterForm.value;
    this.auditLogService.exportAuditLogs({
      module: filters.module,
      actionType: filters.actionType,
      startDate: filters.startDate ? new Date(filters.startDate).toISOString() : undefined,
      endDate: filters.endDate ? new Date(filters.endDate).toISOString() : undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `audit_logs_${new Date().toISOString().split('T')[0]}.csv`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Failed to export logs', err)
    });
  }

  translateActionType(type: string): string {
    return this.actionTypeTranslations[type] || type;
  }

  translateModule(module: string): string {
    return this.moduleTranslations[module] || module;
  }

  translateStatus(status: string): string {
    return this.statusTranslations[status] || status;
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Success': return 'bg-emerald-100 text-emerald-700';
      case 'Failed': return 'bg-red-100 text-red-700';
      case 'Error': return 'bg-amber-100 text-amber-700';
      default: return 'bg-gray-100 text-gray-700';
    }
  }

  getActionTypeClass(type: string): string {
    switch (type) {
      case 'Create': return 'bg-blue-100 text-blue-700';
      case 'Update': return 'bg-purple-100 text-purple-700';
      case 'Delete': return 'bg-red-100 text-red-700';
      case 'Login': return 'bg-green-100 text-green-700';
      case 'Logout': return 'bg-gray-100 text-gray-700';
      case 'Print': return 'bg-indigo-100 text-indigo-700';
      case 'Export': return 'bg-cyan-100 text-cyan-700';
      default: return 'bg-slate-100 text-slate-700';
    }
  }

  formatDateTime(date: string): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleString('fa-AF', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatJson(jsonStr: string | null | undefined): string {
    if (!jsonStr) return '';
    try {
      const obj = JSON.parse(jsonStr);
      return JSON.stringify(obj, null, 2);
    } catch {
      return jsonStr;
    }
  }

  getPaginationArray(): number[] {
    const pages: number[] = [];
    const start = Math.max(1, this.currentPage - 2);
    const end = Math.min(this.totalPages, this.currentPage + 2);
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }
}
