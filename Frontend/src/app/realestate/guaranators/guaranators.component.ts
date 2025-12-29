import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Guarantor } from 'src/app/models/Guarantor';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { SellerService } from 'src/app/shared/seller.service';
import { FileuploadComponent } from '../fileupload/fileupload.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-guaranators',
  templateUrl: './guaranators.component.html',
  styleUrls: ['./guaranators.component.scss']
})
export class GuaranatorsComponent {
  baseUrl:string=environment.apiURL+'/';
  imagePath:string='assets/img/avatar.png';
  imageName:string='';
  selectedId:number=0;
  IdTypes:any;
  province:any;
  district:any;
  district2:any;
  guaranatorForm: FormGroup = new FormGroup({});
  guaranatorDetails!: Guarantor[];
 
  
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  @ViewChild('childComponent') childComponent!: FileuploadComponent;
  ngAfterViewInit(): void {
    if (this.childComponent) {
      // Child component is ready, call its reset method
      this.childComponent.reset();
    }
  }
  constructor(private fb: FormBuilder,private toastr: ToastrService, private comservice:CompnaydetailService,private selerService:SellerService){
    this.guaranatorForm = this.fb.group({
      id: [0],
      firstName: ['', Validators.required],
      fatherName:['', Validators.required],
      identityCardTypeId:['', Validators.required],
      indentityCardNumber:['', Validators.required],
      jild:['', Validators.required],
      safha:['', Validators.required],
      companyId:[''],
      sabtNumber:['', Validators.required],
      phoneNumber:['', Validators.required],
      paddressProvinceId:['', Validators.required],
      paddressDistrictId:['', Validators.required],
      paddressVillage:['', Validators.required],
      taddressProvinceId:['', Validators.required],
      taddressDistrictId:['', Validators.required],
      taddressVillage:['', Validators.required],
      pothoPath:['']
      });
    }
    ngOnInit() {
      this.selerService.getprovince().subscribe(res => {
        this.province = res;
        });
      this.comservice.getIdentityTypes().subscribe(res => {
        this.IdTypes = res;
      });

      this.comservice.getGuaranatorById(this.id)
      .subscribe(detail => {
        this.guaranatorDetails = detail;
        // this.guaranatorForm.setValue({
        // id: detail[0].id,
        // firstName:detail[0].firstName,
        // fatherName:detail[0].fatherName,
        // identityCardTypeId:detail[0].identityCardTypeId,
        // indentityCardNumber:detail[0].indentityCardNumber,
        // jild:detail[0].jild,
        // safha:detail[0].safha,
        // companyId:detail[0].companyId,
        // sabtNumber:detail[0].sabtNumber,
        // phoneNumber:detail[0].phoneNumber,
        // paddressProvinceId:detail[0].paddressProvinceId,
        // paddressDistrictId:detail[0].paddressDistrictId,
        // paddressVillage:detail[0].paddressVillage,
        // taddressProvinceId:detail[0].taddressProvinceId,
        // taddressDistrictId:detail[0].taddressDistrictId,
        // taddressVillage:detail[0].taddressVillage,
        // pothoPath:detail[0].pothoPath,
        // });
        // this.selerService.getdistrict(detail[0].paddressProvinceId.valueOf()).subscribe(res => {
        //   this.district = res;
          
        // });
        // this.selerService.getdistrict(detail[0].taddressProvinceId.valueOf()).subscribe(res => {
        //   this.district2 = res;
        // });
       // this.selectedId=detail[0].id;
      //   this.imagePath=this.baseUrl+detail[0].pothoPath;
      //   this.imageName=detail.map(item => item.pothoPath).toString();
      //  this.onPropertyTypeChange();
      });
    }
  uploadFinished = (event:string) => { 
    this.imageName=event;
    this.imagePath=this.baseUrl+this.imageName;
  }
  addData(): void {
    const details = this.guaranatorForm.value as Guarantor;
    details.pothoPath = this.imageName;
    details.companyId = this.comservice.mainTableId;
    if (details.id === null) {
      details.id = 0;
    }
  
    this.comservice.addcompanyGuaranator(details).subscribe(
      result => {
        if (result.id !== 0) {
          this.toastr.success("معلومات موفقانه ثبت شد");
          this.selectedId = result.id;
          this.comservice.getGuaranatorById(this.id)
          .subscribe(detail => {
            this.guaranatorDetails = detail;
          });
        }
      },
      error => {
        if (error.status === 400) {
          this.toastr.error("شما نمی توانید بشتر از دو ضامن ثبت سیستم کنید");
        } else if (error.status === 312) {
          this.toastr.error("لطفا ابتدا معلومات جدول اصلی را ثبت کنید");
        } else {
          this.toastr.error("An error occurred");
        }
      }
    );
  }
  updateData(): void {
    const details = this.guaranatorForm.value as Guarantor;
		  details.companyId=this.comservice.mainTableId;
		details.pothoPath=this.imageName;
		if(details.id===0 && this.selectedId!==0 || this.selectedId!==null){
			details.id=this.selectedId;
		}
		this.comservice.updateGuaranator(details).subscribe(result => {
		  if(result.id!==0)
		   this.selectedId=result.id;
		   this.toastr.info("معلومات موفقانه تغیر یافت ");
		
		});
  }
  resetForms():void{
    if (this.childComponent) {
      // Child component is available, reset it
      this.childComponent.reset();
        this.imagePath='assets/img/avatar.png';
      }
   this.selectedId=0;
   this.guaranatorForm.reset();
  }
  filterResults(getId:any) {
    
    this.selerService.getdistrict(getId.id).subscribe(res => {
      this.district = res;
      
    });
  }
  filterResults2(getId:any) {
    
    this.selerService.getdistrict(getId.id).subscribe(res => {
      this.district2 = res;
      
    });
  }
  onPropertyTypeChange() {
    const identityCardTypeId = this.guaranatorForm.get('identityCardTypeId')?.value;
    const jild = this.guaranatorForm.get('jild');
    const safha = this.guaranatorForm.get('safha');
	const sabtNumber = this.guaranatorForm.get('sabtNumber');
    if (identityCardTypeId === 1) {
		jild?.setValue(0);
		jild?.disable();
		safha?.setValue(0);
		safha?.disable();
		sabtNumber?.setValue(0);
		sabtNumber?.disable();
    } else {
		jild?.enable();
		jild?.setValue(null);
		safha?.enable();
		safha?.setValue(null);
		sabtNumber?.enable();
		sabtNumber?.setValue(null);
    }
  }
  BindValu(id: number) {
    const selectedOwnerAddress = this.guaranatorDetails.find(w => w.id === id);
    if (selectedOwnerAddress) {
      this.guaranatorForm.patchValue({
        
        id: selectedOwnerAddress.id,
        firstName: selectedOwnerAddress.firstName,
        fatherName: selectedOwnerAddress.fatherName,
        identityCardTypeId: selectedOwnerAddress.identityCardTypeId,
        indentityCardNumber: selectedOwnerAddress.indentityCardNumber,
        jild: selectedOwnerAddress.jild,
        safha: selectedOwnerAddress.safha,
        companyId: selectedOwnerAddress.companyId,
        sabtNumber: selectedOwnerAddress.sabtNumber,
        phoneNumber: selectedOwnerAddress.phoneNumber,
        paddressProvinceId: selectedOwnerAddress.paddressProvinceId,
        paddressDistrictId: selectedOwnerAddress.paddressDistrictId,
        paddressVillage: selectedOwnerAddress.paddressVillage,
        taddressProvinceId: selectedOwnerAddress.taddressProvinceId,
        taddressDistrictId: selectedOwnerAddress.taddressDistrictId,
        taddressVillage: selectedOwnerAddress.taddressVillage,
        pothoPath: selectedOwnerAddress.pothoPath,
      });
      this.selerService.getdistrict(selectedOwnerAddress.paddressProvinceId.valueOf()).subscribe(res => {
        this.district = res;
        
      });
      this.selerService.getdistrict(selectedOwnerAddress.taddressProvinceId.valueOf()).subscribe(res => {
        this.district2 = res;
        
      });
      this.selectedId=id;
      this.imagePath=this.baseUrl+selectedOwnerAddress.pothoPath;
      this.imageName=selectedOwnerAddress.pothoPath;
     this.onPropertyTypeChange();
    }
   
  }
  get firstName() { return this.guaranatorForm.get('firstName'); }
  get fatherName() { return this.guaranatorForm.get('fatherName'); }
  get identityCardTypeId() { return this.guaranatorForm.get('identityCardTypeId'); }
  get indentityCardNumber() { return this.guaranatorForm.get('indentityCardNumber'); }
  get jild() { return this.guaranatorForm.get('jild'); }
  get safha() { return this.guaranatorForm.get('safha'); }
  get companyId() { return this.guaranatorForm.get('companyId'); }
  get sabtNumber() { return this.guaranatorForm.get('sabtNumber'); }
  get pothoPath() { return this.guaranatorForm.get('pothoPath'); }
  get phoneNumber() { return this.guaranatorForm.get('phoneNumber'); }
  get paddressVillage() { return this.guaranatorForm.get('paddressVillage'); }
  get taddressVillage() { return this.guaranatorForm.get('taddressVillage'); }
  get paddressProvinceId() { return this.guaranatorForm.get('paddressProvinceId'); }
  get paddressDistrictId() { return this.guaranatorForm.get('paddressDistrictId'); }
  get taddressProvinceId() { return this.guaranatorForm.get('taddressProvinceId'); }
  get taddressDistrictId() { return this.guaranatorForm.get('taddressDistrictId'); }
}
