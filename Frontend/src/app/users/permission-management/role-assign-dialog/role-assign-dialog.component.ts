import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-role-assign-dialog',
  templateUrl: './role-assign-dialog.component.html',
  styleUrls: ['./role-assign-dialog.component.scss']
})
export class RoleAssignDialogComponent extends BaseComponent implements OnInit {
  selectedRole: string = '';
  isSubmitting = false;
  private readonly baseUrl = environment.apiUrl;

  constructor(
    public dialogRef: MatDialogRef<RoleAssignDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private http: HttpClient,
    private toastr: ToastrService
  ) {
    super();
  }

  ngOnInit(): void {
    this.selectedRole = this.data.user.role;
  }

  onRoleSelect(roleId: string): void {
    this.selectedRole = roleId;
  }

  isCurrentRole(roleId: string): boolean {
    return roleId === this.data.user.role;
  }

  isChanged(): boolean {
    return this.selectedRole !== this.data.user.role;
  }

  getRoleDari(roleId: string): string {
    return this.data.roles.find((r: any) => r.id === roleId)?.dari || roleId;
  }

  getRoleColor(role: string): string {
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
    return colors[role] || 'border-gray-300 bg-gray-50 text-gray-700';
  }

  onSave(): void {
    if (!this.isChanged()) { this.dialogRef.close(false); return; }
    this.isSubmitting = true;
    this.http.post(`${this.baseUrl}/ApplicationUser/UpdateUserRole`, {
      userId: this.data.user.id,
      newRole: this.selectedRole
    }).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        this.toastr.success(res.message || 'نقش کاربر با موفقیت تغییر کرد');
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.toastr.error(err.error?.message || 'خطا در تغییر نقش کاربر');
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
