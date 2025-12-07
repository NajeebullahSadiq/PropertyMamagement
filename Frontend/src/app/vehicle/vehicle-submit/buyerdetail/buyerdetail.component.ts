import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { VBuyerDetail } from 'src/app/models/SellerDetail';
import { SellerService } from 'src/app/shared/seller.service';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { VehiclesubService } from 'src/app/shared/vehiclesub.service';
import { LocalizationService } from 'src/app/shared/localization.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-buyerdetail',
  templateUrl: './buyerdetail.component.html',
  styleUrls: ['./buyerdetail.component.scss']
})
export class BuyerdetailComponent {
  baseUrl:string=environment.apiURL+'/';
  imagePath:string='assets/img/avatar.png';
  imageName:string='';
  nationalIdFileName:string='';
  authorizationLetterName:string='';
  mainTableId:number=0;
  buyerForm: FormGroup = new FormGroup({});
  selectedbuyerId: number=0;
  buyerDetails!: VBuyerDetail[];
  province:any;
  district:any;
  district2:any;
  roleTypes: any;
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  constructor(private vehicleService: VehicleService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService, private vehiclesubservice:VehiclesubService,
    private localizationService: LocalizationService){
    // console.log(propertyService.mainTableId);
    // this.mainTableId=propertyService.mainTableId;
    this.buyerForm = this.fb.group({
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
      roleType: ['Buyer', Validators.required],
      authorizationLetter: ['']
    });

    // Add dynamic validation for authorization letter
    this.buyerForm.get('roleType')?.valueChanges.subscribe(roleType => {
      const authLetterControl = this.buyerForm.get('authorizationLetter');
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
      this.localizationService.roleTypes.buyer,
      this.localizationService.roleTypes.buyerAgent
    ];
    
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    this.loadBuyerDetails();
  }

  loadBuyerDetails() {
    this.vehiclesubservice.getBuyerById(this.id)
    .subscribe(sellers => {
      this.buyerDetails = sellers || [];
      if (sellers && sellers.length > 0) {
        // Load first buyer for editing if exists
        const firstBuyer = sellers[0];
        this.buyerForm.setValue({
          id: firstBuyer.id,
          firstName:firstBuyer.firstName,
          fatherName: firstBuyer.fatherName,
          grandFather: firstBuyer.grandFather,
          indentityCardNumber: firstBuyer.indentityCardNumber,
          phoneNumber: firstBuyer.phoneNumber,
          propertyDetailsId: firstBuyer.propertyDetailsId,
          paddressProvinceId: firstBuyer.paddressProvinceId,
          paddressDistrictId: firstBuyer.paddressDistrictId,
          paddressVillage: firstBuyer.paddressVillage,
          taddressProvinceId: firstBuyer.taddressProvinceId,
          taddressDistrictId: firstBuyer.taddressDistrictId,
          taddressVillage: firstBuyer.taddressVillage,
          photo:firstBuyer.photo,
          nationalIdCardPath: firstBuyer.nationalIdCardPath || '',
          roleType: firstBuyer.roleType || 'Buyer',
          authorizationLetter: firstBuyer.authorizationLetter || '',
        });
        this.imagePath=this.baseUrl+firstBuyer.photo;
        this.imageName=firstBuyer.photo || '';
        this.nationalIdFileName=firstBuyer.nationalIdCardPath || '';
        this.authorizationLetterName=firstBuyer.authorizationLetter || '';
        this.selectedbuyerId=firstBuyer.id;
        if (firstBuyer.paddressProvinceId) {
          this.selerService.getdistrict(firstBuyer.paddressProvinceId.valueOf()).subscribe(res => {
            this.district = res;
          });
        }
        if (firstBuyer.taddressProvinceId) {
          this.selerService.getdistrict(firstBuyer.taddressProvinceId.valueOf()).subscribe(res => {
            this.district2 = res;
          });
        }
        this.vehiclesubservice.buyerId=firstBuyer.id;
      } else {
        // No buyers yet, reset form
        this.buyerDetails = [];
        this.selectedbuyerId = 0;
      }
    });
  }
  addBuyerDetail(): void {
    const vbuyerdetails = this.buyerForm.value as VBuyerDetail;
    vbuyerdetails.photo=this.imageName;
    vbuyerdetails.nationalIdCardPath=this.nationalIdFileName;
    vbuyerdetails.authorizationLetter=this.authorizationLetterName;
    vbuyerdetails.propertyDetailsId=this.vehicleService.mainTableId;
    if (vbuyerdetails.id === null) {
      vbuyerdetails.id = 0;
    }
    this.vehiclesubservice.addBuyerdetails(vbuyerdetails).subscribe(
      (result) => {
        if (result.id !== 0) {
          this.toastr.success("معلومات موفقانه ثبت شد");
          this.vehiclesubservice.buyerId = result.id;
          this.selectedbuyerId=result.id;
          // Small delay to ensure database commit
          setTimeout(() => {
            this.loadBuyerDetails(); // Reload the list
            this.resetChild(); // Reset form for next entry
          }, 300);
        }
      },
      (error) => {
        if (error.status === 400) {
          this.toastr.error("خطا در ثبت معلومات");
        } else {
          this.toastr.error("An error occurred");
        }
      }
    );
}

updateBuyerDetails(): void {
  const sellerDetails = this.buyerForm.value as VBuyerDetail;
   sellerDetails.photo=this.imageName;
   sellerDetails.nationalIdCardPath=this.nationalIdFileName;
   sellerDetails.authorizationLetter=this.authorizationLetterName;
  if(sellerDetails.id===0 && this.selectedbuyerId!==0 || this.selectedbuyerId!==null){
    sellerDetails.id=this.selectedbuyerId;
    sellerDetails.propertyDetailsId=this.vehicleService.mainTableId;
  }
  this.vehiclesubservice.updateBuyerdetails(sellerDetails).subscribe(result => {
    if(result.id!==0) {
      this.toastr.info("معلومات موفقانه تغیر کرد");
      this.vehiclesubservice.udateBuyerId(result.id);
      this.loadBuyerDetails(); // Reload the list
      this.resetChild(); // Reset form
    }
 });
}

BindValue(id: number) {
  const selectedBuyer = this.buyerDetails.find(b => b.id === id);
  if (selectedBuyer) {
    this.buyerForm.patchValue({
      id: selectedBuyer.id,
      firstName: selectedBuyer.firstName,
      fatherName: selectedBuyer.fatherName,
      grandFather: selectedBuyer.grandFather,
      indentityCardNumber: selectedBuyer.indentityCardNumber,
      phoneNumber: selectedBuyer.phoneNumber,
      propertyDetailsId: selectedBuyer.propertyDetailsId,
      paddressProvinceId: selectedBuyer.paddressProvinceId,
      paddressDistrictId: selectedBuyer.paddressDistrictId,
      paddressVillage: selectedBuyer.paddressVillage,
      taddressProvinceId: selectedBuyer.taddressProvinceId,
      taddressDistrictId: selectedBuyer.taddressDistrictId,
      taddressVillage: selectedBuyer.taddressVillage,
      photo: selectedBuyer.photo,
      nationalIdCardPath: selectedBuyer.nationalIdCardPath || '',
      roleType: selectedBuyer.roleType || 'Buyer',
      authorizationLetter: selectedBuyer.authorizationLetter || '',
    });
    this.imagePath = this.baseUrl + (selectedBuyer.photo || 'assets/img/avatar.png');
    this.imageName = selectedBuyer.photo || '';
    this.nationalIdFileName = selectedBuyer.nationalIdCardPath || '';
    this.authorizationLetterName = selectedBuyer.authorizationLetter || '';
    this.selectedbuyerId = selectedBuyer.id;
    
    if (selectedBuyer.paddressProvinceId) {
      this.selerService.getdistrict(selectedBuyer.paddressProvinceId.valueOf()).subscribe(res => {
        this.district = res;
      });
    }
    if (selectedBuyer.taddressProvinceId) {
      this.selerService.getdistrict(selectedBuyer.taddressProvinceId.valueOf()).subscribe(res => {
        this.district2 = res;
      });
    }
  }
}

deleteBuyer(id: number) {
  if (confirm('آیا مطمئن هستید که می‌خواهید این خریدار را حذف کنید؟')) {
    this.vehiclesubservice.deleteBuyer(id).subscribe(
      () => {
        this.toastr.success("خریدار با موفقیت حذف شد");
        this.loadBuyerDetails();
        this.resetChild();
      },
      (error) => {
        this.toastr.error("خطا در حذف خریدار");
      }
    );
  }
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
      this.buyerForm.reset();
      this.buyerForm.patchValue({ roleType: 'Buyer' });
      this.selectedbuyerId=0;
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
  this.buyerForm.patchValue({nationalIdCardPath: this.nationalIdFileName});
  console.log('National ID uploaded: '+event);
}

authorizationLetterUploadFinished = (event:string) => {
  this.authorizationLetterName="Resources\\Images\\"+event;
  this.buyerForm.patchValue({ authorizationLetter: this.authorizationLetterName });
  console.log('Authorization Letter uploaded: '+event);
}

isAuthorizedAgent(): boolean {
  const roleType = this.buyerForm.get('roleType')?.value;
  return roleType && roleType.includes('Agent');
}

  get firstName() { return this.buyerForm.get('firstName'); }
  get fatherName() { return this.buyerForm.get('fatherName'); }
  get grandFather() { return this.buyerForm.get('grandFather'); }
  get indentityCardNumber() { return this.buyerForm.get('indentityCardNumber'); }
  get phoneNumber() { return this.buyerForm.get('phoneNumber'); }
  get paddressProvinceId() { return this.buyerForm.get('paddressProvinceId'); }
  get paddressDistrictId() { return this.buyerForm.get('paddressDistrictId'); }
  get paddressVillage() { return this.buyerForm.get('paddressVillage'); }
  get taddressProvinceId() { return this.buyerForm.get('taddressProvinceId'); }
  get transactionTypeId() { return this.buyerForm.get('transactionTypeId'); }
  get taddressDistrictId() { return this.buyerForm.get('taddressDistrictId'); }
  get taddressVillage() { return this.buyerForm.get('taddressVillage'); }
  get propertyDetailsId() { return this.buyerForm.get('propertyDetailsId'); }
  get photo() { return this.buyerForm.get('photo'); }
  get nationalIdCardPath() { return this.buyerForm.get('nationalIdCardPath'); }
  get roleType() { return this.buyerForm.get('roleType'); }
  get authorizationLetter() { return this.buyerForm.get('authorizationLetter'); }
}
