import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatIconModule } from '@angular/material/icon';

import { PetitionWriterReportComponent } from './petition-writer-report.component';
import { SharedModule } from '../shared/shared.module';

const routes: Routes = [
    { path: '', component: PetitionWriterReportComponent }
];

@NgModule({
    declarations: [
        PetitionWriterReportComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild(routes),
        NgbModule,
        MatIconModule,
        SharedModule
    ]
})
export class PetitionWriterReportModule { }
