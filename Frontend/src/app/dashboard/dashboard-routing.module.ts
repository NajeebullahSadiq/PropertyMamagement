import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { MasterlayoutComponent } from './masterlayout/masterlayout.component';
import { ReportComponent } from './report/report.component';
import { UserReportComponent } from './user-report/user-report.component';

const routes: Routes = [
  { path: '', component: MasterlayoutComponent,
children: [
{ path: 'estate',  loadChildren: () => import('../estate/estate.module').then(m => m.EstateModule)},
{ path: 'vehicle', loadChildren: () => import('../vehicle/vehicle.module').then(m => m.VehicleModule) },
{ path: 'realestate', loadChildren: () => import('../realestate/realestate.module').then(m => m.RealestateModule) },
{ path: 'Auth', loadChildren: () => import('../auth/auth.module').then(m => m.AuthModule) }, 
{ path: 'dashboard', component: DashboardComponent },
{ path: 'report', component: ReportComponent },
{path:'userreport',component:UserReportComponent}
]},
]
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
