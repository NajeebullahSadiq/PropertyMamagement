import { Component, EventEmitter, Injectable, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbDate, NgbDateParserFormatter, NgbDateStruct, NgbCalendar, NgbDatepickerI18n, NgbCalendarPersian } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Guarantor, GuaranteeTypeEnum } from 'src/app/models/Guarantor';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { SellerService } from 'src/app/shared/seller.service';
import { ProfileImageCropperComponent } from 'src/app/shared/profile-image-cropper/profile-image-cropper.component';
import { environment } from 'src/environments/environment';
import { FileuploadComponent } from '../fileupload/fileupload.component';
import { LocalizationService } from 'src/app/shared/localization.service';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import '@angular/localize/init';

const WEEKDAYS_SHORT = ['د', 'س', 'چ', 'پ', 'ج', 'ش', 'ی'];
const MONTHS = ['حمل', 'ثور', 'جوزا', 'سرطان', 'اسد', 'سنبله', 'میزان', 'عقرب', 'قوس', 'جدی', 'دلو', 'حوت'];

@Injectable()
export class NgbDatepickerI18nPersian extends NgbDatepickerI18n {
  getWeekdayLabel(weekday: number) {
    return WEEKDAYS_SHORT[weekday - 1];
  }
  getMonthShortName(month: number) {
    return MONTHS[month - 1];
  }
  getMonthFullName(month: number) {
    return MONTHS[month - 1];
  }
  getDayAriaLabel(date: NgbDateStruct): string {
    return `${date.year}-${this.getMonthFullName(date.month)}-${date.day}`;
  }
}

@Component({
  selector: 'app-guaranators',
  templateUrl: './guaranators.component.html',
  styleUrls: ['./guaranators.component.scss'],
  providers: [
    { provide: NgbCalendar, useClass: NgbCalendarPersian },
    { provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
  ],
})
export class GuaranatorsComponent {
  maxDate = { year: 1410, month: 12, day: 31 };
  minDate = { year: 1320, month: 12, day: 31 };

  baseUrl: string = environment.apiURL + '/';
  imagePath: string = 'assets/img/avatar.png';
  imageName: string = '';
  guaranteeDocName: string = '';
  selectedId: number = 0;
  IdTypes: any;
  province: any;
  district: any;
  district2: any;
  GuaranteeTypes: any;
  localizedGuaranteeTypes: any;
  guaranatorForm: FormGroup = new FormGroup({});
  guaranatorDetails!: Guarantor[];
  
  // Districts for guarantee (Customary Deed)
  guaranteeDistricts: any;
  
  // Guarantee Type visibility flags
  showShariaDeedFields = false;
  showCustomaryDeedFields = false;
  showCashFields = false;

  @Input() id: number = 0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  @ViewChild('childComponent') childComponent!: ProfileImageCropperComponent;
  @ViewChild('guaranteeFileUpload') guaranteeFileUpload!: FileuploadComponent;
  private pendingImagePath: string = '';

  ngAfterViewInit(): void {
    if (this.pendingImagePath && this.childComponent) {
      this.childComponent.setExistingImage(this.pendingImagePath);
      this.pendingImagePath = '';
    }
  }

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private comservice: CompnaydetailService,
    private selerService: SellerService,
    private ngbDateParserFormatter: NgbDateParserFormatter,
    private localizationService: LocalizationService,
    private calendarConversionService: CalendarConversionService,
    private calendarService: CalendarService
  ) {
    this.guaranatorForm = this.fb.group({
      id: [0],
      firstName: ['', Validators.required],
      fatherName: ['', Validators.required],
      grandFatherName: [''],
      electronicNationalIdNumber: ['', Validators.required],
      companyId: [''],
      phoneNumber: ['', Validators.required],
      paddressProvinceId: ['', Validators.required],
      paddressDistrictId: ['', Validators.required],
      paddressVillage: ['', Validators.required],
      taddressProvinceId: ['', Validators.required],
      taddressDistrictId: ['', Validators.required],
      taddressVillage: ['', Validators.required],
      pothoPath: [''],
      // Guarantee fields
      guaranteeTypeId: ['', Validators.required],
      propertyDocumentNumber: [''],
      propertyDocumentDate: [''],
      senderMaktobNumber: [''],
      senderMaktobDate: [''],
      answerdMaktobNumber: [''],
      answerdMaktobDate: [''],
      dateofGuarantee: [''],
      guaranteeDocNumber: [''],
      guaranteeDate: [''],
      guaranteeDocPath: [''],
      // Conditional fields - Sharia Deed (قباله شرعی)
      courtName: [''],
      collateralNumber: [''],
      // Conditional fields - Customary Deed (قباله عرفی)
      setSerialNumber: [''],
      guaranteeDistrictId: [''],
      // Conditional fields - Cash (پول نقد)
      bankName: [''],
      depositNumber: [''],
      depositDate: [''],
    });
  }

  ngOnInit() {
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    this.comservice.getGuaranteeType().subscribe(res => {
      this.GuaranteeTypes = res;
      this.localizedGuaranteeTypes = this.mapGuaranteeTypesToLocalized(res as any[]);
    });

    this.comservice.getGuaranatorById(this.id)
      .subscribe(detail => {
        this.guaranatorDetails = detail;
      });
  }

  private formatDateForBackend(dateValue: any): string {
    const currentCalendar = this.calendarService.getSelectedCalendar();

    if (dateValue instanceof Date) {
      const calendarDate = this.calendarConversionService.fromGregorian(dateValue, currentCalendar);
      const year = calendarDate.year;
      const month = String(calendarDate.month).padStart(2, '0');
      const day = String(calendarDate.day).padStart(2, '0');
      return `${year}-${month}-${day}`;
    } else if (typeof dateValue === 'object' && dateValue?.year) {
      const year = dateValue.year;
      const month = String(dateValue.month).padStart(2, '0');
      const day = String(dateValue.day).padStart(2, '0');
      return `${year}-${month}-${day}`;
    } else if (typeof dateValue === 'string') {
      return dateValue.replace(/\//g, '-');
    }
    return '';
  }

  uploadFinished = (event: string) => {
    this.imageName = event;
    this.imagePath = event ? (this.baseUrl + event) : 'assets/img/avatar.png';
  }

  profilePreviewChanged = (localObjectUrl: string) => {
    if (localObjectUrl) {
      this.imagePath = localObjectUrl;
      return;
    }

    if (this.imageName) {
      this.imagePath = this.baseUrl + this.imageName;
      return;
    }

    this.imagePath = 'assets/img/avatar.png';
  }

  profileImageUploaded = (dbPath: string) => {
    this.imageName = dbPath || '';
    this.guaranatorForm.patchValue({ pothoPath: this.imageName });
    this.imagePath = this.imageName ? (this.baseUrl + this.imageName) : 'assets/img/avatar.png';
  }

  guaranteeDocUploadFinished = (event: string) => {
    this.guaranteeDocName = event;
    this.guaranatorForm.patchValue({ guaranteeDocPath: this.guaranteeDocName });
  }

  addData(): void {
    const details = this.guaranatorForm.value as Guarantor;
    const currentCalendar = this.calendarService.getSelectedCalendar();

    details.pothoPath = this.imageName;
    details.companyId = this.comservice.mainTableId;
    details.guaranteeDocPath = this.guaranteeDocName;
    details.calendarType = currentCalendar;

    // Format guarantee dates
    details.propertyDocumentDate = this.formatDateForBackend(this.guaranatorForm.get('propertyDocumentDate')?.value);
    details.senderMaktobDate = this.formatDateForBackend(this.guaranatorForm.get('senderMaktobDate')?.value);
    details.answerdMaktobDate = this.formatDateForBackend(this.guaranatorForm.get('answerdMaktobDate')?.value);
    details.dateofGuarantee = this.formatDateForBackend(this.guaranatorForm.get('dateofGuarantee')?.value);
    details.guaranteeDate = this.formatDateForBackend(this.guaranatorForm.get('guaranteeDate')?.value);
    
    // Format conditional date field (Cash)
    details.depositDate = this.formatDateForBackend(this.guaranatorForm.get('depositDate')?.value);

    // Convert string form values to numbers for backend
    details.propertyDocumentNumber = Number(details.propertyDocumentNumber) || undefined;
    details.answerdMaktobNumber = Number(details.answerdMaktobNumber) || undefined;
    details.guaranteeDocNumber = Number(details.guaranteeDocNumber) || undefined;
    details.guaranteeTypeId = Number(details.guaranteeTypeId) || undefined;
    details.guaranteeDistrictId = Number(details.guaranteeDistrictId) || undefined;

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
    const currentCalendar = this.calendarService.getSelectedCalendar();

    details.companyId = this.comservice.mainTableId;
    details.pothoPath = this.imageName;
    details.guaranteeDocPath = this.guaranteeDocName;
    details.calendarType = currentCalendar;

    // Format guarantee dates
    details.propertyDocumentDate = this.formatDateForBackend(this.guaranatorForm.get('propertyDocumentDate')?.value);
    details.senderMaktobDate = this.formatDateForBackend(this.guaranatorForm.get('senderMaktobDate')?.value);
    details.answerdMaktobDate = this.formatDateForBackend(this.guaranatorForm.get('answerdMaktobDate')?.value);
    details.dateofGuarantee = this.formatDateForBackend(this.guaranatorForm.get('dateofGuarantee')?.value);
    details.guaranteeDate = this.formatDateForBackend(this.guaranatorForm.get('guaranteeDate')?.value);
    
    // Format conditional date field (Cash)
    details.depositDate = this.formatDateForBackend(this.guaranatorForm.get('depositDate')?.value);

    // Convert string form values to numbers for backend
    details.propertyDocumentNumber = Number(details.propertyDocumentNumber) || undefined;
    details.answerdMaktobNumber = Number(details.answerdMaktobNumber) || undefined;
    details.guaranteeDocNumber = Number(details.guaranteeDocNumber) || undefined;
    details.guaranteeTypeId = Number(details.guaranteeTypeId) || undefined;
    details.guaranteeDistrictId = Number(details.guaranteeDistrictId) || undefined;

    if (details.id === 0 && this.selectedId !== 0 || this.selectedId !== null) {
      details.id = this.selectedId;
    }
    this.comservice.updateGuaranator(details).subscribe(result => {
      if (result.id !== 0)
        this.selectedId = result.id;
      this.toastr.info("معلومات موفقانه تغیر یافت ");
    });
  }

  resetForms(): void {
    if (this.childComponent) {
      this.childComponent.reset();
    }
    if (this.guaranteeFileUpload) {
      this.guaranteeFileUpload.reset();
    }
    this.imagePath = 'assets/img/avatar.png';
    this.imageName = '';
    this.guaranteeDocName = '';
    this.selectedId = 0;
    this.guaranatorForm.reset();
    
    // Reset visibility flags
    this.showShariaDeedFields = false;
    this.showCustomaryDeedFields = false;
    this.showCashFields = false;
    
    // Clear conditional validators
    this.clearConditionalValidators();
    this.updateConditionalFieldsValidity();
  }

  filterResults(getId: any) {
    this.selerService.getdistrict(getId.id).subscribe(res => {
      this.district = res;
    });
  }

  filterResults2(getId: any) {
    this.selerService.getdistrict(getId.id).subscribe(res => {
      this.district2 = res;
    });
  }

  /**
   * Handle guarantee type change - show/hide conditional fields and update validators
   */
  onGuaranteeTypeChange() {
    const guaranteeTypeId = Number(this.guaranatorForm.get('guaranteeTypeId')?.value);
    
    // Reset visibility flags
    this.showShariaDeedFields = false;
    this.showCustomaryDeedFields = false;
    this.showCashFields = false;
    
    // Clear all conditional field validators first
    this.clearConditionalValidators();
    
    // Clear all conditional field values
    this.clearConditionalFieldValues();
    
    // Set visibility and validators based on selected type
    switch (guaranteeTypeId) {
      case GuaranteeTypeEnum.Cash: // پول نقد
        this.showCashFields = true;
        this.guaranatorForm.get('bankName')?.setValidators([Validators.required]);
        this.guaranatorForm.get('depositNumber')?.setValidators([Validators.required]);
        this.guaranatorForm.get('depositDate')?.setValidators([Validators.required]);
        break;
        
      case GuaranteeTypeEnum.ShariaDeed: // قباله شرعی
        this.showShariaDeedFields = true;
        this.guaranatorForm.get('courtName')?.setValidators([Validators.required]);
        this.guaranatorForm.get('collateralNumber')?.setValidators([Validators.required]);
        break;
        
      case GuaranteeTypeEnum.CustomaryDeed: // قباله عرفی
        this.showCustomaryDeedFields = true;
        this.guaranatorForm.get('setSerialNumber')?.setValidators([Validators.required]);
        this.guaranatorForm.get('guaranteeDistrictId')?.setValidators([Validators.required]);
        break;
    }
    
    // Update validity for all conditional fields
    this.updateConditionalFieldsValidity();
  }
  
  /**
   * Clear validators from all conditional fields
   */
  private clearConditionalValidators() {
    // Sharia Deed fields
    this.guaranatorForm.get('courtName')?.clearValidators();
    this.guaranatorForm.get('collateralNumber')?.clearValidators();
    // Customary Deed fields
    this.guaranatorForm.get('setSerialNumber')?.clearValidators();
    this.guaranatorForm.get('guaranteeDistrictId')?.clearValidators();
    // Cash fields
    this.guaranatorForm.get('bankName')?.clearValidators();
    this.guaranatorForm.get('depositNumber')?.clearValidators();
    this.guaranatorForm.get('depositDate')?.clearValidators();
  }
  
  /**
   * Clear values from all conditional fields
   */
  private clearConditionalFieldValues() {
    // Sharia Deed fields
    this.guaranatorForm.patchValue({
      courtName: '',
      collateralNumber: '',
      // Customary Deed fields
      setSerialNumber: '',
      guaranteeDistrictId: '',
      // Cash fields
      bankName: '',
      depositNumber: '',
      depositDate: '',
    });
  }
  
  /**
   * Update validity status for all conditional fields
   */
  private updateConditionalFieldsValidity() {
    this.guaranatorForm.get('courtName')?.updateValueAndValidity();
    this.guaranatorForm.get('collateralNumber')?.updateValueAndValidity();
    this.guaranatorForm.get('setSerialNumber')?.updateValueAndValidity();
    this.guaranatorForm.get('guaranteeDistrictId')?.updateValueAndValidity();
    this.guaranatorForm.get('bankName')?.updateValueAndValidity();
    this.guaranatorForm.get('depositNumber')?.updateValueAndValidity();
    this.guaranatorForm.get('depositDate')?.updateValueAndValidity();
  }
  
  /**
   * Load districts for guarantee (Customary Deed) based on province
   */
  filterGuaranteeDistricts(event: any) {
    if (event?.id) {
      this.selerService.getdistrict(event.id).subscribe(res => {
        this.guaranteeDistricts = res;
      });
    }
  }

  BindValu(id: number) {
    const selectedOwnerAddress = this.guaranatorDetails.find(w => w.id === id);
    if (selectedOwnerAddress) {
      this.guaranatorForm.patchValue({
        id: selectedOwnerAddress.id,
        firstName: selectedOwnerAddress.firstName,
        fatherName: selectedOwnerAddress.fatherName,
        grandFatherName: selectedOwnerAddress.grandFatherName,
        electronicNationalIdNumber: selectedOwnerAddress.electronicNationalIdNumber,
        companyId: selectedOwnerAddress.companyId,
        phoneNumber: selectedOwnerAddress.phoneNumber,
        paddressProvinceId: selectedOwnerAddress.paddressProvinceId,
        paddressDistrictId: selectedOwnerAddress.paddressDistrictId,
        paddressVillage: selectedOwnerAddress.paddressVillage,
        taddressProvinceId: selectedOwnerAddress.taddressProvinceId,
        taddressDistrictId: selectedOwnerAddress.taddressDistrictId,
        taddressVillage: selectedOwnerAddress.taddressVillage,
        pothoPath: selectedOwnerAddress.pothoPath,
        // Guarantee fields
        guaranteeTypeId: selectedOwnerAddress.guaranteeTypeId,
        propertyDocumentNumber: selectedOwnerAddress.propertyDocumentNumber,
        propertyDocumentDate: selectedOwnerAddress.propertyDocumentDate,
        senderMaktobNumber: selectedOwnerAddress.senderMaktobNumber,
        senderMaktobDate: selectedOwnerAddress.senderMaktobDate,
        answerdMaktobNumber: selectedOwnerAddress.answerdMaktobNumber,
        answerdMaktobDate: selectedOwnerAddress.answerdMaktobDate,
        dateofGuarantee: selectedOwnerAddress.dateofGuarantee,
        guaranteeDocNumber: selectedOwnerAddress.guaranteeDocNumber,
        guaranteeDate: selectedOwnerAddress.guaranteeDate,
        guaranteeDocPath: selectedOwnerAddress.guaranteeDocPath,
        // Conditional fields - Sharia Deed
        courtName: selectedOwnerAddress.courtName,
        collateralNumber: selectedOwnerAddress.collateralNumber,
        // Conditional fields - Customary Deed
        setSerialNumber: selectedOwnerAddress.setSerialNumber,
        guaranteeDistrictId: selectedOwnerAddress.guaranteeDistrictId,
        // Conditional fields - Cash
        bankName: selectedOwnerAddress.bankName,
        depositNumber: selectedOwnerAddress.depositNumber,
        depositDate: selectedOwnerAddress.depositDate,
      });

      this.selerService.getdistrict(selectedOwnerAddress.paddressProvinceId.valueOf()).subscribe(res => {
        this.district = res;
      });
      this.selerService.getdistrict(selectedOwnerAddress.taddressProvinceId.valueOf()).subscribe(res => {
        this.district2 = res;
      });
      this.selectedId = id;
      this.imagePath = selectedOwnerAddress.pothoPath ? (this.baseUrl + selectedOwnerAddress.pothoPath) : 'assets/img/avatar.png';
      this.imageName = selectedOwnerAddress.pothoPath || '';
      this.guaranteeDocName = selectedOwnerAddress.guaranteeDocPath || '';

      if (selectedOwnerAddress.pothoPath) {
        if (this.childComponent) {
          this.childComponent.setExistingImage(this.baseUrl + selectedOwnerAddress.pothoPath);
        } else {
          this.pendingImagePath = this.baseUrl + selectedOwnerAddress.pothoPath;
        }
      }

      // Parse guarantee dates
      this.parseAndSetDates(selectedOwnerAddress);
      
      // Set visibility flags based on loaded guarantee type (without clearing values)
      this.setGuaranteeTypeVisibility(selectedOwnerAddress.guaranteeTypeId);
    }
  }
  
  /**
   * Set visibility flags based on guarantee type without clearing values
   * Used when loading existing data
   */
  private setGuaranteeTypeVisibility(guaranteeTypeId: number | undefined) {
    this.showShariaDeedFields = false;
    this.showCustomaryDeedFields = false;
    this.showCashFields = false;
    
    // Clear validators first
    this.clearConditionalValidators();
    
    switch (guaranteeTypeId) {
      case GuaranteeTypeEnum.Cash:
        this.showCashFields = true;
        this.guaranatorForm.get('bankName')?.setValidators([Validators.required]);
        this.guaranatorForm.get('depositNumber')?.setValidators([Validators.required]);
        this.guaranatorForm.get('depositDate')?.setValidators([Validators.required]);
        break;
        
      case GuaranteeTypeEnum.ShariaDeed:
        this.showShariaDeedFields = true;
        this.guaranatorForm.get('courtName')?.setValidators([Validators.required]);
        this.guaranatorForm.get('collateralNumber')?.setValidators([Validators.required]);
        break;
        
      case GuaranteeTypeEnum.CustomaryDeed:
        this.showCustomaryDeedFields = true;
        this.guaranatorForm.get('setSerialNumber')?.setValidators([Validators.required]);
        this.guaranatorForm.get('guaranteeDistrictId')?.setValidators([Validators.required]);
        break;
    }
    
    this.updateConditionalFieldsValidity();
  }

  private parseAndSetDates(data: Guarantor) {
    const dateFields = [
      { field: 'propertyDocumentDate', value: data.propertyDocumentDate },
      { field: 'senderMaktobDate', value: data.senderMaktobDate },
      { field: 'answerdMaktobDate', value: data.answerdMaktobDate },
      { field: 'dateofGuarantee', value: data.dateofGuarantee },
      { field: 'guaranteeDate', value: data.guaranteeDate },
      { field: 'depositDate', value: data.depositDate }, // Cash conditional field
    ];

    dateFields.forEach(({ field, value }) => {
      if (value) {
        const parsedDateStruct = this.ngbDateParserFormatter.parse(value);
        if (parsedDateStruct) {
          const parsedDate = new NgbDate(parsedDateStruct.year, parsedDateStruct.month, parsedDateStruct.day);
          this.guaranatorForm.patchValue({ [field]: parsedDate });
        }
      }
    });
  }

  mapGuaranteeTypesToLocalized(backendTypes: any[]): any[] {
    return backendTypes.map(type => {
      // First try to match by English value
      let localized = this.localizationService.guaranteeTypes.find(
        gt => gt.value.toLowerCase() === type.name.toLowerCase()
      );
      
      // If not found, try to match by Dari label (in case DB has Dari names)
      if (!localized) {
        localized = this.localizationService.guaranteeTypes.find(
          gt => gt.label === type.name
        );
      }
      
      // If still not found, try to match by ID
      if (!localized) {
        localized = this.localizationService.guaranteeTypes.find(
          gt => gt.id === type.id
        );
      }
      
      return {
        id: type.id,
        name: localized ? localized.label : type.name
      };
    });
  }

  getGuaranteeTypeName(guaranteeTypeId: number | undefined): string {
    if (!guaranteeTypeId || !this.localizedGuaranteeTypes) {
      return '-';
    }
    const type = this.localizedGuaranteeTypes.find((t: any) => t.id === guaranteeTypeId);
    return type ? type.name : '-';
  }

  get firstName() { return this.guaranatorForm.get('firstName'); }
  get fatherName() { return this.guaranatorForm.get('fatherName'); }
  get grandFatherName() { return this.guaranatorForm.get('grandFatherName'); }
  get electronicNationalIdNumber() { return this.guaranatorForm.get('electronicNationalIdNumber'); }
  get companyId() { return this.guaranatorForm.get('companyId'); }
  get pothoPath() { return this.guaranatorForm.get('pothoPath'); }
  get phoneNumber() { return this.guaranatorForm.get('phoneNumber'); }
  get paddressVillage() { return this.guaranatorForm.get('paddressVillage'); }
  get taddressVillage() { return this.guaranatorForm.get('taddressVillage'); }
  get paddressProvinceId() { return this.guaranatorForm.get('paddressProvinceId'); }
  get paddressDistrictId() { return this.guaranatorForm.get('paddressDistrictId'); }
  get taddressProvinceId() { return this.guaranatorForm.get('taddressProvinceId'); }
  get taddressDistrictId() { return this.guaranatorForm.get('taddressDistrictId'); }
  // Guarantee getters
  get guaranteeTypeId() { return this.guaranatorForm.get('guaranteeTypeId'); }
  get propertyDocumentNumber() { return this.guaranatorForm.get('propertyDocumentNumber'); }
  get propertyDocumentDate() { return this.guaranatorForm.get('propertyDocumentDate'); }
  get senderMaktobNumber() { return this.guaranatorForm.get('senderMaktobNumber'); }
  get senderMaktobDate() { return this.guaranatorForm.get('senderMaktobDate'); }
  get answerdMaktobNumber() { return this.guaranatorForm.get('answerdMaktobNumber'); }
  get answerdMaktobDate() { return this.guaranatorForm.get('answerdMaktobDate'); }
  get dateofGuarantee() { return this.guaranatorForm.get('dateofGuarantee'); }
  get guaranteeDocNumber() { return this.guaranatorForm.get('guaranteeDocNumber'); }
  get guaranteeDate() { return this.guaranatorForm.get('guaranteeDate'); }
  get guaranteeDocPath() { return this.guaranatorForm.get('guaranteeDocPath'); }
  // Conditional field getters - Sharia Deed
  get courtName() { return this.guaranatorForm.get('courtName'); }
  get collateralNumber() { return this.guaranatorForm.get('collateralNumber'); }
  // Conditional field getters - Customary Deed
  get setSerialNumber() { return this.guaranatorForm.get('setSerialNumber'); }
  get guaranteeDistrictId() { return this.guaranatorForm.get('guaranteeDistrictId'); }
  // Conditional field getters - Cash
  get bankName() { return this.guaranatorForm.get('bankName'); }
  get depositNumber() { return this.guaranatorForm.get('depositNumber'); }
  get depositDate() { return this.guaranatorForm.get('depositDate'); }
}
