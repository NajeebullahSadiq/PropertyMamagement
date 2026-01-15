import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/authSetting/auth.guard';
import { RoleGuard, AdminGuard, PropertyModuleGuard, VehicleModuleGuard, CompanyModuleGuard } from './auth/authSetting/role.guard';
import { ForbiddenComponent } from './auth/forbidden/forbidden.component';
import { PrintComponent } from './print/print.component';
import { PrintvehicledataComponent } from './printvehicledata/printvehicledata.component';
import { PrintLicenseComponent } from './print-license/print-license.component';
import { PrintSecuritiesComponent } from './print-securities/print-securities.component';
import { PrintPetitionWriterSecuritiesComponent } from './print-petition-writer-securities/print-petition-writer-securities.component';
import { MasterlayoutComponent } from './dashboard/masterlayout/masterlayout.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ReportComponent } from './dashboard/report/report.component';
import { UserReportComponent } from './dashboard/user-report/user-report.component';
import { RegisterComponent } from './auth/register/register.component';

const routes: Routes = [
  // Auth routes - standalone without masterlayout
  { path: '', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule) }, 
  { path: 'Auth', redirectTo: '', pathMatch: 'full' },
  { path: 'Auth/Login', redirectTo: '', pathMatch: 'full' },
  // Public verification portal (no auth required)
  { path: 'verify', loadChildren: () => import('./verify/verify.module').then(m => m.VerifyModule) },
  // Register page with master layout (authenticated)
  { 
    path: 'Auth/Register', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', component: RegisterComponent }
    ]
  },
  { 
    path: 'dashboard', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', component: DashboardComponent }
    ]
  },
  { 
    path: 'estate', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard, PropertyModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./estate/estate.module').then(m => m.EstateModule) }
    ]
  },
  { 
    path: 'realestate', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard, CompanyModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./realestate/realestate.module').then(m => m.RealestateModule) }
    ]
  },
  { 
    path: 'vehicle', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard, VehicleModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./vehicle/vehicle.module').then(m => m.VehicleModule) }
    ]
  },
  { 
    path: 'report', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', component: ReportComponent }
    ]
  },
  { 
    path: 'userreport', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', component: UserReportComponent }
    ]
  },
  { 
    path: 'users', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard, AdminGuard],
    children: [
      { path: '', loadChildren: () => import('./users/users.module').then(m => m.UsersModule) }
    ]
  },
  { 
    path: 'securities', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', loadChildren: () => import('./securities/securities.module').then(m => m.SecuritiesModule) }
    ]
  },
  { 
    path: 'securities-control', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', loadChildren: () => import('./securities-control/securities-control.module').then(m => m.SecuritiesControlModule) }
    ]
  },
  { 
    path: 'petition-writer-securities', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', loadChildren: () => import('./petition-writer-securities/petition-writer-securities.module').then(m => m.PetitionWriterSecuritiesModule) }
    ]
  },
  { 
    path: 'securities-report', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', loadChildren: () => import('./securities-report/securities-report.module').then(m => m.SecuritiesReportModule) }
    ]
  },
  { 
    path: 'petition-writer-report', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', loadChildren: () => import('./petition-writer-report/petition-writer-report.module').then(m => m.PetitionWriterReportModule) }
    ]
  },
  { 
    path: 'license-applications', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard, CompanyModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./license-applications/license-applications.module').then(m => m.LicenseApplicationsModule) }
    ]
  },
  { 
    path: 'petition-writer-license', 
    component: MasterlayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', loadChildren: () => import('./petition-writer-license/petition-writer-license.module').then(m => m.PetitionWriterLicenseModule) }
    ]
  },
  { path: 'forbidden', component: ForbiddenComponent },
  { path: 'print/:id', component: PrintComponent },
  { path: 'print', component: PrintComponent },
  { path: 'printvehicledata/:id', component: PrintvehicledataComponent },
  { path: 'printvehicledata', component: PrintvehicledataComponent },
  { path: 'printLicense/:id', component: PrintLicenseComponent },
  { path: 'printLicense', component: PrintLicenseComponent },
  { path: 'printSecurities/:id', component: PrintSecuritiesComponent },
  { path: 'printSecurities', component: PrintSecuritiesComponent },
  { path: 'printPetitionWriterSecurities/:id', component: PrintPetitionWriterSecuritiesComponent },
  { path: 'printPetitionWriterSecurities', component: PrintPetitionWriterSecuritiesComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
