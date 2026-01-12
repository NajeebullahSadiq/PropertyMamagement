import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { VehicleRoutingModule } from './vehicle-routing.module';
import { VehicleComponent } from './vehicle.component';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatTabsModule } from '@angular/material/tabs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { NgxPaginationModule } from 'ngx-pagination';
import { MatDialogModule } from '@angular/material/dialog';
import { VehicleSubmitComponent } from './vehicle-submit/vehicle-submit.component';
import { BuyerdetailComponent } from './vehicle-submit/buyerdetail/buyerdetail.component';
import { SellerdetailComponent } from './vehicle-submit/sellerdetail/sellerdetail.component';
import { WitnessdetailComponent } from './vehicle-submit/witnessdetail/witnessdetail.component';
import { VehicleFileuploadComponent } from './vehicle-submit/vehicle-fileupload/vehicle-fileupload.component';
import { VehiclelistComponent } from './vehiclelist/vehiclelist.component';
import { VehicleNationalidUploadComponent } from './vehicle-submit/nationalid-upload/nationalid-upload.component';
import { VehicledetailsviewComponent } from './vehicledetailsview/vehicledetailsview.component';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    VehicleComponent,
    VehicleSubmitComponent,
    BuyerdetailComponent,
    SellerdetailComponent,
    WitnessdetailComponent,
    VehicleFileuploadComponent,
    VehiclelistComponent,
    VehicleNationalidUploadComponent,
    VehicledetailsviewComponent
    
  ],
  imports: [
    CommonModule,
    VehicleRoutingModule,
    MatButtonModule,
    MatSidenavModule,
    MatMenuModule,
    MatToolbarModule,
    MatIconModule,
    MatListModule,
    MatExpansionModule,
    MatTooltipModule,
    MatInputModule,
    MatDividerModule,
    MatTabsModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    MatPaginatorModule,
    MatTableModule,
    NgxPaginationModule,
    MatDialogModule,
    SharedModule
  ]
})
export class VehicleModule { }
