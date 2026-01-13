import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SecuritiesFormComponent } from './securities-form/securities-form.component';
import { SecuritiesListComponent } from './securities-list/securities-list.component';
import { SecuritiesViewComponent } from './securities-view/securities-view.component';

const routes: Routes = [
    { path: '', component: SecuritiesFormComponent },
    { path: 'list', component: SecuritiesListComponent },
    { path: 'view/:id', component: SecuritiesViewComponent },
    { path: 'edit/:id', component: SecuritiesFormComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SecuritiesRoutingModule { }
