import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { LicenseApplicationsRoutingModule } from './license-applications-routing.module';
import { LicenseApplicationFormComponent } from './license-application-form/license-application-form.component';
import { LicenseApplicationListComponent } from './license-application-list/license-application-list.component';
import { LicenseApplicationViewComponent } from './license-application-view/license-application-view.component';

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
        LicenseApplicationFormComponent,
        LicenseApplicationListComponent,
        LicenseApplicationViewComponent
    ],
    imports: [
        CommonModule,
        LicenseApplicationsRoutingModule,
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
export class LicenseApplicationsModule { }
