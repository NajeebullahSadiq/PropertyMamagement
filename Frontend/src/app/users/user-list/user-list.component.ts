import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { UserEditDialogComponent } from '../user-edit-dialog/user-edit-dialog.component';
import { environment } from 'src/environments/environment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

export interface UserData {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  companyId: number;
  licenseType: string;
  isLocked: boolean;
  photoPath: string;
  createdAt: string;
  role: string;
  roleDari: string;
  isCompanyUser: boolean;
}

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  users: UserData[] = [];
  roles: any[] = [];
  systemRoles: any[] = [];
  companyRoles: any[] = [];

  // Active tab: 'system' | 'company'
  activeTab: 'system' | 'company' = 'system';

  // Filters
  searchTerm = '';
  roleFilter = '';
  statusFilter = '';

  // Pagination
  page = 1;
  pageSize = 15;
  total = 0;

  isLoading = false;
  baseUrl = environment.apiURL + '/';

  private searchSubject = new Subject<string>();

  constructor(
    private authService: AuthService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadRoles();
    // Debounce search input
    this.searchSubject.pipe(debounceTime(350), distinctUntilChanged()).subscribe(() => {
      this.page = 1;
      this.loadUsers();
    });
    this.loadUsers();
  }

  loadRoles(): void {
    this.authService.getRoles().subscribe({
      next: (res: any) => {
        this.roles = res;
        const companyRoleIds = ['PROPERTY_OPERATOR', 'VEHICLE_OPERATOR'];
        this.companyRoles = res.filter((r: any) => companyRoleIds.includes(r.id));
        this.systemRoles = res.filter((r: any) => !companyRoleIds.includes(r.id));
      }
    });
  }

  loadUsers(): void {
    this.isLoading = true;
    this.authService.getUserProfile({
      search: this.searchTerm || undefined,
      userType: this.activeTab,
      role: this.roleFilter || undefined,
      status: this.statusFilter || undefined,
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

  switchTab(tab: 'system' | 'company'): void {
    if (this.activeTab === tab) return;
    this.activeTab = tab;
    this.roleFilter = '';
    this.page = 1;
    this.loadUsers();
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
    this.statusFilter = '';
    this.page = 1;
    this.loadUsers();
  }

  onPageChange(p: number): void {
    this.page = p;
    this.loadUsers();
  }

  onEdit(user: UserData): void {
    const dialogRef = this.dialog.open(UserEditDialogComponent, {
      width: '600px',
      maxWidth: '95vw',
      data: { user, roles: this.roles }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadUsers();
    });
  }

  onToggleStatus(user: UserData): void {
    const newStatus = !user.isLocked;
    const action = newStatus ? 'غیرفعال' : 'فعال';
    if (confirm(`آیا مطمئن هستید که می‌خواهید حساب "${user.userName}" را ${action} کنید؟`)) {
      this.authService.lockUser(user.userName, newStatus).subscribe({
        next: () => {
          this.toastr.success(`حساب کاربری ${action} شد`);
          this.loadUsers();
        },
        error: () => this.toastr.error('خطا در تغییر وضعیت کاربر')
      });
    }
  }

  get currentRoles(): any[] {
    return this.activeTab === 'company' ? this.companyRoles : this.systemRoles;
  }

  get totalPages(): number {
    return Math.ceil(this.total / this.pageSize);
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

  getLicenseTypeText(licenseType: string): string {
    switch (licenseType) {
      case 'realEstate': return 'املاک';
      case 'carSale': return 'موتر فروشی';
      default: return '—';
    }
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('fa-AF');
  }

  getRoleColor(role: string): string {
    const colors: Record<string, string> = {
      'ADMIN': 'bg-red-100 text-red-700',
      'AUTHORITY': 'bg-purple-100 text-purple-700',
      'COMPANY_REGISTRAR': 'bg-blue-100 text-blue-700',
      'LICENSE_REVIEWER': 'bg-cyan-100 text-cyan-700',
      'PROPERTY_OPERATOR': 'bg-green-100 text-green-700',
      'VEHICLE_OPERATOR': 'bg-teal-100 text-teal-700',
      'LICENSE_APPLICATION_MANAGER': 'bg-orange-100 text-orange-700',
      'ACTIVITY_MONITORING_MANAGER': 'bg-yellow-100 text-yellow-700',
      'SECURITIES_MANAGER': 'bg-indigo-100 text-indigo-700',
      'SECURITIES_ENTRY_MANAGER': 'bg-violet-100 text-violet-700',
      'PETITION_WRITER_SECURITIES_ENTRY_MANAGER': 'bg-pink-100 text-pink-700',
      'PETITION_WRITER_LICENSE_MANAGER': 'bg-rose-100 text-rose-700',
    };
    return colors[role] || 'bg-gray-100 text-gray-700';
  }
}
