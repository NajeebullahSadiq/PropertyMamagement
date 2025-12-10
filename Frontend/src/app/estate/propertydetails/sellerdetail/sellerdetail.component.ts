import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { SellerDetail } from 'src/app/models/SellerDetail';
import { PropertyService } from 'src/app/shared/property.service';
import { SellerService } from 'src/app/shared/seller.service';
import { LocalizationService } from 'src/app/shared/localization.service';
import { UploadComponent } from '../../upload/upload.component';
import { NationalidUploadComponent } from '../../nationalid-upload/nationalid-upload.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-sellerdetail',
  templateUrl: './sellerdetail.component.html',
  styleUrls: ['./sellerdetail.component.scss']
})
export class SellerdetailComponent {
  imagePath:string='assets/img/avatar.png';
  baseUrl:string=environment.apiURL+'/';
  imageName:string='';
  nationalIdCardName:string='';
  authorizationLetterName:string='';
  heirsLetterName:string='';
  mainTableId:number=0;
  sellerForm: FormGroup = new FormGroup({});
  selectedSellerId: number=0;
  sellerDetails!: SellerDetail[];
  province:any;
  district:any;
  district2:any;
  roleTypes: any[] = [];
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  @ViewChild('childComponent') childComponent!: UploadComponent;
  @ViewChild('nationalIdComponent') nationalIdComponent!: NationalidUploadComponent;
  @ViewChild('authLetterComponent') authLetterComponent!: UploadComponent;
  @ViewChild('heirsLetterComponent') heirsLetterComponent!: UploadComponent;
  ngAfterViewInit(): void {
    if (this.childComponent) {
      // Child component is ready, call its reset method
      this.childComponent.reset();
    }
  }
  constructor(private propertyDetailsService: PropertyService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService, private localizationService: LocalizationService){
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
      roleType: ['Seller', Validators.required],
      authorizationLetter: [''],
      heirsLetter: ['']
    });

    // Add dynamic validation for authorization letter and heirs letter based on roleType
    this.sellerForm.get('roleType')?.valueChanges.subscribe(roleType => {
      const authLetterControl = this.sellerForm.get('authorizationLetter');
      const heirsLetterControl = this.sellerForm.get('heirsLetter');
      
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
    this.selerService.getSellerById(this.id)
    .subscribe(sellers => {
      this.sellerDetails = sellers || [];
      if (sellers && sellers.length > 0) {
        // Load first seller for editing if exists
        const firstSeller = sellers[0];
        this.sellerForm.setValue({
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
          nationalIdCard:firstSeller.nationalIdCardPath || '',
          roleType: firstSeller.roleType || 'Seller',
          authorizationLetter: firstSeller.authorizationLetter || '',
          heirsLetter: firstSeller.heirsLetter || '',
        });
        this.imagePath=this.baseUrl+firstSeller.photo;
        this.imageName=firstSeller.photo || '';
        this.nationalIdCardName=firstSeller.nationalIdCardPath || '';
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
        this.selerService.sellerId=firstSeller.id;
      } else {
        // No sellers yet, reset form
        this.sellerDetails = [];
        this.selectedSellerId = 0;
      }
    });
  }
  addSellerDetails(): void {
    const sellerDetails = this.sellerForm.value as SellerDetail;
    sellerDetails.photo=this.imageName;
    sellerDetails.nationalIdCardPath=this.nationalIdCardName;
    sellerDetails.authorizationLetter=this.authorizationLetterName;
    sellerDetails.heirsLetter=this.heirsLetterName;
    sellerDetails.propertyDetailsId=this.propertyDetailsService.mainTableId;
    if (sellerDetails.id === null) {
      sellerDetails.id = 0;
    }
    this.selerService.addSellerdetails(sellerDetails).subscribe(
      (result) => {
        if (result.id !== 0) {
          this.toastr.success("معلومات موفقانه ثبت شد");
          this.selerService.sellerId = result.id;
          this.selectedSellerId=result.id;
          // Small delay to ensure database commit
          setTimeout(() => {
            this.loadSellerDetails(); // Reload the list
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

  updateSellerDetails(): void {
    const sellerDetails = this.sellerForm.value as SellerDetail;
    sellerDetails.photo=this.imageName;
    sellerDetails.nationalIdCardPath=this.nationalIdCardName;
    sellerDetails.authorizationLetter=this.authorizationLetterName;
    sellerDetails.heirsLetter=this.heirsLetterName;
    if(sellerDetails.id===0 && this.selectedSellerId!==0 || this.selectedSellerId!==null){
      sellerDetails.id=this.selectedSellerId;
    }
    this.selerService.updateSellerdetails(sellerDetails).subscribe(result => {
      if(result.id!==0) {
        this.toastr.info("معلومات موفقانه تغیر کرد");
        this.selerService.udateSellerId(result.id);
        this.loadSellerDetails(); // Reload the list
        this.resetChild(); // Reset form
      }
   });
  }

  BindValue(id: number) {
    const selectedSeller = this.sellerDetails.find(s => s.id === id);
    if (selectedSeller) {
      this.sellerForm.patchValue({
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
        nationalIdCard: selectedSeller.nationalIdCardPath || '',
        roleType: selectedSeller.roleType || 'Seller',
        authorizationLetter: selectedSeller.authorizationLetter || '',
        heirsLetter: selectedSeller.heirsLetter || '',
      });
      this.imagePath = this.baseUrl + (selectedSeller.photo || 'assets/img/avatar.png');
      this.imageName = selectedSeller.photo || '';
      this.nationalIdCardName = selectedSeller.nationalIdCardPath || '';
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
      this.selerService.deleteSeller(id).subscribe(
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
    if (this.heirsLetterComponent) {
      // Reset heirs letter component
      this.heirsLetterComponent.reset();
    }
      this.sellerForm.reset();
      this.sellerForm.patchValue({ roleType: 'Seller' });
      this.selectedSellerId=0;
      this.imagePath='assets/img/avatar.png';
      this.nationalIdCardName='';
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
    this.nationalIdCardName="Resources\\Images\\"+event;
    this.sellerForm.patchValue({ nationalIdCard: this.nationalIdCardName });
    console.log('National ID uploaded: '+event+'=======================');
  }

  authorizationLetterUploadFinished = (event:string) => {
    this.authorizationLetterName="Resources\\Images\\"+event;
    this.sellerForm.patchValue({ authorizationLetter: this.authorizationLetterName });
    console.log('Authorization Letter uploaded: '+event+'=======================');
  }

  heirsLetterUploadFinished = (event:string) => {
    this.heirsLetterName="Resources\\Images\\"+event;
    this.sellerForm.patchValue({ heirsLetter: this.heirsLetterName });
    console.log('Heirs Letter uploaded: '+event+'=======================');
  }

  isAuthorizedAgent(): boolean {
    const roleType = this.sellerForm.get('roleType')?.value;
    const agentRoles = ['Sales Agent', 'Lease Agent', 'Agent for a revocable sale'];
    return agentRoles.includes(roleType);
  }

  isHeirs(): boolean {
    const roleType = this.sellerForm.get('roleType')?.value;
    return roleType === 'Heirs';
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
  get heirsLetter() { return this.sellerForm.get('heirsLetter'); }
}
