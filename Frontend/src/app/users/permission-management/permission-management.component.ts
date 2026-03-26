import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { RoleAssignDialogComponent } from './role-assign-dialog/role-assign-dialog.component';
import { environment } from 'src/environments/environment';

export interface UserPermissionData {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  roleDari: string;
  isLocked: boolean;
  photoPath: string;
}

@Component({
  selector: 'app-permission-management',
  templateUrl: './permission-management.component.html',
  styleUrls: ['./permission-management.component.scss']
})
export class PermissionManagementComponent implements OnInit {
  users: UserPermissionData[] = [];
  filteredUsers: UserPermissionData[] = [];
  roles: any[] = [];
  searchTerm = '';
  roleFilter = '';
  isLoading = false;
  baseUrl = environment.apiURL + '/';

  page = 1;
  tableSize = 15;
  count = 0;

  constructor(
    private authService: AuthService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.authService.getRoles().subscribe({
      next: (roles: any) => {
        this.roles = roles;
        this.authService.getUserProfile().subscribe({
          next: (users: any) => {
            this.users = users;
            this.applyFilters();
            this.isLoading = false;
          },
          error: () => {
            this.toastr.error('خطا در بارگذاری کاربران');
            this.isLoading = false;
          }
        });
      },
      error: () => {
        this.toastr.error('خطا در بارگذاری نقش‌ها');
        this.isLoading = false;
      }
    });
  }

  applyFilters(): void {
    let result = [...this.users];
    if (this.searchTerm) {
      const t = this.searchTerm.toLowerCase();
      result = result.filter(u =>
        u.userName?.toLowerCase().includes(t) ||
        u.firstName?.toLowerCase().includes(t) ||
        u.lastName?.toLowerCase().includes(t)
      );
    }
    if (this.roleFilter) {
      result = result.filter(u => u.role === this.roleFilter);
    }
    this.filteredUsers = result;
    this.count = result.length;
    this.page = 1;
  }

  openRoleAssign(user: UserPermissionData): void {
    const dialogRef = this.dialog.open(RoleAssignDialogComponent, {
      width: '700px',
      maxWidth: '95vw',
      data: { user, roles: this.roles }
    });

    dialogRef.afterClosed().subscribe(changed => {
      if (changed) this.loadData();
    });
  }

  getUserPhoto(photoPath: string): string {
    return photoPath ? this.baseUrl + photoPath : 'assets/img/avatar.png';
  }

  getRoleColor(role: string): string {
    const colors: Record<string, string> = {
      'ADMIN': 'bg-red-100 text-red-700 border-red-200',
      'AUTHORITY': 'bg-purple-100 text-purple-700 border-purple-200',
      'COMPANY_REGISTRAR': 'bg-blue-100 text-blue-700 border-blue-200',
      'LICENSE_REVIEWER': 'bg-cyan-100 text-cyan-700 border-cyan-200',
      'PROPERTY_OPERATOR': 'bg-green-100 text-green-700 border-green-200',
      'VEHICLE_OPERATOR': 'bg-teal-100 text-teal-700 border-teal-200',
      'LICENSE_APPLICATION_MANAGER': 'bg-orange-100 text-orange-700 border-orange-200',
      'ACTIVITY_MONITORING_MANAGER': 'bg-yellow-100 text-yellow-700 border-yellow-200',
      'SECURITIES_MANAGER': 'bg-indigo-100 text-indigo-700 border-indigo-200',
      'SECURITIES_ENTRY_MANAGER': 'bg-violet-100 text-violet-700 border-violet-200',
      'PETITION_WRITER_SECURITIES_ENTRY_MANAGER': 'bg-pink-100 text-pink-700 border-pink-200',
      'PETITION_WRITER_LICENSE_MANAGER': 'bg-rose-100 text-rose-700 border-rose-200',
    };
    return colors[role] || 'bg-gray-100 text-gray-700 border-gray-200';
  }

  onTableDataChange(event: number): void {
    this.page = event;
  }
}
