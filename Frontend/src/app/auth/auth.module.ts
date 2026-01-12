import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { AuthRoutingModule } from './auth-routing.module';
import { AuthComponent } from './auth.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UploadComponent } from './register/upload/upload.component';
import { SharedModule } from '../shared/shared.module';

// Note: ChangepasswordComponent, ResetpasswordComponent, and LockuserComponent
// have been moved to SharedModule for global availability as dialogs

@NgModule({
  declarations: [
    AuthComponent,
    LoginComponent,
    RegisterComponent,
    UploadComponent,
    ForbiddenComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    SharedModule
  ]
})
export class AuthModule { }
