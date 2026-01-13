import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SecuritiesRoutingModule } from './securities-routing.module';
import { SecuritiesFormComponent } from './securities-form/securities-form.component';
import { SecuritiesListComponent } from './securities-list/securities-list.component';
import { SecuritiesViewComponent } from './securities-view/securities-view.component';

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
        SecuritiesFormComponent,
        SecuritiesListComponent,
        SecuritiesViewComponent
    ],
    imports: [
        CommonModule,
        SecuritiesRoutingModule,
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
export class SecuritiesModule { }
