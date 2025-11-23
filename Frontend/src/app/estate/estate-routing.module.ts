import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EstateComponent } from './estate.component';
import { PropertydetailsComponent } from './propertydetails/propertydetails.component';
import { PropertydetailslistComponent } from './propertydetailslist/propertydetailslist.component';

const routes: Routes = [{ path: '', component: EstateComponent },
{ path: 'list', component: PropertydetailslistComponent }];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EstateRoutingModule { }
