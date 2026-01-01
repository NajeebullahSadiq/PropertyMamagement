import { Component, EventEmitter, Injectable, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import '@angular/localize/init';
import {
	NgbDateStruct,
	NgbCalendar,
	NgbDatepickerI18n,
	NgbCalendarPersian,
  NgbDate,
  NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { companyowner } from 'src/app/models/companyowner';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { SellerService } from 'src/app/shared/seller.service';
import { FileuploadComponent } from '../fileupload/fileupload.component';
import { environment } from 'src/environments/environment';


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
  selector: 'app-companyowner',
  templateUrl: './companyowner.component.html',
  styleUrls: ['./companyowner.component.scss'],
  providers: [
		{ provide: NgbCalendar, useClass: NgbCalendarPersian },
		{ provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
	],
})
export class CompanyownerComponent {
	maxDate ={year:1410,month: 12, day: 31}
	minDate ={year:1320,month: 12, day: 31}
	
  baseUrl:string=environment.apiURL+'/';
  imagePath:string='assets/img/avatar.png';
  imageName:string='';
  selectedId:number=0;
  selectedDate!: NgbDate;
  IdTypes:any;
  EducationLevel:any;
  ownerForm: FormGroup = new FormGroup({});
  ownerDetails!: companyowner[];
  filteredIdTypes:any; // Filtered list for Electric and Paper ID only

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
	private ngbDateParserFormatter: NgbDateParserFormatter){
	this.ownerForm = this.fb.group({
		id: [0],
		firstName: ['', Validators.required],
		fatherName:['', Validators.required],
		grandFatherName:['', Validators.required],
		educationLevelId:['', Validators.required],
		dateofBirth:['', Validators.required],
		identityCardTypeId:['', Validators.required],
		indentityCardNumber:['', Validators.required],
		jild:[''], // Will be conditionally required
		safha:[''], // Will be conditionally required
		companyId:[''],
		sabtNumber:[''], // Will be conditionally required
		pothoPath:['']
	  });
  }
  ngOnInit() {
	this.comservice.getIdentityTypes().subscribe(res => {
	  this.IdTypes = res;
	  // Filter to show only Electric ID and Paper ID based on exact Dari names
	  this.filteredIdTypes = (res as any[]).filter((item: any) => {
	    const name = item.name || '';
	    // Match exact Dari names: الکترونیکی (Electric) and کاغذی (Paper)
	    return name === 'الکترونیکی' || name === 'کاغذی';
	  }).map((item: any) => ({
	    ...item,
	    displayName: item.name // Use name field for display
	  }));
	});
	this.comservice.getEducationLevel().subscribe(res => {
		this.EducationLevel = res;
	  });

	  this.comservice.getOwnerById(this.id)
		.subscribe({
		  next: (detail) => {
		    if (detail && detail.length > 0) {
		      this.ownerDetails = detail;
		      this.ownerForm.setValue({
			    id: detail[0].id,
			    firstName:detail[0].firstName,
			    fatherName:detail[0].fatherName,
			    grandFatherName:detail[0].grandFatherName,
			    educationLevelId:detail[0].educationLevelId,
			    dateofBirth:detail[0].dateofBirth,
			    identityCardTypeId:detail[0].identityCardTypeId,
			    indentityCardNumber:detail[0].indentityCardNumber,
			    jild:detail[0].jild,
			    safha:detail[0].safha,
			    companyId:detail[0].companyId,
			    sabtNumber:detail[0].sabtNumber,
			    pothoPath:detail[0].pothoPath,

		      });
		      this.comservice.ownerId=detail[0].id;
		      this.selectedId=detail[0].id;
		      const dateString = detail[0].dateofBirth;
		      this.imagePath=this.baseUrl+detail[0].pothoPath;
		      this.imageName=detail.map(item => item.pothoPath).toString();
		      const parsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(dateString);
		      let parsedDate: NgbDate | null = null;
		      
		      if (parsedDateStruct) {
			    parsedDate = new NgbDate(parsedDateStruct.year, parsedDateStruct.month, parsedDateStruct.day);
		      }
		      if(parsedDate){
			    this.selectedDate = parsedDate;
		      }
		      this.onPropertyTypeChange();
		    }
		  },
		  error: (error) => {
		    console.error('Error loading owner details:', error);
		  }
		});
}
  uploadFinished = (event:string) => { 
    this.imageName=event;
    this.imagePath=this.baseUrl+this.imageName;
    console.log(event+'=======================');
  }
  onPropertyTypeChange() {
    const identityCardTypeId = this.ownerForm.get('identityCardTypeId')?.value;
    const jild = this.ownerForm.get('jild');
    const safha = this.ownerForm.get('safha');
	const sabtNumber = this.ownerForm.get('sabtNumber');
    
    // Find the selected ID type to check its name
    const selectedType = (this.filteredIdTypes || []).find((item: any) => item.id === identityCardTypeId);
    const selectedName = (selectedType?.name || '').toLowerCase();
    const isElectricId = selectedName.includes('electric') || selectedName.includes('الکترونی');
    
    if (isElectricId) {
      // Electric ID selected - hide and disable the fields
		jild?.setValue('');
		jild?.clearAsyncValidators();
		jild?.clearValidators();
		jild?.disable();
		jild?.updateValueAndValidity();
		
		safha?.setValue('');
		safha?.clearAsyncValidators();
		safha?.clearValidators();
		safha?.disable();
		safha?.updateValueAndValidity();
		
		sabtNumber?.setValue('');
		sabtNumber?.clearAsyncValidators();
		sabtNumber?.clearValidators();
		sabtNumber?.disable();
		sabtNumber?.updateValueAndValidity();
    } else {
      // Paper ID selected - show and make mandatory
		jild?.enable();
		jild?.setValue('');
		jild?.setValidators(Validators.required);
		jild?.updateValueAndValidity();
		
		safha?.enable();
		safha?.setValue('');
		safha?.setValidators(Validators.required);
		safha?.updateValueAndValidity();
		
		sabtNumber?.enable();
		sabtNumber?.setValue('');
		sabtNumber?.setValidators(Validators.required);
		sabtNumber?.updateValueAndValidity();
    }
  }
  addOwner(): void {
	const details = this.ownerForm.value as companyowner;
	const date = new Date(
		Date.UTC(
		this.selectedDate.year,
		this.selectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
		this.selectedDate.day
		)
	  );
	  details.dateofBirth = date.toISOString();
	  details.pothoPath=this.imageName;
	  details.companyId=this.comservice.mainTableId;
	 if(details.id===null){
		details.id=0;
	}
	this.comservice.addcompanyOwner(details).subscribe(
	  result => {
	    console.log('Owner added successfully:', result);
	    if(result.id!==0) {
	      this.toastr.success("معلومات موفقانه ثبت شد");
	      this.comservice.ownerId=result.id;
	      this.selectedId=result.id;
	      this.onNextClick();
	    }
	  },
	  error => {
	    console.error('Error adding owner:', error);
	    this.toastr.error("خرابی در ثبت معلومات: " + (error.message || 'نامعلوم'));
	  }
	);
  }
  updateOwner(): void {
	const details = this.ownerForm.value as companyowner;
		const date = new Date(
			this.selectedDate.year,
			this.selectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
			this.selectedDate.day
		  );
		  details.dateofBirth = date.toISOString();
		  details.companyId=this.comservice.mainTableId;
		details.pothoPath=this.imageName;
		if(details.id===0 && this.selectedId!==0 || this.selectedId!==null){
			details.id=this.selectedId;
		}
		this.comservice.updateowner(details).subscribe(
		  result => {
		    console.log('Owner updated successfully:', result);
		    if(result.id!==0) {
		      this.selectedId=result.id;
		      this.toastr.info("معلومات موفقانه تغیر یافت ");
		      this.onNextClick();
		    }
		  },
		  error => {
		    console.error('Error updating owner:', error);
		    this.toastr.error("خرابی در تغیر معلومات: " + (error.message || 'نامعلوم'));
		  }
		);
  }
  resetForms():void{
	if (this.childComponent) {
		// Child component is available, reset it
		this.childComponent.reset();
	    this.imagePath='assets/img/avatar.png';
	  }
	this.ownerForm.reset();
	this.comservice.ownerId=0;
	this.selectedId=0;
  }
  get firstName() { return this.ownerForm.get('firstName'); }
  get fatherName() { return this.ownerForm.get('fatherName'); }
  get grandFatherName() { return this.ownerForm.get('grandFatherName'); }
  get educationLevelId() { return this.ownerForm.get('educationLevelId'); }
  get dateofBirth() { return this.ownerForm.get('dateofBirth'); }
  get identityCardTypeId() { return this.ownerForm.get('identityCardTypeId'); }
  get indentityCardNumber() { return this.ownerForm.get('indentityCardNumber'); }
  get jild() { return this.ownerForm.get('jild'); }
  get safha() { return this.ownerForm.get('safha'); }
  get companyId() { return this.ownerForm.get('companyId'); }
  get sabtNumber() { return this.ownerForm.get('sabtNumber'); }
  get pothoPath() { return this.ownerForm.get('pothoPath'); }
  
  isElectricIdSelected(): boolean {
    const identityCardTypeId = this.ownerForm.get('identityCardTypeId')?.value;
    const selectedType = (this.filteredIdTypes || []).find((item: any) => item.id === identityCardTypeId);
    const selectedName = selectedType?.name || '';
    // Check if the selected type is Electric ID (الکترونیکی)
    return selectedName === 'الکترونیکی';
  }
}
