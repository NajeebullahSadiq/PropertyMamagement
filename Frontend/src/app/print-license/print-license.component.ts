import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../shared/auth.service';
import { PropertyService } from '../shared/property.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-print-license',
  templateUrl: './print-license.component.html',
  styleUrls: ['./print-license.component.scss']
})
export class PrintLicenseComponent {
  filePath:string='assets/img/avatar2.png';
  baseUrl=environment.apiURL+'/';
  data:any=[];

  constructor( public service: AuthService,private route: ActivatedRoute,private pservice:PropertyService) { 
  
  }
  ngOnInit(): void {
  
    const code = this.route.snapshot.paramMap.get('id');
    if (code) {
      this.pservice.getLicensePrintData(code).subscribe(res => {
        this.data = res;
        this.filePath=this.baseUrl+res.ownerPhoto;
       
      });
    } 
  }
}
