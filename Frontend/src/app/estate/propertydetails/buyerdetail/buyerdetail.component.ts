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
  private suppressTransactionTypeHandling: boolean = false;
  private lastTransactionType: string | null = null;
  ainTableId:number=0;
  sellerForm: FormGroup = new FormGroup({});
  selectedSellerId: number=0;
  sellerDetails!: SellerDetail[];
  province:any;
  district:any;
  district2:any;
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
  private pendingImagePath: string = '';

  onEditBuyer(id: number, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.BindValue(id);
  }
  ngAfterViewInit(): void {
    // If we have a pending image path from ngOnInit, set it now
    if (this.pendingImagePath && this.childComponent) {
      this.childComponent.setExistingImage(this.pendingImagePath);
      this.pendingImagePath = '';
    }
  }

  onSubmit(): void {
    if (this.sellerForm.invalid) {
      this.sellerForm.markAllAsTouched();
      const invalidControls = Object.keys(this.sellerForm.controls)
        .filter(key => this.sellerForm.get(key)?.invalid);
      console.log('Invalid buyerForm controls:', invalidControls);
      this.toastr.error('لطفاً تمام فیلد های الزامی را خانه پُری کنید');
      return;
    }

    if (this.selectedSellerId) {
      this.updateBuyerDetails();
    } else {
      this.addBuyerDetail();
    }
  }

  private normalizeTazkiraType(value: any, buyer?: any): string {
    const raw = (value ?? '').toString().trim();

    if (!raw) {
      const hasPaperFields = !!(buyer?.tazkiraVolume || buyer?.tazkiraPage || buyer?.tazkiraNumber);
      return hasPaperFields ? 'Paper' : 'Electronic';
    }

    const lower = raw.toLowerCase();
    if (lower === 'paper' || lower.includes('paper') || raw.includes('کاغذ')) {
      return 'Paper';
    }
    if (lower === 'electronic' || lower.includes('electronic') || raw.includes('الکترون')) {
      return 'Electronic';
    }

    if (raw === 'Paper' || raw === 'Electronic') {
      return raw;
    }

    return 'Electronic';
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
      tazkiraType: ['', Validators.required],
      tazkiraVolume: [''],
      tazkiraPage: [''],
      tazkiraNumber: [''],
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
      price: [''],
      priceText: [''],
      royaltyAmount: [''],
      halfPrice: [''],
      rentStartDate: [''],
      rentEndDate: [''],
      transactionType: ['', Validators.required],
      transactionTypeDescription: [''],
      taxIdentificationNumber: [''],
      additionalDetails: ['']
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
      const priceControl = this.sellerForm.get('price');
      const priceTextControl = this.sellerForm.get('priceText');

      const supportedTypes = ['Purchase', 'Rent', 'Revocable Sale'];
      const shouldResetPricePair =
        !this.suppressTransactionTypeHandling &&
        transactionType &&
        supportedTypes.includes(transactionType) &&
        this.lastTransactionType !== null &&
        this.lastTransactionType !== transactionType;

      if (shouldResetPricePair) {
        priceControl?.reset();
        priceTextControl?.reset();
        this.sellerForm.patchValue({ royaltyAmount: null, halfPrice: null }, { emitEvent: false });
      }

      this.updatePriceValidators(transactionType);
      
      if (transactionType === 'Other') {
        descriptionControl?.setValidators([Validators.required]);

        if (!this.suppressTransactionTypeHandling) {
          priceControl?.reset();
          priceTextControl?.reset();
          this.sellerForm.patchValue({ royaltyAmount: null, halfPrice: null }, { emitEvent: false });
        }
      } else {
        descriptionControl?.clearValidators();
        descriptionControl?.reset();
      }
      descriptionControl?.updateValueAndValidity();

      if (!this.suppressTransactionTypeHandling) {
        this.calculateDerivedAmounts();
      }

      this.lastTransactionType = transactionType || null;
    });

    this.sellerForm.get('tazkiraType')?.valueChanges.subscribe(tazkiraType => {
      const volumeControl = this.sellerForm.get('tazkiraVolume');
      const pageControl = this.sellerForm.get('tazkiraPage');
      const numberControl = this.sellerForm.get('tazkiraNumber');
      
      if (tazkiraType === 'Paper') {
        volumeControl?.setValidators([Validators.required]);
        pageControl?.setValidators([Validators.required]);
        numberControl?.setValidators([Validators.required]);
      } else {
        volumeControl?.clearValidators();
        pageControl?.clearValidators();
        numberControl?.clearValidators();
        volumeControl?.reset();
        pageControl?.reset();
        numberControl?.reset();
      }
      
      volumeControl?.updateValueAndValidity();
      pageControl?.updateValueAndValidity();
      numberControl?.updateValueAndValidity();
    });
  }
  ngOnInit() {
    // Initialize role types from localization service
    this.roleTypes = [
      this.localizationService.roleTypes.buyer,
      this.localizationService.roleTypes.buyers,
      this.localizationService.roleTypes.revocableSaleBuyer,
      this.localizationService.roleTypes.revocableSaleBuyers,
      this.localizationService.roleTypes.lessee,
      this.localizationService.roleTypes.lessees,
      this.localizationService.roleTypes.buyerAgent,
      this.localizationService.roleTypes.revocableSaleBuyerAgent,
      this.localizationService.roleTypes.leaseReceiverAgent
    ];
    
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    
    // Load transaction types from localization service
    this.transactionTypes = this.localizationService.transactionTypes;

    // Recalculate derived amounts whenever the numeric price changes
    this.sellerForm.get('price')?.valueChanges.subscribe(() => {
      this.calculateDerivedAmounts();
    });

    this.loadBuyerDetails();
  }

  loadBuyerDetails() {
    const effectiveId = this.id > 0 ? this.id : this.propertyDetailsService.mainTableId;
    if (effectiveId === 0) {
      this.sellerDetails = [];
      this.selectedSellerId = 0;
      return;
    }
    this.selerService.getBuyerById(effectiveId)
    .subscribe(sellers => {
      this.sellerDetails = sellers || [];
      if (sellers && sellers.length > 0) {
        // Load first buyer for editing if exists
        const firstBuyer = sellers[0];
        const normalizedTazkiraType = this.normalizeTazkiraType((firstBuyer as any).tazkiraType, firstBuyer);
        this.suppressTransactionTypeHandling = true;
        this.sellerForm.setValue({
          id: firstBuyer.id,
          firstName:firstBuyer.firstName,
          fatherName: firstBuyer.fatherName,
          grandFather: firstBuyer.grandFather,
          indentityCardNumber: firstBuyer.indentityCardNumber,
          tazkiraType: normalizedTazkiraType,
          tazkiraVolume: firstBuyer.tazkiraVolume || '',
          tazkiraPage: firstBuyer.tazkiraPage || '',
          tazkiraNumber: firstBuyer.tazkiraNumber || '',
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
          price: firstBuyer.price || '',
          priceText: firstBuyer.priceText || '',
          royaltyAmount: firstBuyer.royaltyAmount || '',
          halfPrice: firstBuyer.halfPrice || '',
          rentStartDate: firstBuyer.rentStartDate || '',
          rentEndDate: firstBuyer.rentEndDate || '',
          transactionType: firstBuyer.transactionType || '',
          transactionTypeDescription: firstBuyer.transactionTypeDescription || '',
          taxIdentificationNumber: (firstBuyer as any).taxIdentificationNumber || '',
          additionalDetails: (firstBuyer as any).additionalDetails || '',
        });
        this.suppressTransactionTypeHandling = false;
        this.updatePriceValidators(this.sellerForm.get('transactionType')?.value);
        this.calculateDerivedAmounts();
        this.selectedSellerId=firstBuyer.id;
        this.imagePath=this.baseUrl+firstBuyer.photo;
        this.imageName=firstBuyer.photo || '';
        this.nationalIdCardName=firstBuyer.nationalIdCard || '';
        this.authorizationLetterName=firstBuyer.authorizationLetter || '';
        
        if (firstBuyer.photo) {
          if (this.childComponent) {
            this.childComponent.setExistingImage(this.baseUrl + firstBuyer.photo);
          } else {
            this.pendingImagePath = this.baseUrl + firstBuyer.photo;
          }
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
    sellerDetails.nationalIdCard = this.nationalIdCardName;
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

                // Auto-redirect to next step for singular roles
                if (this.isSingularRole()) {
                  this.onNextClick();
                }
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
   sellerDetails.nationalIdCard = this.nationalIdCardName;
   sellerDetails.authorizationLetter=this.authorizationLetterName;

  const effectivePropertyId = this.id > 0 ? this.id : this.propertyDetailsService.mainTableId;
  if (!(sellerDetails as any).propertyDetailsId && effectivePropertyId) {
    (sellerDetails as any).propertyDetailsId = effectivePropertyId;
  }

  if ((!sellerDetails.id || sellerDetails.id === 0) && this.selectedSellerId) {
    sellerDetails.id = this.selectedSellerId;
  }

  this.selerService.updateBuyerdetails(sellerDetails).subscribe({
    next: (result: any) => {
      if(result.id!==0) {
        this.toastr.info("معلومات موفقانه تغیر کرد");
        this.selerService.udateBuyerId(result.id);
        this.loadBuyerDetails(); // Reload the list
        this.resetChild(); // Reset form
      }
    },
    error: (error) => {
      if (error?.status === 400) {
        this.toastr.error(error?.error || 'خطا در تغیر معلومات');
      } else if (error?.status === 401) {
        this.toastr.warning('جلسه شما ختم شده است. لطفاً دوباره وارد شوید.');
      } else {
        this.toastr.error('خطا در تغیر معلومات');
      }
      console.log('Update buyer error:', error);
    }
  });
}

 BindValue(id: number) {
  const selectedBuyer = this.sellerDetails.find(b => b.id === id);
  if (selectedBuyer) {
    const normalizedTazkiraType = this.normalizeTazkiraType((selectedBuyer as any).tazkiraType, selectedBuyer);
    this.suppressTransactionTypeHandling = true;
    this.sellerForm.patchValue({
      id: selectedBuyer.id,
      firstName: selectedBuyer.firstName,
      fatherName: selectedBuyer.fatherName,
      grandFather: selectedBuyer.grandFather,
      indentityCardNumber: selectedBuyer.indentityCardNumber,
      tazkiraType: normalizedTazkiraType,
      tazkiraVolume: selectedBuyer.tazkiraVolume || '',
      tazkiraPage: selectedBuyer.tazkiraPage || '',
      tazkiraNumber: selectedBuyer.tazkiraNumber || '',
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
      price: selectedBuyer.price || '',
      priceText: selectedBuyer.priceText || '',
      royaltyAmount: selectedBuyer.royaltyAmount || '',
      halfPrice: selectedBuyer.halfPrice || '',
      rentStartDate: selectedBuyer.rentStartDate || '',
      rentEndDate: selectedBuyer.rentEndDate || '',
      transactionType: selectedBuyer.transactionType || '',
      transactionTypeDescription: selectedBuyer.transactionTypeDescription || '',
      taxIdentificationNumber: (selectedBuyer as any).taxIdentificationNumber || '',
      additionalDetails: (selectedBuyer as any).additionalDetails || '',
    });
    this.suppressTransactionTypeHandling = false;
    this.updatePriceValidators(this.sellerForm.get('transactionType')?.value);
    this.calculateDerivedAmounts();
    this.imagePath = this.baseUrl + (selectedBuyer.photo || 'assets/img/avatar.png');
    this.imageName = selectedBuyer.photo || '';
    this.nationalIdCardName = selectedBuyer.nationalIdCard || '';
    this.authorizationLetterName = selectedBuyer.authorizationLetter || '';
    this.selectedSellerId = selectedBuyer.id;
    
    if (selectedBuyer.photo) {
      if (this.childComponent) {
        this.childComponent.setExistingImage(this.baseUrl + selectedBuyer.photo);
      } else {
        this.pendingImagePath = this.baseUrl + selectedBuyer.photo;
      }
    }
    
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

deleteBuyer(id: number, event?: Event) {
  if (event) {
    event.stopPropagation();
  }
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

 private updatePriceValidators(transactionType: string): void {
   const priceControl = this.sellerForm.get('price');
   const priceTextControl = this.sellerForm.get('priceText');
   if (!priceControl || !priceTextControl) {
     return;
   }

   const supportedTypes = ['Purchase', 'Rent', 'Revocable Sale'];
   if (transactionType && supportedTypes.includes(transactionType)) {
     priceControl.setValidators([Validators.required]);
     priceTextControl.setValidators([Validators.required]);
   } else {
     priceControl.clearValidators();
     priceTextControl.clearValidators();

     if (!this.suppressTransactionTypeHandling) {
       priceControl.reset();
       priceTextControl.reset();
       this.sellerForm.patchValue({ royaltyAmount: null, halfPrice: null }, { emitEvent: false });
     }
   }

   priceControl.updateValueAndValidity({ emitEvent: false });
   priceTextControl.updateValueAndValidity({ emitEvent: false });
 }

 private calculateDerivedAmounts(): void {
   const transactionType = this.sellerForm.get('transactionType')?.value;
   const rawPrice = this.sellerForm.get('price')?.value;
   const price = rawPrice === '' || rawPrice === null || rawPrice === undefined ? NaN : Number(rawPrice);

   if (!transactionType || transactionType === 'Other' || Number.isNaN(price) || price <= 0) {
     this.sellerForm.patchValue({ royaltyAmount: null, halfPrice: null }, { emitEvent: false });
     return;
   }

   // Half price (مناصفه قیمت)
   const halfPrice = price / 2;

   // Commission (مبلغ حق‌العمل رهنمای معاملات)
   // - Purchase / Revocable Sale: 1.5%
   // - Rent: 50% of monthly rent
   let royaltyAmount: number | null = null;
   if (transactionType === 'Purchase' || transactionType === 'Revocable Sale') {
     royaltyAmount = price * 0.015;
   } else if (transactionType === 'Rent') {
     royaltyAmount = halfPrice;
   }

   this.sellerForm.patchValue(
     {
       royaltyAmount: royaltyAmount,
       halfPrice: halfPrice
     },
     { emitEvent: false }
   );
 }

 showPriceFields(): boolean {
   const transactionType = this.sellerForm.get('transactionType')?.value;
   return transactionType === 'Purchase' || transactionType === 'Rent' || transactionType === 'Revocable Sale';
 }

 getPriceNumberLabel(): string {
   const transactionType = this.sellerForm.get('transactionType')?.value;
   if (transactionType === 'Purchase') {
     return 'قیمت خرید ملکیت به عدد افغانی';
   }
   if (transactionType === 'Rent') {
     return 'قیمت کرایه ماهانه ملکیت به عدد افغانی';
   }
   if (transactionType === 'Revocable Sale') {
     return 'قیمت بیع جایزی ملکیت به عدد افغانی';
   }
   return 'قیمت به عدد';
 }

 getPriceTextLabel(): string {
   const transactionType = this.sellerForm.get('transactionType')?.value;
   if (transactionType === 'Purchase') {
     return 'قیمت خرید ملکیت به حروف افغانی';
   }
   if (transactionType === 'Rent') {
     return 'قیمت کرایه ماهانه ملکیت به حروف افغانی';
   }
   if (transactionType === 'Revocable Sale') {
     return 'قیمت بیع جایزی ملکیت به حروف افغانی';
   }
   return 'قیمت به حروف';
 }
onlyNumberKey(event:any) {
  const keyCode = event.which || event.keyCode;
  const keyValue = String.fromCharCode(keyCode);

  if (/\D/.test(keyValue)) {
    event.preventDefault();
  }
}
uploadFinished = (event:string) => { 
  this.imageName=event;
  this.imagePath=this.baseUrl+this.imageName;
  console.log(event+'=======================');
}

nationalIdUploadFinished = (event:string) => { 
  this.nationalIdCardName=event;
  this.sellerForm.patchValue({ nationalIdCard: this.nationalIdCardName });
  this.sellerForm.get('nationalIdCard')?.markAsTouched();
  this.sellerForm.get('nationalIdCard')?.markAsDirty();
  this.sellerForm.get('nationalIdCard')?.updateValueAndValidity();
  console.log('National ID uploaded: '+event+'=======================');
}

authorizationLetterUploadFinished = (event:string) => {
  this.authorizationLetterName=event;
  this.sellerForm.patchValue({ authorizationLetter: this.authorizationLetterName });
  this.sellerForm.get('authorizationLetter')?.markAsTouched();
  this.sellerForm.get('authorizationLetter')?.markAsDirty();
  this.sellerForm.get('authorizationLetter')?.updateValueAndValidity();
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
  this.sellerForm.get('photo')?.markAsTouched();
  this.sellerForm.get('photo')?.markAsDirty();
  this.sellerForm.get('photo')?.updateValueAndValidity();
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

  get firstName() { return this.sellerForm.get('firstName'); }
  get fatherName() { return this.sellerForm.get('fatherName'); }
  get grandFather() { return this.sellerForm.get('grandFather'); }
  get indentityCardNumber() { return this.sellerForm.get('indentityCardNumber'); }
  get tazkiraType() { return this.sellerForm.get('tazkiraType'); }
  get tazkiraVolume() { return this.sellerForm.get('tazkiraVolume'); }
  get tazkiraPage() { return this.sellerForm.get('tazkiraPage'); }
  get tazkiraNumber() { return this.sellerForm.get('tazkiraNumber'); }
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
  get price() { return this.sellerForm.get('price'); }
  get priceText() { return this.sellerForm.get('priceText'); }
  get royaltyAmount() { return this.sellerForm.get('royaltyAmount'); }
  get halfPrice() { return this.sellerForm.get('halfPrice'); }
  get rentStartDate() { return this.sellerForm.get('rentStartDate'); }
  get rentEndDate() { return this.sellerForm.get('rentEndDate'); }
  get transactionType() { return this.sellerForm.get('transactionType'); }
  get transactionTypeDescription() { return this.sellerForm.get('transactionTypeDescription'); }

  isOtherTransactionType(): boolean {
    return this.sellerForm.get('transactionType')?.value === 'Other';
  }

  isLesseeRole(): boolean {
    const roleType = this.sellerForm.get('roleType')?.value;
    const lesseeRoles = ['Lessee', 'Lessees', 'Agent for lessee'];
    return roleType && lesseeRoles.includes(roleType);
  }

  allowsMultipleBuyers(): boolean {
    const roleType = this.sellerForm.get('roleType')?.value;
    const pluralRoles = ['Buyers', 'Lessees', 'Buyers in a revocable sale'];
    return roleType && pluralRoles.includes(roleType);
  }

  isSingularRole(): boolean {
    return !this.allowsMultipleBuyers();
  }

  getRoleTypeLabel(roleType: string): string {
    const role = this.roleTypes.find((r: any) => r.value === roleType);
    return role ? role.label : roleType;
  }

  isPaperTazkira(): boolean {
    const tazkiraType = this.sellerForm.get('tazkiraType')?.value;
    return tazkiraType === 'Paper';
  }
}
