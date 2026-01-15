import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LicenseApplicationFormComponent } from './license-application-form/license-application-form.component';
import { LicenseApplicationListComponent } from './license-application-list/license-application-list.component';
import { LicenseApplicationViewComponent } from './license-application-view/license-application-view.component';

const routes: Routes = [
    { path: '', component: LicenseApplicationFormComponent },
    { path: 'list', component: LicenseApplicationListComponent },
    { path: 'view/:id', component: LicenseApplicationViewComponent },
    { path: 'edit/:id', component: LicenseApplicationFormComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LicenseApplicationsRoutingModule { }
