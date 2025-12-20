import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ViewChild, AfterViewInit, Input, Output, EventEmitter } from '@angular/core'
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PropertyDetails } from 'src/app/models/PropertyDetail';
import { PunitType } from 'src/app/models/PunitType';
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
        pnumber: ['', Validators.required],
        parea: ['', Validators.required],
        punitTypeId: ['', Validators.required],
        numofFloor: ['', Validators.required],
        numofRooms: ['', Validators.required],
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
    this.selectedPropertyId=this.id;
  if (this.id > 0) {
    this.propertyDetailsService.getPropertyDetailsById(this.id)
        .subscribe(properties => {
          this.properties = properties;
          this.propertyForm.setValue({
            id: properties[0].id,
            pnumber: properties[0].pnumber,
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
          this.propertyDetailsService.updateMainTableId(this.id);
          this.updateDocumentFieldValidation();
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
    this.imageName="Resources\\Images\\"+event;
  }
  
  previousDocumentsUploadFinished = (event:string) => { 
    this.previousDocumentsPath="Resources\\Images\\"+event;
  }
  
  existingDocumentsUploadFinished = (event:string) => { 
    this.existingDocumentsPath="Resources\\Images\\"+event;
  }
  resetChild(): void {
    if (this.childComponent) {
      // Child component is available, reset it
      this.childComponent.reset();
      this.propertyDetailsService.mainTableId=0;
      this.selectedPropertyId=0;
      this.id=0;
      this.router.navigate(['/dashboard/estate']);
    }
    this.propertyForm.reset({
      id: 0,
      pnumber:'',
      parea:'',
      punitTypeId:'',
      numofFloor:'',
      numofRooms:'',
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

  get pnumber() { return this.propertyForm.get('pnumber'); }
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


}
