import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VehicleService } from '../shared/vehicle.service';
import { VehiclesubService } from '../shared/vehiclesub.service';
import { BuyerdetailComponent } from './vehicle-submit/buyerdetail/buyerdetail.component';
import { SellerdetailComponent } from './vehicle-submit/sellerdetail/sellerdetail.component';
import { VehicleSubmitComponent } from './vehicle-submit/vehicle-submit.component';
import { WitnessdetailComponent } from './vehicle-submit/witnessdetail/witnessdetail.component';

@Component({
  selector: 'app-vehicle',
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.scss']
})
export class VehicleComponent {
  PropertyId: number=0;
  currentTab: number = 0;
  @ViewChild('propertyDetails') propertyDetails!: VehicleSubmitComponent;
  @ViewChild('propertySeller') propertySeller!: SellerdetailComponent;
  @ViewChild('propertyBuyer') propertyBuyer!: BuyerdetailComponent;
  @ViewChild('propertyWitness') propertyWitness!:WitnessdetailComponent;
  constructor(private route: ActivatedRoute,public vehicleService: VehicleService, public vehicleSubservice:VehiclesubService){ 
  }
  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.PropertyId = params['id'];
     
    });
  }
  
  setTab(index: number) {
    this.currentTab = index;
  }
  
  nextTab() {
    console.log('nextTab called');
    console.log('Current tab index:', this.currentTab);
    console.log('Seller ID:', this.vehicleSubservice.sellerId);
    const nextIndex = this.currentTab + 1;
    const tabCount = 4;
    console.log('Moving to tab index:', nextIndex);
    this.currentTab = nextIndex % tabCount;
  }
  
  resetChild(){
    this.propertyDetails.resetChild();
    this.propertySeller.resetChild();
   this.propertyBuyer.resetChild();
   this.propertyWitness.resetChild();
   this.currentTab = 0;
  
 
  }
}
