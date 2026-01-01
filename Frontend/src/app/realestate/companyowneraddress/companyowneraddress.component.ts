import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { companyOwnerAddress, companyOwnerAddressData } from 'src/app/models/companyOwnerAddress';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { SellerService } from 'src/app/shared/seller.service';

@Component({
  selector: 'app-companyowneraddress',
  templateUrl: './companyowneraddress.component.html',
  styleUrls: ['./companyowneraddress.component.scss']
})
export class CompanyowneraddressComponent {
  owneraddressForm: FormGroup = new FormGroup({});
  province: any;
  district: any;
  addressType: any;
  selectedId: number = 0;
  @Input() id: number = 0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  owneraddressDetails!: companyOwnerAddressData[];
  constructor(private fb: FormBuilder, private toastr: ToastrService, private comservice: CompnaydetailService, private selerService: SellerService) {
    this.owneraddressForm = this.fb.group({
      id: [0],
      companyOwnerId: [''],
      addressTypeId: ['', Validators.required],
      provinceId: ['', Validators.required],
      districtId: ['', Validators.required],
      village: ['', Validators.required],
    });


  }
  ngOnInit() {
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    this.comservice.getAddressType().subscribe(res => {
      this.addressType = res;
    });
    this.comservice.getOwnerAddressById(this.id)
      .subscribe(add => {
        this.owneraddressDetails = add;
      });
  }
  addData(): void {
    const details = this.owneraddressForm.value as companyOwnerAddress;
    details.companyOwnerId = this.comservice.ownerId;

    if (details.id === null) {
      details.id = 0;
    }

    this.comservice.addcompanyOwnerAddress(details).subscribe(
      result => {
        if (result.id !== 0) {
          this.toastr.success("معلومات موفقانه ثبت شد");
          console.log(result.companyOwnerId, 'OwerId');
          this.comservice.getOwnerAddressById(this.id)
            .subscribe(add => {
              this.owneraddressDetails = add;
            });
        }

        this.selectedId = result.id;

      }, error => {
        if (error.status === 400) {
          this.toastr.error("شما فقط میتوانید یک آدرس دایمی و یک آدرس فعلی اضافه کنید");
        } else if (error.status === 312) {
          this.toastr.error("لطفا ابتدا معلومات مالک دفتر  را ثبت کنید");
        } else {
          this.toastr.error("An error occurred");
        }
      }
    );


  }
  updateData(): void {
    const details = this.owneraddressForm.value as companyOwnerAddress;
    details.companyOwnerId = this.comservice.ownerId;
    if (details.id === 0 && this.selectedId !== 0 || this.selectedId !== null) {
      details.id = this.selectedId;
    }
    this.comservice.updateownerAddress(details).subscribe(result => {
      if (result.id !== 0)
        this.selectedId = result.id;
      this.toastr.info("معلومات موفقانه تغیر یافت ");
      this.comservice.getOwnerAddressById(this.id)
      .subscribe(add => {
        this.owneraddressDetails = add;
      });

    });
  }
  filterResults(getId: any) {

    this.selerService.getdistrict(getId.id).subscribe(res => {
      this.district = res;

    });
  }
  resetForms(): void {
    this.owneraddressForm.reset();
    this.selectedId = 0;
  }
  BindValu(id: number) {
    const selectedOwnerAddress = this.owneraddressDetails.find(w => w.id === id);
    if (selectedOwnerAddress) {
      this.owneraddressForm.patchValue({

        id: selectedOwnerAddress.id,
        companyOwnerId: selectedOwnerAddress.companyOwnerId,
        addressTypeId: selectedOwnerAddress.addressTypeId,
        provinceId: selectedOwnerAddress.provinceId,
        districtId: selectedOwnerAddress.districtId,
        village: selectedOwnerAddress.village
      });
      this.selectedId = selectedOwnerAddress.id;
      this.selerService.getdistrict(selectedOwnerAddress.provinceId).subscribe(res => {
        this.district = res;

      });
    }

  }
  get addressTypeId() { return this.owneraddressForm.get('addressTypeId'); }
  get companyOwnerId() { return this.owneraddressForm.get('companyOwnerId'); }
  get provinceId() { return this.owneraddressForm.get('provinceId'); }
  get districtId() { return this.owneraddressForm.get('districtId'); }
  get village() { return this.owneraddressForm.get('village'); }
}
