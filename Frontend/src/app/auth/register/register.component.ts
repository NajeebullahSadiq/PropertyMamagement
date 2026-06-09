import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
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
export class RegisterComponent extends BaseComponent implements OnInit {
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
  companyFieldsLocked = false;

  // License search
  showLicenseSearch = false;
  searchLicenseNumber: string = '';
  searchProvinceId: number | null = null;
  searchResults: any[] = [];
  isSearching: boolean = false;
  selectedCompanyInfo: any = null;
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
  ) {
    super();
  }

  get isCompanyUserMode(): boolean {
    const role = this.service.formModel.get('Role')?.value;
    return role === UserRoles.PropertyOperator || role === UserRoles.VehicleOperator;
  }

  ngOnInit(): void {
    this.service.formModel.reset();
    this.updateFormValidatorsForRole('');
    this.service.getCompanies().pipe(takeUntil(this.destroy$)).subscribe(res => {
      this.companylist = res;
    });
    this.service.getRoles().pipe(takeUntil(this.destroy$)).subscribe(res => {
      this.roles = res;
    });
    
    // Load provinces for license search
    this.companyService.getProvinces().pipe(takeUntil(this.destroy$)).subscribe(res => {
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

    if (this.showCompanySelect && (!this.service.formModel.get('LicenseNumber')?.value || !this.service.formModel.get('CompanyId')?.value)) {
      this.isSubmitting = false;
      this.errorMessage = 'لطفاً رهنما را بر اساس شماره جواز انتخاب کنید.';
      this.toastr.error(this.errorMessage);
      return;
    }

    if (this.selectedCompanyInfo?.hasExistingUser) {
      this.isSubmitting = false;
      this.errorMessage = 'این دفتر رهنما قبلاً دارای کاربر است. هر دفتر فقط می‌تواند یک کاربر داشته باشد.';
      this.toastr.error(this.errorMessage, 'ثبت نام مجاز نیست');
      return;
    }

    this.service.register().subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        if (res.succeeded) {
          this.service.formModel.reset();
          this.successMessage = 'کاربر جدید موفقانه ایجاد گردید!';
          this.toastr.success('New user created!', 'معلومات موفقانه ثبت سیستم گردید');
          this.callChildMethod();
          this.filePath = 'assets/img/avatar.png';
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
          this.clearCompanyAutofill();
          this.service.getUserProfile().subscribe(profileRes => {
            this.userDetails = profileRes;
          });

          setTimeout(() => {
            this.successMessage = '';
          }, 5000);
        } else if (res.errors) {
          this.handleRegistrationErrors(res.errors);
        }
      },
      error: (err) => {
        this.isSubmitting = false;
        this.handleRegistrationHttpError(err);
      }
    });
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
    this.selectedCompanyInfo = null;
    this.companyFieldsLocked = false;
    this.successMessage = '';
    this.errorMessage = '';
    this.showPassword = false;
    this.showConfirmPassword = false;
    this.updateFormValidatorsForRole('');
    if (this.imageCropper) {
      this.imageCropper.reset();
    }
  }

  private updateFormValidatorsForRole(role: string | null | undefined): void {
    role = role ?? '';
    const lastNameControl = this.service.formModel.get('LastName');
    const emailControl = this.service.formModel.get('Email');

    if (role === UserRoles.PropertyOperator || role === UserRoles.VehicleOperator) {
      lastNameControl?.clearValidators();
      emailControl?.setValidators([Validators.email]);
    } else {
      lastNameControl?.setValidators([Validators.required]);
      emailControl?.setValidators([Validators.email]);
    }

    lastNameControl?.updateValueAndValidity();
    emailControl?.updateValueAndValidity();
  }

  private clearCompanyAutofill(): void {
    this.companyFieldsLocked = false;
    this.selectedCompanyInfo = null;
    this.filePath = 'assets/img/avatar.png';
    this.imageName = '';
    this.service.photoPath = '';

    const firstNameControl = this.service.formModel.get('FirstName');
    const lastNameControl = this.service.formModel.get('LastName');
    const phoneControl = this.service.formModel.get('PhoneNumber');
    const provinceIdControl = this.service.formModel.get('ProvinceId');

    firstNameControl?.setValue('');
    lastNameControl?.setValue('');
    phoneControl?.setValue('');
    provinceIdControl?.setValue(null);

    if (this.imageCropper) {
      this.imageCropper.reset();
    }
  }

  private populateFromCompany(result: any): void {
    const firstNameControl = this.service.formModel.get('FirstName');
    const lastNameControl = this.service.formModel.get('LastName');
    const phoneControl = this.service.formModel.get('PhoneNumber');
    const provinceIdControl = this.service.formModel.get('ProvinceId');

    const ownerName = (result.ownerName || result.OwnerName || '').trim();
    const ownerFatherName = (result.ownerFatherName || result.OwnerFatherName || '').trim();
    const ownerGrandFatherName = (result.ownerGrandFatherName || result.OwnerGrandFatherName || '').trim();
    const ownerPhone = (result.ownerPhoneNumber || result.OwnerPhoneNumber || '').trim();
    const ownerPhoto = (result.ownerPhotoPath || result.OwnerPhotoPath || '').trim();

    if (firstNameControl) {
      firstNameControl.setValue(ownerName);
    }

    if (lastNameControl) {
      const defaultLastName = ownerFatherName || ownerGrandFatherName;
      lastNameControl.setValue(defaultLastName);
    }

    if (phoneControl && ownerPhone) {
      phoneControl.setValue(ownerPhone);
    }

    if (provinceIdControl && result.provinceId) {
      provinceIdControl.setValue(result.provinceId);
    }

    if (ownerPhoto) {
      this.applyOwnerPhoto(ownerPhoto);
    }

    this.companyFieldsLocked = true;
  }

  private applyOwnerPhoto(photoPath: string): void {
    this.imageName = photoPath;
    this.service.photoPath = photoPath;
    this.filePath = this.getOwnerPhotoUrl(photoPath);
    if (this.imageCropper) {
      this.imageCropper.setExistingImage(this.filePath);
    }
  }

  private applyOwnerContact(owner: any): void {
    const phone = (owner?.phoneNumber || owner?.PhoneNumber || owner?.whatsAppNumber || owner?.WhatsAppNumber || '').trim();
    const photo = (owner?.pothoPath || owner?.PothoPath || '').trim();

    if (phone) {
      this.service.formModel.get('PhoneNumber')?.setValue(phone);
    }

    if (photo) {
      this.applyOwnerPhoto(photo);
    }

    if (this.selectedCompanyInfo) {
      this.selectedCompanyInfo = {
        ...this.selectedCompanyInfo,
        ownerPhoneNumber: phone || this.selectedCompanyInfo.ownerPhoneNumber,
        ownerPhotoPath: photo || this.selectedCompanyInfo.ownerPhotoPath
      };
    }
  }

  private normalizeResponseArray(data: any): any[] {
    if (!data) {
      return [];
    }
    if (Array.isArray(data)) {
      return data;
    }
    if (data.$values && Array.isArray(data.$values)) {
      return data.$values;
    }
    return [data];
  }

  private loadOwnerContactForCompany(companyId: number): void {
    this.companyService.getOwnerById(companyId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (owners) => {
          const ownerList = this.normalizeResponseArray(owners);
          if (ownerList.length > 0) {
            this.applyOwnerContact(ownerList[0]);
          }
        },
        error: (error) => {
          console.error('Failed to load company owner contact info:', error);
        }
      });
  }

  getOwnerPhotoUrl(path: string | null | undefined): string {
    if (!path) return 'assets/img/avatar.png';
    if (path.startsWith('http://') || path.startsWith('https://') || path.startsWith('blob:') || path.startsWith('assets/')) {
      return path;
    }
    const cleanPath = path.startsWith('/') ? path.substring(1) : path;
    if (cleanPath.startsWith('Resources/')) {
      return `${environment.apiURL}/Upload/view/${cleanPath}`;
    }
    return `${environment.apiURL}/Upload/view/${cleanPath}`;
  }

  getLicenseTypeLabel(licenseType: string): string {
    if (licenseType === 'carSale') return 'موتر فروشی';
    if (licenseType === 'realEstate') return 'املاک';
    return licenseType || '-';
  }

  onPropertyTypeChange() {
    if (this.service.formModel) {
      const roleControl = this.service.formModel.get('Role');
      const companyIdControl = this.service.formModel.get('CompanyId');
      const licenseTypeControl = this.service.formModel.get('LicenseType');
      const licenseNumberControl = this.service.formModel.get('LicenseNumber');
      const provinceIdControl = this.service.formModel.get('ProvinceId');

      if (roleControl && companyIdControl) {
        const selectedRole = roleControl.value;
        this.updateFormValidatorsForRole(selectedRole);

        // Determine which fields to show based on role
        if (selectedRole === UserRoles.Admin || 
            selectedRole === UserRoles.Authority || 
            selectedRole === UserRoles.LicenseReviewer ||
            selectedRole === UserRoles.LicenseApplicationManager ||
            selectedRole === UserRoles.ActivityMonitoringManager ||
            selectedRole === UserRoles.SecuritiesManager ||
            selectedRole === UserRoles.SecuritiesEntryManager ||
            selectedRole === UserRoles.PetitionWriterSecuritiesEntryManager) {
          // System-level roles don't need company or province
          companyIdControl.setValue(0);
          licenseTypeControl?.setValue('');
          licenseNumberControl?.setValue('');
          licenseNumberControl?.clearValidators();
          licenseNumberControl?.updateValueAndValidity();
          provinceIdControl?.setValue(null);
          this.clearCompanyAutofill();
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
          this.showProvinceSelect = false;
        } else if (selectedRole === UserRoles.CompanyRegistrar || selectedRole === UserRoles.PetitionWriterLicenseManager) {
          // Company registrar and petition writer license manager need province but not company association
          companyIdControl.setValue(0);
          licenseTypeControl?.setValue('');
          licenseNumberControl?.setValue('');
          licenseNumberControl?.clearValidators();
          licenseNumberControl?.updateValueAndValidity();
          this.clearCompanyAutofill();
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
          this.showProvinceSelect = true;
          // Make province required
          provinceIdControl?.setValidators([Validators.required]);
          provinceIdControl?.updateValueAndValidity();
        } else if (selectedRole === UserRoles.PropertyOperator || 
                   selectedRole === UserRoles.VehicleOperator) {
          // Company operators need company and license type
          this.showCompanySelect = true;
          this.showLicenseTypeSelect = true;
          this.showLicenseSearch = true;
          this.showProvinceSelect = false;
          if (!this.selectedCompanyInfo) {
            companyIdControl.setValue(0);
          }
          licenseNumberControl?.setValue('');
          licenseNumberControl?.setValidators([Validators.required]);
          licenseNumberControl?.updateValueAndValidity();
          this.clearCompanyAutofill();
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
          this.showLicenseSearch = true;
          this.showLicenseTypeSelect = false;
          licenseNumberControl?.setValidators([Validators.required]);
          licenseNumberControl?.updateValueAndValidity();
          this.showProvinceSelect = false;
          provinceIdControl?.setValue(null);
          provinceIdControl?.clearValidators();
          provinceIdControl?.updateValueAndValidity();
        }
      }
    }
  }

  onCompanyChange(selectedCompany: any) {
    // When company is selected, automatically populate provinceId
    if (selectedCompany && selectedCompany.provinceId) {
      const provinceIdControl = this.service.formModel.get('ProvinceId');
      if (provinceIdControl) {
        provinceIdControl.setValue(selectedCompany.provinceId);
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
          if (results[0].hasExistingUser) {
            this.toastr.error('این دفتر رهنما قبلاً دارای کاربر است. هر دفتر فقط یک کاربر می‌تواند داشته باشد.', 'ثبت نام مجاز نیست');
          } else {
            this.selectCompanyFromSearch(results[0]);
            this.toastr.success('رهنما یافت شد و انتخاب گردید', 'موفق');
          }
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
    if (result?.hasExistingUser) {
      this.toastr.error('این دفتر رهنما قبلاً دارای کاربر است. هر دفتر فقط یک کاربر می‌تواند داشته باشد.', 'ثبت نام مجاز نیست');
      return;
    }

    const companyIdControl = this.service.formModel.get('CompanyId');
    const licenseTypeControl = this.service.formModel.get('LicenseType');
    const licenseNumberControl = this.service.formModel.get('LicenseNumber');
    const provinceIdControl = this.service.formModel.get('ProvinceId');
    const roleControl = this.service.formModel.get('Role');
    
    if (companyIdControl && result.companyId) {
      companyIdControl.setValue(result.companyId);
    }
    
    if (licenseTypeControl && result.licenseType) {
      licenseTypeControl.setValue(result.licenseType);
    }

    // Keep role aligned with the selected license type
    if (roleControl && result.licenseType) {
      if (result.licenseType === 'carSale') {
        roleControl.setValue(UserRoles.VehicleOperator);
      } else if (result.licenseType === 'realEstate') {
        roleControl.setValue(UserRoles.PropertyOperator);
      }
    }

    if (licenseNumberControl && result.licenseNumber) {
      licenseNumberControl.setValue(result.licenseNumber);
    }
    
    // Auto-populate provinceId from search result
    if (provinceIdControl && result.provinceId) {
      provinceIdControl.setValue(result.provinceId);
    }
    
    this.selectedCompanyInfo = result;
    this.populateFromCompany(result);

    if (result.companyId) {
      this.loadOwnerContactForCompany(result.companyId);
    }

    // Clear search
    this.searchResults = [];
    this.searchLicenseNumber = result.licenseNumber || '';
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

  private normalizeRegistrationErrors(errors: any): Array<{ code: string; description?: string }> {
    if (!errors) {
      return [];
    }
    if (Array.isArray(errors)) {
      return errors;
    }
    if (errors.$values && Array.isArray(errors.$values)) {
      return errors.$values;
    }
    return [];
  }

  private handleRegistrationErrors(errors: any): void {
    const normalizedErrors = this.normalizeRegistrationErrors(errors);
    if (!normalizedErrors.length) {
      this.errorMessage = 'خطا در ثبت کاربر. لطفاً دوباره تلاش کنید.';
      this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
      return;
    }

    normalizedErrors.forEach((element) => {
      switch (element.code) {
        case 'DuplicateUserName':
          this.errorMessage = 'این نام کاربری قبلاً استفاده شده است. لطفاً نام کاربری دیگری انتخاب کنید.';
          this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
          break;
        case 'DuplicateEmail':
          this.errorMessage = 'این ایمیل آدرس قبلاً ثبت شده است.';
          this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
          break;
        case 'InvalidEmail':
          this.errorMessage = 'ایمیل آدرس درست نیست.';
          this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
          break;
        case 'PasswordTooShort':
          this.errorMessage = 'پسورد باید حداقل ۴ کرکتر باشد.';
          this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
          break;
        case 'PasswordRequiresNonAlphanumeric':
          this.errorMessage = 'پسورد باید شامل کرکتر های خاص باشد.';
          this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
          break;
        case 'PasswordRequiresDigit':
          this.errorMessage = 'پسورد باید شامل اعداد باشد.';
          this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
          break;
        case 'PasswordRequiresUpper':
          this.errorMessage = 'پسورد باید شامل حروف بزرگ باشد.';
          this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
          break;
        default:
          this.errorMessage = element.description || 'خطا در ثبت کاربر. لطفاً دوباره تلاش کنید.';
          this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
          break;
      }
    });
  }

  private handleRegistrationHttpError(err: any): void {
    const body = err?.error;

    if (err?.status === 400) {
      const identityErrors = body?.errors;
      if (identityErrors) {
        this.handleRegistrationErrors(identityErrors);
        return;
      }

      if (body?.message) {
        this.errorMessage = body.message;
        this.toastr.error(body.message, 'ثبت نام ناموفق');
        return;
      }
    }

    if (err?.status === 401) {
      this.errorMessage = 'نشست شما منقضی شده است. لطفاً دوباره وارد شوید.';
      this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
      return;
    }

    if (err?.status === 403) {
      this.errorMessage = 'شما مجوز ثبت کاربر جدید را ندارید.';
      this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
      return;
    }

    if (err?.status === 500) {
      this.errorMessage = body?.message || body?.error || 'خطای سرور در ثبت کاربر. لطفاً دوباره تلاش کنید.';
      this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
      return;
    }

    if (err?.status === 0) {
      this.errorMessage = 'خطا در اتصال به سرور. لطفاً اتصال انترنت خود را بررسی کنید.';
      this.toastr.error(this.errorMessage, 'خطا در اتصال');
      return;
    }

    this.errorMessage = body?.message || 'خطا در ثبت کاربر. لطفاً دوباره تلاش کنید.';
    this.toastr.error(this.errorMessage, 'ثبت نام ناموفق');
  }
}
