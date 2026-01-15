import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PetitionWriterLicenseFormComponent } from './petition-writer-license-form/petition-writer-license-form.component';
import { PetitionWriterLicenseListComponent } from './petition-writer-license-list/petition-writer-license-list.component';
import { PetitionWriterLicenseViewComponent } from './petition-writer-license-view/petition-writer-license-view.component';

const routes: Routes = [
    { path: '', component: PetitionWriterLicenseFormComponent },
    { path: 'list', component: PetitionWriterLicenseListComponent },
    { path: 'view/:id', component: PetitionWriterLicenseViewComponent },
    { path: 'edit/:id', component: PetitionWriterLicenseFormComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PetitionWriterLicenseRoutingModule { }
