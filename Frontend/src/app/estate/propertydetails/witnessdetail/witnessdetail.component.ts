import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { witnessDetail } from 'src/app/models/witnessDetail';
import { PropertyService } from 'src/app/shared/property.service';
import { SellerService } from 'src/app/shared/seller.service';
import { NationalidUploadComponent } from '../../nationalid-upload/nationalid-upload.component';

@Component({
  selector: 'app-witnessdetail',
  templateUrl: './witnessdetail.component.html',
  styleUrls: ['./witnessdetail.component.scss']
})
export class WitnessdetailComponent {
  withnessForm: FormGroup = new FormGroup({});
  selectedId: number=0;
  witnessDetails!: witnessDetail[];
  nationalIdCardName: string = '';
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
      this.withnessForm = this.fb.group({
        id: [0],
        firstName: ['', Validators.required],
        fatherName: ['', Validators.required],
        electronicNationalIdNumber: ['', Validators.required],
        phoneNumber: ['', Validators.required],
        nationalIdCard: ['', Validators.required]
      });
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
        this.witnessDetails = witness;
        if (witness && witness.length > 0) {
          this.withnessForm.setValue({
            id: witness[0].id,
            firstName:witness[0].firstName,
            fatherName: witness[0].fatherName,
            electronicNationalIdNumber: witness[0].electronicNationalIdNumber || '',
            phoneNumber: witness[0].phoneNumber,
            nationalIdCard: witness[0].nationalIdCard || ''
          });
          this.nationalIdCardName = witness[0].nationalIdCard || '';
          this.selerService.withnessId=witness[0].id;
          this.selectedId=witness[0].id;
        }
      });
    }
    addwithnessDetails(): void {
      const withnessDetails = this.withnessForm.value as witnessDetail;
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
              this.witnessDetails = witness;
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
    wDetails.nationalIdCard = this.nationalIdCardName;
    wDetails.propertyDetailsId=this.propertyDetailsService.mainTableId;
    this.selerService.updateWitnessDetails(wDetails).subscribe(result => {
      if(result.id!==0)
      this.toastr.info("معلومات موفقانه تغیر کرد");
      this.selerService.udateSellerId(result.id);
      this.selerService.getWitnessById(this.propertyDetailsService.mainTableId)
            .subscribe(witness => {
              this.witnessDetails = witness;
            });
     // this.onNextClick();
   });
  }

resetChild(){
    if (this.nationalIdComponent) {
      this.nationalIdComponent.reset();
    }
    this.selectedId=0;
    this.nationalIdCardName='';
    this.withnessForm.reset(); 
 }
 resetlist(){
  this.witnessDetails=[];
 }
BindValu(id: number) {
  const selectedWitness = this.witnessDetails.find(w => w.id === id);
  if (selectedWitness) {
    this.withnessForm.patchValue({
      
      id: selectedWitness.id,
      firstName: selectedWitness.firstName,
      fatherName: selectedWitness.fatherName,
      electronicNationalIdNumber: selectedWitness.electronicNationalIdNumber || '',
      phoneNumber: selectedWitness.phoneNumber,
      propertyDetailsId:selectedWitness.propertyDetailsId,
      nationalIdCard: selectedWitness.nationalIdCard || ''
    });
    this.nationalIdCardName = selectedWitness.nationalIdCard || '';
    this.selectedId=selectedWitness.id;
  }
}
  nationalIdUploadFinished = (event:string) => { 
    this.nationalIdCardName=event;
    this.withnessForm.patchValue({ nationalIdCard: this.nationalIdCardName });
    console.log('Witness National ID uploaded: '+event+'=======================');
  }

  get firstName() { return this.withnessForm.get('firstName'); }
  get fatherName() { return this.withnessForm.get('fatherName'); }
  get electronicNationalIdNumber() { return this.withnessForm.get('electronicNationalIdNumber'); }
  get phoneNumber() { return this.withnessForm.get('phoneNumber'); }
  get nationalIdCard() { return this.withnessForm.get('nationalIdCard'); }
 
}
