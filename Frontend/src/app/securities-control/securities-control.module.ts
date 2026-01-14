import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SecuritiesControlRoutingModule } from './securities-control-routing.module';
import { SecuritiesControlFormComponent } from './securities-control-form/securities-control-form.component';
import { SecuritiesControlListComponent } from './securities-control-list/securities-control-list.component';
import { SecuritiesControlViewComponent } from './securities-control-view/securities-control-view.component';

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
        SecuritiesControlFormComponent,
        SecuritiesControlListComponent,
        SecuritiesControlViewComponent
    ],
    imports: [
        CommonModule,
        SecuritiesControlRoutingModule,
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
export class SecuritiesControlModule { }