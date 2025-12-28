import { Component, Input, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { propertyAddress } from 'src/app/models/propertyAddress';
import { PropertyService } from 'src/app/shared/property.service';
import { SellerService } from 'src/app/shared/seller.service';
import { EstateComponent } from '../../estate.component';
import { BuyerdetailComponent } from '../buyerdetail/buyerdetail.component';
import { SellerdetailComponent } from '../sellerdetail/sellerdetail.component';
import { WitnessdetailComponent } from '../witnessdetail/witnessdetail.component';

@Component({
  selector: 'app-propertyaddress',
  templateUrl: './propertyaddress.component.html',
  styleUrls: ['./propertyaddress.component.scss']
})
export class PropertyaddressComponent {
  @ViewChild('propertySeller') propertySeller!: SellerdetailComponent;
  @ViewChild('propertyBuyer') propertyBuyer!: BuyerdetailComponent;
  @ViewChild('propertyWitness') propertyWitness!:WitnessdetailComponent;
  propertyAddressForm: FormGroup = new FormGroup({});
  selectedId: number=0;
  province:any;
  districts:any;
  propertyAddressDetails!: propertyAddress[];
  @Input() id: number=0;
  constructor(private propertyDetailsService: PropertyService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService,private parentComponent: EstateComponent,
    private router: Router)
    {
      this.propertyAddressForm = this.fb.group({
        id: [0],
        provinceId: ['', Validators.required],
        districtId: ['', Validators.required],
        village: ['', Validators.required]
      });
    }
    ngOnInit() {
      this.selerService.getprovince().subscribe(res => {
        this.province = res;
      });

      this.selerService.getPaddressById(this.id)
      .subscribe(addr => {
        this.propertyAddressDetails = addr;
        if (addr && addr.length > 0) {
          this.propertyAddressForm.setValue({
            id: addr[0].id,
            provinceId:addr[0].provinceId,
            districtId:addr[0].districtId,
            village:addr[0].village,
            
          
          });
          this.selerService.getdistrict(addr[0].provinceId.valueOf()).subscribe(res => {
            this.districts = res;
            
          });
          this.selectedId=addr[0].id;
        }
      
      });
     
    }
    addPropertyAddress(): void {
      const paddress = this.propertyAddressForm.value as propertyAddress;
      paddress.PropertyDetailsId=this.propertyDetailsService.mainTableId;
      if (paddress.id === null) {
        paddress.id = 0;
      }
      this.selerService.addPaddress(paddress).subscribe(result => {
        if(result.id!==0){
          this.toastr.success("معلومات این این معامله موفقانه ثبت و تکمیل گردید");

          this.propertyDetailsService.getPropertyPrintData(paddress.PropertyDetailsId).subscribe({
            next: () => {
              const tree = this.router.createUrlTree(['/print', paddress.PropertyDetailsId]);
              const url = tree.toString();
              const absoluteUrl = `${window.location.origin}${url.startsWith('/') ? url : `/${url}`}`;
              const newWindow = window.open(absoluteUrl, '_blank', 'noopener,noreferrer');
              if (newWindow) {
                newWindow.opener = null;
              } else {
                this.router.navigateByUrl(tree);
              }
            },
            error: (err) => {
              if (err?.status === 404) {
                this.toastr.info('دیتای چاپ پیدا نشد. ممکن است این معامله هنوز تکمیل نشده باشد.');
              } else if (err?.status === 401) {
                this.toastr.warning('جلسه شما ختم شده است. لطفاً دوباره وارد شوید.');
              } else {
                this.toastr.error('خطا در دریافت معلومات چاپ. لطفاً دوباره تلاش کنید.');
              }
            }
          });
        }
      });
      this.parentComponent.resetChild();
  }

  updatePropertyAddress(): void {
    const address = this.propertyAddressForm.value as propertyAddress;
     address.PropertyDetailsId=this.propertyDetailsService.mainTableId;
    this.selerService.updatePaddress(address).subscribe(result => {
      if(result.id!==0)
      this.toastr.info("معلومات موفقانه تغیر کرد");
     
   });
  }
  filterResults(getId:any) {
    
    this.selerService.getdistrict(getId.id).subscribe(res => {
      this.districts = res;
      
    });
  }
  resetChild(){
    this.propertyAddressForm.reset();
    this.id=0;
    this.selectedId=0;
  }

  get provinceId() { return this.propertyAddressForm.get('provinceId'); }
  get districtId() { return this.propertyAddressForm.get('districtId'); }
  get village() { return this.propertyAddressForm.get('village'); }
  get propertyDetailsId() { return this.propertyAddressForm.get('propertyDetailsId'); }
  
}
