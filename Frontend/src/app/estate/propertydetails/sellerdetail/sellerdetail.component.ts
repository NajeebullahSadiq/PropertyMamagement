import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { SellerDetail } from 'src/app/models/SellerDetail';
import { PropertyService } from 'src/app/shared/property.service';
import { SellerService } from 'src/app/shared/seller.service';
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
  mainTableId:number=0;
  sellerForm: FormGroup = new FormGroup({});
  selectedSellerId: number=0;
  sellerDetails!: SellerDetail[];
  province:any;
  district:any;
  district2:any;
  roleTypes = [
    { value: 'Seller', label: 'فروشنده' },
    { value: 'Authorized Agent (Seller)', label: 'وکیل فروشنده' }
  ];
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  @ViewChild('childComponent') childComponent!: UploadComponent;
  @ViewChild('nationalIdComponent') nationalIdComponent!: NationalidUploadComponent;
  @ViewChild('authLetterComponent') authLetterComponent!: UploadComponent;
  ngAfterViewInit(): void {
    if (this.childComponent) {
      // Child component is ready, call its reset method
      this.childComponent.reset();
    }
  }
  constructor(private propertyDetailsService: PropertyService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService){
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
      authorizationLetter: ['']
    });

    // Add dynamic validation for authorization letter
    this.sellerForm.get('roleType')?.valueChanges.subscribe(roleType => {
      const authLetterControl = this.sellerForm.get('authorizationLetter');
      if (roleType && roleType.includes('Agent')) {
        authLetterControl?.setValidators([Validators.required]);
      } else {
        authLetterControl?.clearValidators();
      }
      authLetterControl?.updateValueAndValidity();
    });
  }
  ngOnInit() {
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    this.selerService.getSellerById(this.id)
    .subscribe(sellers => {
      this.sellerDetails = sellers;
      this.sellerForm.setValue({
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
        nationalIdCard:sellers[0].nationalIdCardPath || '',
        roleType: sellers[0].roleType || 'Seller',
        authorizationLetter: sellers[0].authorizationLetter || '',
      });
      this.imagePath=this.baseUrl+sellers[0].photo;
      this.imageName=sellers.map(item => item.photo).toString();
      this.nationalIdCardName=sellers[0].nationalIdCardPath || '';
      this.authorizationLetterName=sellers[0].authorizationLetter || '';
      this.selectedSellerId=sellers[0].id;
      this.selerService.getdistrict(sellers[0].paddressProvinceId.valueOf()).subscribe(res => {
        this.district = res;
      });
      this.selerService.getdistrict(sellers[0].taddressProvinceId.valueOf()).subscribe(res => {
        this.district2 = res;
      });
      this.selerService.sellerId=sellers[0].id;
    });
    
  }
  addSellerDetails(): void {
    const sellerDetails = this.sellerForm.value as SellerDetail;
    sellerDetails.photo=this.imageName;
    sellerDetails.nationalIdCardPath=this.nationalIdCardName;
    sellerDetails.authorizationLetter=this.authorizationLetterName;
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
    const sellerDetails = this.sellerForm.value as SellerDetail;
    sellerDetails.photo=this.imageName;
    sellerDetails.nationalIdCardPath=this.nationalIdCardName;
    sellerDetails.authorizationLetter=this.authorizationLetterName;
    if(sellerDetails.id===0 && this.selectedSellerId!==0 || this.selectedSellerId!==null){
      sellerDetails.id=this.selectedSellerId;
    }
    this.selerService.updateSellerdetails(sellerDetails).subscribe(result => {
      if(result.id!==0)
      this.toastr.info("معلومات موفقانه تغیر کرد");
      this.selerService.udateSellerId(result.id);
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
      this.sellerForm.patchValue({ roleType: 'Seller' });
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

  isAuthorizedAgent(): boolean {
    const roleType = this.sellerForm.get('roleType')?.value;
    return roleType && roleType.includes('Agent');
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
}
