import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { MatDialogModule } from '@angular/material/dialog';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedModule } from '../shared/shared.module';

import { VehicleReportComponent } from './vehicle-report.component';

const routes: Routes = [
  { path: '', component: VehicleReportComponent }
];

@NgModule({
  declarations: [
    VehicleReportComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    NgxPaginationModule,
    SharedModule
  ]
})
export class VehicleReportModule { }
