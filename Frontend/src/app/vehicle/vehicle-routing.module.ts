import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VehicleComponent } from './vehicle.component';
import { VehiclelistComponent } from './vehiclelist/vehiclelist.component';
import { VehicledetailsviewComponent } from './vehicledetailsview/vehicledetailsview.component';

const routes: Routes = [
  { path: '', component: VehicleComponent },
  { path: 'list', component: VehiclelistComponent },
  { path: 'view/:id', component: VehicledetailsviewComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VehicleRoutingModule { }
