import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

// Role constants matching backend
export const UserRoles = {
  Admin: 'ADMIN',
  Authority: 'AUTHORITY',
  CompanyRegistrar: 'COMPANY_REGISTRAR',
  LicenseReviewer: 'LICENSE_REVIEWER',
  PropertyOperator: 'PROPERTY_OPERATOR',
  VehicleOperator: 'VEHICLE_OPERATOR',
  LicenseApplicationManager: 'LICENSE_APPLICATION_MANAGER',
  ActivityMonitoringManager: 'ACTIVITY_MONITORING_MANAGER',
  SecuritiesManager: 'SECURITIES_MANAGER',
  SecuritiesEntryManager: 'SECURITIES_ENTRY_MANAGER',
  PetitionWriterSecuritiesEntryManager: 'PETITION_WRITER_SECURITIES_ENTRY_MANAGER',
  PetitionWriterLicenseManager: 'PETITION_WRITER_LICENSE_MANAGER'
} as const;

// Permission constants matching backend
export const Permissions = {
  // User Management
  UsersView: 'users.view',
  UsersCreate: 'users.create',
  UsersEdit: 'users.edit',
  UsersDelete: 'users.delete',
  UsersLock: 'users.lock',

  // License Application Management
  LicenseApplicationView: 'licenseapplication.view',
  LicenseApplicationCreate: 'licenseapplication.create',
  LicenseApplicationEdit: 'licenseapplication.edit',
  LicenseApplicationDelete: 'licenseapplication.delete',

  // Company Management
  CompanyView: 'company.view',
  CompanyCreate: 'company.create',
  CompanyEdit: 'company.edit',
  CompanyDelete: 'company.delete',
  CompanyApprove: 'company.approve',

  // Property Management
  PropertyView: 'property.view',
  PropertyCreate: 'property.create',
  PropertyEdit: 'property.edit',
  PropertyEditOwn: 'property.edit.own',
  PropertyDelete: 'property.delete',

  // Vehicle Management
  VehicleView: 'vehicle.view',
  VehicleCreate: 'vehicle.create',
  VehicleEdit: 'vehicle.edit',
  VehicleEditOwn: 'vehicle.edit.own',
  VehicleDelete: 'vehicle.delete',

  // License Management
  LicenseView: 'license.view',
  LicenseCreate: 'license.create',
  LicenseEdit: 'license.edit',
  LicenseApprove: 'license.approve',

  // Securities
  SecuritiesView: 'securities.view',
  SecuritiesCreate: 'securities.create',
  SecuritiesEdit: 'securities.edit',
  SecuritiesDelete: 'securities.delete',

  // Petition Writer Securities
  PetitionWriterSecuritiesView: 'petitionwritersecurities.view',
  PetitionWriterSecuritiesCreate: 'petitionwritersecurities.create',
  PetitionWriterSecuritiesEdit: 'petitionwritersecurities.edit',
  PetitionWriterSecuritiesDelete: 'petitionwritersecurities.delete',

  // Petition Writer License
  PetitionWriterLicenseView: 'petitionwriterlicense.view',
  PetitionWriterLicenseCreate: 'petitionwriterlicense.create',
  PetitionWriterLicenseEdit: 'petitionwriterlicense.edit',
  PetitionWriterLicenseDelete: 'petitionwriterlicense.delete',

  // Activity Monitoring
  ActivityMonitoringView: 'activitymonitoring.view',
  ActivityMonitoringCreate: 'activitymonitoring.create',
  ActivityMonitoringEdit: 'activitymonitoring.edit',
  ActivityMonitoringDelete: 'activitymonitoring.delete',

  // Petition Writer Monitoring
  PetitionWriterMonitoringView: 'petitionwritermonitoring.view',
  PetitionWriterMonitoringCreate: 'petitionwritermonitoring.create',
  PetitionWriterMonitoringEdit: 'petitionwritermonitoring.edit',
  PetitionWriterMonitoringDelete: 'petitionwritermonitoring.delete',

  // Reports
  ReportsView: 'reports.view',
  ReportsExport: 'reports.export',

  // Dashboard
  DashboardView: 'dashboard.view',

  // System
  SystemConfigure: 'system.configure',

  // Audit Log
  AuditLogView: 'auditlog.view',
  AuditLogExport: 'auditlog.export'
} as const;

export interface UserProfile {
  userId: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  photoPath: string;
  companyId: number;
  companyName: string;
  phoneNumber: string;
  licenseType: string;
  role: string;
  roleDari: string;
  permissions: string[];
  isViewOnly: boolean;
  canAccessCompany: boolean;
  canAccessProperty: boolean;
  canAccessVehicle: boolean;
  canAccessReports: boolean;
  canAccessDashboard: boolean;
  canAccessUsers: boolean;
}

export interface RoleInfo {
  id: string;
  name: string;
  dari: string;
  permissions: string[];
}

@Injectable({
  providedIn: 'root'
})
export class RbacService {
  private readonly BaseURI = environment.apiUrl;
  private userProfile$ = new BehaviorSubject<UserProfile | null>(null);
  private permissions$ = new BehaviorSubject<string[]>([]);
  private cachedTokenPayload: any = null;

  constructor(private http: HttpClient) {
    this.loadUserFromToken();
  }

  private getTokenPayload(): any {
    if (this.cachedTokenPayload) return this.cachedTokenPayload;
    const token = localStorage.getItem('token');
    if (token) {
      try {
        this.cachedTokenPayload = JSON.parse(atob(token.split('.')[1]));
      } catch {
        this.cachedTokenPayload = null;
      }
    }
    return this.cachedTokenPayload;
  }

  // Load user info from JWT token
  loadUserFromToken(): void {
    const payload = this.getTokenPayload();
    if (payload) {
      const permissions = Array.isArray(payload.permission) 
        ? payload.permission 
        : payload.permission ? [payload.permission] : [];
      this.permissions$.next(permissions);
    }
  }

  // Get current user profile from API
  loadUserProfile(): Observable<UserProfile> {
    return new Observable(observer => {
      this.http.get<UserProfile>(`${this.BaseURI}/UserProfile/getProfile`).subscribe({
        next: (profile) => {
          this.userProfile$.next(profile);
          this.permissions$.next(profile.permissions || []);
          observer.next(profile);
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  // Get user profile observable
  getUserProfile(): Observable<UserProfile | null> {
    return this.userProfile$.asObservable();
  }

  // Get current user role from token
  getCurrentRole(): string {
    const payload = this.getTokenPayload();
    if (payload) {
      return payload.userRole || payload.role || '';
    }
    return '';
  }

  // Get current user ID from token
  getCurrentUserId(): string {
    const payload = this.getTokenPayload();
    if (payload) {
      return payload.UserID || '';
    }
    return '';
  }

  // Get company ID from token
  getCompanyId(): number {
    const payload = this.getTokenPayload();
    if (payload) {
      return parseInt(payload.companyId) || 0;
    }
    return 0;
  }

  // Get license type from token
  getLicenseType(): string {
    const payload = this.getTokenPayload();
    if (payload) {
      return payload.licenseType || '';
    }
    return '';
  }

  // Check if user is view-only (has no create/edit permissions at all)
  isViewOnly(): boolean {
    return !this.hasAnyPermission([
      Permissions.CompanyCreate, Permissions.PropertyCreate, Permissions.VehicleCreate,
      Permissions.SecuritiesCreate, Permissions.PetitionWriterSecuritiesCreate,
      Permissions.PetitionWriterLicenseCreate, Permissions.ActivityMonitoringCreate,
      Permissions.PetitionWriterMonitoringCreate, Permissions.LicenseApplicationCreate, Permissions.LicenseCreate
    ]);
  }

  // Check if user can create/edit in Property module
  canCreateProperty(): boolean {
    return this.hasPermission(Permissions.PropertyCreate) || this.hasPermission(Permissions.PropertyEdit);
  }

  // Check if user can create/edit in Vehicle module
  canCreateVehicle(): boolean {
    return this.hasPermission(Permissions.VehicleCreate) || this.hasPermission(Permissions.VehicleEdit);
  }

  // Check if user can create/edit in Company module
  canCreateCompany(): boolean {
    return this.hasPermission(Permissions.CompanyCreate) || this.hasPermission(Permissions.CompanyEdit);
  }

  // Check if user can create securities
  canCreateSecurities(): boolean {
    return this.hasPermission(Permissions.SecuritiesCreate);
  }

  // Check if user can edit securities
  canEditSecurities(): boolean {
    return this.hasPermission(Permissions.SecuritiesEdit);
  }

  // Check if user can create activity monitoring entries
  canCreateActivityMonitoring(): boolean {
    return this.hasPermission(Permissions.ActivityMonitoringCreate);
  }

  // Check if user can create petition writer monitoring entries
  canCreatePetitionWriterMonitoring(): boolean {
    return this.hasPermission(Permissions.PetitionWriterMonitoringCreate);
  }

  // Check if user can create petition writer securities
  canCreatePetitionWriterSecurities(): boolean {
    return this.hasPermission(Permissions.PetitionWriterSecuritiesCreate);
  }

  // Check if user can edit petition writer securities
  canEditPetitionWriterSecurities(): boolean {
    return this.hasPermission(Permissions.PetitionWriterSecuritiesEdit);
  }

  // Check if user can create petition writer license
  canCreatePetitionWriterLicense(): boolean {
    return this.hasPermission(Permissions.PetitionWriterLicenseCreate);
  }

  // Check if user has specific permission
  hasPermission(permission: string): boolean {
    const permissions = this.permissions$.getValue();
    return permissions.includes(permission);
  }

  // Check if user has any of the specified permissions
  hasAnyPermission(permissions: string[]): boolean {
    return permissions.some(p => this.hasPermission(p));
  }

  // Check if user has all specified permissions
  hasAllPermissions(permissions: string[]): boolean {
    return permissions.every(p => this.hasPermission(p));
  }

  // Check if user has specific role
  hasRole(role: string): boolean {
    return this.getCurrentRole() === role;
  }

  // Check if user has any of the specified roles
  hasAnyRole(roles: string[]): boolean {
    return roles.includes(this.getCurrentRole());
  }

  // Check if user is admin
  isAdmin(): boolean {
    return this.hasRole(UserRoles.Admin);
  }

  // Check if user can access module
  canAccessModule(module: string): boolean {
    // Admin has all permissions in token, so permission checks cover admin automatically
    switch (module.toLowerCase()) {
      case 'dashboard':
        return this.hasPermission(Permissions.DashboardView);
      case 'company':
        return this.hasPermission(Permissions.CompanyView);
      case 'property':
        return this.hasPermission(Permissions.PropertyView) || this.hasPermission(Permissions.PropertyEditOwn);
      case 'vehicle':
        return this.hasPermission(Permissions.VehicleView) || this.hasPermission(Permissions.VehicleEditOwn);
      case 'securities':
        return this.hasPermission(Permissions.SecuritiesView);
      case 'petitionwriter':
        return this.hasPermission(Permissions.PetitionWriterSecuritiesView) || this.hasPermission(Permissions.PetitionWriterLicenseView);
      case 'activitymonitoring':
        return this.hasPermission(Permissions.ActivityMonitoringView);
      case 'petitionwritermonitoring':
        return this.hasPermission(Permissions.PetitionWriterMonitoringView);
      case 'licenseapplications':
        return this.hasPermission(Permissions.LicenseApplicationView);
      case 'reports':
        return this.hasPermission(Permissions.ReportsView);
      case 'verification':
        return true; // Public to all authenticated users
      case 'users':
        return this.hasPermission(Permissions.UsersView);
      case 'auditlog':
        return this.hasPermission(Permissions.AuditLogView);
      default:
        return false;
    }
  }

  // Check if user can edit record (ownership check for property/vehicle operators)
  canEditRecord(createdBy: string): boolean {
    if (this.isAdmin()) return true;
    if (this.isViewOnly()) return false;
    // Users with edit.own permission can only edit their own records
    if (this.hasPermission(Permissions.PropertyEditOwn) || this.hasPermission(Permissions.VehicleEditOwn)) {
      return createdBy === this.getCurrentUserId();
    }
    return this.hasAnyPermission([
      Permissions.CompanyEdit, Permissions.PropertyEdit, Permissions.VehicleEdit
    ]);
  }

  // Get all available roles (for admin UI)
  getRoles(): Observable<RoleInfo[]> {
    return this.http.get<RoleInfo[]>(`${this.BaseURI}/ApplicationUser/GetRoles`);
  }

  // Get Dari name for role
  getRoleDari(role: string): string {
    const roleNames: { [key: string]: string } = {
      [UserRoles.Admin]: 'مدیر سیستم',
      [UserRoles.Authority]: 'مقام / رهبری',
      [UserRoles.CompanyRegistrar]: 'کاربر ثبت جواز رهنما',
      [UserRoles.LicenseReviewer]: 'ریاست بررسی و ثبت جواز',
      [UserRoles.PropertyOperator]: 'کاربر عملیاتی املاک',
      [UserRoles.VehicleOperator]: 'کاربر عملیاتی موتر فروشی',
      [UserRoles.LicenseApplicationManager]: 'کاربر مدیریت درخواست جواز',
      [UserRoles.ActivityMonitoringManager]: 'کاربر مدیریت نظارت بر فعالیت‌ها',
      [UserRoles.SecuritiesManager]: 'کاربر مدیریت اسناد بهادار',
      [UserRoles.SecuritiesEntryManager]: 'کاربر ثبت اسناد بهادار',
      [UserRoles.PetitionWriterSecuritiesEntryManager]: 'کاربر ثبت سند بهادار عریضه‌نویسان',
      [UserRoles.PetitionWriterLicenseManager]: 'کاربر مدیریت جواز عریضه‌نویسان'
    };
    return roleNames[role] || role;
  }

  // Clear user data on logout
  clearUser(): void {
    this.userProfile$.next(null);
    this.permissions$.next([]);
    this.cachedTokenPayload = null;
  }

  // Refresh permissions from server and update token
  refreshPermissions(): Observable<any> {
    return new Observable(observer => {
      this.http.get<any>(`${this.BaseURI}/ApplicationUser/RefreshPermissions`).subscribe({
        next: (response) => {
          // Update token in localStorage
          localStorage.setItem('token', response.token);
          this.cachedTokenPayload = null; // Clear cache so next call parses new token
          
          // Reload permissions from new token
          this.loadUserFromToken();
          
          // Reload user profile
          this.loadUserProfile().subscribe({
            next: () => {
              observer.next(response);
              observer.complete();
            },
            error: (err) => {
              // Even if profile load fails, we have the new token
              observer.next(response);
              observer.complete();
            }
          });
        },
        error: (err) => observer.error(err)
      });
    });
  }
}
