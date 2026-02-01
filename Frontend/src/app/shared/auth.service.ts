import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { loginModel } from '../models/loginModel';
import { environment } from 'src/environments/environment';
import { UserRoles } from './rbac.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  readonly BaseURI = environment.apiUrl;
  photoPath='';

  constructor(private fb: FormBuilder, private http: HttpClient) { }
  
  formModel = this.fb.group({
    FirstName: ['',Validators.required],
    LastName:['',Validators.required],
    CompanyId:[0, Validators.required],
    UserName: ['', Validators.required],
    Email: ['', Validators.email],
    Role:['',Validators.required],
    PhoneNumber:[''],
    PhotoPath:[],
    LicenseType: [''],
    ProvinceId: [null], // Province assignment for COMPANY_REGISTRAR
    Passwords: this.fb.group({
      Password: ['', [Validators.required, Validators.minLength(4)]],
      ConfirmPassword: ['', Validators.required]
    }, { validator: this.comparePasswords })
  });

  comparePasswords(fb: FormGroup) {
    let confirmPswrdCtrl = fb.get('ConfirmPassword');
    if(confirmPswrdCtrl){
      if (confirmPswrdCtrl.errors == null || 'passwordMismatch' in confirmPswrdCtrl.errors) {
        if (fb.get('Password')?.value != confirmPswrdCtrl.value)
          confirmPswrdCtrl.setErrors({ passwordMismatch: true });
        else
          confirmPswrdCtrl.setErrors(null);
      }
    }
  }

  login(formData: any) {
    return this.http.post(this.BaseURI + '/ApplicationUser/Login', formData);
  }

  register() {
    var body = {
      userName: this.formModel.value.UserName,
      email: this.formModel.value.Email,
      firstName: this.formModel.value.FirstName,
      lastName: this.formModel.value.LastName,
      role: this.formModel.value.Role,
      password: this.formModel.value.Passwords.Password,
      phoneNumber: this.formModel.value.PhoneNumber,
      companyId: this.formModel.value.CompanyId,
      photoPath: this.photoPath,
      licenseType: this.formModel.value.LicenseType,
      provinceId: this.formModel.value.ProvinceId
    };
    return this.http.post(this.BaseURI + '/ApplicationUser/Register', body);
  }

  getUserProfile() {
    return this.http.get(this.BaseURI + '/ApplicationUser/GetAllUsers');
  }

  getCurrentUserProfile() {
    return this.http.get(this.BaseURI + '/UserProfile/getProfile');
  }

  upload(file: File): Observable<HttpEvent<any>> {
    const formData: FormData = new FormData();
    formData.append('file', file);
    const req = new HttpRequest('POST', `${this.BaseURI}/upload`, formData, {
      reportProgress: true,
      responseType: 'json',
    });
    return this.http.request(req);
  }

  getFiles(): Observable<any> {
    return this.http.get(`${this.BaseURI}/files`);
  }

  roleMatch(allowedRoles: any[]): boolean {
    var isMatch = false;
    var token = localStorage.getItem('token');
    if (token) {
      var payLoad = JSON.parse(window.atob(token.split('.')[1]));
      var userRole = payLoad.role || payLoad.userRole;
      allowedRoles.forEach(element => {
        if (userRole == element) {
          isMatch = true;
          return false;
        } else {
          return;
        }
      });
    }
    return isMatch;
  }

  getCompanies() {
    return this.http.get(this.BaseURI + '/CompanyDetails/getCompanies');
  }

  getRoles() {
    return this.http.get(this.BaseURI + '/ApplicationUser/GetRoles');
  }

  changePassword(model: any): Observable<any> {
    return this.http.post(this.BaseURI + '/ApplicationUser/ChangePassword', model);
  }

  resetPassword(model: loginModel): Observable<any> {
    return this.http.post(this.BaseURI + '/ApplicationUser/ResetPassword', model);
  }

  getUsers() {
    return this.http.get(this.BaseURI + '/ApplicationUser/UserInfo');
  }

  lockUser(userName: string, isLocked: boolean) {
    const data = { UserName: userName, IsLooked: isLocked };
    return this.http.post(`${this.BaseURI}/ApplicationUser/LockUser`, data);
  }

  updateUserRole(userId: string, newRole: string) {
    return this.http.post(`${this.BaseURI}/ApplicationUser/UpdateUserRole`, { userId, newRole });
  }

  createCompanyUser(userData: any) {
    return this.http.post(`${this.BaseURI}/ApplicationUser/CreateCompanyUser`, userData);
  }

  updateUser(userData: any) {
    return this.http.post(`${this.BaseURI}/ApplicationUser/UpdateUser`, userData);
  }

  toggleUserStatus(userId: string, isLocked: boolean) {
    return this.http.post(`${this.BaseURI}/ApplicationUser/ToggleUserStatus`, { userId, isLocked });
  }

  getUser(userId: string) {
    return this.http.get(`${this.BaseURI}/ApplicationUser/GetUser/${userId}`);
  }

  // Province-based access control methods
  getUserProvinceId(): number | null {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payLoad = JSON.parse(window.atob(token.split('.')[1]));
        return payLoad.province_id ? parseInt(payLoad.province_id) : null;
      } catch {
        return null;
      }
    }
    return null;
  }

  isCompanyRegistrar(): boolean {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payLoad = JSON.parse(window.atob(token.split('.')[1]));
        const userRole = payLoad.role || payLoad.userRole;
        return userRole === UserRoles.CompanyRegistrar;
      } catch {
        return false;
      }
    }
    return false;
  }

  isAdministrator(): boolean {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payLoad = JSON.parse(window.atob(token.split('.')[1]));
        const userRole = payLoad.role || payLoad.userRole;
        return userRole === UserRoles.Admin;
      } catch {
        return false;
      }
    }
    return false;
  }

  getProvinces() {
    return this.http.get(`${this.BaseURI}/Setup/GetProvinces`);
  }
}
