import { Component, EventEmitter, Injectable, Input, Output, ViewChild } from '@angular/core';
import { NgbDate, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import '@angular/localize/init';
import {
	NgbDateStruct,
	NgbCalendar,
	NgbDatepickerI18n,
	NgbCalendarPersian,
} from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { companydetails } from 'src/app/models/companydetails';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { FileuploadComponent } from '../fileupload/fileupload.component';
import { PropertyService } from 'src/app/shared/property.service';
import { RealestateComponent } from '../realestate.component';


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
  selector: 'app-companydetails',
  templateUrl: './companydetails.component.html',
  styleUrls: ['./companydetails.component.scss'],
  providers: [
		{ provide: NgbCalendar, useClass: NgbCalendarPersian },
		{ provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
	],
})
export class CompanydetailsComponent {

	maxDate ={year:1410,month: 12, day: 31}
	minDate ={year:1320,month: 12, day: 31}

  imageName:string=''
  selectedDate!: NgbDate;
  companyForm: FormGroup = new FormGroup({});
  selectedId:number=0;
  companyDetails!: companydetails[];
  @ViewChild('childComponent') childComponent!: FileuploadComponent;
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
  constructor(private fb: FormBuilder,private toastr: ToastrService, private comservice:CompnaydetailService,private ngbDateParserFormatter: NgbDateParserFormatter,
	private propertyDetailsService: PropertyService, private parentComponent: RealestateComponent){
	this.companyForm = this.fb.group({
		id: [0],
		title: ['', Validators.required],
		phoneNumber: ['',Validators.required],
		licenseNumber: ['',Validators.required],
		petitionDate: ['',Validators.required],
		petitionNumber: ['',Validators.required],
		tin:['',Validators.required],
		docPath: [''],
	  });
	  this.comservice.mainTableId=0;
	  this.comservice.ownerId=0;
	}
	
	ngOnInit() {
		// var yyyy=2021;
		// this.selectedDate = new NgbDate(2021, 1, 1);
		this.comservice.getCompanyById(this.id)
		.subscribe(detail => {
		  this.companyDetails = detail;
		  this.companyForm.setValue({
			id: detail[0].id,
			title:detail[0].title,
			phoneNumber:detail[0].phoneNumber,
			licenseNumber:detail[0].licenseNumber,
			petitionDate:detail[0].petitionDate,
			petitionNumber:detail[0].petitionNumber,
			tin:detail[0].tin,
			docPath:detail[0].docPath

		  });
		  this.comservice.mainTableId=detail[0].id;
		  this.selectedId=detail[0].id;
		  const dateString = detail[0].petitionDate;
		  const parsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(dateString);
		  let parsedDate: NgbDate | null = null;
		  
		  if (parsedDateStruct) {
			parsedDate = new NgbDate(parsedDateStruct.year, parsedDateStruct.month, parsedDateStruct.day);
		  }
		  if(parsedDate){
			this.selectedDate = parsedDate;
		  }
		  
		});
	}

	addCompanyDetails(): void {
		const companyDetail = this.companyForm.value as companydetails;
		const date = new Date(
			this.selectedDate.year,
			this.selectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
			this.selectedDate.day
		  );
		companyDetail.petitionDate = date.toISOString();
		companyDetail.docPath=this.imageName;
		 if(companyDetail.id===null){
		  companyDetail.id=0;
		}
		this.comservice.addcompanies(companyDetail).subscribe(
		  result => {
		    console.log('Company added successfully:', result);
		    if(result.id!==0) {
		      this.toastr.success("معلومات موفقانه ثبت شد");
		      this.comservice.updateMainTableId(result.id);
		      this.selectedId=result.id;
		      this.onNextClick();
		    }
		  },
		  error => {
		    console.error('Error adding company:', error);
		    this.toastr.error("خرابی در ثبت معلومات: " + (error.message || 'نامعلوم'));
		  }
		);
	}
	updateCompanyDetails():void{
 
		const companyDetail = this.companyForm.value as companydetails;
		const date = new Date(
			Date.UTC(
			this.selectedDate.year,
			this.selectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
			this.selectedDate.day
			)
		  );
		companyDetail.petitionDate = date.toISOString();
		companyDetail.docPath=this.imageName;
		if(companyDetail.id===0 && this.selectedId!==0 || this.selectedId!==null){
		  companyDetail.id=this.selectedId;
		}
		this.comservice.updatecompanies(companyDetail).subscribe(
		  result => {
		    console.log('Company updated successfully:', result);
		    if(result.id!==0) {
		      this.comservice.updateMainTableId(result.id);
		      this.selectedId=result.id;
		      this.toastr.info("معلومات موفقانه تغیر یافت ");
		      this.onNextClick();
		    }
		  },
		  error => {
		    console.error('Error updating company:', error);
		    this.toastr.error("خرابی در تغیر معلومات: " + (error.message || 'نامعلوم'));
		  }
		);
	  
	}
  uploadFinished = (event:string) => { 
    this.imageName=event;
  }

 downloadFiles() {
    const filePath = this.companyForm.get('docPath')?.value;
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
  resetForms(): void {
	this.parentComponent.resetChild();
   }
  resetChild(){
	if (this.childComponent) {
		// Child component is available, reset it
		this.childComponent.reset();
	}
	this.companyForm.reset();
	this.comservice.mainTableId=0;
	this.selectedId=0;
  }
 get title() { return this.companyForm.get('title'); }
 get phoneNumber() { return this.companyForm.get('phoneNumber'); }
 get licenseNumber() { return this.companyForm.get('licenseNumber'); }
 get petitionDate() { return this.companyForm.get('petitionDate'); }
 get petitionNumber() { return this.companyForm.get('petitionNumber'); }
 get tin() { return this.companyForm.get('tin'); }
 get docPath() { return this.companyForm.get('docPath'); }
}
