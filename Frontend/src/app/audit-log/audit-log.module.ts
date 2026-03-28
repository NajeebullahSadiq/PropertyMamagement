import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';

import { AuditLogComponent } from './audit-log.component';
import { AuditLogService } from './audit-log.service';

const routes: Routes = [
  { path: '', component: AuditLogComponent }
];

@NgModule({
  declarations: [
    AuditLogComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    MatIconModule,
    MatDialogModule
  ],
  providers: [AuditLogService]
})
export class AuditLogModule { }
