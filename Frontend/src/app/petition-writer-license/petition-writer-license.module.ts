import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PetitionWriterLicenseRoutingModule } from './petition-writer-license-routing.module';
import { PetitionWriterLicenseFormComponent } from './petition-writer-license-form/petition-writer-license-form.component';
import { PetitionWriterLicenseListComponent } from './petition-writer-license-list/petition-writer-license-list.component';
import { PetitionWriterLicenseViewComponent } from './petition-writer-license-view/petition-writer-license-view.component';

import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedModule } from '../shared/shared.module';

@NgModule({
    declarations: [
        PetitionWriterLicenseFormComponent,
        PetitionWriterLicenseListComponent,
        PetitionWriterLicenseViewComponent
    ],
    imports: [
        CommonModule,
        PetitionWriterLicenseRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        MatIconModule,
        MatTabsModule,
        MatButtonModule,
        MatTableModule,
        MatDialogModule,
        NgSelectModule,
        NgbModule,
        NgxPaginationModule,
        SharedModule
    ]
})
export class PetitionWriterLicenseModule { }
