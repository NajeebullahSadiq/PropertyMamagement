import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RealestateComponent } from './realestate.component';
import { RealestatelistComponent } from './realestatelist/realestatelist.component';
import { ExpiredlicenselistComponent } from './expiredlicenselist/expiredlicenselist.component';
import { CompanydetailsviewComponent } from './companydetailsview/companydetailsview.component';

const routes: Routes = [
  { path: '', component: RealestateComponent },
  { path: 'list', component: RealestatelistComponent },
  { path: 'view/:id', component: CompanydetailsviewComponent },
  { path: 'exlist', component: ExpiredlicenselistComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RealestateRoutingModule { }
