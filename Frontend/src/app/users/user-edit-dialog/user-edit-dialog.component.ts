import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { UserRoles } from 'src/app/shared/rbac.service';

@Component({
  selector: 'app-user-edit-dialog',
  templateUrl: './user-edit-dialog.component.html',
  styleUrls: ['./user-edit-dialog.component.scss']
})
export class UserEditDialogComponent implements OnInit {
  editForm!: FormGroup;
  isSubmitting = false;
  companies: any[] = [];
  showCompanySelect = false;
  showLicenseTypeSelect = false;

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
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadCompanies();
    this.updateFieldVisibility(this.data.user.role);
  }

  initForm(): void {
    this.editForm = this.fb.group({
      userId: [this.data.user.id],
      firstName: [this.data.user.firstName, Validators.required],
      lastName: [this.data.user.lastName, Validators.required],
      email: [this.data.user.email, [Validators.required, Validators.email]],
      phoneNumber: [this.data.user.phoneNumber],
      role: [this.data.user.role, Validators.required],
      companyId: [this.data.user.companyId || 0],
      licenseType: [this.data.user.licenseType || '']
    });
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

  onRoleChange(): void {
    const selectedRole = this.editForm.get('role')?.value;
    this.updateFieldVisibility(selectedRole);
  }

  updateFieldVisibility(role: string): void {
    const companyIdControl = this.editForm.get('companyId');
    const licenseTypeControl = this.editForm.get('licenseType');

    if (role === UserRoles.Admin || 
        role === UserRoles.Authority || 
        role === UserRoles.LicenseReviewer ||
        role === UserRoles.CompanyRegistrar) {
      // System-level roles don't need company
      companyIdControl?.setValue(0);
      licenseTypeControl?.setValue('');
      this.showCompanySelect = false;
      this.showLicenseTypeSelect = false;
    } else if (role === UserRoles.PropertyOperator || 
               role === UserRoles.VehicleOperator) {
      // Company operators need company and license type
      this.showCompanySelect = true;
      this.showLicenseTypeSelect = true;
      // Auto-set license type based on role
      if (role === UserRoles.PropertyOperator) {
        licenseTypeControl?.setValue('realEstate');
      } else {
        licenseTypeControl?.setValue('carSale');
      }
    } else {
      this.showCompanySelect = true;
      this.showLicenseTypeSelect = false;
    }
  }

  onSubmit(): void {
    if (this.editForm.invalid) {
      this.toastr.error('لطفاً تمام فیلدهای ضروری را پر کنید');
      return;
    }

    this.isSubmitting = true;
    const formData = this.editForm.value;

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
}
