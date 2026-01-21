import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatCardModule } from '@angular/material/card';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxPaginationModule } from 'ngx-pagination';

import { ActivityMonitoringRoutingModule } from './activity-monitoring-routing.module';
import { ActivityMonitoringFormComponent } from './activity-monitoring-form/activity-monitoring-form.component';
import { ActivityMonitoringListComponent } from './activity-monitoring-list/activity-monitoring-list.component';
import { ActivityMonitoringViewComponent } from './activity-monitoring-view/activity-monitoring-view.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
    declarations: [
        ActivityMonitoringFormComponent,
        ActivityMonitoringListComponent,
        ActivityMonitoringViewComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        ActivityMonitoringRoutingModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        MatButtonModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatDatepickerModule,
        MatCardModule,
        NgbModule,
        NgSelectModule,
        NgxPaginationModule,
        SharedModule
    ]
})
export class ActivityMonitoringModule { }
