import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { VBuyerDetail } from 'src/app/models/SellerDetail';
import { SellerService } from 'src/app/shared/seller.service';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { VehiclesubService } from 'src/app/shared/vehiclesub.service';
import { DuplicateCheckService } from 'src/app/shared/duplicate-check.service';
import { LocalizationService } from 'src/app/shared/localization.service';
import { ProfileImageCropperComponent } from 'src/app/shared/profile-image-cropper/profile-image-cropper.component';
import { environment } from 'src/environments/environment';
import { VehicleComponent } from '../../vehicle.component';

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
  duplicateError: string = '';
  isDuplicateCheckLoading: boolean = false;
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    console.log('onNextClick called - emitting next event');
    this.next.emit();
  }

  reset() {
    this.parentComponent.resetChild();
  }

  getRoleTypeLabel(roleType: string): string {
    const role = this.roleTypes?.find((r: any) => r?.value === roleType);
    return role ? role.label : roleType;
  }

  onEditSeller(id: number, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.BindValue(id);
  }

  get sellerDetails(): VBuyerDetail[] {
    return this.SellerDetails || [];
  }

  @ViewChild('childComponent') childComponent!: ProfileImageCropperComponent;
  private pendingImagePath: string = '';

  ngAfterViewInit(): void {
    // If we have a pending image path from ngOnInit, set it now
    if (this.pendingImagePath && this.childComponent) {
      this.childComponent.setExistingImage(this.pendingImagePath);
      this.pendingImagePath = '';
    }
  }
  constructor(private vehicleService: VehicleService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService, private vehiclesubservice:VehiclesubService,
    private localizationService: LocalizationService, private duplicateCheckService: DuplicateCheckService,
    private parentComponent: VehicleComponent){
    // console.log(propertyService.mainTableId);
    // this.mainTableId=propertyService.mainTableId;
    this.SellerForm = this.fb.group({
      id: [0],
      firstName: ['', Validators.required],
      fatherName: ['', Validators.required],
      grandFather: ['', Validators.required],
      electronicNationalIdNumber: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      paddressProvinceId: ['', Validators.required],
      paddressDistrictId: ['', Validators.required],
      paddressVillage: ['', Validators.required],
      taddressProvinceId: ['', Validators.required],
      taddressDistrictId: ['', Validators.required],
      taddressVillage: ['', Validators.required],
      propertyDetailsId:[''],
      photo:[''],
      nationalIdCard: ['', Validators.required],
      roleType: ['Seller', Validators.required],
      authorizationLetter: [''],
      heirsLetter: ['']
    });

    // Add dynamic validation for authorization letter and heirs letter based on roleType
    this.SellerForm.get('roleType')?.valueChanges.subscribe(roleType => {
      const authLetterControl = this.SellerForm.get('authorizationLetter');
      const heirsLetterControl = this.SellerForm.get('heirsLetter');
      
      // Check if it's an agent role (only Sales Agent for Vehicle module)
      const isAgent = roleType === 'Sales Agent';
      
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
    // Initialize role types - restricted to only 4 approved options for Vehicle module
    // فروشنده (Seller), فروشندگان (Sellers), وکیل فروش (Sales Agent), ورثه (Heirs)
    this.roleTypes = [
      this.localizationService.roleTypes.seller,      // فروشنده - single seller
      this.localizationService.roleTypes.sellers,     // فروشندگان - multiple sellers
      this.localizationService.roleTypes.sellerAgent, // وکیل فروش - single seller
      this.localizationService.roleTypes.heirs        // ورثه - multiple sellers
    ];
    
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    this.loadSellerDetails();
  }

  loadSellerDetails() {
    // Only fetch seller details if we have a valid ID
    if (!this.id || this.id === 0) {
      this.SellerDetails = [];
      this.selectedSellerId = 0;
      return;
    }
    
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
          electronicNationalIdNumber: firstSeller.electronicNationalIdNumber || '',
          phoneNumber: firstSeller.phoneNumber,
          propertyDetailsId: firstSeller.propertyDetailsId,
          paddressProvinceId: firstSeller.paddressProvinceId,
          paddressDistrictId: firstSeller.paddressDistrictId,
          paddressVillage: firstSeller.paddressVillage,
          taddressProvinceId: firstSeller.taddressProvinceId,
          taddressDistrictId: firstSeller.taddressDistrictId,
          taddressVillage: firstSeller.taddressVillage,
          photo:firstSeller.photo,
          nationalIdCard: firstSeller.nationalIdCard || '',
          roleType: firstSeller.roleType || 'Seller',
          authorizationLetter: firstSeller.authorizationLetter || '',
          heirsLetter: firstSeller.heirsLetter || '',
        });
        this.imagePath=this.baseUrl+firstSeller.photo;
        this.imageName=firstSeller.photo || '';
        this.nationalIdFileName=firstSeller.nationalIdCard || '';
        this.authorizationLetterName=firstSeller.authorizationLetter || '';
        this.heirsLetterName=firstSeller.heirsLetter || '';
        this.selectedSellerId=firstSeller.id;
        
        if (firstSeller.photo) {
          if (this.childComponent) {
            this.childComponent.setExistingImage(this.baseUrl + firstSeller.photo);
          } else {
            this.pendingImagePath = this.baseUrl + firstSeller.photo;
          }
        }
        
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
    vbuyerdetails.nationalIdCard = this.nationalIdFileName;
    vbuyerdetails.authorizationLetter=this.authorizationLetterName;
    vbuyerdetails.heirsLetter=this.heirsLetterName;
    vbuyerdetails.propertyDetailsId=this.vehicleService.mainTableId;
    if (vbuyerdetails.id === null) {
      vbuyerdetails.id = 0;
    }

    // Check for duplicate property registration
    this.isDuplicateCheckLoading = true;
    this.duplicateError = '';
    
    this.duplicateCheckService.checkDuplicateVehicleSeller(
      vbuyerdetails.firstName,
      vbuyerdetails.fatherName,
      vbuyerdetails.grandFather,
      vbuyerdetails.propertyDetailsId,
      vbuyerdetails.id
    ).subscribe(
      (response) => {
        this.isDuplicateCheckLoading = false;
        
        if (response.isDuplicate) {
          this.duplicateError = response.message;
          this.toastr.error(response.message);
          return;
        }

        // No duplicate found, proceed with saving
        this.vehiclesubservice.addSellerdetails(vbuyerdetails).subscribe(
          (result) => {
            if (result.id !== 0) {
              this.toastr.success("معلومات موفقانه ثبت شد");
              this.selectedSellerId=result.id;
              this.vehiclesubservice.udateSellerId(result.id);
              this.duplicateError = '';
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
      },
      (error) => {
        this.isDuplicateCheckLoading = false;
        this.toastr.error("خطا در بررسی تکراری");
      }
    );
}

updateSellerDetails(): void {
  const sellerDetails = this.SellerForm.value as VBuyerDetail;
  sellerDetails.photo=this.imageName;
  sellerDetails.nationalIdCard = this.nationalIdFileName;
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
      electronicNationalIdNumber: selectedSeller.electronicNationalIdNumber || '',
      phoneNumber: selectedSeller.phoneNumber,
      propertyDetailsId: selectedSeller.propertyDetailsId,
      paddressProvinceId: selectedSeller.paddressProvinceId,
      paddressDistrictId: selectedSeller.paddressDistrictId,
      paddressVillage: selectedSeller.paddressVillage,
      taddressProvinceId: selectedSeller.taddressProvinceId,
      taddressDistrictId: selectedSeller.taddressDistrictId,
      taddressVillage: selectedSeller.taddressVillage,
      photo: selectedSeller.photo,
      nationalIdCard: selectedSeller.nationalIdCard || '',
      roleType: selectedSeller.roleType || 'Seller',
      authorizationLetter: selectedSeller.authorizationLetter || '',
      heirsLetter: selectedSeller.heirsLetter || '',
    });
    this.imagePath = this.baseUrl + (selectedSeller.photo || 'assets/img/avatar.png');
    this.imageName = selectedSeller.photo || '';
    this.nationalIdFileName = selectedSeller.nationalIdCard || '';
    this.authorizationLetterName = selectedSeller.authorizationLetter || '';
    this.heirsLetterName = selectedSeller.heirsLetter || '';
    this.selectedSellerId = selectedSeller.id;
    
    if (selectedSeller.photo) {
      if (this.childComponent) {
        this.childComponent.setExistingImage(this.baseUrl + selectedSeller.photo);
      } else {
        this.pendingImagePath = this.baseUrl + selectedSeller.photo;
      }
    }
    
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

      this.imageName='';
      this.imagePath='assets/img/avatar.png';

      if (this.childComponent) {
        this.childComponent.reset();
      }
}
nationalIdUploadFinished = (event:string) => { 
  this.nationalIdFileName=event;
  this.SellerForm.patchValue({nationalIdCard: this.nationalIdFileName});
  console.log('National ID uploaded: '+event);
}

authorizationLetterUploadFinished = (event:string) => {
  this.authorizationLetterName=event;
  this.SellerForm.patchValue({ authorizationLetter: this.authorizationLetterName });
  console.log('Authorization Letter uploaded: '+event);
}

heirsLetterUploadFinished = (event:string) => {
  this.heirsLetterName=event;
  this.SellerForm.patchValue({ heirsLetter: this.heirsLetterName });
  console.log('Heirs Letter uploaded: '+event);
}

profilePreviewChanged = (localObjectUrl: string) => {
  if (localObjectUrl) {
    this.imagePath = localObjectUrl;
    return;
  }

  if (this.imageName) {
    this.imagePath = this.baseUrl + this.imageName;
    return;
  }

  this.imagePath = 'assets/img/avatar.png';
}

profileImageUploaded = (dbPath: string) => {
  this.imageName = dbPath || '';
  this.SellerForm.patchValue({ photo: this.imageName });
  this.imagePath = this.imageName ? (this.baseUrl + this.imageName) : 'assets/img/avatar.png';
}

isAuthorizedAgent(): boolean {
  const roleType = this.SellerForm.get('roleType')?.value;
  // Only Sales Agent for Vehicle module seller
  return roleType === 'Sales Agent';
}

// Check if role allows multiple sellers (فروشندگان or ورثه)
allowsMultipleSellers(): boolean {
  const roleType = this.SellerForm.get('roleType')?.value;
  return roleType === 'Sellers' || roleType === 'Heirs';
}

// Check if role allows only single seller (فروشنده or وکیل فروش)
isSingleSellerRole(): boolean {
  const roleType = this.SellerForm.get('roleType')?.value;
  return roleType === 'Seller' || roleType === 'Sales Agent';
}

// Check if can add more sellers based on role type
canAddMoreSellers(): boolean {
  if (this.allowsMultipleSellers()) {
    return true; // Multiple sellers allowed
  }
  // Single seller roles - check if already has a seller
  return this.SellerDetails?.length === 0;
}

isHeirs(): boolean {
  const roleType = this.SellerForm.get('roleType')?.value;
  return roleType === 'Heirs';
}

  get firstName() { return this.SellerForm.get('firstName'); }
  get fatherName() { return this.SellerForm.get('fatherName'); }
  get grandFather() { return this.SellerForm.get('grandFather'); }
  get electronicNationalIdNumber() { return this.SellerForm.get('electronicNationalIdNumber'); }
  get phoneNumber() { return this.SellerForm.get('phoneNumber'); }
  get paddressProvinceId() { return this.SellerForm.get('paddressProvinceId'); }
  get paddressDistrictId() { return this.SellerForm.get('paddressDistrictId'); }
  get paddressVillage() { return this.SellerForm.get('paddressVillage'); }
  get taddressProvinceId() { return this.SellerForm.get('taddressProvinceId'); }
  get transactionTypeId() { return this.SellerForm.get('transactionTypeId'); }
  get taddressDistrictId() { return this.SellerForm.get('taddressDistrictId'); }
  get taddressVillage() { return this.SellerForm.get('taddressVillage'); }
  get propertyDetailsId() { return this.SellerForm.get('propertyDetailsId'); }
  get nationalIdCard() { return this.SellerForm.get('nationalIdCard'); }
  get roleType() { return this.SellerForm.get('roleType'); }
  get authorizationLetter() { return this.SellerForm.get('authorizationLetter'); }
  get heirsLetter() { return this.SellerForm.get('heirsLetter'); }
}
