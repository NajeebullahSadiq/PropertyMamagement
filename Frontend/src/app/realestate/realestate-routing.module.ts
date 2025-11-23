import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RealestateComponent } from './realestate.component';
import { RealestatelistComponent } from './realestatelist/realestatelist.component';
import { ExpiredlicenselistComponent } from './expiredlicenselist/expiredlicenselist.component';

const routes: Routes = [
  { path: '', component: RealestateComponent },
  { path: 'list', component: RealestatelistComponent },
  { path: 'exlist', component: ExpiredlicenselistComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RealestateRoutingModule { }
