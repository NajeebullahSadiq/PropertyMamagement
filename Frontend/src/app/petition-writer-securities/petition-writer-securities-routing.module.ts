import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PetitionWriterSecuritiesFormComponent } from './petition-writer-securities-form/petition-writer-securities-form.component';
import { PetitionWriterSecuritiesListComponent } from './petition-writer-securities-list/petition-writer-securities-list.component';
import { PetitionWriterSecuritiesViewComponent } from './petition-writer-securities-view/petition-writer-securities-view.component';

const routes: Routes = [
    { path: '', component: PetitionWriterSecuritiesFormComponent },
    { path: 'list', component: PetitionWriterSecuritiesListComponent },
    { path: 'view/:id', component: PetitionWriterSecuritiesViewComponent },
    { path: 'edit/:id', component: PetitionWriterSecuritiesFormComponent }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PetitionWriterSecuritiesRoutingModule { }
