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
  private formInitialized = false;
  availableWitnessSides: string[] = ['Buyer', 'Seller'];
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  @ViewChild('nationalIdComponent') nationalIdComponent!: VehicleNationalidUploadComponent;
  onNextClick() {
    this.next.emit();
  }

  onEditWitness(id: number, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.BindValu(id);
  }
  constructor(private vehicleService: VehicleService,private toastr: ToastrService
    ,private fb: FormBuilder, private vehiclesub:VehiclesubService,private parentComponent: VehicleComponent)
    {
      this.withnessForm = this.fb.group({
        id: [0],
        firstName: ['', Validators.required],
        fatherName: ['', Validators.required],
        grandFatherName: [''],
        electronicNationalIdNumber: ['', Validators.required],
        phoneNumber: ['', Validators.required],
        witnessSide: ['', Validators.required],
        des: [''],
        nationalIdCardPath: ['', Validators.required]
      });

      // Only log invalid controls after form has been initialized and user has interacted
      this.withnessForm.statusChanges.subscribe(status => {
        if (status === 'INVALID' && this.formInitialized) {
          this.logInvalidControls();
        }
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

    const usedSides = otherWitnesses.map(w => w.witnessSide).filter(side => side);
    return ['Buyer', 'Seller'].filter(side => !usedSides.includes(side));
  }

  isWitnessSideDisabled(side: string): boolean {
    const availableSides = this.getAvailableWitnessSides();
    return !availableSides.includes(side);
  }

    private setNationalIdRequired(isRequired: boolean): void {
      const ctrl = this.withnessForm.get('nationalIdCardPath');
      if (!ctrl) {
        return;
      }

      if (isRequired) {
        ctrl.setValidators([Validators.required]);
      } else {
        ctrl.clearValidators();
      }

      ctrl.updateValueAndValidity();
    }

    private logInvalidControls(): void {
      const invalidControls = Object.keys(this.withnessForm.controls).filter((key) => {
        const ctrl = this.withnessForm.get(key);
        return !!ctrl && ctrl.invalid;
      });

      if (invalidControls.length > 0) {
        console.warn('Witness form invalid controls:', invalidControls, this.withnessForm.value);
      }
    }

    ngOnInit() {
      // Only fetch witness details if we have a valid ID
      if (!this.id || this.id === 0) {
        this.witnessDetails = [];
        this.selectedId = 0;
        this.setNationalIdRequired(true);
        // Mark form as initialized after a short delay to avoid initial validation warnings
        setTimeout(() => this.formInitialized = true, 100);
        return;
      }
      
      this.vehiclesub.getWitnessById(this.id)
      .subscribe(witness => {
        this.witnessDetails = witness;
        if (!witness || witness.length === 0) {
          this.selectedId = 0;
          this.setNationalIdRequired(true);
          setTimeout(() => this.formInitialized = true, 100);
          return;
        }
        const existingNationalIdPath = witness?.[0]?.nationalIdCardPath || witness?.[0]?.nationalIdCard || '';
        this.withnessForm.setValue({
          id: witness[0].id,
          firstName:witness[0].firstName,
          fatherName: witness[0].fatherName,
          grandFatherName: witness[0].grandFatherName || '',
          electronicNationalIdNumber: witness[0].electronicNationalIdNumber || '',
          phoneNumber: witness[0].phoneNumber,
          witnessSide: witness[0].witnessSide || '',
          des: witness[0].des || '',
          nationalIdCardPath: existingNationalIdPath
        });
        this.nationalIdCardName = existingNationalIdPath;
        this.vehiclesub.withnessId=witness[0].id;
        this.selectedId=witness[0].id;

        // When editing an existing witness, do not block edits if national ID is missing.
        this.setNationalIdRequired(false);

        this.withnessForm.updateValueAndValidity();
        
        // Mark form as initialized after data is loaded
        setTimeout(() => this.formInitialized = true, 100);
      });
    }
    addwithnessDetails(): void {
      const withnessDetails = this.withnessForm.value as witnessDetail;
      
      // Validate witness side is not already used
      const selectedSide = withnessDetails.witnessSide;
      const otherWitnesses = this.witnessDetails?.filter(w => w.id !== withnessDetails.id) || [];
      const sideAlreadyUsed = otherWitnesses.some(w => w.witnessSide === selectedSide);
      
      if (sideAlreadyUsed) {
        this.toastr.error(`شاهد از طرف ${selectedSide === 'Buyer' ? 'مشتری' : 'فروشنده'} قبلاً ثبت شده است. لطفاً طرف دیگر را انتخاب کنید`);
        return;
      }

      withnessDetails.nationalIdCard = this.nationalIdCardName;
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
    
    // Validate witness side is not already used by another witness
    const selectedSide = wDetails.witnessSide;
    const otherWitnesses = this.witnessDetails?.filter(w => w.id !== wDetails.id) || [];
    const sideAlreadyUsed = otherWitnesses.some(w => w.witnessSide === selectedSide);
    
    if (sideAlreadyUsed) {
      this.toastr.error(`شاهد از طرف ${selectedSide === 'Buyer' ? 'مشتری' : 'فروشنده'} قبلاً ثبت شده است. لطفاً طرف دیگر را انتخاب کنید`);
      return;
    }

    wDetails.nationalIdCard = this.nationalIdCardName;
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

    // In add mode, national ID upload is required.
    this.setNationalIdRequired(true);

    this.withnessForm.updateValueAndValidity();
  
}
BindValu(id: number) {
  const selectedWitness = this.witnessDetails.find(w => w.id === id);
  if (selectedWitness) {
    const existingNationalIdPath = selectedWitness.nationalIdCardPath || selectedWitness.nationalIdCard || '';
    this.withnessForm.patchValue({
      
      id: selectedWitness.id,
      firstName: selectedWitness.firstName,
      fatherName: selectedWitness.fatherName,
      grandFatherName: selectedWitness.grandFatherName || '',
      electronicNationalIdNumber: selectedWitness.electronicNationalIdNumber || '',
      phoneNumber: selectedWitness.phoneNumber,
      witnessSide: selectedWitness.witnessSide || '',
      des: selectedWitness.des || '',
      nationalIdCardPath: existingNationalIdPath
    });
    this.nationalIdCardName = existingNationalIdPath;
    this.selectedId=selectedWitness.id;

    // When editing an existing witness, do not block edits if national ID is missing.
    this.setNationalIdRequired(false);

    this.withnessForm.updateValueAndValidity();
  }
}

  nationalIdUploadFinished = (event:string) => { 
    this.nationalIdCardName=event;
    this.withnessForm.patchValue({ nationalIdCardPath: this.nationalIdCardName });
    this.withnessForm.updateValueAndValidity();
    this.logInvalidControls();
    console.log('Vehicle Witness National ID uploaded: '+event+'=======================');
  }

  get firstName() { return this.withnessForm.get('firstName'); }
  get fatherName() { return this.withnessForm.get('fatherName'); }
  get grandFather() { return this.withnessForm.get('grandFather'); }
  get grandFatherName() { return this.withnessForm.get('grandFatherName'); }
  get electronicNationalIdNumber() { return this.withnessForm.get('electronicNationalIdNumber'); }
  get phoneNumber() { return this.withnessForm.get('phoneNumber'); }
  get witnessSide() { return this.withnessForm.get('witnessSide'); }
  get des() { return this.withnessForm.get('des'); }
  get nationalIdCardPath() { return this.withnessForm.get('nationalIdCardPath'); }
}
