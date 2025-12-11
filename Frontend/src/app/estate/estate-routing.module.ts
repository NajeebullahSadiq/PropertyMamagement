import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EstateComponent } from './estate.component';
import { PropertydetailsComponent } from './propertydetails/propertydetails.component';
import { PropertydetailslistComponent } from './propertydetailslist/propertydetailslist.component';
import { CancellationPageComponent } from './cancellation-page/cancellation-page.component';

const routes: Routes = [{ path: '', component: EstateComponent },
{ path: 'list', component: PropertydetailslistComponent },
{ path: 'cancellation', component: CancellationPageComponent }];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EstateRoutingModule { }
