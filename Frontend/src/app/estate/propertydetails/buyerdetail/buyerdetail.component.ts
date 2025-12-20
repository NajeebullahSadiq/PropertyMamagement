import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { SellerDetail } from 'src/app/models/SellerDetail';
import { PropertyService } from 'src/app/shared/property.service';
import { SellerService } from 'src/app/shared/seller.service';
import { DuplicateCheckService } from 'src/app/shared/duplicate-check.service';
import { UploadComponent } from '../../upload/upload.component';
import { NationalidUploadComponent } from '../../nationalid-upload/nationalid-upload.component';
import { ProfileImageCropperComponent } from 'src/app/shared/profile-image-cropper/profile-image-cropper.component';
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
  nationalIdCardName:string='';
  authorizationLetterName:string='';
  ainTableId:number=0;
  sellerForm: FormGroup = new FormGroup({});
  selectedSellerId: number=0;
  sellerDetails!: SellerDetail[];
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
  @ViewChild('nationalIdComponent') nationalIdComponent!: NationalidUploadComponent;
  @ViewChild('authLetterComponent') authLetterComponent!: UploadComponent;
  ngAfterViewInit(): void {
    if (this.childComponent) {
      // Child component is ready, call its reset method
      this.childComponent.reset();
    }
  }
  constructor(private propertyDetailsService: PropertyService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService, private localizationService: LocalizationService,
    private duplicateCheckService: DuplicateCheckService){
    // console.log(propertyService.mainTableId);
    // this.mainTableId=propertyService.mainTableId;
    this.sellerForm = this.fb.group({
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
    this.sellerForm.get('roleType')?.valueChanges.subscribe(roleType => {
      const authLetterControl = this.sellerForm.get('authorizationLetter');
      const rentStartDateControl = this.sellerForm.get('rentStartDate');
      const rentEndDateControl = this.sellerForm.get('rentEndDate');
      
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
    this.sellerForm.get('transactionType')?.valueChanges.subscribe(transactionType => {
      const descriptionControl = this.sellerForm.get('transactionTypeDescription');
      
      if (transactionType === 'Other') {
        descriptionControl?.setValidators([Validators.required]);
      } else {
        descriptionControl?.clearValidators();
        descriptionControl?.reset();
      }
      descriptionControl?.updateValueAndValidity();
    });

    // Add dynamic validation for custom property type when "Other" is selected
    this.sellerForm.get('propertyTypeId')?.valueChanges.subscribe(propertyTypeId => {
      const customPropertyTypeControl = this.sellerForm.get('customPropertyType');
      
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
    this.sellerForm.get('price')?.valueChanges.subscribe(price => {
      if (price) {
        const calculatedHalfPrice = price / 2;
        this.sellerForm.patchValue({ halfPrice: calculatedHalfPrice }, { emitEvent: false });
      }
    });
    this.loadBuyerDetails();
  }

  loadBuyerDetails() {
    this.selerService.getBuyerById(this.id)
    .subscribe(sellers => {
      this.sellerDetails = sellers || [];
      if (sellers && sellers.length > 0) {
        // Load first buyer for editing if exists
        const firstBuyer = sellers[0];
        this.sellerForm.setValue({
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
          nationalIdCard:firstBuyer.nationalIdCardPath || '',
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
        this.selectedSellerId=firstBuyer.id;
        this.imagePath=this.baseUrl+firstBuyer.photo;
        this.imageName=firstBuyer.photo || '';
        this.nationalIdCardName=firstBuyer.nationalIdCardPath || '';
        this.authorizationLetterName=firstBuyer.authorizationLetter || '';
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
        this.selerService.buyerId=firstBuyer.id;
      } else {
        // No buyers yet, reset form
        this.sellerDetails = [];
        this.selectedSellerId = 0;
      }
    });
  }
  addBuyerDetail(): void {
    const sellerDetails = this.sellerForm.value as SellerDetail;
    sellerDetails.photo=this.imageName;
    sellerDetails.nationalIdCardPath=this.nationalIdCardName;
    sellerDetails.authorizationLetter=this.authorizationLetterName;
    sellerDetails.propertyDetailsId=this.propertyDetailsService.mainTableId;
    if (sellerDetails.id === null) {
      sellerDetails.id = 0;
    }

    // Check for duplicate property registration
    this.isDuplicateCheckLoading = true;
    this.duplicateError = '';
    
    this.duplicateCheckService.checkDuplicateBuyer(
      sellerDetails.firstName,
      sellerDetails.fatherName,
      sellerDetails.grandFather,
      sellerDetails.propertyDetailsId,
      sellerDetails.id
    ).subscribe(
      (response) => {
        this.isDuplicateCheckLoading = false;
        
        if (response.isDuplicate) {
          this.duplicateError = response.message;
          this.toastr.error(response.message);
          return;
        }

        // No duplicate found, proceed with saving
        this.selerService.addBuyerdetails(sellerDetails).subscribe(
          (result) => {
            if (result.id !== 0) {
              this.toastr.success("معلومات موفقانه ثبت شد");
              this.selerService.buyerId = result.id;
              this.selectedSellerId=result.id;
              this.duplicateError = '';
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
      },
      (error) => {
        this.isDuplicateCheckLoading = false;
        this.toastr.error("خطا در بررسی تکراری");
      }
    );
}

updateBuyerDetails(): void {
  const sellerDetails = this.sellerForm.value as SellerDetail;
   sellerDetails.photo=this.imageName;
   sellerDetails.nationalIdCardPath=this.nationalIdCardName;
   sellerDetails.authorizationLetter=this.authorizationLetterName;
  if(sellerDetails.id===0 && this.selectedSellerId!==0 || this.selectedSellerId!==null){
    sellerDetails.id=this.selectedSellerId;
    sellerDetails.propertyDetailsId=this.propertyDetailsService.mainTableId;
  }
  this.selerService.updateBuyerdetails(sellerDetails).subscribe(result => {
    if(result.id!==0) {
      this.toastr.info("معلومات موفقانه تغیر کرد");
      this.selerService.udateBuyerId(result.id);
      this.loadBuyerDetails(); // Reload the list
      this.resetChild(); // Reset form
    }
 });
}

 BindValue(id: number) {
  const selectedBuyer = this.sellerDetails.find(b => b.id === id);
  if (selectedBuyer) {
    this.sellerForm.patchValue({
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
      nationalIdCard: selectedBuyer.nationalIdCardPath || '',
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
    this.nationalIdCardName = selectedBuyer.nationalIdCardPath || '';
    this.authorizationLetterName = selectedBuyer.authorizationLetter || '';
    this.selectedSellerId = selectedBuyer.id;
    
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
    this.selerService.deleteBuyer(id).subscribe(
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
    if (this.childComponent) {
      // Child component is available, reset it
      this.childComponent.reset();
    }
    if (this.nationalIdComponent) {
      // Reset National ID component
      this.nationalIdComponent.reset();
    }
    if (this.authLetterComponent) {
      // Reset authorization letter component
      this.authLetterComponent.reset();
    }
      this.sellerForm.reset();
      this.sellerForm.patchValue({ roleType: 'Buyer' });
      this.selectedSellerId=0;
      this.imagePath='assets/img/avatar.png';
      this.nationalIdCardName='';
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
  this.nationalIdCardName="Resources\\Images\\"+event;
  this.sellerForm.patchValue({ nationalIdCard: this.nationalIdCardName });
  console.log('National ID uploaded: '+event+'=======================');
}

authorizationLetterUploadFinished = (event:string) => {
  this.authorizationLetterName="Resources\\Images\\"+event;
  this.sellerForm.patchValue({ authorizationLetter: this.authorizationLetterName });
  console.log('Authorization Letter uploaded: '+event+'=======================');
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
  this.sellerForm.patchValue({ photo: this.imageName });
  this.imagePath = this.imageName ? (this.baseUrl + this.imageName) : 'assets/img/avatar.png';
}

isAuthorizedAgent(): boolean {
  const roleType = this.sellerForm.get('roleType')?.value;
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

  get firstName() { return this.sellerForm.get('firstName'); }
  get fatherName() { return this.sellerForm.get('fatherName'); }
  get grandFather() { return this.sellerForm.get('grandFather'); }
  get indentityCardNumber() { return this.sellerForm.get('indentityCardNumber'); }
  get phoneNumber() { return this.sellerForm.get('phoneNumber'); }
  get paddressProvinceId() { return this.sellerForm.get('paddressProvinceId'); }
  get paddressDistrictId() { return this.sellerForm.get('paddressDistrictId'); }
  get paddressVillage() { return this.sellerForm.get('paddressVillage'); }
  get taddressProvinceId() { return this.sellerForm.get('taddressProvinceId'); }
  get transactionTypeId() { return this.sellerForm.get('transactionTypeId'); }
  get taddressDistrictId() { return this.sellerForm.get('taddressDistrictId'); }
  get taddressVillage() { return this.sellerForm.get('taddressVillage'); }
  get propertyDetailsId() { return this.sellerForm.get('propertyDetailsId'); }
  get photo() { return this.sellerForm.get('photo'); }
  get nationalIdCard() { return this.sellerForm.get('nationalIdCard'); }
  get roleType() { return this.sellerForm.get('roleType'); }
  get authorizationLetter() { return this.sellerForm.get('authorizationLetter'); }
  get propertyTypeId() { return this.sellerForm.get('propertyTypeId'); }
  get price() { return this.sellerForm.get('price'); }
  get priceText() { return this.sellerForm.get('priceText'); }
  get royaltyAmount() { return this.sellerForm.get('royaltyAmount'); }
  get halfPrice() { return this.sellerForm.get('halfPrice'); }
  get rentStartDate() { return this.sellerForm.get('rentStartDate'); }
  get rentEndDate() { return this.sellerForm.get('rentEndDate'); }
  get transactionType() { return this.sellerForm.get('transactionType'); }
  get transactionTypeDescription() { return this.sellerForm.get('transactionTypeDescription'); }
  get customPropertyType() { return this.sellerForm.get('customPropertyType'); }

  isOtherTransactionType(): boolean {
    return this.sellerForm.get('transactionType')?.value === 'Other';
  }

  isOtherPropertyType(): boolean {
    const propertyTypeId = this.sellerForm.get('propertyTypeId')?.value;
    const selectedPropertyType = this.localizedPropertyTypes?.find((pt: any) => pt.id === propertyTypeId);
    return selectedPropertyType && selectedPropertyType.name === 'سایر';
  }

  isLesseeRole(): boolean {
    const roleType = this.sellerForm.get('roleType')?.value;
    const lesseeRoles = ['Lessee', 'Agent for lessee'];
    return roleType && lesseeRoles.includes(roleType);
  }
}
