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
  selector: 'app-sellerdetail',
  templateUrl: './sellerdetail.component.html',
  styleUrls: ['./sellerdetail.component.scss']
})
export class SellerdetailComponent {
  baseUrl:string=environment.apiURL+'/';
  imagePath:string='assets/img/avatar.png';
  imageName:string='';
  nationalIdFileName:string='';
  authorizationLetterName:string='';
  heirsLetterName:string='';
  mainTableId:number=0;
  SellerForm: FormGroup = new FormGroup({});
  selectedSellerId: number=0;
  SellerDetails!: VBuyerDetail[];
  province:any;
  district:any;
  district2:any;
  roleTypes: any[] = [];
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
      authorizationLetter: [''],
      heirsLetter: ['']
    });

    // Add dynamic validation for authorization letter and heirs letter based on roleType
    this.SellerForm.get('roleType')?.valueChanges.subscribe(roleType => {
      const authLetterControl = this.SellerForm.get('authorizationLetter');
      const heirsLetterControl = this.SellerForm.get('heirsLetter');
      
      // Check if it's an agent role (Sales Agent, Lease Agent, Agent for revocable sale)
      const agentRoles = ['Sales Agent', 'Lease Agent', 'Agent for a revocable sale'];
      const isAgent = agentRoles.includes(roleType);
      
      // Check if it's heirs role
      const isHeirs = roleType === 'Heirs';
      
      // Set validation for authorization letter (required for agents)
      if (isAgent) {
        authLetterControl?.setValidators([Validators.required]);
      } else {
        authLetterControl?.clearValidators();
      }
      
      // Set validation for heirs letter (required for heirs)
      if (isHeirs) {
        heirsLetterControl?.setValidators([Validators.required]);
      } else {
        heirsLetterControl?.clearValidators();
      }
      
      authLetterControl?.updateValueAndValidity();
      heirsLetterControl?.updateValueAndValidity();
    });
  }
  ngOnInit() {
    // Initialize role types from localization service
    this.roleTypes = [
      this.localizationService.roleTypes.seller,
      this.localizationService.roleTypes.sellers,
      this.localizationService.roleTypes.lessor,
      this.localizationService.roleTypes.revocableSaleSeller,
      this.localizationService.roleTypes.heirs,
      this.localizationService.roleTypes.sellerAgent,
      this.localizationService.roleTypes.leaseAgent,
      this.localizationService.roleTypes.revocableSaleAgent
    ];
    
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    this.loadSellerDetails();
  }

  loadSellerDetails() {
    this.vehiclesubservice.getSellerById(this.id)
    .subscribe(sellers => {
      this.SellerDetails = sellers || [];
      if (sellers && sellers.length > 0) {
        // Load first seller for editing if exists
        const firstSeller = sellers[0];
        this.SellerForm.setValue({
          id: firstSeller.id,
          firstName:firstSeller.firstName,
          fatherName: firstSeller.fatherName,
          grandFather: firstSeller.grandFather,
          indentityCardNumber: firstSeller.indentityCardNumber,
          phoneNumber: firstSeller.phoneNumber,
          propertyDetailsId: firstSeller.propertyDetailsId,
          paddressProvinceId: firstSeller.paddressProvinceId,
          paddressDistrictId: firstSeller.paddressDistrictId,
          paddressVillage: firstSeller.paddressVillage,
          taddressProvinceId: firstSeller.taddressProvinceId,
          taddressDistrictId: firstSeller.taddressDistrictId,
          taddressVillage: firstSeller.taddressVillage,
          photo:firstSeller.photo,
          nationalIdCardPath: firstSeller.nationalIdCardPath || '',
          roleType: firstSeller.roleType || 'Seller',
          authorizationLetter: firstSeller.authorizationLetter || '',
          heirsLetter: firstSeller.heirsLetter || '',
        });
        this.imagePath=this.baseUrl+firstSeller.photo;
        this.imageName=firstSeller.photo || '';
        this.nationalIdFileName=firstSeller.nationalIdCardPath || '';
        this.authorizationLetterName=firstSeller.authorizationLetter || '';
        this.heirsLetterName=firstSeller.heirsLetter || '';
        this.selectedSellerId=firstSeller.id;
        if (firstSeller.paddressProvinceId) {
          this.selerService.getdistrict(firstSeller.paddressProvinceId.valueOf()).subscribe(res => {
            this.district = res;
          });
        }
        if (firstSeller.taddressProvinceId) {
          this.selerService.getdistrict(firstSeller.taddressProvinceId.valueOf()).subscribe(res => {
            this.district2 = res;
          });
        }
        this.vehiclesubservice.sellerId=firstSeller.id;
      } else {
        // No sellers yet, reset form
        this.SellerDetails = [];
        this.selectedSellerId = 0;
      }
    });
  }
  addSellerDetail(): void {
    const vbuyerdetails = this.SellerForm.value as VBuyerDetail;
    vbuyerdetails.photo=this.imageName;
    vbuyerdetails.nationalIdCardPath=this.nationalIdFileName;
    vbuyerdetails.authorizationLetter=this.authorizationLetterName;
    vbuyerdetails.heirsLetter=this.heirsLetterName;
    vbuyerdetails.propertyDetailsId=this.vehicleService.mainTableId;
    if (vbuyerdetails.id === null) {
      vbuyerdetails.id = 0;
    }
    this.vehiclesubservice.addSellerdetails(vbuyerdetails).subscribe(
      (result) => {
        if (result.id !== 0) {
          this.toastr.success("معلومات موفقانه ثبت شد");
          this.selectedSellerId=result.id;
          this.vehiclesubservice.udateSellerId(result.id);
          // Reload the list immediately
          this.loadSellerDetails();
          this.resetChild(); // Reset form for next entry
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

updateSellerDetails(): void {
  const sellerDetails = this.SellerForm.value as VBuyerDetail;
  sellerDetails.photo=this.imageName;
  sellerDetails.nationalIdCardPath=this.nationalIdFileName;
  sellerDetails.authorizationLetter=this.authorizationLetterName;
  sellerDetails.heirsLetter=this.heirsLetterName;
  if(sellerDetails.id===0 && this.selectedSellerId!==0 || this.selectedSellerId!==null){
    sellerDetails.id=this.selectedSellerId;
    sellerDetails.propertyDetailsId=this.vehicleService.mainTableId;
  }
  this.vehiclesubservice.updateSellerdetails(sellerDetails).subscribe(result => {
    if(result.id!==0) {
      this.toastr.info("معلومات موفقانه تغیر کرد");
      this.vehiclesubservice.udateSellerId(result.id);
      this.loadSellerDetails(); // Reload the list
      this.resetChild(); // Reset form
    }
 });
}

 BindValue(id: number) {
  const selectedSeller = this.SellerDetails.find(s => s.id === id);
  if (selectedSeller) {
    this.SellerForm.patchValue({
      id: selectedSeller.id,
      firstName: selectedSeller.firstName,
      fatherName: selectedSeller.fatherName,
      grandFather: selectedSeller.grandFather,
      indentityCardNumber: selectedSeller.indentityCardNumber,
      phoneNumber: selectedSeller.phoneNumber,
      propertyDetailsId: selectedSeller.propertyDetailsId,
      paddressProvinceId: selectedSeller.paddressProvinceId,
      paddressDistrictId: selectedSeller.paddressDistrictId,
      paddressVillage: selectedSeller.paddressVillage,
      taddressProvinceId: selectedSeller.taddressProvinceId,
      taddressDistrictId: selectedSeller.taddressDistrictId,
      taddressVillage: selectedSeller.taddressVillage,
      photo: selectedSeller.photo,
      nationalIdCardPath: selectedSeller.nationalIdCardPath || '',
      roleType: selectedSeller.roleType || 'Seller',
      authorizationLetter: selectedSeller.authorizationLetter || '',
      heirsLetter: selectedSeller.heirsLetter || '',
    });
    this.imagePath = this.baseUrl + (selectedSeller.photo || 'assets/img/avatar.png');
    this.imageName = selectedSeller.photo || '';
    this.nationalIdFileName = selectedSeller.nationalIdCardPath || '';
    this.authorizationLetterName = selectedSeller.authorizationLetter || '';
    this.heirsLetterName = selectedSeller.heirsLetter || '';
    this.selectedSellerId = selectedSeller.id;
    
    if (selectedSeller.paddressProvinceId) {
      this.selerService.getdistrict(selectedSeller.paddressProvinceId.valueOf()).subscribe(res => {
        this.district = res;
      });
    }
    if (selectedSeller.taddressProvinceId) {
      this.selerService.getdistrict(selectedSeller.taddressProvinceId.valueOf()).subscribe(res => {
        this.district2 = res;
      });
    }
  }
}

deleteSeller(id: number) {
  if (confirm('آیا مطمئن هستید که می‌خواهید این فروشنده را حذف کنید؟')) {
    this.vehiclesubservice.deleteSeller(id).subscribe(
      () => {
        this.toastr.success("فروشنده با موفقیت حذف شد");
        this.loadSellerDetails();
        this.resetChild();
      },
      (error) => {
        this.toastr.error("خطا در حذف فروشنده");
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
      this.SellerForm.reset();
      this.SellerForm.patchValue({ roleType: 'Seller' });
      this.selectedSellerId=0;
      this.nationalIdFileName='';
      this.authorizationLetterName='';
      this.heirsLetterName='';
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

heirsLetterUploadFinished = (event:string) => {
  this.heirsLetterName="Resources\\Images\\"+event;
  this.SellerForm.patchValue({ heirsLetter: this.heirsLetterName });
  console.log('Heirs Letter uploaded: '+event);
}

isAuthorizedAgent(): boolean {
  const roleType = this.SellerForm.get('roleType')?.value;
  const agentRoles = ['Sales Agent', 'Lease Agent', 'Agent for a revocable sale'];
  return agentRoles.includes(roleType);
}

isHeirs(): boolean {
  const roleType = this.SellerForm.get('roleType')?.value;
  return roleType === 'Heirs';
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
  get heirsLetter() { return this.SellerForm.get('heirsLetter'); }
}
