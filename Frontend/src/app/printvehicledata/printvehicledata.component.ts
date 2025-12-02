import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../shared/auth.service';
import { PropertyService } from '../shared/property.service';
import { VehicleService } from '../shared/vehicle.service';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-printvehicledata',
  templateUrl: './printvehicledata.component.html',
  styleUrls: ['./printvehicledata.component.scss']
})
export class PrintvehicledataComponent implements OnInit {
  userDetails:any=[];
  SellerfilePath:string='assets/img/avatar2.png';
  BuyerfilePath:string='assets/img/avatar2.png';
  baseUrl=environment.apiURL+'/';
  documentData:any=[];

  constructor( public service: AuthService,private route: ActivatedRoute,private pservice:VehicleService) { 
  
  }

  ngOnInit(): void {
    this.service.getCurrentUserProfile().subscribe(
      res => {
        this.userDetails = res;
      },
      err => {
        console.log(err);
      },
    );
    const code = this.route.snapshot.paramMap.get('id');
    if (code) {
      this.pservice.getVehiclePropertyPrintData(code).subscribe(res => {
        this.documentData = res;
        this.SellerfilePath=this.baseUrl+res.sellerPhoto;
        this.BuyerfilePath=this.baseUrl+res.buyerPhoto;
       
      });
    }
  }
}
