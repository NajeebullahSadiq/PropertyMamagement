import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  formModel = {
    UserName: '',
    Password: ''
  }
  userRole:any='ADMIN';
  showPassword: boolean = false;
  
  constructor(private router: Router,private service: AuthService,private toastr: ToastrService) { }

  ngOnInit(): void {
    // if (localStorage.getItem('token') != null)
    //   this.router.navigateByUrl('/home');
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
  onSubmit(form: NgForm) {
    this.service.login(form.value).subscribe(
      (res: any) => {
        localStorage.setItem('token', res.token);
        const token = res.token;
        let userRole = '';
        
        if (token) {
          const payLoad = JSON.parse(window.atob(token.split('.')[1]));
          userRole = payLoad?.role || '';
        }
        this.userRole=userRole;
        console.log(this.userRole,'this is userRole') 
        if(this.userRole==='ADMIN')
        this.router.navigateByUrl('/dashboard/dashboard');
        else 
        this.router.navigateByUrl('/dashboard');
      },
      err => {
        if (err.status == 400)
          this.toastr.error('عملیه ناموفق، نام کاربری ویا پسورد اشتباه است', '');
        else if(err.status==418)
        this.toastr.error('اکونت شما توسط مدیر سیستم قفل گردیده است!!', '');
        else
          console.log(err);
        //  this.toastr.error('ارتباط با سرور برقرار نیست', 'عملیه ناموفق');
      }
    );
  }
}
