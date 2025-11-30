import { AfterViewInit, Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UploadComponent } from 'src/app/estate/upload/upload.component';
import { VehicleDetails } from 'src/app/models/vehicle';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { VehiclesubService } from 'src/app/shared/vehiclesub.service';
import { VehicleComponent } from '../vehicle.component';
import { LocalizationService } from 'src/app/shared/localization.service';

@Component({
  selector: 'app-vehicle-submit',
  templateUrl: './vehicle-submit.component.html',
  styleUrls: ['./vehicle-submit.component.scss']
})
export class VehicleSubmitComponent implements AfterViewInit{
  onePercent:number=0;
  imageName:string='';
  selectedVehicleId:number=0;
  vehicleForm: FormGroup = new FormGroup({});
  properties!: VehicleDetails[];
  vehicleHandOptions: any;
  @ViewChild('childComponent') childComponent!: UploadComponent;
  ngAfterViewInit(): void {
    if (this.childComponent) {
      // Child component is ready, call its reset method
      this.childComponent.reset();
    }
  }
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  constructor(private fb: FormBuilder,private toastr: ToastrService, private vehicleService: VehicleService,
    private parentComponent: VehicleComponent,private router: Router,private vehiclesubservice:VehiclesubService,
    private localizationService: LocalizationService){
    this.vehicleForm = this.fb.group({
      id: [0],
      permitNo: ['', Validators.required],
      pilateNo: ['', Validators.required],
      typeOfVehicle: ['', Validators.required],
      model: ['', Validators.required],
      enginNo: ['', Validators.required],
      shasiNo: ['', Validators.required],
      color: ['', Validators.required],
      price: ['', Validators.required],
      priceText: ['', Validators.required],
      royaltyAmount: [''],
      des: ['', Validators.required],
      filePath: [''],
      vehicleHand: ['', Validators.required],
      iscomplete: [false],
      iseditable: [false],
      
    });
    this.vehicleService.mainTableId=0;
    this.vehiclesubservice.buyerId=0;
    this.vehiclesubservice.sellerId=0;
    this.vehiclesubservice.withnessId=0;
  }
  ngOnInit() {
    // Initialize vehicle hand options from localization service
    this.vehicleHandOptions = this.localizationService.vehicleHandOptions;
    
    this.selectedVehicleId=this.id;
    this.vehicleService.getPropertyDetailsById(this.id)
    .subscribe(properties => {
      this.properties = properties;
      this.vehicleForm.setValue({
        id: properties[0].id,
        permitNo: properties[0].permitNo,
        pilateNo: properties[0].pilateNo,
        typeOfVehicle: properties[0].typeOfVehicle,
        model: properties[0].model,
        enginNo: properties[0].enginNo,
        shasiNo: properties[0].shasiNo,
        color: properties[0].color,
        price: properties[0].price,
        priceText: properties[0].priceText,
        royaltyAmount: properties[0].royaltyAmount,
        des: properties[0].des,
        filePath: properties[0].filePath,
        vehicleHand: properties[0].vehicleHand || '',
        iscomplete: properties[0].iscomplete,
        iseditable:properties[0].iseditable
      });
      // 
      
      this.onePercent = properties[0].price * 0.01;
      this.imageName=properties.map(item => item.filePath).toString();
      this.vehicleService.updateMainTableId(this.id);
    });
  }
  addVehicleDetails(): void {
    const vehicleDetail = this.vehicleForm.value as VehicleDetails;
    vehicleDetail.royaltyAmount=this.onePercent;
    vehicleDetail.filePath=this.imageName;
     if(vehicleDetail.id===null){
      vehicleDetail.id=0;
    }
    this.vehicleService.addVehicles(vehicleDetail).subscribe(result => {
      if(result.id!==0)
       this.toastr.success("معلومات موفقانه ثبت شد");
       this.vehicleService.updateMainTableId(result.id);
       this.selectedVehicleId=result.id;
       this.onNextClick();
    });
}
resetForms(): void {
  this.parentComponent.resetChild();
 }
updateVehicleDetails():void{
 
    const vehicleDetails = this.vehicleForm.value as VehicleDetails;
    vehicleDetails.filePath=this.imageName;
    vehicleDetails.royaltyAmount=this.onePercent;
    if(vehicleDetails.id===0 && this.selectedVehicleId!==0 || this.selectedVehicleId!==null){
      vehicleDetails.id=this.selectedVehicleId;
    }
    this.vehicleService.updateVehicleDetails(vehicleDetails).subscribe(result => {
      if(result.id!==0)
       this.vehicleService.updateMainTableId(result.id);
       this.selectedVehicleId=result.id;
       this.toastr.info("معلومات موفقانه تغیر یافت ");
    
    });
  
}

  updateOnePercent() {
    const priceControl = this.price;
    if (priceControl && priceControl.value) {
      this.onePercent = priceControl.value * 0.01;
    } else {
      this.onePercent = 0;
    }
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
  }
  resetChild(): void {
    if (this.childComponent) {
      // Child component is available, reset it
      this.childComponent.reset();
      this.vehicleService.mainTableId=0;
      this.selectedVehicleId=0;
      this.id=0;
     // this.router.navigate(['/dashboard/vehicle']);
    }
    this.vehicleForm.reset();
  }
  downloadFiles() {
    const filePath = this.vehicleForm.get('filePath')?.value;
    console.log(filePath);
  
    const filename = filePath?.split('/').pop() ?? 'file';
  
    this.vehicleService.downloadFile(filePath).subscribe(
      (response: any) => {
        const url = URL.createObjectURL(response);
        const a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = filename;
        a.click();
        URL.revokeObjectURL(url);
        a.remove();
      },
      (error) => {
        if (error.status === 404) {
          this.toastr.info("به این معامله فایل وجود ندارد")
          // Show error message to user
        } else {
          console.log('An error occurred:', error);
          // Show generic error message to user
        }
      }
    );
  }
  get permitNo() { return this.vehicleForm.get('permitNo'); }
  get pilateNo() { return this.vehicleForm.get('pilateNo'); }
  get typeOfVehicle() { return this.vehicleForm.get('typeOfVehicle'); }
  get model() { return this.vehicleForm.get('model'); }
  get enginNo() { return this.vehicleForm.get('enginNo'); }
  get shasiNo() { return this.vehicleForm.get('shasiNo'); }
  get color() { return this.vehicleForm.get('color'); }
  get price() { return this.vehicleForm.get('price'); }
  get priceText() { return this.vehicleForm.get('priceText'); }
  get royaltyAmount() { return this.vehicleForm.get('royaltyAmount'); }
  get des() { return this.vehicleForm.get('des'); }
  get filePath() { return this.vehicleForm.get('filePath'); }
  get vehicleHand() { return this.vehicleForm.get('vehicleHand'); }

}
