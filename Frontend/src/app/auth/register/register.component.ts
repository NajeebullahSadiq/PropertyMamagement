import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { ProfileImageCropperComponent } from 'src/app/shared/profile-image-cropper/profile-image-cropper.component';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  filePath: string = 'assets/img/avatar.png';
  baseUrl = environment.apiURL + '/';
  imageName: string = '';
  companylist: any;
  servicesDetails: any;
  roles: any;
  userDetails: any;
  userRole: any;
  showCompanySelect = false;
  showLicenseTypeSelect = false;
  showProvinceSelect = false; // Show province field for COMPANY_REGISTRAR

  // License search
  showLicenseSearch = false;
  searchLicenseNumber: string = '';
  searchProvinceId: number | null = null;
  searchResults: any[] = [];
  isSearching: boolean = false;
  provinces: any = [];

  // UI State
  successMessage: string = '';
  errorMessage: string = '';
  isSubmitting: boolean = false;
  showPassword: boolean = false;
  showConfirmPassword: boolean = false;

  // License types for company operators
  licenseTypes = [
    { id: 'realEstate', name: 'Real Estate', dari: 'املاک' },
    { id: 'carSale', name: 'Car Sale', dari: 'موټر فروشی' }
  ];

  @ViewChild('imageCropper') imageCropper!: ProfileImageCropperComponent;
  @ViewChild('secondDiv', { static: false }) secondDivRef!: ElementRef;

  constructor(
    public service: AuthService,
    private rbacService: RbacService,
    private companyService: CompnaydetailService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.service.formModel.reset();
    this.service.getCompanies().subscribe(res => {
      this.companylist = res;
    });
    this.service.getRoles().subscribe(res => {
      this.roles = res;
    });
    
    // Load provinces for license search
    this.companyService.getProvinces().subscribe(res => {
      this.provinces = res;
    });
    
    // UserProfile
    this.service.getUserProfile().subscribe(
      res => {
        this.userDetails = res;
      },
      err => {
        console.log(err);
      },
    );
    
    this.userRole = this.rbacService.getCurrentRole();
  }

  onSubmit() {
    // Clear previous messages
    this.successMessage = '';
    this.errorMessage = '';
    this.isSubmitting = true;

    this.service.photoPath = this.imageName;

    this.service.register().subscribe(
      (res: any) => {
        this.isSubmitting = false;
        if (res.succeeded) {
          this.service.formModel.reset();
          this.successMessage = 'کاربر جدید موفقانه ایجاد گردید!';
          this.toastr.success('New user created!', 'معلومات موفقانه ثبت سیستم گردید');
          this.callChildMethod();
          this.filePath = 'assets/img/avatar.png';
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
          // Refresh user list
          this.service.getUserProfile().subscribe(res => {
            this.userDetails = res;
          });
          
          // Auto-hide success message after 5 seconds
          setTimeout(() => {
            this.successMessage = '';
          }, 5000);
        } else {
          res.errors.forEach((element: { code: any; description: string | undefined; }) => {
            switch (element.code) {
              case 'DuplicateUserName':
                this.errorMessage = 'این نام کاربری قبلاً استفاده شده است. لطفاً نام کاربری دیگری انتخاب کنید.';
                this.toastr.error('Username is already taken', 'Registration failed.');
                break;
              case 'DuplicateEmail':
                this.errorMessage = 'این ایمیل آدرس قبلاً ثبت شده است.';
                this.toastr.error('Email is already registered', 'Registration failed.');
                break;
              case 'PasswordTooShort':
                this.errorMessage = 'پسورد باید حداقل ۶ کرکتر باشد.';
                this.toastr.error('Password is too short', 'Registration failed.');
                break;
              case 'PasswordRequiresNonAlphanumeric':
                this.errorMessage = 'پسورد باید شامل کرکتر های خاص باشد.';
                this.toastr.error('Password requires special characters', 'Registration failed.');
                break;
              case 'PasswordRequiresDigit':
                this.errorMessage = 'پسورد باید شامل اعداد باشد.';
                this.toastr.error('Password requires digits', 'Registration failed.');
                break;
              case 'PasswordRequiresUpper':
                this.errorMessage = 'پسورد باید شامل حروف بزرگ باشد.';
                this.toastr.error('Password requires uppercase letters', 'Registration failed.');
                break;
              default:
                this.errorMessage = element.description || 'خطا در ثبت کاربر. لطفاً دوباره تلاش کنید.';
                this.toastr.error(element.description, 'Registration failed.');
                break;
            }
          });
        }
      },
      err => {
        this.isSubmitting = false;
        console.log(err);
        this.errorMessage = 'خطا در اتصال به سرور. لطفاً اتصال انترنت خود را بررسی کنید.';
        this.toastr.error('Connection error', 'خطا در اتصال');
      }
    );
  }

  uploadFinished = (event: string) => {
    this.imageName = event;
    this.filePath = this.baseUrl + this.imageName;
  }

  profilePreviewChanged = (localObjectUrl: string) => {
    // This is called when user crops an image but before upload
    // The localObjectUrl is a temporary blob URL for preview
    if (localObjectUrl) {
      this.filePath = localObjectUrl;
    }
  }

  callChildMethod() {
    if (this.imageCropper) {
      this.imageCropper.reset();
    }
  }

  resetForm() {
    this.service.formModel.reset();
    this.filePath = 'assets/img/avatar.png';
    this.imageName = '';
    this.showCompanySelect = false;
    this.showLicenseTypeSelect = false;
    this.successMessage = '';
    this.errorMessage = '';
    this.showPassword = false;
    this.showConfirmPassword = false;
    if (this.imageCropper) {
      this.imageCropper.reset();
    }
  }

  onPropertyTypeChange() {
    if (this.service.formModel) {
      const roleControl = this.service.formModel.get('Role');
      const companyIdControl = this.service.formModel.get('CompanyId');
      const licenseTypeControl = this.service.formModel.get('LicenseType');
      const provinceIdControl = this.service.formModel.get('ProvinceId');

      if (roleControl && companyIdControl) {
        const selectedRole = roleControl.value;

        // Determine which fields to show based on role
        if (selectedRole === UserRoles.Admin || 
            selectedRole === UserRoles.Authority || 
            selectedRole === UserRoles.LicenseReviewer) {
          // System-level roles don't need company or province
          companyIdControl.setValue(0);
          licenseTypeControl?.setValue('');
          provinceIdControl?.setValue(null);
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
          this.showProvinceSelect = false;
        } else if (selectedRole === UserRoles.CompanyRegistrar) {
          // Company registrar needs province but not company association
          companyIdControl.setValue(0);
          licenseTypeControl?.setValue('');
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
          this.showProvinceSelect = true;
          // Make province required for COMPANY_REGISTRAR
          provinceIdControl?.setValidators([Validators.required]);
          provinceIdControl?.updateValueAndValidity();
        } else if (selectedRole === UserRoles.PropertyOperator || 
                   selectedRole === UserRoles.VehicleOperator) {
          // Company operators need company and license type
          this.showCompanySelect = true;
          this.showLicenseTypeSelect = true;
          this.showProvinceSelect = false;
          provinceIdControl?.setValue(null);
          provinceIdControl?.clearValidators();
          provinceIdControl?.updateValueAndValidity();
          // Auto-set license type based on role
          if (selectedRole === UserRoles.PropertyOperator) {
            licenseTypeControl?.setValue('realEstate');
          } else {
            licenseTypeControl?.setValue('carSale');
          }
        } else {
          this.showCompanySelect = true;
          this.showLicenseTypeSelect = false;
          this.showProvinceSelect = false;
          provinceIdControl?.setValue(null);
          provinceIdControl?.clearValidators();
          provinceIdControl?.updateValueAndValidity();
        }
      }
    }
  }

  getRoleDari(role: string): string {
    return this.rbacService.getRoleDari(role);
  }

  searchCompanyByLicense() {
    if (!this.searchLicenseNumber || this.searchLicenseNumber.trim() === '') {
      this.toastr.warning('لطفاً شماره جواز را وارد کنید', 'هشدار');
      return;
    }

    this.isSearching = true;
    this.searchResults = [];

    this.companyService.searchCompanyByLicense(
      this.searchLicenseNumber.trim(),
      this.searchProvinceId || undefined
    ).subscribe(
      (results: any[]) => {
        this.isSearching = false;
        this.searchResults = results;
        
        if (results.length === 0) {
          this.toastr.info('هیچ رهنمای با این شماره جواز یافت نشد', 'نتیجه جستجو');
        } else if (results.length === 1) {
          // Auto-select if only one result
          this.selectCompanyFromSearch(results[0]);
          this.toastr.success('رهنما یافت شد و انتخاب گردید', 'موفق');
        } else {
          this.toastr.info(`${results.length} رهنما یافت شد`, 'نتیجه جستجو');
        }
      },
      (error) => {
        this.isSearching = false;
        console.error('Search error:', error);
        if (error.status === 404) {
          this.toastr.info('هیچ رهنمای با این شماره جواز یافت نشد', 'نتیجه جستجو');
        } else {
          this.toastr.error('خطا در جستجوی رهنما', 'خطا');
        }
      }
    );
  }

  selectCompanyFromSearch(result: any) {
    const companyIdControl = this.service.formModel.get('CompanyId');
    const licenseTypeControl = this.service.formModel.get('LicenseType');
    
    if (companyIdControl && result.companyId) {
      companyIdControl.setValue(result.companyId);
    }
    
    if (licenseTypeControl && result.licenseType) {
      licenseTypeControl.setValue(result.licenseType);
    }
    
    // Clear search
    this.searchResults = [];
    this.searchLicenseNumber = '';
    this.searchProvinceId = null;
    this.showLicenseSearch = false;
  }

  toggleLicenseSearch() {
    this.showLicenseSearch = !this.showLicenseSearch;
    if (!this.showLicenseSearch) {
      this.searchResults = [];
      this.searchLicenseNumber = '';
      this.searchProvinceId = null;
    }
  }

  clearSearch() {
    this.searchResults = [];
    this.searchLicenseNumber = '';
    this.searchProvinceId = null;
  }
}
