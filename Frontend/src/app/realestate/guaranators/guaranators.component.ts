import { Component, EventEmitter, Injectable, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbDate, NgbDateParserFormatter, NgbDateStruct, NgbCalendar, NgbDatepickerI18n, NgbCalendarPersian } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Guarantor } from 'src/app/models/Guarantor';
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
      identityCardTypeId: ['', Validators.required],
      indentityCardNumber: ['', Validators.required],
      jild: ['', Validators.required],
      safha: ['', Validators.required],
      companyId: [''],
      sabtNumber: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      paddressProvinceId: ['', Validators.required],
      paddressDistrictId: ['', Validators.required],
      paddressVillage: ['', Validators.required],
      taddressProvinceId: ['', Validators.required],
      taddressDistrictId: ['', Validators.required],
      taddressVillage: ['', Validators.required],
      pothoPath: [''],
      // Guarantee fields
      guaranteeTypeId: [''],
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
    });
  }

  ngOnInit() {
    this.selerService.getprovince().subscribe(res => {
      this.province = res;
    });
    this.comservice.getIdentityTypes().subscribe(res => {
      this.IdTypes = res;
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

    // Convert string form values to numbers for backend
    details.propertyDocumentNumber = Number(details.propertyDocumentNumber) || undefined;
    details.answerdMaktobNumber = Number(details.answerdMaktobNumber) || undefined;
    details.guaranteeDocNumber = Number(details.guaranteeDocNumber) || undefined;
    details.guaranteeTypeId = Number(details.guaranteeTypeId) || undefined;

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

    // Convert string form values to numbers for backend
    details.propertyDocumentNumber = Number(details.propertyDocumentNumber) || undefined;
    details.answerdMaktobNumber = Number(details.answerdMaktobNumber) || undefined;
    details.guaranteeDocNumber = Number(details.guaranteeDocNumber) || undefined;
    details.guaranteeTypeId = Number(details.guaranteeTypeId) || undefined;

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

  onPropertyTypeChange() {
    const identityCardTypeId = this.guaranatorForm.get('identityCardTypeId')?.value;
    const jild = this.guaranatorForm.get('jild');
    const safha = this.guaranatorForm.get('safha');
    const sabtNumber = this.guaranatorForm.get('sabtNumber');
    if (identityCardTypeId === 1) {
      jild?.setValue(0);
      jild?.disable();
      safha?.setValue(0);
      safha?.disable();
      sabtNumber?.setValue(0);
      sabtNumber?.disable();
    } else {
      jild?.enable();
      jild?.setValue(null);
      safha?.enable();
      safha?.setValue(null);
      sabtNumber?.enable();
      sabtNumber?.setValue(null);
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
        identityCardTypeId: selectedOwnerAddress.identityCardTypeId,
        indentityCardNumber: selectedOwnerAddress.indentityCardNumber,
        jild: selectedOwnerAddress.jild,
        safha: selectedOwnerAddress.safha,
        companyId: selectedOwnerAddress.companyId,
        sabtNumber: selectedOwnerAddress.sabtNumber,
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
      this.onPropertyTypeChange();
    }
  }

  private parseAndSetDates(data: Guarantor) {
    const dateFields = [
      { field: 'propertyDocumentDate', value: data.propertyDocumentDate },
      { field: 'senderMaktobDate', value: data.senderMaktobDate },
      { field: 'answerdMaktobDate', value: data.answerdMaktobDate },
      { field: 'dateofGuarantee', value: data.dateofGuarantee },
      { field: 'guaranteeDate', value: data.guaranteeDate },
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
      const localized = this.localizationService.guaranteeTypes.find(
        gt => gt.value.toLowerCase() === type.name.toLowerCase()
      );
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
  get identityCardTypeId() { return this.guaranatorForm.get('identityCardTypeId'); }
  get indentityCardNumber() { return this.guaranatorForm.get('indentityCardNumber'); }
  get jild() { return this.guaranatorForm.get('jild'); }
  get safha() { return this.guaranatorForm.get('safha'); }
  get companyId() { return this.guaranatorForm.get('companyId'); }
  get sabtNumber() { return this.guaranatorForm.get('sabtNumber'); }
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
}
