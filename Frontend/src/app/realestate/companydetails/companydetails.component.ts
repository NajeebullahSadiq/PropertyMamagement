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
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarType } from 'src/app/models/calendar-type';


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
  selectedDate: NgbDate;
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
	private propertyDetailsService: PropertyService, private parentComponent: RealestateComponent, private calendar: NgbCalendar,
	private calendarConversionService: CalendarConversionService, private calendarService: CalendarService){
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
	  this.selectedDate = this.calendar.getToday();
	}
	
	ngOnInit() {
		if (this.id && this.id > 0) {
			this.comservice.getCompanyById(this.id)
			.subscribe({
			  next: (detail) => {
			    if (detail && detail.length > 0) {
			      this.companyDetails = detail;
			      
			      // Keep the date as a string - it's already in the correct calendar format from the backend
			      const dateString = detail[0].petitionDate;
			      
			      this.companyForm.setValue({
				    id: detail[0].id,
				    title:detail[0].title,
				    phoneNumber:detail[0].phoneNumber,
				    licenseNumber:detail[0].licenseNumber,
				    petitionDate: dateString, // Keep as string
				    petitionNumber:detail[0].petitionNumber,
				    tin:detail[0].tin,
				    docPath:detail[0].docPath
			      });
			      
			      // Set imageName from existing docPath
			      this.imageName = detail[0].docPath || '';
			      
			      this.comservice.mainTableId=detail[0].id;
			      this.selectedId=detail[0].id;
			    }
			  },
			  error: (error) => {
			    console.error('Error loading company details:', error);
			  }
			});
		}
	}

	addCompanyDetails(): void {
		const companyDetail = this.companyForm.value as companydetails;
		const petitionDateValue = this.companyForm.get('petitionDate')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();
		
		if (petitionDateValue) {
			// Format date based on current calendar type
			if (petitionDateValue instanceof Date) {
				// Date object from multi-calendar-datepicker - format for the current calendar
				const calendarDate = this.calendarConversionService.fromGregorian(petitionDateValue, currentCalendar);
				const year = calendarDate.year;
				const month = String(calendarDate.month).padStart(2, '0');
				const day = String(calendarDate.day).padStart(2, '0');
				companyDetail.petitionDate = `${year}-${month}-${day}`;
			} else if (typeof petitionDateValue === 'object' && petitionDateValue.year) {
				// NgbDateStruct - already in calendar format
				const year = petitionDateValue.year;
				const month = String(petitionDateValue.month).padStart(2, '0');
				const day = String(petitionDateValue.day).padStart(2, '0');
				companyDetail.petitionDate = `${year}-${month}-${day}`;
			} else if (typeof petitionDateValue === 'string') {
				// Already a string - normalize format (replace / with -)
				companyDetail.petitionDate = petitionDateValue.replace(/\//g, '-');
			} else {
				this.toastr.error("فرمت تاریخ نامعتبر است");
				return;
			}
		} else {
			this.toastr.error("لطفا تاریخ ارائه عریضه را انتخاب کنید");
			return;
		}
		
		// Send calendar type to backend
		companyDetail.calendarType = currentCalendar;
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
		const petitionDateValue = this.companyForm.get('petitionDate')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();
		
		if (petitionDateValue) {
			// Format date based on current calendar type
			if (petitionDateValue instanceof Date) {
				// Date object from multi-calendar-datepicker - format for the current calendar
				const calendarDate = this.calendarConversionService.fromGregorian(petitionDateValue, currentCalendar);
				const year = calendarDate.year;
				const month = String(calendarDate.month).padStart(2, '0');
				const day = String(calendarDate.day).padStart(2, '0');
				companyDetail.petitionDate = `${year}-${month}-${day}`;
			} else if (typeof petitionDateValue === 'object' && petitionDateValue.year) {
				// NgbDateStruct - already in calendar format
				const year = petitionDateValue.year;
				const month = String(petitionDateValue.month).padStart(2, '0');
				const day = String(petitionDateValue.day).padStart(2, '0');
				companyDetail.petitionDate = `${year}-${month}-${day}`;
			} else if (typeof petitionDateValue === 'string') {
				// Already a string - normalize format (replace / with -)
				companyDetail.petitionDate = petitionDateValue.replace(/\//g, '-');
			} else {
				this.toastr.error("فرمت تاریخ نامعتبر است");
				return;
			}
		} else {
			this.toastr.error("لطفا تاریخ ارائه عریضه را انتخاب کنید");
			return;
		}
		
		// Send calendar type to backend
		companyDetail.calendarType = currentCalendar;
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
