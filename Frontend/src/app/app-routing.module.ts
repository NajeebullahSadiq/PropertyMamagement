import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/authSetting/auth.guard';
import { GeolocationGuard } from './auth/authSetting/geolocation.guard';
import { RoleGuard, AdminGuard, PropertyModuleGuard, VehicleModuleGuard, CompanyModuleGuard, DashboardGuard, SecuritiesModuleGuard, PetitionWriterModuleGuard, ActivityMonitoringGuard, PetitionWriterMonitoringGuard } from './auth/authSetting/role.guard';
import { ForbiddenComponent } from './auth/forbidden/forbidden.component';
import { AccessDeniedComponent } from './auth/access-denied/access-denied.component';
import { PrintComponent } from './print/print.component';
import { PrintvehicledataComponent } from './printvehicledata/printvehicledata.component';
import { PrintLicenseComponent } from './print-license/print-license.component';
import { PrintSecuritiesComponent } from './print-securities/print-securities.component';
import { PrintPetitionWriterSecuritiesComponent } from './print-petition-writer-securities/print-petition-writer-securities.component';
import { PrintPetitionWriterLicenseComponent } from './print-petition-writer-license/print-petition-writer-license.component';
import { MasterlayoutComponent } from './dashboard/masterlayout/masterlayout.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ReportComponent } from './dashboard/report/report.component';
import { UserReportComponent } from './dashboard/user-report/user-report.component';
import { RegisterComponent } from './auth/register/register.component';

const routes: Routes = [
  // Access Denied Route (no geolocation check needed)
  { path: 'access-denied', component: AccessDeniedComponent },
  
  // Auth routes - standalone without masterlayout (with geolocation check)
  { path: '', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule), canActivate: [GeolocationGuard] }, 
  { path: 'Auth', redirectTo: '', pathMatch: 'full' },
  { path: 'Auth/Login', redirectTo: '', pathMatch: 'full' },
  
  // Public verification portal (with geolocation check)
  { path: 'verify', loadChildren: () => import('./verify/verify.module').then(m => m.VerifyModule), canActivate: [GeolocationGuard] },
  
  // Register page with master layout (authenticated + geolocation)
  { 
    path: 'Auth/Register', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard],
    children: [
      { path: '', component: RegisterComponent }
    ]
  },
  { 
    path: 'dashboard', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, DashboardGuard],
    children: [
      { path: '', component: DashboardComponent }
    ]
  },
  { 
    path: 'estate', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, PropertyModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./estate/estate.module').then(m => m.EstateModule) }
    ]
  },
  { 
    path: 'realestate', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, CompanyModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./realestate/realestate.module').then(m => m.RealestateModule) }
    ]
  },
  { 
    path: 'vehicle', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, VehicleModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./vehicle/vehicle.module').then(m => m.VehicleModule) }
    ]
  },
  { 
    path: 'report', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard],
    children: [
      { path: '', component: ReportComponent }
    ]
  },
  { 
    path: 'userreport', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard],
    children: [
      { path: '', component: UserReportComponent }
    ]
  },
  { 
    path: 'users', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, AdminGuard],
    children: [
      { path: '', loadChildren: () => import('./users/users.module').then(m => m.UsersModule) }
    ]
  },
  { 
    path: 'securities', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, SecuritiesModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./securities/securities.module').then(m => m.SecuritiesModule) }
    ]
  },
  { 
    path: 'securities-control', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, SecuritiesModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./securities-control/securities-control.module').then(m => m.SecuritiesControlModule) }
    ]
  },
  { 
    path: 'petition-writer-securities', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, PetitionWriterModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./petition-writer-securities/petition-writer-securities.module').then(m => m.PetitionWriterSecuritiesModule) }
    ]
  },
  { 
    path: 'securities-report', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, SecuritiesModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./securities-report/securities-report.module').then(m => m.SecuritiesReportModule) }
    ]
  },
  { 
    path: 'petition-writer-report', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, PetitionWriterModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./petition-writer-report/petition-writer-report.module').then(m => m.PetitionWriterReportModule) }
    ]
  },
  { 
    path: 'license-applications', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, CompanyModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./license-applications/license-applications.module').then(m => m.LicenseApplicationsModule) }
    ]
  },
  { 
    path: 'petition-writer-license', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, PetitionWriterModuleGuard],
    children: [
      { path: '', loadChildren: () => import('./petition-writer-license/petition-writer-license.module').then(m => m.PetitionWriterLicenseModule) }
    ]
  },
  { 
    path: 'activity-monitoring', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, ActivityMonitoringGuard],
    children: [
      { path: '', loadChildren: () => import('./activity-monitoring/activity-monitoring.module').then(m => m.ActivityMonitoringModule) }
    ]
  },
  { 
    path: 'petition-writer-monitoring', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, PetitionWriterMonitoringGuard],
    children: [
      { path: '', loadChildren: () => import('./petition-writer-monitoring/petition-writer-monitoring.module').then(m => m.PetitionWriterMonitoringModule) }
    ]
  },
  { 
    path: 'audit-log', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, AdminGuard],
    children: [
      { path: '', loadChildren: () => import('./audit-log/audit-log.module').then(m => m.AuditLogModule) }
    ]
  },
  { 
    path: 'configuration', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, AdminGuard],
    children: [
      { path: '', loadChildren: () => import('./configuration/configuration.module').then(m => m.ConfigurationModule) }
    ]
  },
  { 
    path: 'district-management', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, AdminGuard],
    children: [
      { path: '', loadChildren: () => import('./district-management/district-management.module').then(m => m.DistrictManagementModule) }
    ]
  },
  { 
    path: 'petition-writer-activity-location-management', 
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard, AdminGuard],
    children: [
      { path: '', loadChildren: () => import('./petition-writer-activity-location-management/petition-writer-activity-location-management.module').then(m => m.PetitionWriterActivityLocationManagementModule) }
    ]
  },
  { path: 'forbidden', component: ForbiddenComponent },
  { path: 'print/:id', component: PrintComponent, canActivate: [GeolocationGuard] },
  { path: 'print', component: PrintComponent, canActivate: [GeolocationGuard] },
  { path: 'printvehicledata/:id', component: PrintvehicledataComponent, canActivate: [GeolocationGuard] },
  { path: 'printvehicledata', component: PrintvehicledataComponent, canActivate: [GeolocationGuard] },
  { path: 'printLicense/:id', component: PrintLicenseComponent, canActivate: [GeolocationGuard] },
  { path: 'printLicense', component: PrintLicenseComponent, canActivate: [GeolocationGuard] },
  { path: 'printSecurities/:id', component: PrintSecuritiesComponent, canActivate: [GeolocationGuard] },
  { path: 'printSecurities', component: PrintSecuritiesComponent, canActivate: [GeolocationGuard] },
  { path: 'printPetitionWriterSecurities/:id', component: PrintPetitionWriterSecuritiesComponent, canActivate: [GeolocationGuard] },
  { path: 'printPetitionWriterSecurities', component: PrintPetitionWriterSecuritiesComponent, canActivate: [GeolocationGuard] },
  { path: 'printPetitionWriterLicense/:id', component: PrintPetitionWriterLicenseComponent, canActivate: [GeolocationGuard] },
  { path: 'printPetitionWriterLicense', component: PrintPetitionWriterLicenseComponent, canActivate: [GeolocationGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
