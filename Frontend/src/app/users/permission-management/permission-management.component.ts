import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { RoleAssignDialogComponent } from './role-assign-dialog/role-assign-dialog.component';
import { environment } from 'src/environments/environment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';

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
export class PermissionManagementComponent extends BaseComponent implements OnInit {
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
  ) {
    super();
  }

  ngOnInit(): void {
    this.authService.getRoles().subscribe({
      next: (roles: any) => { this.roles = roles; },
      error: () => this.toastr.error('خطا در بارگذاری نقش‌ها')
    });

    this.searchSubject.pipe(debounceTime(350), distinctUntilChanged(), takeUntil(this.destroy$)).subscribe(() => {
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
    return this.constructImageUrl(photoPath);
  }

  private constructImageUrl(path: string | null | undefined): string {
    if (!path) return 'assets/img/avatar.png';
    
    // If path already starts with http/https or is a blob URL, return as is
    if (path.startsWith('http://') || path.startsWith('https://') || path.startsWith('blob:')) {
      return path;
    }
    
    // If it's an assets path, return as is
    if (path.startsWith('assets/')) {
      return path;
    }
    
    // If path starts with /api/, remove it to avoid duplication
    let cleanPath = path;
    if (cleanPath.startsWith('/api/')) {
      cleanPath = cleanPath.substring(5); // Remove '/api/'
    } else if (cleanPath.startsWith('api/')) {
      cleanPath = cleanPath.substring(4); // Remove 'api/'
    }
    
    // If path starts with Resources/, use Upload/view endpoint
    if (cleanPath.startsWith('Resources/') || cleanPath.startsWith('/Resources/')) {
      const resourcePath = cleanPath.startsWith('/') ? cleanPath.substring(1) : cleanPath;
      return `${this.baseUrl}Upload/view/${resourcePath}`;
    }
    
    // Otherwise, use Upload/view endpoint
    return `${this.baseUrl}Upload/view/${cleanPath}`;
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
