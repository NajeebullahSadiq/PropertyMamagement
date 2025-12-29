import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { VBuyerDetail } from 'src/app/models/SellerDetail';
import { SellerService } from 'src/app/shared/seller.service';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { VehiclesubService } from 'src/app/shared/vehiclesub.service';
import { DuplicateCheckService } from 'src/app/shared/duplicate-check.service';
import { LocalizationService } from 'src/app/shared/localization.service';
import { PropertyService } from 'src/app/shared/property.service';
import { ProfileImageCropperComponent } from 'src/app/shared/profile-image-cropper/profile-image-cropper.component';
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
  propertyTypes:any;
  localizedPropertyTypes:any;
  transactionTypes:any;
  roleTypes: any = [];
  duplicateError: string = '';
  isDuplicateCheckLoading: boolean = false;
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }

  @ViewChild('childComponent') childComponent!: ProfileImageCropperComponent;

  ngAfterViewInit(): void {
    if (this.childComponent) {
      this.childComponent.reset();
    }
  }
  constructor(private vehicleService: VehicleService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService, private vehiclesubservice:VehiclesubService,
    private localizationService: LocalizationService, private propertyDetailsService: PropertyService,
    private duplicateCheckService: DuplicateCheckService){
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
      nationalIdCard:['', Validators.required],
      roleType: ['Buyer', Validators.required],
      authorizationLetter: [''],
      propertyTypeId: [''],
      customPropertyType: [''],
      price: [''],
      priceText: [''],
      royaltyAmount: [''],
      halfPrice: [''],
      rentStartDate: [''],
      rentEndDate: [''],
      transactionType: [''],
      transactionTypeDescription: ['']
    });

    // Add dynamic validation for authorization letter based on agent roles
    this.buyerForm.get('roleType')?.valueChanges.subscribe(roleType => {
      const authLetterControl = this.buyerForm.get('authorizationLetter');
      const rentStartDateControl = this.buyerForm.get('rentStartDate');
      const rentEndDateControl = this.buyerForm.get('rentEndDate');
      
      // Agent roles that require Power of Attorney (وکالت‌نامه)
      const agentRoles = [
        'Purchase Agent',
        'Agent for buyer in a revocable sale',
        'Agent for lessee'
      ];
      
      // Lessee roles that require rental dates
      const lesseeRoles = [
        'Lessee',
        'Agent for lessee'
      ];
      
      if (roleType && agentRoles.includes(roleType)) {
        authLetterControl?.setValidators([Validators.required]);
      } else {
        authLetterControl?.clearValidators();
      }
      authLetterControl?.updateValueAndValidity();

      // Set rental date validation based on lessee roles
      if (roleType && lesseeRoles.includes(roleType)) {
        rentStartDateControl?.setValidators([Validators.required]);
        rentEndDateControl?.setValidators([Validators.required]);
      } else {
        rentStartDateControl?.clearValidators();
        rentEndDateControl?.clearValidators();
        rentStartDateControl?.reset();
        rentEndDateControl?.reset();
      }
      rentStartDateControl?.updateValueAndValidity();
      rentEndDateControl?.updateValueAndValidity();
    });

    // Add dynamic validation for transaction type description when "Other" is selected
    this.buyerForm.get('transactionType')?.valueChanges.subscribe(transactionType => {
      const descriptionControl = this.buyerForm.get('transactionTypeDescription');
      
      if (transactionType === 'Other') {
        descriptionControl?.setValidators([Validators.required]);
      } else {
        descriptionControl?.clearValidators();
        descriptionControl?.reset();
      }
      descriptionControl?.updateValueAndValidity();
    });

    // Add dynamic validation for custom property type when "Other" is selected
    this.buyerForm.get('propertyTypeId')?.valueChanges.subscribe(propertyTypeId => {
      const customPropertyTypeControl = this.buyerForm.get('customPropertyType');
      
      // Find if the selected property type is "Other"
      const selectedPropertyType = this.localizedPropertyTypes?.find((pt: any) => pt.id === propertyTypeId);
      
      if (selectedPropertyType && selectedPropertyType.name === 'سایر') {
        customPropertyTypeControl?.setValidators([Validators.required]);
      } else {
        customPropertyTypeControl?.clearValidators();
        customPropertyTypeControl?.reset();
      }
      customPropertyTypeControl?.updateValueAndValidity();
    });
  }
  ngOnInit() {
    // Initialize role types from localization service
    this.roleTypes = [
      this.localizationService.roleTypes.buyer,
      this.localizationService.roleTypes.revocableSaleBuyer,
      this.localizationService.roleTypes.lessee,
      this.localizationService.roleTypes.buyerAgent,
      this.localizationService.roleTypes.revocableSaleBuyerAgent,
      this.localizationService.roleTypes.leaseReceiverAgent
    ];
    
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    
    // Load transaction types from localization service
    this.transactionTypes = this.localizationService.transactionTypes;
    // Load property types
    this.propertyDetailsService.getPropertyType().subscribe(res => {
      this.propertyTypes = res;
      this.localizedPropertyTypes = this.mapPropertyTypesToLocalized(res as any[]);
    });
    // Add price change listener for half-price calculation
    this.buyerForm.get('price')?.valueChanges.subscribe(() => {
      this.calculateDerivedAmounts();
    });

    this.buyerForm.get('transactionType')?.valueChanges.subscribe(() => {
      this.calculateDerivedAmounts();
    });
    this.loadBuyerDetails();
  }

  private calculateDerivedAmounts(): void {
    const transactionType = this.buyerForm.get('transactionType')?.value;
    const rawPrice = this.buyerForm.get('price')?.value;
    const price = rawPrice === '' || rawPrice === null || rawPrice === undefined ? NaN : Number(rawPrice);

    if (!transactionType || transactionType === 'Other' || Number.isNaN(price) || price <= 0) {
      this.buyerForm.patchValue({ royaltyAmount: null, halfPrice: null }, { emitEvent: false });
      return;
    }

    const halfPrice = price / 2;

    let royaltyAmount: number | null = null;
    if (transactionType === 'Purchase' || transactionType === 'Revocable Sale') {
      royaltyAmount = price * 0.015;
    } else if (transactionType === 'Rent') {
      royaltyAmount = halfPrice;
    }

    this.buyerForm.patchValue({ royaltyAmount, halfPrice }, { emitEvent: false });
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
          nationalIdCard: firstBuyer.nationalIdCard || '',
          roleType: firstBuyer.roleType || 'Buyer',
          authorizationLetter: firstBuyer.authorizationLetter || '',
          propertyTypeId: firstBuyer.propertyTypeId || '',
          customPropertyType: firstBuyer.customPropertyType || '',
          price: firstBuyer.price || '',
          priceText: firstBuyer.priceText || '',
          royaltyAmount: firstBuyer.royaltyAmount || '',
          halfPrice: firstBuyer.halfPrice || '',
          rentStartDate: firstBuyer.rentStartDate || '',
          rentEndDate: firstBuyer.rentEndDate || '',
          transactionType: firstBuyer.transactionType || '',
          transactionTypeDescription: firstBuyer.transactionTypeDescription || '',
        });
        this.imagePath=this.baseUrl+firstBuyer.photo;
        this.imageName=firstBuyer.photo || '';
        this.nationalIdFileName=firstBuyer.nationalIdCard || '';
        this.authorizationLetterName=firstBuyer.authorizationLetter || '';
        this.selectedbuyerId=firstBuyer.id;
        
        if (this.childComponent && firstBuyer.photo) {
          this.childComponent.setExistingImage(this.baseUrl + firstBuyer.photo);
        }
        
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
    vbuyerdetails.nationalIdCard = this.nationalIdFileName;
    vbuyerdetails.authorizationLetter=this.authorizationLetterName;
    vbuyerdetails.propertyDetailsId=this.vehicleService.mainTableId;
    if (vbuyerdetails.id === null) {
      vbuyerdetails.id = 0;
    }

    // Check for duplicate property registration
    this.isDuplicateCheckLoading = true;
    this.duplicateError = '';
    
    this.duplicateCheckService.checkDuplicateVehicleBuyer(
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
        this.vehiclesubservice.addBuyerdetails(vbuyerdetails).subscribe(
          (result) => {
            if (result.id !== 0) {
              this.toastr.success("معلومات موفقانه ثبت شد");
              this.vehiclesubservice.buyerId = result.id;
              this.selectedbuyerId=result.id;
              this.duplicateError = '';
              // Reload the list immediately
              this.loadBuyerDetails();
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

updateBuyerDetails(): void {
  const sellerDetails = this.buyerForm.value as VBuyerDetail;
   sellerDetails.photo=this.imageName;
   sellerDetails.nationalIdCard = this.nationalIdFileName;
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
      nationalIdCard: selectedBuyer.nationalIdCard || '',
      roleType: selectedBuyer.roleType || 'Buyer',
      authorizationLetter: selectedBuyer.authorizationLetter || '',
      propertyTypeId: selectedBuyer.propertyTypeId || '',
      customPropertyType: selectedBuyer.customPropertyType || '',
      price: selectedBuyer.price || '',
      priceText: selectedBuyer.priceText || '',
      royaltyAmount: selectedBuyer.royaltyAmount || '',
      halfPrice: selectedBuyer.halfPrice || '',
      rentStartDate: selectedBuyer.rentStartDate || '',
      rentEndDate: selectedBuyer.rentEndDate || '',
      transactionType: selectedBuyer.transactionType || '',
      transactionTypeDescription: selectedBuyer.transactionTypeDescription || '',
    });
    this.imagePath = this.baseUrl + (selectedBuyer.photo || 'assets/img/avatar.png');
    this.imageName = selectedBuyer.photo || '';
    this.nationalIdFileName = selectedBuyer.nationalIdCard || '';
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
      this.imageName='';
      this.imagePath='assets/img/avatar.png';

      if (this.childComponent) {
        this.childComponent.reset();
      }
}
onlyNumberKey(event:any) {
  const keyCode = event.which || event.keyCode;
  const keyValue = String.fromCharCode(keyCode);

  if (/\D/.test(keyValue)) {
    event.preventDefault();
  }
}
nationalIdUploadFinished = (event:string) => { 
  this.nationalIdFileName=event;
  this.buyerForm.patchValue({nationalIdCard: this.nationalIdFileName});
  console.log('National ID uploaded: '+event);
}

authorizationLetterUploadFinished = (event:string) => {
  this.authorizationLetterName=event;
  this.buyerForm.patchValue({ authorizationLetter: this.authorizationLetterName });
  console.log('Authorization Letter uploaded: '+event);
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
  this.buyerForm.patchValue({ photo: this.imageName });
  this.imagePath = this.imageName ? (this.baseUrl + this.imageName) : 'assets/img/avatar.png';
}

isAuthorizedAgent(): boolean {
  const roleType = this.buyerForm.get('roleType')?.value;
  // Agent roles that require Power of Attorney (وکالت‌نامه)
  const agentRoles = [
    'Purchase Agent',
    'Agent for buyer in a revocable sale',
    'Agent for lessee'
  ];
  return roleType && agentRoles.includes(roleType);
}

/**
 * Map backend property types to localized versions with Dari labels
 */
mapPropertyTypesToLocalized(backendTypes: any[]): any[] {
  return backendTypes.map(type => {
    const localized = this.localizationService.propertyTypes.find(
      pt => pt.value.toLowerCase() === type.name.toLowerCase()
    );
    return {
      id: type.id,
      name: localized ? localized.label : type.name
    };
  });
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
  get nationalIdCard() { return this.buyerForm.get('nationalIdCard'); }
  get roleType() { return this.buyerForm.get('roleType'); }
  get authorizationLetter() { return this.buyerForm.get('authorizationLetter'); }
  get propertyTypeId() { return this.buyerForm.get('propertyTypeId'); }
  get price() { return this.buyerForm.get('price'); }
  get priceText() { return this.buyerForm.get('priceText'); }
  get royaltyAmount() { return this.buyerForm.get('royaltyAmount'); }
  get halfPrice() { return this.buyerForm.get('halfPrice'); }
  get rentStartDate() { return this.buyerForm.get('rentStartDate'); }
  get rentEndDate() { return this.buyerForm.get('rentEndDate'); }
  get transactionType() { return this.buyerForm.get('transactionType'); }
  get transactionTypeDescription() { return this.buyerForm.get('transactionTypeDescription'); }
  get customPropertyType() { return this.buyerForm.get('customPropertyType'); }

  isOtherTransactionType(): boolean {
    return this.buyerForm.get('transactionType')?.value === 'Other';
  }

  isOtherPropertyType(): boolean {
    const propertyTypeId = this.buyerForm.get('propertyTypeId')?.value;
    const selectedPropertyType = this.localizedPropertyTypes?.find((pt: any) => pt.id === propertyTypeId);
    return selectedPropertyType && selectedPropertyType.name === 'سایر';
  }

  isLesseeRole(): boolean {
    const roleType = this.buyerForm.get('roleType')?.value;
    const lesseeRoles = ['Lessee', 'Agent for lessee'];
    return roleType && lesseeRoles.includes(roleType);
  }
}
