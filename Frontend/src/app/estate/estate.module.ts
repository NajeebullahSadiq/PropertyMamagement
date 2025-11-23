import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EstateRoutingModule } from './estate-routing.module';
import { EstateComponent } from './estate.component';
import { PropertydetailsComponent } from './propertydetails/propertydetails.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {MatDividerModule} from '@angular/material/divider';
import { MatMenuModule } from '@angular/material/menu';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import {MatInputModule} from '@angular/material/input';
import { MatTabsModule } from '@angular/material/tabs';
import { BuyerdetailComponent } from './propertydetails/buyerdetail/buyerdetail.component';
import { SellerdetailComponent } from './propertydetails/sellerdetail/sellerdetail.component';
import { WitnessdetailComponent } from './propertydetails/witnessdetail/witnessdetail.component';
import { PropertyaddressComponent } from './propertydetails/propertyaddress/propertyaddress.component';
import { FormsModule } from '@angular/forms';
import { UploadComponent } from './upload/upload.component';
import { NationalidUploadComponent } from './nationalid-upload/nationalid-upload.component';
import { ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { PropertydetailslistComponent } from './propertydetailslist/propertydetailslist.component';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatTableModule} from '@angular/material/table';
import { NgxPaginationModule } from 'ngx-pagination';

@NgModule({
  declarations: [
    EstateComponent,
    PropertydetailsComponent,
    BuyerdetailComponent,
    SellerdetailComponent,
    WitnessdetailComponent,
    PropertyaddressComponent,
    UploadComponent,
    NationalidUploadComponent,
    PropertydetailslistComponent
  ],
  imports: [
    CommonModule,
    EstateRoutingModule,
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
    NgxPaginationModule


  ]
})
export class EstateModule { }
