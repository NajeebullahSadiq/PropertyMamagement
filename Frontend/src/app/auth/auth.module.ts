import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { AuthRoutingModule } from './auth-routing.module';
import { AuthComponent } from './auth.component';
import { ChangepasswordComponent } from './changepassword/changepassword.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { LockuserComponent } from './lockuser/lockuser.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UploadComponent } from './register/upload/upload.component';
import { ResetpasswordComponent } from './resetpassword/resetpassword.component';
@NgModule({
  declarations: [
    AuthComponent,
    LoginComponent,
    RegisterComponent,
    UploadComponent,
    ChangepasswordComponent,
    ResetpasswordComponent,
    LockuserComponent,
    ForbiddenComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule
  ]
})
export class AuthModule { }
