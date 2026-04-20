import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';

import { PetitionWriterActivityLocationManagementComponent } from './petition-writer-activity-location-management.component';

const routes: Routes = [
  { path: '', component: PetitionWriterActivityLocationManagementComponent }
];

@NgModule({
  declarations: [
    PetitionWriterActivityLocationManagementComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    SharedModule
  ]
})
export class PetitionWriterActivityLocationManagementModule { }
