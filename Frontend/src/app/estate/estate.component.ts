import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { MatTabGroup } from '@angular/material/tabs';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { PropertyService } from '../shared/property.service';
import { SellerService } from '../shared/seller.service';
import { BuyerdetailComponent } from './propertydetails/buyerdetail/buyerdetail.component';
import { PropertydetailsComponent } from './propertydetails/propertydetails.component';
import { SellerdetailComponent } from './propertydetails/sellerdetail/sellerdetail.component';
import { WitnessdetailComponent } from './propertydetails/witnessdetail/witnessdetail.component';

@Component({
  selector: 'app-estate',
  templateUrl: './estate.component.html',
  styleUrls: ['./estate.component.scss']
})
export class EstateComponent {
  @ViewChild(MatTabGroup) tabGroup!: MatTabGroup;
  PropertyId: number=0;
  @ViewChild('propertyDetails') propertyDetails!:PropertydetailsComponent;
  @ViewChild('propertySeller') propertySeller!: SellerdetailComponent;
  @ViewChild('propertyBuyer') propertyBuyer!: BuyerdetailComponent;
  @ViewChild('propertyWitness') propertyWitness!:WitnessdetailComponent;
 
 
  constructor(public propertyService: PropertyService,public sellerService:SellerService,private toastr: ToastrService,private route: ActivatedRoute){
    console.log(propertyService.mainTableId, sellerService.sellerId,sellerService.buyerId, sellerService.withnessId);
   
  }
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

  onTabChange(event: any) {
    const index = event.index;
    if (index === 0) { // Property tab
      if (this.propertyDetails) {
        this.propertyDetails.loadPropertyDetails();
      }
    } else if (index === 1) { // Seller tab
      if (this.propertySeller) {
        this.propertySeller.loadSellerDetails();
      }
    } else if (index === 2) { // Buyer tab
      if (this.propertyBuyer) {
        this.propertyBuyer.loadBuyerDetails();
      }
    } else if (index === 3) { // Witness tab
      if (this.propertyWitness) {
        this.propertyWitness.loadWitnessDetails();
      }
    }
  }
  
  resetChild(){
    this.propertySeller.resetChild();
    this.propertyBuyer.resetChild();
    this.propertyWitness.resetChild();
    this.propertyDetails.resetChild();
    this.propertyWitness.resetlist();

  }
  onChildEvent(event: string) {
    if (event === 'resetChild') {
      this.resetChild();
    }
  }
}
