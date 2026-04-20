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

  // Single MasterlayoutComponent parent - all module routes as children
  // This prevents MasterlayoutComponent from being destroyed/recreated on navigation
  { 
    path: '',
    component: MasterlayoutComponent,
    canActivate: [GeolocationGuard, AuthGuard],
    children: [
      { path: 'Auth/Register', component: RegisterComponent, canActivate: [AuthGuard] },
      { path: 'dashboard', component: DashboardComponent, canActivate: [DashboardGuard] },
      { path: 'estate', loadChildren: () => import('./estate/estate.module').then(m => m.EstateModule), canActivate: [PropertyModuleGuard] },
      { path: 'realestate', loadChildren: () => import('./realestate/realestate.module').then(m => m.RealestateModule), canActivate: [CompanyModuleGuard] },
      { path: 'vehicle', loadChildren: () => import('./vehicle/vehicle.module').then(m => m.VehicleModule), canActivate: [VehicleModuleGuard] },
      { path: 'report', component: ReportComponent },
      { path: 'userreport', component: UserReportComponent, canActivate: [AdminGuard] },
      { path: 'users', loadChildren: () => import('./users/users.module').then(m => m.UsersModule), canActivate: [AdminGuard] },
      { path: 'securities', loadChildren: () => import('./securities/securities.module').then(m => m.SecuritiesModule), canActivate: [SecuritiesModuleGuard] },
      { path: 'securities-control', loadChildren: () => import('./securities-control/securities-control.module').then(m => m.SecuritiesControlModule), canActivate: [SecuritiesModuleGuard] },
      { path: 'petition-writer-securities', loadChildren: () => import('./petition-writer-securities/petition-writer-securities.module').then(m => m.PetitionWriterSecuritiesModule), canActivate: [PetitionWriterModuleGuard] },
      { path: 'securities-report', loadChildren: () => import('./securities-report/securities-report.module').then(m => m.SecuritiesReportModule), canActivate: [SecuritiesModuleGuard] },
      { path: 'petition-writer-report', loadChildren: () => import('./petition-writer-report/petition-writer-report.module').then(m => m.PetitionWriterReportModule), canActivate: [PetitionWriterModuleGuard] },
      { path: 'license-applications', loadChildren: () => import('./license-applications/license-applications.module').then(m => m.LicenseApplicationsModule), canActivate: [CompanyModuleGuard] },
      { path: 'petition-writer-license', loadChildren: () => import('./petition-writer-license/petition-writer-license.module').then(m => m.PetitionWriterLicenseModule), canActivate: [PetitionWriterModuleGuard] },
      { path: 'activity-monitoring', loadChildren: () => import('./activity-monitoring/activity-monitoring.module').then(m => m.ActivityMonitoringModule), canActivate: [ActivityMonitoringGuard] },
      { path: 'petition-writer-monitoring', loadChildren: () => import('./petition-writer-monitoring/petition-writer-monitoring.module').then(m => m.PetitionWriterMonitoringModule), canActivate: [PetitionWriterMonitoringGuard] },
      { path: 'audit-log', loadChildren: () => import('./audit-log/audit-log.module').then(m => m.AuditLogModule), canActivate: [AdminGuard] },
      { path: 'configuration', loadChildren: () => import('./configuration/configuration.module').then(m => m.ConfigurationModule), canActivate: [AdminGuard] },
      { path: 'district-management', loadChildren: () => import('./district-management/district-management.module').then(m => m.DistrictManagementModule), canActivate: [AdminGuard] },
      { path: 'petition-writer-activity-location-management', loadChildren: () => import('./petition-writer-activity-location-management/petition-writer-activity-location-management.module').then(m => m.PetitionWriterActivityLocationManagementModule), canActivate: [AdminGuard] },
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
