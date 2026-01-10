import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { environment } from 'src/environments/environment';

import { UploadComponent } from './upload/upload.component';

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

  // License types for company operators
  licenseTypes = [
    { id: 'realEstate', name: 'Real Estate', dari: 'املاک' },
    { id: 'carSale', name: 'Car Sale', dari: 'موټر فروشی' }
  ];

  @ViewChild(UploadComponent) childComponent!: UploadComponent;
  @ViewChild('secondDiv', { static: false }) secondDivRef!: ElementRef;

  constructor(
    public service: AuthService,
    private rbacService: RbacService,
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
    this.service.photoPath = this.imageName;

    this.service.register().subscribe(
      (res: any) => {
        if (res.succeeded) {
          this.service.formModel.reset();
          this.toastr.success('New user created!', 'معلومات موفقانه ثبت سیستم گردید');
          this.callChildMethod();
          this.filePath = 'assets/img/avatar.png';
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
          // Refresh user list
          this.service.getUserProfile().subscribe(res => {
            this.userDetails = res;
          });
        } else {
          res.errors.forEach((element: { code: any; description: string | undefined; }) => {
            switch (element.code) {
              case 'DuplicateUserName':
                this.toastr.error('Username is already taken', 'Registration failed.');
                break;
              default:
                this.toastr.error(element.description, 'Registration failed.');
                break;
            }
          });
        }
      },
      err => {
        console.log(err);
      }
    );
  }

  uploadFinished = (event: string) => {
    this.imageName = event;
    this.filePath = this.baseUrl + this.imageName;
  }

  callChildMethod() {
    this.childComponent.childMethod();
  }

  onPropertyTypeChange() {
    if (this.service.formModel) {
      const roleControl = this.service.formModel.get('Role');
      const companyIdControl = this.service.formModel.get('CompanyId');
      const licenseTypeControl = this.service.formModel.get('LicenseType');

      if (roleControl && companyIdControl) {
        const selectedRole = roleControl.value;

        // Determine which fields to show based on role
        if (selectedRole === UserRoles.Admin || 
            selectedRole === UserRoles.Authority || 
            selectedRole === UserRoles.LicenseReviewer) {
          // System-level roles don't need company
          companyIdControl.setValue(0);
          licenseTypeControl?.setValue('');
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
        } else if (selectedRole === UserRoles.CompanyRegistrar) {
          // Company registrar doesn't need company association
          companyIdControl.setValue(0);
          licenseTypeControl?.setValue('');
          this.showCompanySelect = false;
          this.showLicenseTypeSelect = false;
        } else if (selectedRole === UserRoles.PropertyOperator || 
                   selectedRole === UserRoles.VehicleOperator) {
          // Company operators need company and license type
          this.showCompanySelect = true;
          this.showLicenseTypeSelect = true;
          // Auto-set license type based on role
          if (selectedRole === UserRoles.PropertyOperator) {
            licenseTypeControl?.setValue('realEstate');
          } else {
            licenseTypeControl?.setValue('carSale');
          }
        } else {
          this.showCompanySelect = true;
          this.showLicenseTypeSelect = false;
        }
      }
    }
  }

  getRoleDari(role: string): string {
    return this.rbacService.getRoleDari(role);
  }
}
