import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  formModel = {
    UserName: '',
    Password: ''
  }
  userRole: string = '';
  showPassword: boolean = false;
  
  constructor(
    private router: Router,
    private service: AuthService,
    private rbacService: RbacService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    // If already logged in, redirect to dashboard
    if (localStorage.getItem('token') != null) {
      this.router.navigateByUrl('/dashboard');
    }
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(form: NgForm) {
    this.service.login(form.value).subscribe(
      (res: any) => {
        localStorage.setItem('token', res.token);
        
        // Load user permissions from token
        this.rbacService.loadUserFromToken();
        
        // Get role from response or token
        this.userRole = res.role || this.rbacService.getCurrentRole();
        
        // Navigate based on role and module access
        if (this.userRole === UserRoles.Admin) {
          this.router.navigateByUrl('/dashboard/dashboard');
        } else if (this.userRole === UserRoles.LicenseReviewer) {
          // License reviewer goes to company list (view-only)
          this.router.navigateByUrl('/dashboard/realestate/list');
        } else if (this.userRole === UserRoles.PropertyOperator) {
          // Property operator goes to property list
          this.router.navigateByUrl('/dashboard/estate/list');
        } else if (this.userRole === UserRoles.VehicleOperator) {
          // Vehicle operator goes to vehicle list
          this.router.navigateByUrl('/dashboard/vehicle/list');
        } else {
          // Default to dashboard
          this.router.navigateByUrl('/dashboard');
        }
      },
      err => {
        if (err.status == 400) {
          const message = err.error?.message || 'نام کاربری ویا پسورد اشتباه است';
          this.toastr.error(message, 'عملیه ناموفق');
        } else if (err.status == 418) {
          const message = err.error?.message || 'اکونت شما توسط مدیر سیستم قفل گردیده است';
          this.toastr.error(message, 'حساب قفل شده');
        } else {
          console.log(err);
          this.toastr.error('خطا در ارتباط با سرور', 'عملیه ناموفق');
        }
      }
    );
  }
}
