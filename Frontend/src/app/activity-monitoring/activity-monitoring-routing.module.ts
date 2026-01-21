import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ActivityMonitoringFormComponent } from './activity-monitoring-form/activity-monitoring-form.component';
import { ActivityMonitoringListComponent } from './activity-monitoring-list/activity-monitoring-list.component';
import { ActivityMonitoringViewComponent } from './activity-monitoring-view/activity-monitoring-view.component';
import { AuthGuard } from '../auth/authSetting/auth.guard';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'list',
        pathMatch: 'full'
    },
    {
        path: 'list',
        component: ActivityMonitoringListComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'form',
        component: ActivityMonitoringFormComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'form/:id',
        component: ActivityMonitoringFormComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'view/:id',
        component: ActivityMonitoringViewComponent,
        canActivate: [AuthGuard]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ActivityMonitoringRoutingModule { }
