import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PetitionWriterSecuritiesRoutingModule } from './petition-writer-securities-routing.module';
import { PetitionWriterSecuritiesFormComponent } from './petition-writer-securities-form/petition-writer-securities-form.component';
import { PetitionWriterSecuritiesListComponent } from './petition-writer-securities-list/petition-writer-securities-list.component';
import { PetitionWriterSecuritiesViewComponent } from './petition-writer-securities-view/petition-writer-securities-view.component';

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
        PetitionWriterSecuritiesFormComponent,
        PetitionWriterSecuritiesListComponent,
        PetitionWriterSecuritiesViewComponent
    ],
    imports: [
        CommonModule,
        PetitionWriterSecuritiesRoutingModule,
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
export class PetitionWriterSecuritiesModule { }
