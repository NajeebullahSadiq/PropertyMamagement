import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { witnessDetail } from 'src/app/models/witnessDetail';
import { PropertyService } from 'src/app/shared/property.service';
import { SellerService } from 'src/app/shared/seller.service';
import { NationalidUploadComponent } from '../../nationalid-upload/nationalid-upload.component';

@Component({
  selector: 'app-witnessdetail',
  templateUrl: './witnessdetail.component.html',
  styleUrls: ['./witnessdetail.component.scss']
})
export class WitnessdetailComponent extends BaseComponent {
  withnessForm: FormGroup = new FormGroup({});
  selectedId: number=0;
  witnessDetails!: witnessDetail[];
  nationalIdCardName: string = '';
  availableWitnessSides: string[] = ['Buyer', 'Seller'];
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  @ViewChild('nationalIdComponent') nationalIdComponent!: NationalidUploadComponent;
  onEditWitness(id: number, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.BindValu(id);
  }
  onNextClick() {
    this.next.emit();
  }
  constructor(private propertyDetailsService: PropertyService,private toastr: ToastrService
    ,private fb: FormBuilder, private selerService:SellerService)
    {
      super();
      this.withnessForm = this.fb.group({
        id: [0],
        firstName: ['', Validators.required],
        fatherName: ['', Validators.required],
        grandFatherName: [''],
        electronicNationalIdNumber: ['', Validators.required],
        phoneNumber: ['', Validators.required],
        witnessSide: ['', Validators.required],
        des: [''],
        nationalIdCard: ['', Validators.required]
      });
    }

  getAvailableWitnessSides(): string[] {
    if (!this.witnessDetails || this.witnessDetails.length === 0) {
      return ['Buyer', 'Seller'];
    }

    const currentWitnessId = this.withnessForm.get('id')?.value;
    const otherWitnesses = this.witnessDetails.filter(w => w.id !== currentWitnessId);
    
    if (otherWitnesses.length === 0) {
      return ['Buyer', 'Seller'];
    }

    const usedSides = otherWitnesses
      .map(w => this.normalizeWitnessSide(w.witnessSide))
      .filter(side => side);
    return ['Buyer', 'Seller'].filter(side => !usedSides.includes(side));
  }

  canAddNewWitness(): boolean {
    return (this.witnessDetails?.length ?? 0) < 2;
  }

  getDefaultWitnessSideForNew(): string {
    const availableSides = this.getAvailableWitnessSides();
    return availableSides.length === 1 ? availableSides[0] : '';
  }

  normalizeWitnessSide(side: string | null | undefined): string {
    const value = (side || '').trim().toLowerCase();
    if (value === 'buyer' || value === 'مشتری') {
      return 'Buyer';
    }
    if (value === 'seller' || value === 'بایع') {
      return 'Seller';
    }
    return side || '';
  }

  isWitnessSideDisabled(side: string): boolean {
    const availableSides = this.getAvailableWitnessSides();
    return !availableSides.includes(side);
  }
    ngOnInit() {
      this.loadWitnessDetails();
    }

    loadWitnessDetails() {
      const effectiveId = this.id > 0 ? this.id : this.propertyDetailsService.mainTableId;
      if (effectiveId === 0) {
        this.witnessDetails = [];
        this.selectedId = 0;
        return;
      }
      this.selerService.getWitnessById(effectiveId)
      .subscribe(witness => {
        this.witnessDetails = (witness || []).map(w => ({
          ...w,
          witnessSide: this.normalizeWitnessSide(w.witnessSide)
        }));

        if (this.witnessDetails.length >= 2) {
          this.bindWitnessToForm(this.witnessDetails[0]);
          return;
        }

        if (this.witnessDetails.length === 1) {
          this.prepareNewWitnessForm();
          return;
        }

        this.prepareNewWitnessForm();
      });
    }

    private bindWitnessToForm(witness: witnessDetail): void {
      this.withnessForm.setValue({
        id: witness.id,
        firstName: witness.firstName,
        fatherName: witness.fatherName,
        grandFatherName: witness.grandFatherName || '',
        electronicNationalIdNumber: witness.electronicNationalIdNumber || '',
        phoneNumber: witness.phoneNumber,
        witnessSide: this.normalizeWitnessSide(witness.witnessSide),
        des: witness.des || '',
        nationalIdCard: witness.nationalIdCard || ''
      });
      this.nationalIdCardName = witness.nationalIdCard || '';
      this.selerService.withnessId = witness.id;
      this.selectedId = witness.id;
    }

    private prepareNewWitnessForm(): void {
      this.selectedId = 0;
      this.selerService.withnessId = 0;
      this.nationalIdCardName = '';
      if (this.nationalIdComponent) {
        this.nationalIdComponent.reset();
      }
      this.withnessForm.reset({
        id: 0,
        firstName: '',
        fatherName: '',
        grandFatherName: '',
        electronicNationalIdNumber: '',
        phoneNumber: '',
        witnessSide: this.getDefaultWitnessSideForNew(),
        des: '',
        nationalIdCard: ''
      });
    }
    addwithnessDetails(): void {
      const withnessDetails = this.withnessForm.value as witnessDetail;
      
      // Validate witness side is not already used
      const selectedSide = this.normalizeWitnessSide(withnessDetails.witnessSide);
      withnessDetails.witnessSide = selectedSide;
      const otherWitnesses = this.witnessDetails?.filter(w => w.id !== withnessDetails.id) || [];
      const sideAlreadyUsed = otherWitnesses.some(w => this.normalizeWitnessSide(w.witnessSide) === selectedSide);
      
      if (sideAlreadyUsed) {
        this.toastr.error(`شاهد از طرف ${selectedSide === 'Buyer' ? 'مشتری' : 'بایع'} قبلاً ثبت شده است. لطفاً طرف دیگر را انتخاب کنید`);
        return;
      }

      withnessDetails.nationalIdCard = this.nationalIdCardName;
      withnessDetails.propertyDetailsId = this.propertyDetailsService.mainTableId;
      if (withnessDetails.id === null) {
        withnessDetails.id = 0;
      }
    
      this.selerService.addWitnessdetails(withnessDetails).subscribe(
        (result) => {
          if (result.id !== 0) {
            this.toastr.success("معلومات موفقانه ثبت شد");
            this.selerService.withnessId = result.id;
            this.selerService.getWitnessById(this.propertyDetailsService.mainTableId)
            .subscribe(witness => {
              this.witnessDetails = (witness || []).map(w => ({
                ...w,
                witnessSide: this.normalizeWitnessSide(w.witnessSide)
              }));
              if ((withnessDetails.id ?? 0) === 0) {
                this.prepareNewWitnessForm();
              }
            });
          }
        },
        (error) => {
          if (error.status === 400) {
            this.toastr.error("شما نمی توانید بشتر از دو شاهد را در سیستم ثبت کنید");
          }
          else if(error.status === 312){
            this.toastr.error("لطفا ابتدا معلومات ملکیت را ثبت کنید ");
          } else {
            this.toastr.error("An error occurred");
          }
        }
      );
    }
  updateWitnessDetails(): void {
    const wDetails = this.withnessForm.value as witnessDetail;
    
    const selectedSide = this.normalizeWitnessSide(wDetails.witnessSide);
    wDetails.witnessSide = selectedSide;
    const otherWitnesses = this.witnessDetails?.filter(w => w.id !== wDetails.id) || [];
    const sideAlreadyUsed = otherWitnesses.some(w => this.normalizeWitnessSide(w.witnessSide) === selectedSide);
    
    if (sideAlreadyUsed) {
      this.toastr.error(`شاهد از طرف ${selectedSide === 'Buyer' ? 'مشتری' : 'بایع'} قبلاً ثبت شده است. لطفاً طرف دیگر را انتخاب کنید`);
      return;
    }

    wDetails.nationalIdCard = this.nationalIdCardName;
    wDetails.propertyDetailsId=this.propertyDetailsService.mainTableId;
    this.selerService.updateWitnessDetails(wDetails).subscribe(result => {
      if(result.id!==0)
      this.toastr.info("معلومات موفقانه تغیر کرد");
      this.selerService.udateSellerId(result.id);
      this.selerService.getWitnessById(this.propertyDetailsService.mainTableId)
            .subscribe(witness => {
              this.witnessDetails = (witness || []).map(w => ({
                ...w,
                witnessSide: this.normalizeWitnessSide(w.witnessSide)
              }));
            });
   });
  }

resetChild(){
    if (this.nationalIdComponent) {
      this.nationalIdComponent.reset();
    }
    this.prepareNewWitnessForm();
 }
 resetlist(){
  this.witnessDetails=[];
 }
BindValu(id: number) {
  const selectedWitness = this.witnessDetails.find(w => w.id === id);
  if (selectedWitness) {
    this.bindWitnessToForm(selectedWitness);
  }
}
  nationalIdUploadFinished = (event:string) => { 
    this.nationalIdCardName=event;
    this.withnessForm.patchValue({ nationalIdCard: this.nationalIdCardName });
    console.log('Witness National ID uploaded: '+event+'=======================');
  }

  get firstName() { return this.withnessForm.get('firstName'); }
  get fatherName() { return this.withnessForm.get('fatherName'); }
  get grandFatherName() { return this.withnessForm.get('grandFatherName'); }
  get electronicNationalIdNumber() { return this.withnessForm.get('electronicNationalIdNumber'); }
  get phoneNumber() { return this.withnessForm.get('phoneNumber'); }
  get witnessSide() { return this.withnessForm.get('witnessSide'); }
  get des() { return this.withnessForm.get('des'); }
  get nationalIdCard() { return this.withnessForm.get('nationalIdCard'); }

  isEditing(): boolean {
    return this.selectedId > 0;
  }
 
}
