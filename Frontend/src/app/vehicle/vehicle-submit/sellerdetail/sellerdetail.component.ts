import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { VBuyerDetail } from 'src/app/models/SellerDetail';
import { SellerService } from 'src/app/shared/seller.service';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { VehiclesubService } from 'src/app/shared/vehiclesub.service';
import { LocalizationService } from 'src/app/shared/localization.service';

@Component({
  selector: 'app-sellerdetail',
  templateUrl: './sellerdetail.component.html',
  styleUrls: ['./sellerdetail.component.scss']
})
export class SellerdetailComponent {
  baseUrl:string='http://localhost:5143/';
  imagePath:string='assets/img/avatar.png';
  imageName:string='';
  nationalIdFileName:string='';
  authorizationLetterName:string='';
  mainTableId:number=0;
  SellerForm: FormGroup = new FormGroup({});
  selectedSellerId: number=0;
  SellerDetails!: VBuyerDetail[];
  province:any;
  district:any;
  district2:any;
  roleTypes: any;
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    console.log('onNextClick called - emitting next event');
    this.next.emit();
  }
  constructor(private vehicleService: VehicleService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService, private vehiclesubservice:VehiclesubService,
    private localizationService: LocalizationService){
    // console.log(propertyService.mainTableId);
    // this.mainTableId=propertyService.mainTableId;
    this.SellerForm = this.fb.group({
      id: [0],
      firstName: ['', Validators.required],
      fatherName: ['', Validators.required],
      grandFather: ['', Validators.required],
      indentityCardNumber: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      paddressProvinceId: ['', Validators.required],
      paddressDistrictId: ['', Validators.required],
      paddressVillage: ['', Validators.required],
      taddressProvinceId: ['', Validators.required],
      taddressDistrictId: ['', Validators.required],
      taddressVillage: ['', Validators.required],
      propertyDetailsId:[''],
      photo:[''],
      nationalIdCardPath:['', Validators.required],
      roleType: ['Seller', Validators.required],
      authorizationLetter: ['']
    });

    // Add dynamic validation for authorization letter
    this.SellerForm.get('roleType')?.valueChanges.subscribe(roleType => {
      const authLetterControl = this.SellerForm.get('authorizationLetter');
      if (roleType && roleType.includes('Agent')) {
        authLetterControl?.setValidators([Validators.required]);
      } else {
        authLetterControl?.clearValidators();
      }
      authLetterControl?.updateValueAndValidity();
    });
  }
  ngOnInit() {
    // Initialize role types from localization service
    this.roleTypes = [
      this.localizationService.roleTypes.seller,
      this.localizationService.roleTypes.sellerAgent
    ];
    
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    this.vehiclesubservice.getSellerById(this.id)
    .subscribe(sellers => {
      this.SellerDetails = sellers;
      this.SellerForm.setValue({
        id: sellers[0].id,
        firstName:sellers[0].firstName,
        fatherName: sellers[0].fatherName,
        grandFather: sellers[0].grandFather,
        indentityCardNumber: sellers[0].indentityCardNumber,
        phoneNumber: sellers[0].phoneNumber,
        propertyDetailsId: sellers[0].propertyDetailsId,
        paddressProvinceId: sellers[0].paddressProvinceId,
        paddressDistrictId: sellers[0].paddressDistrictId,
        paddressVillage: sellers[0].paddressVillage,
        taddressProvinceId: sellers[0].taddressProvinceId,
        taddressDistrictId: sellers[0].taddressDistrictId,
        taddressVillage: sellers[0].taddressVillage,
        photo:sellers[0].photo,
        nationalIdCardPath: sellers[0].nationalIdCardPath || '',
        roleType: sellers[0].roleType || 'Seller',
        authorizationLetter: sellers[0].authorizationLetter || '',
      });
      this.imagePath=this.baseUrl+sellers[0].photo;
      this.imageName=sellers.map(item => item.photo).toString();
      this.nationalIdFileName=sellers[0].nationalIdCardPath || '';
      this.authorizationLetterName=sellers[0].authorizationLetter || '';
      this.selectedSellerId=sellers[0].id;
      this.selerService.getdistrict(sellers[0].paddressProvinceId.valueOf()).subscribe(res => {
        this.district = res;
      });
      this.selerService.getdistrict(sellers[0].taddressProvinceId.valueOf()).subscribe(res => {
        this.district2 = res;
      });
      this.vehiclesubservice.sellerId=sellers[0].id;
    });
   
  }
  addSellerDetail(): void {
    console.log('Form valid:', this.SellerForm.valid);
    console.log('National ID file:', this.nationalIdFileName);
    const vbuyerdetails = this.SellerForm.value as VBuyerDetail;
    vbuyerdetails.photo=this.imageName;
    vbuyerdetails.nationalIdCardPath=this.nationalIdFileName;
    vbuyerdetails.authorizationLetter=this.authorizationLetterName;
    vbuyerdetails.propertyDetailsId=this.vehicleService.mainTableId;
    if (vbuyerdetails.id === null) {
      vbuyerdetails.id = 0;
    }
    console.log('Submitting seller details:', vbuyerdetails);
    this.vehiclesubservice.addSellerdetails(vbuyerdetails).subscribe(
      (result) => {
        console.log('API response:', result);
        if (result.id !== 0) {
          this.toastr.success("معلومات موفقانه ثبت شد");
          this.selectedSellerId=result.id;
          this.vehiclesubservice.udateSellerId(result.id);
          console.log('Seller ID updated to:', result.id);
          console.log('Calling onNextClick()');
          this.onNextClick();
         
        }
      },
      (error) => {
        if (error.status === 400) {
          this.toastr.error("به این معامله یک فروشنده قبلا ثبت شده");
        } else {
          this.toastr.error("An error occurred");
        }
      }
    );
}

updateSellerDetails(): void {
  const sellerDetails = this.SellerForm.value as VBuyerDetail;
  sellerDetails.photo=this.imageName;
  sellerDetails.nationalIdCardPath=this.nationalIdFileName;
  sellerDetails.authorizationLetter=this.authorizationLetterName;
  if(sellerDetails.id===0 && this.selectedSellerId!==0 || this.selectedSellerId!==null){
    sellerDetails.id=this.selectedSellerId;
    sellerDetails.propertyDetailsId=this.vehicleService.mainTableId;
  }
  this.vehiclesubservice.updateSellerdetails(sellerDetails).subscribe(result => {
    if(result.id!==0)
    this.toastr.info("معلومات موفقانه تغیر کرد");
    this.vehiclesubservice.udateSellerId(result.id);
    this.onNextClick();
 });
}
  filterResults(getId:any) {
    
    this.selerService.getdistrict(getId.id).subscribe(res => {
      this.district = res;
      
    });
  }
  filterResults2(getId:any) {
    
    this.selerService.getdistrict2(getId.id).subscribe(res => {
      this.district2 = res;
      
    });
  }
  resetChild(){
      this.SellerForm.reset();
      this.SellerForm.patchValue({ roleType: 'Seller' });
      this.selectedSellerId=0;
      this.nationalIdFileName='';
      this.authorizationLetterName='';
}
onlyNumberKey(event:any) {
  const keyCode = event.which || event.keyCode;
  const keyValue = String.fromCharCode(keyCode);

  if (/\D/.test(keyValue)) {
    event.preventDefault();
  }
}
uploadFinished = (event:string) => { 
  this.imageName="Resources\\Images\\"+event;
  this.imagePath=this.baseUrl+this.imageName;
  console.log(event+'=======================');
}

nationalIdUploadFinished = (event:string) => { 
  this.nationalIdFileName="Resources\\Images\\"+event;
  this.SellerForm.patchValue({nationalIdCardPath: this.nationalIdFileName});
  console.log('National ID uploaded: '+event);
}

authorizationLetterUploadFinished = (event:string) => {
  this.authorizationLetterName="Resources\\Images\\"+event;
  this.SellerForm.patchValue({ authorizationLetter: this.authorizationLetterName });
  console.log('Authorization Letter uploaded: '+event);
}

isAuthorizedAgent(): boolean {
  const roleType = this.SellerForm.get('roleType')?.value;
  return roleType && roleType.includes('Agent');
}

  get firstName() { return this.SellerForm.get('firstName'); }
  get fatherName() { return this.SellerForm.get('fatherName'); }
  get grandFather() { return this.SellerForm.get('grandFather'); }
  get indentityCardNumber() { return this.SellerForm.get('indentityCardNumber'); }
  get phoneNumber() { return this.SellerForm.get('phoneNumber'); }
  get paddressProvinceId() { return this.SellerForm.get('paddressProvinceId'); }
  get paddressDistrictId() { return this.SellerForm.get('paddressDistrictId'); }
  get paddressVillage() { return this.SellerForm.get('paddressVillage'); }
  get taddressProvinceId() { return this.SellerForm.get('taddressProvinceId'); }
  get transactionTypeId() { return this.SellerForm.get('transactionTypeId'); }
  get taddressDistrictId() { return this.SellerForm.get('taddressDistrictId'); }
  get taddressVillage() { return this.SellerForm.get('taddressVillage'); }
  get propertyDetailsId() { return this.SellerForm.get('propertyDetailsId'); }
  get nationalIdCardPath() { return this.SellerForm.get('nationalIdCardPath'); }
  get roleType() { return this.SellerForm.get('roleType'); }
  get authorizationLetter() { return this.SellerForm.get('authorizationLetter'); }
}
