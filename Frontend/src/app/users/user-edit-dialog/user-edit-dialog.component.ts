import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { UserRoles } from 'src/app/shared/rbac.service';

@Component({
  selector: 'app-user-edit-dialog',
  templateUrl: './user-edit-dialog.component.html',
  styleUrls: ['./user-edit-dialog.component.scss']
})
export class UserEditDialogComponent extends BaseComponent implements OnInit {
  editForm!: FormGroup;
  isSubmitting = false;
  companies: any[] = [];
  showCompanySelect = false;
  showLicenseTypeSelect = false;
  showLicenseSearch = false;
  showProvinceSelect = false;
  searchLicenseNumber = '';
  searchResults: any[] = [];
  isSearching = false;
  selectedCompanyInfo: any = null;
  provinces: any[] = [];

  licenseTypes = [
    { id: 'realEstate', name: 'Real Estate', dari: 'املاک' },
    { id: 'carSale', name: 'Car Sale', dari: 'موتر فروشی' }
  ];

  private readonly baseUrl = environment.apiUrl;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private toastr: ToastrService,
    public dialogRef: MatDialogRef<UserEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: any; roles: any[] }
  ) {
    super();
  }

  ngOnInit(): void {
    this.initForm();
    this.loadCompanies();
    this.loadProvinces();
    this.updateFieldVisibility(this.data.user.role);
  }

  initForm(): void {
    const userProvinceId = this.data.user.userProvinceId || this.data.user.provinceId || null;
    this.editForm = this.fb.group({
      userId: [this.data.user.id],
      firstName: [this.data.user.firstName, Validators.required],
      lastName: [this.data.user.lastName, Validators.required],
      email: [this.data.user.email, [Validators.required, Validators.email]],
      phoneNumber: [this.data.user.phoneNumber],
      role: [this.data.user.role, Validators.required],
      companyId: [this.data.user.companyId || 0],
      licenseNumber: [this.data.user.licenseNumber || ''],
      licenseType: [this.data.user.licenseType || ''],
      provinceId: [userProvinceId]
    });

    if (this.data.user.licenseNumber || this.data.user.companyTitle) {
      this.selectedCompanyInfo = {
        licenseNumber: this.data.user.licenseNumber,
        companyTitle: this.data.user.companyTitle,
        activityLocation: this.data.user.activityLocation,
        companyId: this.data.user.companyId,
        licenseType: this.data.user.licenseType
      };
      this.searchLicenseNumber = this.data.user.licenseNumber || '';
    }
  }

  loadCompanies(): void {
    this.http.get(`${this.baseUrl}/CompanyDetails/getCompanies`).subscribe({
      next: (res: any) => {
        this.companies = res;
      },
      error: (err) => {
        console.error('Error loading companies:', err);
      }
    });
  }

  loadProvinces(): void {
    this.http.get(`${this.baseUrl}/DistrictManagement/provinces`).subscribe({
      next: (res: any) => {
        this.provinces = res || [];
      },
      error: (err) => {
        console.error('Error loading provinces:', err);
      }
    });
  }

  onRoleChange(): void {
    const selectedRole = this.editForm.get('role')?.value;
    this.updateFieldVisibility(selectedRole);
  }

  updateFieldVisibility(role: string): void {
    const companyIdControl = this.editForm.get('companyId');
    const licenseTypeControl = this.editForm.get('licenseType');
    const licenseNumberControl = this.editForm.get('licenseNumber');
    const emailControl = this.editForm.get('email');
    const provinceIdControl = this.editForm.get('provinceId');

    if (role === UserRoles.Admin || 
        role === UserRoles.Authority || 
        role === UserRoles.LicenseReviewer ||
        role === UserRoles.LicenseApplicationManager ||
        role === UserRoles.ActivityMonitoringManager ||
        role === UserRoles.SecuritiesManager ||
        role === UserRoles.SecuritiesEntryManager ||
        role === UserRoles.PetitionWriterSecuritiesEntryManager) {
      emailControl?.setValidators([Validators.required, Validators.email]);
      emailControl?.updateValueAndValidity();
      // System-level roles don't need company or province
      companyIdControl?.setValue(0);
      licenseTypeControl?.setValue('');
      licenseNumberControl?.setValue('');
      licenseNumberControl?.clearValidators();
      licenseNumberControl?.updateValueAndValidity();
      this.selectedCompanyInfo = null;
      this.showCompanySelect = false;
      this.showLicenseTypeSelect = false;
      this.showLicenseSearch = false;
      this.showProvinceSelect = false;
      provinceIdControl?.clearValidators();
      provinceIdControl?.updateValueAndValidity();
    } else if (role === UserRoles.CompanyRegistrar) {
      emailControl?.setValidators([Validators.required, Validators.email]);
      emailControl?.updateValueAndValidity();
      // Company registrar needs province but not company
      companyIdControl?.setValue(0);
      licenseTypeControl?.setValue('');
      licenseNumberControl?.setValue('');
      licenseNumberControl?.clearValidators();
      licenseNumberControl?.updateValueAndValidity();
      this.selectedCompanyInfo = null;
      this.showCompanySelect = false;
      this.showLicenseTypeSelect = false;
      this.showLicenseSearch = false;
      this.showProvinceSelect = true;
      provinceIdControl?.setValidators([Validators.required]);
      provinceIdControl?.updateValueAndValidity();
    } else if (role === UserRoles.PropertyOperator || 
               role === UserRoles.VehicleOperator) {
      emailControl?.setValidators([Validators.email]);
      emailControl?.updateValueAndValidity();
      // Company operators need company, license type, and province
      this.showCompanySelect = true;
      this.showLicenseTypeSelect = true;
      this.showLicenseSearch = true;
      this.showProvinceSelect = true;
      companyIdControl?.setValue(this.selectedCompanyInfo?.companyId || 0);
      licenseNumberControl?.setValidators([Validators.required]);
      licenseNumberControl?.updateValueAndValidity();
      provinceIdControl?.setValidators([Validators.required]);
      provinceIdControl?.updateValueAndValidity();
      // Auto-set license type based on role
      if (role === UserRoles.PropertyOperator) {
        licenseTypeControl?.setValue('realEstate');
      } else {
        licenseTypeControl?.setValue('carSale');
      }
    } else {
      emailControl?.setValidators([Validators.required, Validators.email]);
      emailControl?.updateValueAndValidity();
      this.showCompanySelect = true;
      this.showLicenseTypeSelect = false;
      this.showLicenseSearch = true;
      this.showProvinceSelect = true;
      licenseNumberControl?.setValidators([Validators.required]);
      licenseNumberControl?.updateValueAndValidity();
      provinceIdControl?.setValidators([Validators.required]);
      provinceIdControl?.updateValueAndValidity();
    }
  }

  onSubmit(): void {
    if (this.editForm.invalid) {
      this.toastr.error('لطفاً تمام فیلدهای ضروری را پر کنید');
      return;
    }

    if (this.showCompanySelect && (!this.editForm.get('licenseNumber')?.value || !this.editForm.get('companyId')?.value)) {
      this.toastr.error('لطفاً رهنما را بر اساس شماره جواز انتخاب کنید.');
      return;
    }

    this.isSubmitting = true;
    const formData = {
      ...this.editForm.value,
      email: this.editForm.value.email?.trim() || null
    };

    this.http.post(`${this.baseUrl}/ApplicationUser/UpdateUser`, formData).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        this.toastr.success(res.message || 'معلومات کاربر با موفقیت تغییر کرد');
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting = false;
        console.error('Error updating user:', err);
        this.toastr.error(err.error?.message || 'خطا در تغییر معلومات کاربر');
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  getRoleDari(role: string): string {
    const roleInfo = this.data.roles.find(r => r.id === role);
    return roleInfo?.dari || role;
  }

  searchCompanyByLicense(): void {
    if (!this.searchLicenseNumber || this.searchLicenseNumber.trim() === '') {
      this.toastr.warning('لطفاً شماره جواز را وارد کنید');
      return;
    }

    this.isSearching = true;
    this.searchResults = [];

    const excludeUserId = this.data.user?.id ? `&excludeUserId=${encodeURIComponent(this.data.user.id)}` : '';
    this.http.get<any[]>(`${this.baseUrl}/CompanyDetails/searchByLicense?licenseNumber=${encodeURIComponent(this.searchLicenseNumber.trim())}${excludeUserId}`).subscribe({
      next: (results) => {
        this.isSearching = false;
        this.searchResults = results || [];

        if (this.searchResults.length === 0) {
          this.toastr.info('هیچ رهنمای با این شماره جواز یافت نشد');
        } else if (this.searchResults.length === 1) {
          if (this.searchResults[0].hasExistingUser) {
            this.toastr.error('این دفتر رهنما قبلاً دارای کاربر دیگری است. هر دفتر فقط یک کاربر می‌تواند داشته باشد.', 'تغییر مجاز نیست');
          } else {
            this.selectCompanyFromSearch(this.searchResults[0]);
          }
        }
      },
      error: (err) => {
        this.isSearching = false;
        this.searchResults = [];
        this.toastr.error(err.error?.message || 'خطا در جستجوی رهنما');
      }
    });
  }

  selectCompanyFromSearch(result: any): void {
    if (result?.hasExistingUser) {
      this.toastr.error('این دفتر رهنما قبلاً دارای کاربر دیگری است. هر دفتر فقط یک کاربر می‌تواند داشته باشد.', 'تغییر مجاز نیست');
      return;
    }

    this.selectedCompanyInfo = result;

    const patch: Record<string, unknown> = {
      companyId: result.companyId || 0,
      licenseType: result.licenseType || this.editForm.get('licenseType')?.value,
      licenseNumber: result.licenseNumber || '',
      provinceId: result.provinceId || null
    };

    if (result.licenseType === 'carSale') {
      patch['role'] = UserRoles.VehicleOperator;
    } else if (result.licenseType === 'realEstate') {
      patch['role'] = UserRoles.PropertyOperator;
    }

    this.editForm.patchValue(patch);
    this.updateFieldVisibility(this.editForm.get('role')?.value);
    this.searchLicenseNumber = result.licenseNumber || '';
    this.searchResults = [];
    this.showLicenseSearch = false;
  }
}
