import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { DistrictManagementComponent } from './district-management.component';
import { SharedModule } from '../shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';

const routes: Routes = [
  { path: '', component: DistrictManagementComponent }
];

@NgModule({
  declarations: [
    DistrictManagementComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    TranslateModule,
    RouterModule.forChild(routes)
  ]
})
export class DistrictManagementModule { }
