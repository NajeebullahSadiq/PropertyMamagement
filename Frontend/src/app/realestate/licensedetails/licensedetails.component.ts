import { Component, EventEmitter, Injectable, Input, Output, ViewChild } from '@angular/core';
import {
	NgbDateStruct,
	NgbCalendar,
	NgbDatepickerI18n,
	NgbCalendarPersian,
  NgbDate,
  NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { SellerService } from 'src/app/shared/seller.service';
import { LicenseDetail } from 'src/app/models/LicenseDetail';
import { PropertyService } from 'src/app/shared/property.service';
import { FileuploadComponent } from '../fileupload/fileupload.component';
import { Router } from '@angular/router';
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
  selector: 'app-licensedetails',
  templateUrl: './licensedetails.component.html',
  styleUrls: ['./licensedetails.component.scss'],
  providers: [
		{ provide: NgbCalendar, useClass: NgbCalendarPersian },
		{ provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
	],
})
export class LicensedetailsComponent {

  maxDate ={year:1410,month: 12, day: 31}
	minDate ={year:1320,month: 12, day: 31}

  licenseForm: FormGroup = new FormGroup({});
  selectedId:number=0;
  selectedDateIssue!: NgbDate;
  selectedDateExpire!: NgbDate;
  Areas:any;
  imageName:string=''
  licenseDetails!: LicenseDetail[];
  licenseTypes = [
    { id: 'realEstate', name: 'Real Estate', dari: 'املاک' },
    { id: 'carSale', name: 'Car Sale', dari: 'موټر فروشی' }
  ];
  @Input() id: number=0;
  @Output() next = new EventEmitter<void>();
  onNextClick() {
    this.next.emit();
  }
  @ViewChild('childComponent') childComponent!: FileuploadComponent;
  ngAfterViewInit(): void {
    if (this.childComponent) {
      // Child component is ready, call its reset method
      this.childComponent.reset();
    }
  }
  constructor(private fb: FormBuilder,private toastr: ToastrService, private comservice:CompnaydetailService,private selerService:SellerService,
    private ngbDateParserFormatter: NgbDateParserFormatter,private propertyDetailsService: PropertyService,private router: Router){
    this.licenseForm = this.fb.group({
      id: [0],
      licenseNumber: ['', Validators.required],
      issueDate:['', Validators.required],
      expireDate:['', Validators.required],
      areaId:['', Validators.required],
      officeAddress:['', Validators.required],
      licenseType:['', Validators.required],
      companyId:[''],
      docPath:[''],
      });
    }

    ngOnInit() {
      this.comservice.getArea().subscribe(res => {
        this.Areas = res;
      });

      this.comservice.getLicenseById(this.id)
		.subscribe({
		  next: (detail) => {
		    if (detail && detail.length > 0) {
		      this.licenseDetails = detail;
		      this.licenseForm.setValue({
			    id: detail[0].id,
                licenseNumber:detail[0].licenseNumber,
                issueDate:detail[0].issueDate,
                expireDate:detail[0].expireDate,
                areaId:detail[0].areaId,
                officeAddress:detail[0].officeAddress,
                licenseType:detail[0].licenseType,
                companyId:detail[0].companyId,
                docPath:detail[0].docPath,
		      });
              this.selectedId=detail[0].id;
		      const dateString = detail[0].issueDate;
              const ExdateString = detail[0].expireDate;
		      const parsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(dateString);
              const ExparsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(ExdateString);
		      let parsedDate: NgbDate | null = null;
              let exparsedDate: NgbDate | null = null;
		      if (parsedDateStruct && ExparsedDateStruct) {
			    parsedDate = new NgbDate(parsedDateStruct.year, parsedDateStruct.month, parsedDateStruct.day);
                exparsedDate=new NgbDate(ExparsedDateStruct.year, ExparsedDateStruct.month, ExparsedDateStruct.day);
		      }
		      if(parsedDate && exparsedDate){
			    this.selectedDateIssue = parsedDate;
                this.selectedDateExpire=exparsedDate;
		      }
		    }
		  },
		  error: (error) => {
		    console.error('Error loading license details:', error);
		  }
    });
    }
  updateData():void{
    const details = this.licenseForm.value as LicenseDetail;
    const dateIssue = new Date(
      Date.UTC(
      this.selectedDateIssue.year,
      this.selectedDateIssue.month - 1, // Adjust month value (JavaScript months are 0-based)
      this.selectedDateIssue.day
      )
      );
      const dateExpire = new Date(
        Date.UTC(
          this.selectedDateExpire.year,
          this.selectedDateExpire.month - 1,
          this.selectedDateExpire.day
        )
      );
      details.issueDate = dateIssue.toISOString();
      details.expireDate=dateExpire.toISOString();
		  details.companyId=this.comservice.mainTableId;
		details.docPath=this.imageName;
		if(details.id===0 && this.selectedId!==0 || this.selectedId!==null){
			details.id=this.selectedId;
		}
		this.comservice.updateLicenseDetails(details).subscribe(result => {
		  if(result.id!==0)
		   this.selectedId=result.id;
		   this.toastr.info("معلومات موفقانه تغیر یافت ");
		
		});
  }
  addData():void{
    const details = this.licenseForm.value as LicenseDetail;
    const dateIssue = new Date(
      Date.UTC(
      this.selectedDateIssue.year,
      this.selectedDateIssue.month - 1, // Adjust month value (JavaScript months are 0-based)
      this.selectedDateIssue.day
      )
      );
      const dateExpire = new Date(
        Date.UTC(
          this.selectedDateExpire.year,
          this.selectedDateExpire.month - 1,
          this.selectedDateExpire.day
        )
      );
      details.issueDate = dateIssue.toISOString();
      details.expireDate=dateExpire.toISOString();
      details.docPath=this.imageName;
      details.companyId=this.comservice.mainTableId;
     if(details.id===null){
      details.id=0;
    }
    this.comservice.addLicenseDetails(details).subscribe(result => {
      if(result.id!==0)
       this.toastr.success("معلومات موفقانه ثبت شد");
       this.selectedId=result.id;
       this.onNextClick();
    });
  }
  resetForms():void{
    if (this.childComponent) {
      // Child component is available, reset it
      this.childComponent.reset();
      }
   this.selectedId=0;
   this.licenseForm.reset();
  }
  uploadFinished = (event:string) => { 
    this.imageName=event;
  }
  downloadFiles() {
    const filePath = this.licenseForm.get('docPath')?.value;
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
  navigateToPrint(){
   let id= this.comservice.mainTableId;
   this.router.navigate(['/printLicense', id]);
  }
  get licenseNumber() { return this.licenseForm.get('licenseNumber'); }
  get officeAddress() { return this.licenseForm.get('officeAddress'); }
  get issueDate() { return this.licenseForm.get('issueDate'); }
  get expireDate() { return this.licenseForm.get('expireDate'); }
  get licenseType() { return this.licenseForm.get('licenseType'); }
  get licenareaIdseNumber() { return this.licenseForm.get('areaId'); }
  get companyId() { return this.licenseForm.get('companyId'); }
  get docPath() { return this.licenseForm.get('docPath'); }
}
