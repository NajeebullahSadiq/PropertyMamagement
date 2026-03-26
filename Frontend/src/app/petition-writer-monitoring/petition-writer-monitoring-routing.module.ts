import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PetitionWriterMonitoringFormComponent } from './petition-writer-monitoring-form/petition-writer-monitoring-form.component';
import { PetitionWriterMonitoringListComponent } from './petition-writer-monitoring-list/petition-writer-monitoring-list.component';
import { PetitionWriterMonitoringViewComponent } from './petition-writer-monitoring-view/petition-writer-monitoring-view.component';
import { AuthGuard } from '../auth/authSetting/auth.guard';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'list',
        pathMatch: 'full'
    },
    {
        path: 'list',
        component: PetitionWriterMonitoringListComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'form',
        component: PetitionWriterMonitoringFormComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'form/:id',
        component: PetitionWriterMonitoringFormComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'view/:id',
        component: PetitionWriterMonitoringViewComponent,
        canActivate: [AuthGuard]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PetitionWriterMonitoringRoutingModule { }
