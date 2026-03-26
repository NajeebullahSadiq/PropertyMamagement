import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { NgSelectModule } from '@ng-select/ng-select';
import { ToastrModule } from 'ngx-toastr';

import { PetitionWriterMonitoringRoutingModule } from './petition-writer-monitoring-routing.module';
import { PetitionWriterMonitoringListComponent } from './petition-writer-monitoring-list/petition-writer-monitoring-list.component';
import { PetitionWriterMonitoringFormComponent } from './petition-writer-monitoring-form/petition-writer-monitoring-form.component';
import { PetitionWriterMonitoringViewComponent } from './petition-writer-monitoring-view/petition-writer-monitoring-view.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
    declarations: [
        PetitionWriterMonitoringListComponent,
        PetitionWriterMonitoringFormComponent,
        PetitionWriterMonitoringViewComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        NgxPaginationModule,
        NgSelectModule,
        ToastrModule,
        SharedModule,
        PetitionWriterMonitoringRoutingModule
    ]
})
export class PetitionWriterMonitoringModule { }
