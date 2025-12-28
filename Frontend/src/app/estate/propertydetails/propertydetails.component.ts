import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ViewChild, AfterViewInit, Input, Output, EventEmitter } from '@angular/core'
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PropertyDetails } from 'src/app/models/PropertyDetail';
import { PunitType } from 'src/app/models/PunitType';
import { propertyAddress } from 'src/app/models/propertyAddress';
import { PropertyService } from 'src/app/shared/property.service';
import { PunittypeService } from 'src/app/shared/punittype.service';
import { SellerService } from 'src/app/shared/seller.service';
import { EstateComponent } from '../estate.component';
import { UploadComponent } from '../upload/upload.component';
import { LocalizationService } from 'src/app/shared/localization.service';

@Component({
  selector: 'app-propertydetails',
  templateUrl: './propertydetails.component.html',
  styleUrls: ['./propertydetails.component.scss']
})
export class PropertydetailsComponent  implements AfterViewInit {
  propertyForm: FormGroup = new FormGroup({});
  propertyDetails!: PropertyDetails[];
  unittypes!: PunitType[];
  localizedUnitTypes:any;
  province:any;
  districts:any;
  selectedAddressId: number = 0;
  selectedPropertyId: number=0;
  mainTableId: number =0;
  propertypetype:any;
  localizedPropertyTypes:any;
  properties!: PropertyDetails[];
  onePercent:number=0;
  imageName:string='';
  previousDocumentsPath:string='';
  existingDocumentsPath:string='';
  @ViewChild('childComponent') childComponent!: UploadComponent;
  ngAfterViewInit(): void {

    if (this.childComponent) {
      // Child component is ready, call its reset method
      this.childComponent.reset();
    }
  }
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  @Input() id: number=0;
  constructor(private http: HttpClient,private propertyDetailsService: PropertyService,private toastr: ToastrService
    ,private fb: FormBuilder, 
    private unittypeService: PunittypeService,private route: ActivatedRoute,private router: Router,
    private parentComponent: EstateComponent,private selerService:SellerService,
    private localizationService: LocalizationService) {
      this.propertyForm = this.fb.group({
        id: [0],
        propertyTypeId: ['', Validators.required],
        customPropertyType: [''],
        parea: ['', Validators.required],
        punitTypeId: ['', Validators.required],
        numofFloor: ['', Validators.required],
        numofRooms: ['', Validators.required],
        provinceId: ['', Validators.required],
        districtId: ['', Validators.required],
        village: ['', Validators.required],
        north: ['', Validators.required],
        west: ['', Validators.required],
        east: ['', Validators.required],
        south: ['', Validators.required],
        documentType: ['', Validators.required],
        issuanceNumber: [''],
        issuanceDate: [''],
        serialNumber: [''],
        transactionDate: [''],
        des: ['', Validators.required],
        filePath: [''],
        previousDocumentsPath: [''],
        existingDocumentsPath: [''],
        iscomplete: [false],
        iseditable: [false],

        
      });
    
     // this.loadPropertyDetails();
  this.loadDepartments();
  this.propertyDetailsService.mainTableId=0;
  this.selerService.sellerId=0;
  this.selerService.buyerId=0;
  this.selerService.withnessId=0;
  }
  ngOnInit() {
    this.loadPropertyDetails();
      
  this.propertyDetailsService.getPropertyType().subscribe(res => {
    this.propertypetype = res;
    this.localizedPropertyTypes = this.mapPropertyTypesToStandardizedDari(res as any[]);

    const currentPropertyTypeId = this.propertyForm.get('propertyTypeId')?.value;
    this.applyCustomPropertyTypeValidation(currentPropertyTypeId, true);
  });

  this.propertyForm.get('propertyTypeId')?.valueChanges.subscribe(propertyTypeId => {
    this.onPropertyTypeChange();
    this.applyCustomPropertyTypeValidation(propertyTypeId);
  });

  }

  loadPropertyDetails() {
    this.selectedPropertyId = this.id;

    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });

    const effectiveId = this.id > 0 ? this.id : this.propertyDetailsService.mainTableId;

    if (effectiveId > 0) {
      this.propertyDetailsService.getPropertyDetailsById(effectiveId)
          .subscribe(properties => {
            this.properties = properties;
            this.propertyForm.patchValue({
              id: properties[0].id,
              propertyTypeId: properties[0].propertyTypeId || '',
              customPropertyType: (properties[0] as any).customPropertyType || '',
              parea: properties[0].parea,
              punitTypeId: properties[0].punitTypeId,
              numofFloor: properties[0].numofFloor,
              numofRooms: properties[0].numofRooms,
              des: properties[0].des,
              filePath: properties[0].filePath,
              previousDocumentsPath: properties[0].previousDocumentsPath || '',
              existingDocumentsPath: properties[0].existingDocumentsPath || '',
              iscomplete: properties[0].iscomplete,
              iseditable:properties[0].iseditable,
              north:properties[0].north,
              west:properties[0].west,
              east:properties[0].east,
              south:properties[0].south,
              documentType:properties[0].documentType || '',
              issuanceNumber:properties[0].issuanceNumber || '',
              issuanceDate:properties[0].issuanceDate || '',
              serialNumber:properties[0].serialNumber || '',
              transactionDate:properties[0].transactionDate || '',
            });
            this.imageName=properties.map(item => item.filePath).toString();
            this.previousDocumentsPath = properties[0].previousDocumentsPath || '';
            this.existingDocumentsPath = properties[0].existingDocumentsPath || '';
            this.propertyDetailsService.updateMainTableId(effectiveId);
            this.updateDocumentFieldValidation();
          });

      this.selerService.getPaddressById(effectiveId)
        .subscribe(addr => {
          if (addr && addr.length > 0) {
            this.selectedAddressId = addr[0].id;
            this.propertyForm.patchValue({
              provinceId: addr[0].provinceId,
              districtId: addr[0].districtId,
              village: addr[0].village
            });
            if (addr[0].provinceId) {
              this.selerService.getdistrict(addr[0].provinceId.valueOf()).subscribe(res => {
                this.districts = res;
              });
            }
          }
        });
    }
  }

  loadDepartments(): void {
    this.unittypeService.getUnitTypes().subscribe(unittypes => {
      this.unittypes = unittypes;
      // Map unit types to localized versions with Dari labels
      this.localizedUnitTypes = this.mapUnitTypesToLocalized(unittypes);
      //$('#unittype').select2();
    });
  }
  addPropertyDetails(): void {
    const propertyDetails = this.propertyForm.value as PropertyDetails;
    propertyDetails.filePath=this.imageName;
    propertyDetails.previousDocumentsPath=this.previousDocumentsPath;
    propertyDetails.existingDocumentsPath=this.existingDocumentsPath;

    const address: propertyAddress = {
      id: this.selectedAddressId || 0,
      provinceId: this.propertyForm.get('provinceId')?.value,
      districtId: this.propertyForm.get('districtId')?.value,
      PropertyDetailsId: 0,
      village: this.propertyForm.get('village')?.value
    };
    (propertyDetails as any).propertyAddresses = [address];

     if(propertyDetails.id===null){
      propertyDetails.id=0;
    }
    // Convert empty strings to null for optional fields
    if (!propertyDetails.serialNumber || (propertyDetails.serialNumber as any) === '') {
      propertyDetails.serialNumber = null as any;
    }
    // Convert dates to UTC if they exist
    if(propertyDetails.issuanceDate) {
      const date = new Date(propertyDetails.issuanceDate);
      propertyDetails.issuanceDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000) as any;
    } else {
      propertyDetails.issuanceDate = null as any;
    }
    if(propertyDetails.transactionDate && (propertyDetails.transactionDate as any) !== '') {
      const date = new Date(propertyDetails.transactionDate);
      propertyDetails.transactionDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000) as any;
    } else {
      propertyDetails.transactionDate = null as any;
    }
    this.propertyDetailsService.addPropertyDetails(propertyDetails).subscribe(result => {
      if(result.id!==0) {
       this.propertyDetailsService.updateMainTableId(result.id);
       this.selectedPropertyId=result.id;
       this.toastr.success("معلومات موفقانه ثبت شد");
       // Notify property list to reload
       this.propertyDetailsService.propertyAdded.next();
       this.onNextClick();
      }
    });
}

  updatePropertyDetails(): void {
    const propertyDetails = this.propertyForm.value as PropertyDetails;
    propertyDetails.filePath=this.imageName;
    propertyDetails.previousDocumentsPath=this.previousDocumentsPath;
    propertyDetails.existingDocumentsPath=this.existingDocumentsPath;

    const address: propertyAddress = {
      id: this.selectedAddressId || 0,
      provinceId: this.propertyForm.get('provinceId')?.value,
      districtId: this.propertyForm.get('districtId')?.value,
      PropertyDetailsId: this.selectedPropertyId || 0,
      village: this.propertyForm.get('village')?.value
    };
    (propertyDetails as any).propertyAddresses = [address];

    if(propertyDetails.id===0 && this.selectedPropertyId!==0 || this.selectedPropertyId!==null){
      propertyDetails.id=this.selectedPropertyId;
    }
    // Convert empty strings to null for optional fields
    if (!propertyDetails.serialNumber || (propertyDetails.serialNumber as any) === '') {
      propertyDetails.serialNumber = null as any;
    }
    // Convert dates to UTC if they exist
    if(propertyDetails.issuanceDate) {
      const date = new Date(propertyDetails.issuanceDate);
      propertyDetails.issuanceDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000) as any;
    } else {
      propertyDetails.issuanceDate = null as any;
    }
    if(propertyDetails.transactionDate && (propertyDetails.transactionDate as any) !== '') {
      const date = new Date(propertyDetails.transactionDate);
      propertyDetails.transactionDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000) as any;
    } else {
      propertyDetails.transactionDate = null as any;
    }
    this.propertyDetailsService.updatePropertyDetails(propertyDetails).subscribe(result => {
      if(result.id!==0)
       this.propertyDetailsService.updateMainTableId(result.id);
       this.selectedPropertyId=result.id;
       this.toastr.info("معلومات موفقانه تغیر یافت ");
       this.onNextClick();
    });
  }

 


  onlyNumberKey(event:any) {
    const keyCode = event.which || event.keyCode;
    const keyValue = String.fromCharCode(keyCode);
  
    if (/\D/.test(keyValue)) {
      event.preventDefault();
    }
  }
  uploadFinished = (event:string) => { 
    this.imageName = event;
  }
  
  previousDocumentsUploadFinished = (event:string) => { 
    this.previousDocumentsPath = event;
  }
  
  existingDocumentsUploadFinished = (event:string) => { 
    this.existingDocumentsPath = event;
  }
  resetChild(): void {
    if (this.childComponent) {
      // Child component is available, reset it
      this.childComponent.reset();
      this.propertyDetailsService.mainTableId=0;
      this.selectedPropertyId=0;
      this.selectedAddressId=0;
      this.id=0;
      this.router.navigate(['/dashboard/estate']);
    }
    this.propertyForm.reset({
      id: 0,
      propertyTypeId: '',
      customPropertyType: '',
      parea:'',
      punitTypeId:'',
      numofFloor:'',
      numofRooms:'',
      provinceId:'',
      districtId:'',
      village:'',
      des:'',
      filePath:'',
      previousDocumentsPath:'',
      existingDocumentsPath:'',
      north:'',
      west:'',
      east:'',
      south:'',
      documentType:'',
      issuanceNumber:'',
      issuanceDate:'',
      serialNumber:'',
      transactionDate:'',
    });
    const numofFloorControl = this.propertyForm.get('numofFloor');
    const numofRoomControl = this.propertyForm.get('numofRooms');
    numofFloorControl?.enable();
    numofRoomControl?.enable();
  }
  onPropertyTypeChange() {
    const propertyType = this.propertyForm.get('propertyTypeId')?.value;
    const numofFloorControl = this.propertyForm.get('numofFloor');
    const numofRoomControl = this.propertyForm.get('numofRooms');
    if (propertyType === 2) {
      // For Apartment (ID 2): disable numofFloor but enable numofRooms
      numofFloorControl?.setValue(0);
      numofFloorControl?.disable();
      numofRoomControl?.enable();
      numofRoomControl?.setValue(null);
    } else {
      numofFloorControl?.enable();
      numofFloorControl?.setValue(null);
      numofRoomControl?.enable();
      numofRoomControl?.setValue(null);
    }
  }
  resetForms(): void {
   this.parentComponent.resetChild();
  }

  /**
   * Map backend property types to localized versions with Dari labels
   */
  mapPropertyTypesToLocalized(backendTypes: any[]): any[] {
    return backendTypes.map(type => {
      const localized = this.localizationService.propertyTypes.find(
        pt => pt.value.toLowerCase() === type.name.toLowerCase()
      );
      return {
        id: type.id,
        name: localized ? localized.label : type.name
      };
    });
  }

  mapPropertyTypesToStandardizedDari(backendTypes: any[]): any[] {
    const supportedOrder = ['House', 'Apartment', 'Shop', 'Block', 'Land', 'Garden', 'Hill', 'Other'];
    const byNameLower = new Map<string, any>();
    (backendTypes || []).forEach(t => {
      if (t?.name) {
        const normalized = String(t.name).toLowerCase() === 'othert' ? 'other' : String(t.name).toLowerCase();
        byNameLower.set(normalized, t);
      }
    });

    return supportedOrder
      .map(value => {
        const backend = byNameLower.get(value.toLowerCase());
        if (!backend) {
          return null;
        }
        const localized = this.localizationService.propertyTypes.find(pt => pt.value === value);
        if (!localized?.label) {
          return null;
        }
        return { id: backend.id, name: localized.label };
      })
      .filter(Boolean);
  }

  private applyCustomPropertyTypeValidation(propertyTypeId: any, emitEvent: boolean = false): void {
    const customPropertyTypeControl = this.propertyForm.get('customPropertyType');
    if (!customPropertyTypeControl) {
      return;
    }

    if (!this.localizedPropertyTypes || this.localizedPropertyTypes.length === 0) {
      return;
    }

    const selectedPropertyType = this.localizedPropertyTypes.find((pt: any) => pt.id === propertyTypeId);
    if (selectedPropertyType && selectedPropertyType.name === 'سایر') {
      customPropertyTypeControl.setValidators([Validators.required]);
    } else {
      customPropertyTypeControl.clearValidators();
      customPropertyTypeControl.reset();
    }
    customPropertyTypeControl.updateValueAndValidity({ emitEvent });
  }

  isOtherPropertyType(): boolean {
    const propertyTypeId = this.propertyForm.get('propertyTypeId')?.value;
    const selectedPropertyType = this.localizedPropertyTypes?.find((pt: any) => pt.id === propertyTypeId);
    return selectedPropertyType && selectedPropertyType.name === 'سایر';
  }

  get propertyTypeId() { return this.propertyForm.get('propertyTypeId'); }
  get customPropertyType() { return this.propertyForm.get('customPropertyType'); }

  /**
   * Map backend unit types to localized versions with Dari labels
   */
  mapUnitTypesToLocalized(backendTypes: any[]): any[] {
    return backendTypes.map(type => {
      const localized = this.localizationService.propertyUnitTypes.find(
        ut => ut.value.toLowerCase() === type.name.toLowerCase()
      );
      return {
        id: type.id,
        name: localized ? localized.label : type.name
      };
    });
  }

  downloadFiles() {
    const filePath = this.propertyForm.get('filePath')?.value;
    console.log(filePath);
  
    const filename = filePath?.split('/').pop() ?? 'file';
  
    this.propertyDetailsService.downloadFile(filePath).subscribe(
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

  get parea() { return this.propertyForm.get('parea'); }
  get punitTypeId() { return this.propertyForm.get('punitTypeId'); }
  get numofFloor() { return this.propertyForm.get('numofFloor'); }
  get numofRooms() { return this.propertyForm.get('numofRooms'); }
  get des() { return this.propertyForm.get('des'); }
  get filePath() { return this.propertyForm.get('filePath'); }
  get departmentId() { return this.propertyForm.get('departmentId'); }
  get west() { return this.propertyForm.get('west'); }
  get north() { return this.propertyForm.get('north'); }
  get east() { return this.propertyForm.get('east'); }
  get south() { return this.propertyForm.get('south'); }
  get documentType() { return this.propertyForm.get('documentType'); }
  get issuanceNumber() { return this.propertyForm.get('issuanceNumber'); }
  get issuanceDate() { return this.propertyForm.get('issuanceDate'); }
  get serialNumber() { return this.propertyForm.get('serialNumber'); }
  get transactionDate() { return this.propertyForm.get('transactionDate'); }
  get provinceId() { return this.propertyForm.get('provinceId'); }
  get districtId() { return this.propertyForm.get('districtId'); }
  get village() { return this.propertyForm.get('village'); }

  /**
   * Check if document type requires issuance number and date
   * قباله شرعی and سند ملکیت
   */
  requiresIssuanceFields(): boolean {
    const docType = this.propertyForm.get('documentType')?.value;
    return docType === 'قباله شرعی' || docType === 'سند ملکیت';
  }

  /**
   * Check if document type requires serial number
   * سټه رهنمای معاملات
   */
  requiresSerialNumber(): boolean {
    const docType = this.propertyForm.get('documentType')?.value;
    return docType === 'سټه رهنمای معاملات';
  }

  /**
   * Check if document type requires transaction date
   * سټه رهنمای معاملات and سند دست‌نویس
   */
  requiresTransactionDate(): boolean {
    const docType = this.propertyForm.get('documentType')?.value;
    return docType === 'سټه رهنمای معاملات' || docType === 'سند دست‌نویس';
  }

  /**
   * Update document field validation based on selected document type
   */
  onDocumentTypeChange(): void {
    this.updateDocumentFieldValidation();
  }

  /**
   * Update validators for document fields based on selected type
   */
  updateDocumentFieldValidation(): void {
    const issuanceNumberControl = this.propertyForm.get('issuanceNumber');
    const issuanceDateControl = this.propertyForm.get('issuanceDate');
    const serialNumberControl = this.propertyForm.get('serialNumber');
    const transactionDateControl = this.propertyForm.get('transactionDate');

    // Clear all validators first
    issuanceNumberControl?.clearValidators();
    issuanceDateControl?.clearValidators();
    serialNumberControl?.clearValidators();
    transactionDateControl?.clearValidators();

    // Reset values for fields that are not visible
    if (!this.requiresIssuanceFields()) {
      issuanceNumberControl?.setValue('');
      issuanceDateControl?.setValue('');
    }
    if (!this.requiresSerialNumber()) {
      serialNumberControl?.setValue('');
    }
    if (!this.requiresTransactionDate()) {
      transactionDateControl?.setValue('');
    }

    // Add validators based on document type
    if (this.requiresIssuanceFields()) {
      issuanceNumberControl?.setValidators([Validators.required]);
      issuanceDateControl?.setValidators([Validators.required]);
    }
    if (this.requiresSerialNumber()) {
      serialNumberControl?.setValidators([Validators.required]);
    }
    if (this.requiresTransactionDate()) {
      transactionDateControl?.setValidators([Validators.required]);
    }

    // Update validity
    issuanceNumberControl?.updateValueAndValidity();
    issuanceDateControl?.updateValueAndValidity();
    serialNumberControl?.updateValueAndValidity();
    transactionDateControl?.updateValueAndValidity();
  }

  filterResults(getId:any) {
    this.selerService.getdistrict(getId.id).subscribe(res => {
      this.districts = res;
    });
  }


}
