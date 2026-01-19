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
import { MatDialogModule } from '@angular/material/dialog';
import { CompanydetailsComponent } from './companydetails/companydetails.component';
import { CompanyownerComponent } from './companyowner/companyowner.component';
import { CompanyowneraddressComponent } from './companyowneraddress/companyowneraddress.component';
import { GuaranatorsComponent } from './guaranators/guaranators.component';
import { AccountinfoComponent } from './accountinfo/accountinfo.component';
import { CancellationinfoComponent } from './cancellationinfo/cancellationinfo.component';
import { CompanydetailsviewComponent } from './companydetailsview/companydetailsview.component';

import { LicensedetailsComponent } from './licensedetails/licensedetails.component';
import { FileuploadComponent } from './fileupload/fileupload.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReactiveFormsModule } from '@angular/forms';
import { RealestatelistComponent } from './realestatelist/realestatelist.component'; // Add this line
import { NgxPaginationModule } from 'ngx-pagination';
import { ExpiredlicenselistComponent } from './expiredlicenselist/expiredlicenselist.component';
import { SharedModule } from '../shared/shared.module';
@NgModule({
  declarations: [
    RealestateComponent,
    CompanydetailsComponent,
    CompanyownerComponent,
    CompanyowneraddressComponent,
    GuaranatorsComponent,
    LicensedetailsComponent,
    FileuploadComponent,
    RealestatelistComponent,
    ExpiredlicenselistComponent,
    AccountinfoComponent,
    CancellationinfoComponent,
    CompanydetailsviewComponent
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
    MatDialogModule,
    NgSelectModule,
    FormsModule,
    NgbModule,
    NgxPaginationModule,
    ReactiveFormsModule,
    SharedModule
  ],
  exports: [
    FileuploadComponent
  ]
})
export class RealestateModule { }
