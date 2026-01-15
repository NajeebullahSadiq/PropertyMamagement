import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { VerifyComponent } from './verify.component';

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
    RouterModule.forChild(routes)
  ]
})
export class VerifyModule { }
