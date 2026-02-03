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
import { NumeralService } from 'src/app/shared/numeral.service';
import { RbacService, Permissions } from 'src/app/shared/rbac.service';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { AuthService } from 'src/app/shared/auth.service';

@Component({
  selector: 'app-vehicle-submit',
  templateUrl: './vehicle-submit.component.html',
  styleUrls: ['./vehicle-submit.component.scss']
})
export class VehicleSubmitComponent implements AfterViewInit{
  onePercent:number=0;
  halfPriceValue:number=0;
  imageName:string='';
  selectedVehicleId:number=0;
  vehicleForm: FormGroup = new FormGroup({});
  properties!: VehicleDetails[];
  vehicleHandOptions: any;
  canEditVehicle = this.rbacService.hasPermission(Permissions.VehicleCreate) || this.rbacService.hasPermission(Permissions.VehicleEdit);
  
  // Company selection for Admin
  isAdmin: boolean = false;
  companies: any[] = [];
  selectedCompanyId: number | null = null;
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
    private localizationService: LocalizationService, private numeralService: NumeralService,
    public rbacService: RbacService, private companyService: CompnaydetailService, private authService: AuthService){
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
      halfPrice: [''],
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
    // Check if user is Admin
    this.isAdmin = this.authService.isAdministrator();
    
    // Load companies if Admin
    if (this.isAdmin) {
      this.loadCompanies();
    }
    
    // Initialize vehicle hand options from localization service
    this.vehicleHandOptions = this.localizationService.vehicleHandOptions;
    
    this.selectedVehicleId=this.id;
    
    // Only fetch vehicle details if we have a valid ID
    if (!this.id || this.id === 0) {
      return;
    }
    
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
        halfPrice: properties[0].halfPrice,
        royaltyAmount: properties[0].royaltyAmount,
        des: properties[0].des,
        filePath: properties[0].filePath,
        vehicleHand: properties[0].vehicleHand || '',
        iscomplete: properties[0].iscomplete,
        iseditable:properties[0].iseditable
      });
      // 
      
      // Parse string values to numbers for display
      this.onePercent = properties[0].royaltyAmount ? parseFloat(properties[0].royaltyAmount) : (properties[0].price * 0.01);
      this.halfPriceValue = properties[0].halfPrice ? parseFloat(properties[0].halfPrice) : (properties[0].price / 2);
      this.imageName=properties.map(item => item.filePath).toString();
      this.vehicleService.updateMainTableId(this.id);
    });
  }
  addVehicleDetails(): void {
    // Validate company selection for Admin
    if (this.isAdmin && !this.selectedCompanyId) {
      this.toastr.error('لطفا شرکت را انتخاب کنید');
      return;
    }
    
    const vehicleDetail = this.vehicleForm.value as VehicleDetails;
    vehicleDetail.filePath=this.imageName;
    
    // Convert numeric values to strings to match backend expectation
    vehicleDetail.royaltyAmount = this.onePercent.toString();
    vehicleDetail.halfPrice = this.halfPriceValue.toString();
    
    // Add companyId if Admin has selected one
    if (this.isAdmin && this.selectedCompanyId) {
      vehicleDetail.companyId = this.selectedCompanyId;
    }
    
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

  loadCompanies(): void {
    this.companyService.getComapaniesList().subscribe({
      next: (companies: any) => {
        this.companies = companies;
      },
      error: (err: any) => {
        console.error('Error loading companies:', err);
        this.toastr.error('خطا در بارگذاری لیست شرکت ها');
      }
    });
  }
resetForms(): void {
  this.parentComponent.resetChild();
 }
updateVehicleDetails():void{
 
    const vehicleDetails = this.vehicleForm.value as VehicleDetails;
    vehicleDetails.filePath=this.imageName;
    
    // Convert numeric values to strings to match backend expectation
    vehicleDetails.royaltyAmount = this.onePercent.toString();
    vehicleDetails.halfPrice = this.halfPriceValue.toString();
    
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
      const normalizedPrice = this.numeralService.parseNumber(priceControl.value);
      this.onePercent = normalizedPrice * 0.01;
      
      // Calculate half price automatically
      this.halfPriceValue = normalizedPrice / 2;
      this.vehicleForm.patchValue(
        { halfPrice: this.halfPriceValue },
        { emitEvent: false }
      );
    } else {
      this.onePercent = 0;
      this.halfPriceValue = 0;
      this.vehicleForm.patchValue(
        { halfPrice: null },
        { emitEvent: false }
      );
    }
  }
  uploadFinished = (event:string) => { 
    this.imageName=event;
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
  get halfPrice() { return this.vehicleForm.get('halfPrice'); }
  get royaltyAmount() { return this.vehicleForm.get('royaltyAmount'); }
  get des() { return this.vehicleForm.get('des'); }
  get filePath() { return this.vehicleForm.get('filePath'); }
  get vehicleHand() { return this.vehicleForm.get('vehicleHand'); }

}
