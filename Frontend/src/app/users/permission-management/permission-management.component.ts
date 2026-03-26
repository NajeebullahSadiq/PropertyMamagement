import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { RoleAssignDialogComponent } from './role-assign-dialog/role-assign-dialog.component';
import { environment } from 'src/environments/environment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

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
  roles: any[] = [];
  searchTerm = '';
  roleFilter = '';
  isLoading = false;
  baseUrl = environment.apiURL + '/';

  page = 1;
  pageSize = 15;
  total = 0;

  private searchSubject = new Subject<string>();

  constructor(
    private authService: AuthService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.authService.getRoles().subscribe({
      next: (roles: any) => { this.roles = roles; },
      error: () => this.toastr.error('خطا در بارگذاری نقش‌ها')
    });

    this.searchSubject.pipe(debounceTime(350), distinctUntilChanged()).subscribe(() => {
      this.page = 1;
      this.loadUsers();
    });

    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.authService.getUserProfile({
      search: this.searchTerm || undefined,
      role: this.roleFilter || undefined,
      page: this.page,
      pageSize: this.pageSize
    }).subscribe({
      next: (res: any) => {
        this.users = res.users;
        this.total = res.total;
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('خطا در بارگذاری کاربران');
        this.isLoading = false;
      }
    });
  }

  onSearchInput(): void {
    this.searchSubject.next(this.searchTerm);
  }

  onFilterChange(): void {
    this.page = 1;
    this.loadUsers();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.roleFilter = '';
    this.page = 1;
    this.loadUsers();
  }

  onPageChange(p: number): void {
    this.page = p;
    this.loadUsers();
  }

  get totalPages(): number {
    return Math.ceil(this.total / this.pageSize);
  }

  openRoleAssign(user: UserPermissionData): void {
    const dialogRef = this.dialog.open(RoleAssignDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: { user, roles: this.roles }
    });
    dialogRef.afterClosed().subscribe(changed => {
      if (changed) this.loadUsers();
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
}
