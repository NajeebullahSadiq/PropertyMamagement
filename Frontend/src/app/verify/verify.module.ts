import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { VerifyComponent } from './verify.component';
import { SharedModule } from '../shared/shared.module';

const routes: Routes = [
  { path: '', component: VerifyComponent },
  { path: ':code', component: VerifyComponent }
];

@NgModule({
  declarations: [
    VerifyComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class VerifyModule { }
