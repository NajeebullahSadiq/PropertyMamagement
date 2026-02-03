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
  VehicleOperator: 'VEHICLE_OPERATOR'
} as const;

// Permission constants matching backend
export const Permissions = {
  // User Management
  UsersView: 'users.view',
  UsersCreate: 'users.create',
  UsersEdit: 'users.edit',
  UsersDelete: 'users.delete',
  UsersLock: 'users.lock',

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

  // Reports
  ReportsView: 'reports.view',
  ReportsExport: 'reports.export',

  // Dashboard
  DashboardView: 'dashboard.view',

  // System
  SystemConfigure: 'system.configure'
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

  constructor(private http: HttpClient) {
    this.loadUserFromToken();
  }

  // Load user info from JWT token
  loadUserFromToken(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const permissions = Array.isArray(payload.permission) 
          ? payload.permission 
          : payload.permission ? [payload.permission] : [];
        this.permissions$.next(permissions);
      } catch (e) {
        console.error('Error parsing token', e);
      }
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
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.userRole || payload.role || '';
      } catch (e) {
        return '';
      }
    }
    return '';
  }

  // Get current user ID from token
  getCurrentUserId(): string {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.UserID || '';
      } catch (e) {
        return '';
      }
    }
    return '';
  }

  // Get company ID from token
  getCompanyId(): number {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return parseInt(payload.companyId) || 0;
      } catch (e) {
        return 0;
      }
    }
    return 0;
  }

  // Get license type from token
  getLicenseType(): string {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.licenseType || '';
      } catch (e) {
        return '';
      }
    }
    return '';
  }

  // Check if user is view-only
  isViewOnly(): boolean {
    const role = this.getCurrentRole();
    return role === UserRoles.Authority || role === UserRoles.LicenseReviewer;
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
    const role = this.getCurrentRole();
    const licenseType = this.getLicenseType();

    // Admin and Authority can access all modules
    if (role === UserRoles.Admin || role === UserRoles.Authority) {
      return true;
    }

    switch (module.toLowerCase()) {
      case 'company':
        return role === UserRoles.CompanyRegistrar || role === UserRoles.LicenseReviewer;
      case 'property':
        return role === UserRoles.PropertyOperator || 
               role === UserRoles.CompanyRegistrar ||  // Company registrar can view property
               licenseType === 'realEstate';
      case 'vehicle':
        return role === UserRoles.VehicleOperator || 
               role === UserRoles.CompanyRegistrar ||  // Company registrar can view vehicle
               licenseType === 'carSale';
      case 'securities':
        // Securities module is only for Admin and Authority
        return role === UserRoles.Admin || role === UserRoles.Authority;
      case 'petitionwriter':
        // Petition Writer module is only for Admin and Authority
        return role === UserRoles.Admin || role === UserRoles.Authority;
      case 'activitymonitoring':
        // Activity Monitoring is only for Admin and Authority
        return role === UserRoles.Admin || role === UserRoles.Authority;
      case 'verification':
        // Verification is accessible to all roles
        return true;
      case 'reports':
        // Reports accessible to all except License Reviewer
        return role !== UserRoles.LicenseReviewer;
      case 'dashboard':
        // Dashboard is ONLY for Admin and Authority
        return role === UserRoles.Admin || role === UserRoles.Authority;
      case 'users':
        return role === UserRoles.Admin;
      default:
        return false;
    }
  }

  // Check if user can edit record (ownership check)
  canEditRecord(createdBy: string): boolean {
    const role = this.getCurrentRole();
    
    // Admin can edit all records
    if (role === UserRoles.Admin) {
      return true;
    }

    // View-only roles cannot edit
    if (this.isViewOnly()) {
      return false;
    }

    // Company registrar can edit company records but NOT property/vehicle records
    if (role === UserRoles.CompanyRegistrar) {
      return true; // This will be further restricted by permission checks in components
    }

    // Property/Vehicle operators can only edit their own records
    if (role === UserRoles.PropertyOperator || role === UserRoles.VehicleOperator) {
      return createdBy === this.getCurrentUserId();
    }

    return false;
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
      [UserRoles.VehicleOperator]: 'کاربر عملیاتی موتر فروشی'
    };
    return roleNames[role] || role;
  }

  // Clear user data on logout
  clearUser(): void {
    this.userProfile$.next(null);
    this.permissions$.next([]);
  }
}
