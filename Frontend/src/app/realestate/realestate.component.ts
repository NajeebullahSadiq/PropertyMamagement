import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, ViewChild } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { MatTabGroup } from '@angular/material/tabs';
import { ActivatedRoute, Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { CompnaydetailService } from '../shared/compnaydetail.service';
import { CompanydetailsComponent } from './companydetails/companydetails.component';
import { CompanyownerComponent } from './companyowner/companyowner.component';
import { FileuploadComponent } from './fileupload/fileupload.component';
import { GuaranatorsComponent } from './guaranators/guaranators.component';
import { LicensedetailsComponent } from './licensedetails/licensedetails.component';

@Component({
  selector: 'app-realestate',
  templateUrl: './realestate.component.html',
  styleUrls: ['./realestate.component.scss']
})
export class RealestateComponent {
  PropertyId: number=0;
  @ViewChild('companydetails') companydetails!: CompanydetailsComponent;
  @ViewChild('companyowner') companyowner!: CompanyownerComponent;
  @ViewChild('guaranators') guaranators!:GuaranatorsComponent;
  @ViewChild('licensedetails') licensedetails!:LicensedetailsComponent;

  @ViewChild(MatTabGroup) tabGroup!: MatTabGroup;

  
  constructor(private observer: BreakpointObserver,private route: ActivatedRoute, public comservice:CompnaydetailService) {}
 
  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.PropertyId = params['id'] ? parseInt(params['id'], 10) : 0;
    });

  }
  nextTab() {
    const nextIndex = (this.tabGroup?.selectedIndex ?? 0) + 1;
    if (this.tabGroup) {
      const tabCount = this.tabGroup._tabs.length;
      this.tabGroup.selectedIndex = nextIndex % (tabCount || 1);
    }
  }
  
  resetChild(){
    this.companydetails.resetChild();
    this.companyowner.resetForms();
    this.guaranators.resetForms();
    this.licensedetails.resetForms();
  }
}
