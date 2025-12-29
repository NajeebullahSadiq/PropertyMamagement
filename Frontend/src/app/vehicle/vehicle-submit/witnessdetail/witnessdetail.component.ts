import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { witnessDetail } from 'src/app/models/witnessDetail';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { VehiclesubService } from 'src/app/shared/vehiclesub.service';
import { VehicleComponent } from '../../vehicle.component';
import { VehicleNationalidUploadComponent } from '../nationalid-upload/nationalid-upload.component';

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
  @ViewChild('nationalIdComponent') nationalIdComponent!: VehicleNationalidUploadComponent;
  onNextClick() {
    this.next.emit();
  }
  constructor(private vehicleService: VehicleService,private toastr: ToastrService
    ,private fb: FormBuilder, private vehiclesub:VehiclesubService,private parentComponent: VehicleComponent)
    {
      this.withnessForm = this.fb.group({
        id: [0],
        firstName: ['', Validators.required],
        fatherName: ['', Validators.required],
        indentityCardNumber: ['', Validators.required],
        phoneNumber: ['', Validators.required],
        nationalIdCardPath: ['', Validators.required]
      });
    }
    ngOnInit() {
      this.vehiclesub.getWitnessById(this.id)
      .subscribe(witness => {
        this.witnessDetails = witness;
        this.withnessForm.setValue({
          id: witness[0].id,
          firstName:witness[0].firstName,
          fatherName: witness[0].fatherName,
          indentityCardNumber: witness[0].indentityCardNumber,
          phoneNumber: witness[0].phoneNumber,
          nationalIdCardPath: witness[0].nationalIdCardPath || ''
        });
        this.nationalIdCardName = witness[0].nationalIdCardPath || '';
        this.vehiclesub.withnessId=witness[0].id;
        this.selectedId=witness[0].id;
        
        
      });
    }
    addwithnessDetails(): void {
      const withnessDetails = this.withnessForm.value as witnessDetail;
      withnessDetails.nationalIdCardPath = this.nationalIdCardName;
      withnessDetails.propertyDetailsId = this.vehicleService.mainTableId;
      if (withnessDetails.id === null) {
        withnessDetails.id = 0;
      }
    
      this.vehiclesub.addWitnessdetails(withnessDetails).subscribe(
        (result) => {
          if (result.id !== 0) {
            this.toastr.success("معلومات موفقانه ثبت شد");
            
            this.vehiclesub.withnessId = result.id;
            this.vehiclesub.getWitnessById(this.vehicleService.mainTableId)
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
    wDetails.nationalIdCardPath = this.nationalIdCardName;
    wDetails.propertyDetailsId=this.vehicleService.mainTableId;
    this.vehiclesub.updateWitnessDetails(wDetails).subscribe(result => {
      if(result.id!==0)
      this.toastr.info("معلومات موفقانه تغیر کرد");
      this.vehiclesub.udateSellerId(result.id);
      this.vehiclesub.getWitnessById(this.vehicleService.mainTableId)
            .subscribe(witness => {
              this.witnessDetails = witness;
            });
     // this.onNextClick();
   });
  }
  
reset(){
  this.parentComponent.resetChild();
}
resetChild(){
    if (this.nationalIdComponent) {
      this.nationalIdComponent.reset();
    }
    this.selectedId=0;
    this.nationalIdCardName='';
    this.withnessForm.reset();
  
}
BindValu(id: number) {
  const selectedWitness = this.witnessDetails.find(w => w.id === id);
  if (selectedWitness) {
    this.withnessForm.patchValue({
      
      id: selectedWitness.id,
      firstName: selectedWitness.firstName,
      fatherName: selectedWitness.fatherName,
      indentityCardNumber: selectedWitness.indentityCardNumber,
      phoneNumber: selectedWitness.phoneNumber,
      propertyDetailsId:selectedWitness.propertyDetailsId,
      nationalIdCardPath: selectedWitness.nationalIdCardPath || ''
    });
    this.nationalIdCardName = selectedWitness.nationalIdCardPath || '';
    this.selectedId=selectedWitness.id;
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
    this.nationalIdCardName=event;
    this.withnessForm.patchValue({ nationalIdCardPath: this.nationalIdCardName });
    console.log('Vehicle Witness National ID uploaded: '+event+'=======================');
  }

  get firstName() { return this.withnessForm.get('firstName'); }
  get fatherName() { return this.withnessForm.get('fatherName'); }
  get grandFather() { return this.withnessForm.get('grandFather'); }
  get indentityCardNumber() { return this.withnessForm.get('indentityCardNumber'); }
  get phoneNumber() { return this.withnessForm.get('phoneNumber'); }
  get nationalIdCardPath() { return this.withnessForm.get('nationalIdCardPath'); }
}
