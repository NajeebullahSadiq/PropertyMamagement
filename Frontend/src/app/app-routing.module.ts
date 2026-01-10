import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/authSetting/auth.guard';
import { RoleGuard, PropertyModuleGuard, VehicleModuleGuard, CompanyModuleGuard } from './auth/authSetting/role.guard';
import { ForbiddenComponent } from './auth/forbidden/forbidden.component';
import { PrintComponent } from './print/print.component';
import { PrintvehicledataComponent } from './printvehicledata/printvehicledata.component';
import { PrintLicenseComponent } from './print-license/print-license.component';

const routes: Routes = [
 
  { path: '', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule) }, 
  { path: 'Auth', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule) }, 
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule), canActivate: [AuthGuard] },
  { path: 'estate', loadChildren: () => import('./estate/estate.module').then(m => m.EstateModule), canActivate: [AuthGuard, PropertyModuleGuard] },
  { path: 'realestate', loadChildren: () => import('./realestate/realestate.module').then(m => m.RealestateModule), canActivate: [AuthGuard, CompanyModuleGuard] },
  { path: 'vehicle', loadChildren: () => import('./vehicle/vehicle.module').then(m => m.VehicleModule), canActivate: [AuthGuard, VehicleModuleGuard] },
  { path: 'forbidden', component: ForbiddenComponent },
  { path: 'print/:id', component: PrintComponent },
  { path: 'print', component: PrintComponent },
  { path: 'printvehicledata/:id', component: PrintvehicledataComponent },
  { path: 'printvehicledata', component: PrintvehicledataComponent },
  { path: 'printLicense/:id', component: PrintLicenseComponent },
  { path: 'printLicense', component: PrintLicenseComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
