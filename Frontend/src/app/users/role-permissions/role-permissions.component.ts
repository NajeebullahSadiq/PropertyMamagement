import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';

export interface PermissionGroup {
  label: string;
  icon: string;
  keys: string[];
}

export const ALL_PERMISSION_GROUPS: PermissionGroup[] = [
  {
    label: 'مدیریت کاربران',
    icon: 'fa-users-cog',
    keys: ['users.view', 'users.create', 'users.edit', 'users.delete', 'users.lock']
  },
  {
    label: 'رهنما (شرکت)',
    icon: 'fa-building',
    keys: ['company.view', 'company.create', 'company.edit', 'company.delete', 'company.approve']
  },
  {
    label: 'جواز',
    icon: 'fa-id-card',
    keys: ['license.view', 'license.create', 'license.edit', 'license.approve']
  },
  {
    label: 'درخواست جواز',
    icon: 'fa-file-alt',
    keys: ['licenseapplication.view', 'licenseapplication.create', 'licenseapplication.edit', 'licenseapplication.delete']
  },
  {
    label: 'ملکیت',
    icon: 'fa-home',
    keys: ['property.view', 'property.create', 'property.edit', 'property.edit.own', 'property.delete']
  },
  {
    label: 'وسایط نقلیه',
    icon: 'fa-car',
    keys: ['vehicle.view', 'vehicle.create', 'vehicle.edit', 'vehicle.edit.own', 'vehicle.delete']
  },
  {
    label: 'اسناد بهادار رهنمای معاملات',
    icon: 'fa-receipt',
    keys: ['securities.view', 'securities.create', 'securities.edit', 'securities.delete']
  },
  {
    label: 'سند بهادار عریضه‌نویسان',
    icon: 'fa-scroll',
    keys: ['petitionwritersecurities.view', 'petitionwritersecurities.create', 'petitionwritersecurities.edit', 'petitionwritersecurities.delete']
  },
  {
    label: 'جواز عریضه‌نویسان',
    icon: 'fa-edit',
    keys: ['petitionwriterlicense.view', 'petitionwriterlicense.create', 'petitionwriterlicense.edit', 'petitionwriterlicense.delete']
  },
  {
    label: 'نظارت بر فعالیت دفاتر رهنمای معاملات',
    icon: 'fa-eye',
    keys: ['activitymonitoring.view', 'activitymonitoring.create', 'activitymonitoring.edit', 'activitymonitoring.delete']
  },
  {
    label: 'نظارت بر فعالیت عریضه نویسان',
    icon: 'fa-clipboard-check',
    keys: ['petitionwritermonitoring.view', 'petitionwritermonitoring.create', 'petitionwritermonitoring.edit', 'petitionwritermonitoring.delete']
  },
  {
    label: 'داشبورد و گزارشات',
    icon: 'fa-chart-bar',
    keys: ['dashboard.view', 'reports.view', 'reports.export', 'system.configure']
  }
];

export const PERMISSION_LABELS: Record<string, string> = {
  // Users
  'users.view': 'مشاهده کاربران',
  'users.create': 'ایجاد کاربر',
  'users.edit': 'ویرایش کاربر',
  'users.delete': 'حذف کاربر',
  'users.lock': 'قفل/فعال کاربر',
  // Company
  'company.view': 'مشاهده رهنما',
  'company.create': 'ثبت رهنما',
  'company.edit': 'ویرایش رهنما',
  'company.delete': 'حذف رهنما',
  'company.approve': 'تأیید رهنما',
  // License
  'license.view': 'مشاهده جواز',
  'license.create': 'ثبت جواز',
  'license.edit': 'ویرایش جواز',
  'license.approve': 'تأیید جواز',
  // License Application
  'licenseapplication.view': 'مشاهده درخواست جواز',
  'licenseapplication.create': 'ثبت درخواست جواز',
  'licenseapplication.edit': 'ویرایش درخواست جواز',
  'licenseapplication.delete': 'حذف درخواست جواز',
  // Property
  'property.view': 'مشاهده ملکیت',
  'property.create': 'ثبت ملکیت',
  'property.edit': 'ویرایش ملکیت',
  'property.edit.own': 'ویرایش ملکیت خود',
  'property.delete': 'حذف ملکیت',
  // Vehicle
  'vehicle.view': 'مشاهده وسایط',
  'vehicle.create': 'ثبت وسایط',
  'vehicle.edit': 'ویرایش وسایط',
  'vehicle.edit.own': 'ویرایش وسایط خود',
  'vehicle.delete': 'حذف وسایط',
  // Securities
  'securities.view': 'مشاهده اسناد بهادار',
  'securities.create': 'ثبت اسناد بهادار',
  'securities.edit': 'ویرایش اسناد بهادار',
  'securities.delete': 'حذف اسناد بهادار',
  // Petition Writer Securities
  'petitionwritersecurities.view': 'مشاهده سند بهادار عریضه‌نویسان',
  'petitionwritersecurities.create': 'ثبت سند بهادار عریضه‌نویسان',
  'petitionwritersecurities.edit': 'ویرایش سند بهادار عریضه‌نویسان',
  'petitionwritersecurities.delete': 'حذف سند بهادار عریضه‌نویسان',
  // Petition Writer License
  'petitionwriterlicense.view': 'مشاهده جواز عریضه‌نویسان',
  'petitionwriterlicense.create': 'ثبت جواز عریضه‌نویسان',
  'petitionwriterlicense.edit': 'ویرایش جواز عریضه‌نویسان',
  'petitionwriterlicense.delete': 'حذف جواز عریضه‌نویسان',
  // Activity Monitoring
  'activitymonitoring.view': 'مشاهده نظارت بر فعالیت دفاتر رهنما',
  'activitymonitoring.create': 'ثبت نظارت بر فعالیت دفاتر رهنما',
  'activitymonitoring.edit': 'ویرایش نظارت بر فعالیت دفاتر رهنما',
  'activitymonitoring.delete': 'حذف نظارت بر فعالیت دفاتر رهنما',
  // Petition Writer Monitoring
  'petitionwritermonitoring.view': 'مشاهده نظارت بر فعالیت عریضه نویسان',
  'petitionwritermonitoring.create': 'ثبت نظارت بر فعالیت عریضه نویسان',
  'petitionwritermonitoring.edit': 'ویرایش نظارت بر فعالیت عریضه نویسان',
  'petitionwritermonitoring.delete': 'حذف نظارت بر فعالیت عریضه نویسان',
  // System
  'dashboard.view': 'مشاهده داشبورد',
  'reports.view': 'مشاهده گزارشات',
  'reports.export': 'خروجی گزارشات',
  'system.configure': 'تنظیمات سیستم',
};

@Component({
  selector: 'app-role-permissions',
  templateUrl: './role-permissions.component.html',
  styleUrls: ['./role-permissions.component.scss']
})
export class RolePermissionsComponent implements OnInit {
  roles: any[] = [];
  selectedRole: any = null;
  activePermissions: Record<string, boolean> = {};
  originalPermissions: Record<string, boolean> = {};
  permissionGroups = ALL_PERMISSION_GROUPS;
  permissionLabels = PERMISSION_LABELS;
  isLoading = false;
  isSaving = false;
  hasChanges = false;

  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(preserveRoleId?: string): void {
    this.isLoading = true;
    this.http.get<any[]>(`${this.baseUrl}/ApplicationUser/GetRoles`).subscribe({
      next: (roles) => {
        this.roles = roles;
        this.isLoading = false;
        // Re-select the same role if we were editing one, otherwise pick first
        const toSelect = preserveRoleId
          ? roles.find(r => r.id === preserveRoleId) ?? roles[0]
          : roles[0];
        if (toSelect) this.selectRole(toSelect);
      },
      error: () => {
        this.toastr.error('خطا در بارگذاری نقش‌ها');
        this.isLoading = false;
      }
    });
  }

  selectRole(role: any): void {
    if (this.hasChanges) {
      if (!confirm('تغییرات ذخیره نشده دارید. آیا می‌خواهید بدون ذخیره ادامه دهید؟')) return;
    }
    this.selectedRole = role;
    this.activePermissions = this.toMap(role.permissions || []);
    this.originalPermissions = this.toMap(role.permissions || []);
    this.hasChanges = false;
  }

  private toMap(permissions: string[]): Record<string, boolean> {
    const map: Record<string, boolean> = {};
    permissions.forEach(p => { map[p] = true; });
    return map;
  }

  private toArray(map: Record<string, boolean>): string[] {
    return Object.keys(map).filter(k => map[k]);
  }

  togglePermission(key: string): void {
    if (this.isAdminRole()) return;
    this.activePermissions = { ...this.activePermissions, [key]: !this.activePermissions[key] };
    this.checkChanges();
  }

  toggleGroup(group: PermissionGroup): void {
    if (this.isAdminRole()) return;
    const allOn = group.keys.every(k => this.activePermissions[k]);
    const updated = { ...this.activePermissions };
    group.keys.forEach(k => { updated[k] = !allOn; });
    this.activePermissions = updated;
    this.checkChanges();
  }

  isGroupAllOn(group: PermissionGroup): boolean {
    return group.keys.length > 0 && group.keys.every(k => this.activePermissions[k]);
  }

  isGroupPartial(group: PermissionGroup): boolean {
    const count = group.keys.filter(k => this.activePermissions[k]).length;
    return count > 0 && count < group.keys.length;
  }

  checkChanges(): void {
    const current = this.toArray(this.activePermissions).sort().join(',');
    const original = this.toArray(this.originalPermissions).sort().join(',');
    this.hasChanges = current !== original;
  }

  isAdminRole(): boolean {
    return this.selectedRole?.id === 'ADMIN';
  }

  save(): void {
    if (!this.selectedRole || !this.hasChanges) return;
    this.isSaving = true;
    const roleId = this.selectedRole.id;
    const permissions = this.toArray(this.activePermissions);
    this.http.post(`${this.baseUrl}/ApplicationUser/UpdateRolePermissions`, {
      roleName: roleId,
      permissions
    }).subscribe({
      next: () => {
        // Reload this specific role's permissions directly from DB to confirm
        this.http.get<any>(`${this.baseUrl}/ApplicationUser/GetRolePermissions/${roleId}`).subscribe({
          next: (fresh) => {
            this.isSaving = false;
            this.hasChanges = false;
            // Update the role in the sidebar list
            const idx = this.roles.findIndex(r => r.id === roleId);
            if (idx >= 0) {
              this.roles[idx] = { ...this.roles[idx], permissions: fresh.permissions };
              this.roles = [...this.roles]; // trigger change detection
            }
            // Update selected role state from server response
            this.selectedRole = this.roles.find(r => r.id === roleId) ?? this.selectedRole;
            this.activePermissions = this.toMap(fresh.permissions || []);
            this.originalPermissions = this.toMap(fresh.permissions || []);
            this.toastr.success('صلاحیت‌های نقش با موفقیت ذخیره شد');
            
            // Show additional warning about users needing to refresh
            this.toastr.warning(
              'کاربران با این نقش باید از منوی کاربری گزینه "به‌روزرسانی دسترسی‌ها" را انتخاب کنند یا دوباره وارد شوند',
              'توجه',
              { timeOut: 10000, closeButton: true }
            );
          },
          error: () => {
            this.isSaving = false;
            this.hasChanges = false;
            this.toastr.success('ذخیره شد — صفحه را رفرش کنید');
          }
        });
      },
      error: (err) => {
        this.isSaving = false;
        this.toastr.error(err.error?.message || 'خطا در ذخیره صلاحیت‌ها');
      }
    });
  }

  resetToOriginal(): void {
    this.activePermissions = { ...this.originalPermissions };
    this.hasChanges = false;
  }

  getActiveCount(): number {
    return this.toArray(this.activePermissions).length;
  }

  getTotalCount(): number {
    return this.permissionGroups.reduce((sum, g) => sum + g.keys.length, 0);
  }

  getRoleColor(roleId: string): string {
    const colors: Record<string, string> = {
      'ADMIN': 'border-red-400 bg-red-50 text-red-700',
      'AUTHORITY': 'border-purple-400 bg-purple-50 text-purple-700',
      'COMPANY_REGISTRAR': 'border-blue-400 bg-blue-50 text-blue-700',
      'LICENSE_REVIEWER': 'border-cyan-400 bg-cyan-50 text-cyan-700',
      'PROPERTY_OPERATOR': 'border-green-400 bg-green-50 text-green-700',
      'VEHICLE_OPERATOR': 'border-teal-400 bg-teal-50 text-teal-700',
      'LICENSE_APPLICATION_MANAGER': 'border-orange-400 bg-orange-50 text-orange-700',
      'ACTIVITY_MONITORING_MANAGER': 'border-yellow-500 bg-yellow-50 text-yellow-700',
      'SECURITIES_MANAGER': 'border-indigo-400 bg-indigo-50 text-indigo-700',
      'SECURITIES_ENTRY_MANAGER': 'border-violet-400 bg-violet-50 text-violet-700',
      'PETITION_WRITER_SECURITIES_ENTRY_MANAGER': 'border-pink-400 bg-pink-50 text-pink-700',
      'PETITION_WRITER_LICENSE_MANAGER': 'border-rose-400 bg-rose-50 text-rose-700',
    };
    return colors[roleId] || 'border-gray-300 bg-gray-50 text-gray-700';
  }
}
