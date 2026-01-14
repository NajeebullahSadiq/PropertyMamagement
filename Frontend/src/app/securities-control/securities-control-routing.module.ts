import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SecuritiesControlFormComponent } from './securities-control-form/securities-control-form.component';
import { SecuritiesControlListComponent } from './securities-control-list/securities-control-list.component';
import { SecuritiesControlViewComponent } from './securities-control-view/securities-control-view.component';

const routes: Routes = [
    { path: '', component: SecuritiesControlFormComponent },
    { path: 'list', component: SecuritiesControlListComponent },
    { path: 'view/:id', component: SecuritiesControlViewComponent },
    { path: 'edit/:id', component: SecuritiesControlFormComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SecuritiesControlRoutingModule { }