import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RealestateRoutingModule } from './realestate-routing.module';
import { RealestateComponent } from './realestate.component';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { CompanydetailsComponent } from './companydetails/companydetails.component';
import { CompanyownerComponent } from './companyowner/companyowner.component';
import { CompanyowneraddressComponent } from './companyowneraddress/companyowneraddress.component';
import { GuaranteeComponent } from './guarantee/guarantee.component';
import { GuaranatorsComponent } from './guaranators/guaranators.component';

import { LicensedetailsComponent } from './licensedetails/licensedetails.component';
import { FileuploadComponent } from './fileupload/fileupload.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReactiveFormsModule } from '@angular/forms';
import { RealestatelistComponent } from './realestatelist/realestatelist.component'; // Add this line
import { NgxPaginationModule } from 'ngx-pagination';
import { ExpiredlicenselistComponent } from './expiredlicenselist/expiredlicenselist.component';
@NgModule({
  declarations: [
    RealestateComponent,
    CompanydetailsComponent,
    CompanyownerComponent,
    CompanyowneraddressComponent,
    GuaranteeComponent,
    GuaranatorsComponent,
    LicensedetailsComponent,
    FileuploadComponent,
    RealestatelistComponent,
    ExpiredlicenselistComponent
  ],
  imports: [
    CommonModule,
    RealestateRoutingModule,
    MatIconModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatButtonModule,
    MatDividerModule,
    MatTableModule,
    MatTabsModule,
    NgSelectModule,
    FormsModule,
    NgbModule,
    NgxPaginationModule,
    ReactiveFormsModule
    
  ]
})
export class RealestateModule { }
