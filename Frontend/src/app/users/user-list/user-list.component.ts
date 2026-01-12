import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { UserEditDialogComponent } from '../user-edit-dialog/user-edit-dialog.component';
import { environment } from 'src/environments/environment';

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
}

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  users: UserData[] = [];
  filteredUsers: UserData[] = [];
  searchTerm: string = '';
  roleFilter: string = '';
  statusFilter: string = '';
  
  page: number = 1;
  count: number = 0;
  tableSize: number = 10;
  tableSizes: number[] = [10, 25, 50, 100];
  
  roles: any[] = [];
  baseUrl = environment.apiURL + '/';
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private rbacService: RbacService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.authService.getUserProfile().subscribe({
      next: (res: any) => {
        this.users = res;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading users:', err);
        this.toastr.error('خطا در بارگذاری لیست کاربران');
        this.isLoading = false;
      }
    });
  }

  loadRoles(): void {
    this.authService.getRoles().subscribe({
      next: (res: any) => {
        this.roles = res;
      },
      error: (err) => {
        console.error('Error loading roles:', err);
      }
    });
  }

  applyFilters(): void {
    let filtered = [...this.users];

    // Search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(user =>
        user.userName?.toLowerCase().includes(term) ||
        user.firstName?.toLowerCase().includes(term) ||
        user.lastName?.toLowerCase().includes(term) ||
        user.email?.toLowerCase().includes(term) ||
        user.phoneNumber?.includes(term)
      );
    }

    // Role filter
    if (this.roleFilter) {
      filtered = filtered.filter(user => user.role === this.roleFilter);
    }

    // Status filter
    if (this.statusFilter) {
      const isLocked = this.statusFilter === 'inactive';
      filtered = filtered.filter(user => user.isLocked === isLocked);
    }

    this.filteredUsers = filtered;
    this.count = filtered.length;
  }

  onSearch(): void {
    this.page = 1;
    this.applyFilters();
  }

  onRoleFilterChange(): void {
    this.page = 1;
    this.applyFilters();
  }

  onStatusFilterChange(): void {
    this.page = 1;
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.roleFilter = '';
    this.statusFilter = '';
    this.page = 1;
    this.applyFilters();
  }

  onTableDataChange(event: number): void {
    this.page = event;
  }

  onTableSizeChange(event: any): void {
    this.tableSize = event.target.value;
    this.page = 1;
  }

  onEdit(user: UserData): void {
    const dialogRef = this.dialog.open(UserEditDialogComponent, {
      width: '600px',
      maxWidth: '95vw',
      data: { user, roles: this.roles }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUsers();
      }
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
        error: (err) => {
          console.error('Error toggling user status:', err);
          this.toastr.error('خطا در تغییر وضعیت کاربر');
        }
      });
    }
  }

  getStatusClass(isLocked: boolean): string {
    return isLocked 
      ? 'bg-red-100 text-red-700' 
      : 'bg-green-100 text-green-700';
  }

  getStatusText(isLocked: boolean): string {
    return isLocked ? 'غیرفعال' : 'فعال';
  }

  getLicenseTypeText(licenseType: string): string {
    switch (licenseType) {
      case 'realEstate': return 'املاک';
      case 'carSale': return 'موتر فروشی';
      default: return '-';
    }
  }

  getUserPhoto(photoPath: string): string {
    if (photoPath) {
      return this.baseUrl + photoPath;
    }
    return 'assets/img/avatar.png';
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '-';
    const date = new Date(dateStr);
    return date.toLocaleDateString('fa-AF');
  }
}
