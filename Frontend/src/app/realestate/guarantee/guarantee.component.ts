import { Component, Injectable, Input, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbDate, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Guarantee } from 'src/app/models/Guarantee';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { SellerService } from 'src/app/shared/seller.service';
import '@angular/localize/init';
import {
	NgbDateStruct,
	NgbCalendar,
	NgbDatepickerI18n,
	NgbCalendarPersian,

} from '@ng-bootstrap/ng-bootstrap';
import { PropertyService } from 'src/app/shared/property.service';
import { FileuploadComponent } from '../fileupload/fileupload.component';
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
  selector: 'app-guarantee',
  templateUrl: './guarantee.component.html',
  styleUrls: ['./guarantee.component.scss'],
  providers: [
		{ provide: NgbCalendar, useClass: NgbCalendarPersian },
		{ provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
	],
})
export class GuaranteeComponent {
  maxDate ={year:1410,month: 12, day: 31}
	minDate ={year:1320,month: 12, day: 31}

  gauranteeForm: FormGroup = new FormGroup({});
  selectedId:number=0;
  selectedDate!: NgbDate;
  senderselectedDate!: NgbDate;
  answerselectedDate!: NgbDate;
  dateofGuaranteeSelectedDate!: NgbDate;
  GuaranteeTypes:any;
  guaranteeDateSelectedDate!: NgbDate;
  imageName:string=''
  guaranteeDetails!: Guarantee[];
  @Input() id: number=0;
  @ViewChild('childComponent') childComponent!: FileuploadComponent;
  ngAfterViewInit(): void {
    if (this.childComponent) {
      // Child component is ready, call its reset method
      this.childComponent.reset();
    }
  }
  constructor(private fb: FormBuilder,private toastr: ToastrService, private comservice:CompnaydetailService,private selerService:SellerService,
    private ngbDateParserFormatter: NgbDateParserFormatter,private propertyDetailsService: PropertyService){
    this.gauranteeForm = this.fb.group({
      id: [0],
      guaranteeTypeId: ['', Validators.required],
      propertyDocumentNumber:['', Validators.required],
      senderMaktobNumber:['', Validators.required],
      propertyDocumentDate:['', Validators.required],
      senderMaktobDate:['', Validators.required],
      answerdMaktobNumber:['', Validators.required],
      answerdMaktobDate:['', Validators.required],
      dateofGuarantee:['', Validators.required],
      guaranteeDocNumber:['', Validators.required],
      guaranteeDate:['', Validators.required],
      companyId:[''],
      docPath:[''],
      });
    }
  
  ngOnInit() {
    this.comservice.getGuaranteeType().subscribe(res => {
      this.GuaranteeTypes = res;
    });

    if (this.id > 0) {
      this.comservice.getGuaranteeById(this.id)
  		.subscribe(detail => {
  		  this.guaranteeDetails = detail;
  		  this.gauranteeForm.setValue({
  			id: detail[0].id,
        guaranteeTypeId:detail[0].guaranteeTypeId,
        propertyDocumentNumber:detail[0].propertyDocumentNumber,
        senderMaktobNumber:detail[0].senderMaktobNumber,
        propertyDocumentDate:detail[0].propertyDocumentDate,
        senderMaktobDate:detail[0].senderMaktobDate,
        answerdMaktobNumber:detail[0].answerdMaktobNumber,
        answerdMaktobDate:detail[0].answerdMaktobDate,
        dateofGuarantee:detail[0].dateofGuarantee,
        guaranteeDocNumber:detail[0].guaranteeDocNumber,
        guaranteeDate:detail[0].guaranteeDate,
        companyId:detail[0].companyId,
        docPath:detail[0].docPath,
  		  });
        this.selectedId=detail[0].id;
  		  const dateString = detail[0].propertyDocumentDate;
        const senderMaktobDateString=detail[0].senderMaktobDate;
        const answerMaktobDateString=detail[0].answerdMaktobDate;
        const dateofGuaranteeString=detail[0].dateofGuarantee;
        const guaranteeDateString=detail[0].guaranteeDate;
  		  const parsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(dateString);
        const senderMaktobDateparsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(senderMaktobDateString);
        const answerMaktobDateparsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(answerMaktobDateString);
        const dateofGuaranteeparsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(dateofGuaranteeString);
        const guaranteeDateparsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(guaranteeDateString);
  		  let parsedDate: NgbDate | null = null;
        let senderMaktobDateparsedDate: NgbDate | null = null;
        let answerMaktobDateparsedDate: NgbDate | null = null;
        let dateofGuaranteeparsedDate: NgbDate | null = null;
        let guaranteeDateparsedDate: NgbDate | null = null;
  		  if (parsedDateStruct && senderMaktobDateparsedDateStruct && answerMaktobDateparsedDateStruct && dateofGuaranteeparsedDateStruct && guaranteeDateparsedDateStruct) {
  			parsedDate = new NgbDate(parsedDateStruct.year, parsedDateStruct.month, parsedDateStruct.day);
        senderMaktobDateparsedDate = new NgbDate(senderMaktobDateparsedDateStruct.year, senderMaktobDateparsedDateStruct.month, senderMaktobDateparsedDateStruct.day);
        answerMaktobDateparsedDate=new NgbDate(answerMaktobDateparsedDateStruct.year, answerMaktobDateparsedDateStruct.month, answerMaktobDateparsedDateStruct.day);
        dateofGuaranteeparsedDate=new NgbDate(dateofGuaranteeparsedDateStruct.year, dateofGuaranteeparsedDateStruct.month, dateofGuaranteeparsedDateStruct.day);
        guaranteeDateparsedDate=new NgbDate(guaranteeDateparsedDateStruct.year, guaranteeDateparsedDateStruct.month, guaranteeDateparsedDateStruct.day);
        }
  		  if(parsedDate && senderMaktobDateparsedDate && answerMaktobDateparsedDate && dateofGuaranteeparsedDate && guaranteeDateparsedDate){
  			this.selectedDate = parsedDate;
        this.senderselectedDate=senderMaktobDateparsedDate;
        this.answerselectedDate=answerMaktobDateparsedDate;
        this.dateofGuaranteeSelectedDate=dateofGuaranteeparsedDate;
        this.guaranteeDateSelectedDate=guaranteeDateparsedDate;
  		  }
      }); 
    }
  }
  updateData():void{
    const details = this.gauranteeForm.value as Guarantee;
    const PropertyDocumentDate = new Date(
      Date.UTC(
      this.selectedDate.year,
      this.selectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
      this.selectedDate.day
      )
      );

      const SenderMaktobDate = new Date(
        Date.UTC(
        this.senderselectedDate.year,
        this.senderselectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
        this.senderselectedDate.day
        )
        );

        const DateofGuarantee = new Date(
          Date.UTC(
          this.dateofGuaranteeSelectedDate.year,
          this.dateofGuaranteeSelectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
          this.dateofGuaranteeSelectedDate.day
          )
          );

          const AnswerdMaktobDate = new Date(
            Date.UTC(
            this.answerselectedDate.year,
            this.answerselectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
            this.answerselectedDate.day
            )
            );

            const GuaranteeDate = new Date(
              Date.UTC(
              this.guaranteeDateSelectedDate.year,
              this.guaranteeDateSelectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
              this.guaranteeDateSelectedDate.day
              )
              );
          
        
      details.senderMaktobDate=SenderMaktobDate.toISOString();
      details.propertyDocumentDate=PropertyDocumentDate.toISOString();
      details.dateofGuarantee=DateofGuarantee.toISOString();
      details.answerdMaktobDate=AnswerdMaktobDate.toISOString();
      details.guaranteeDate=GuaranteeDate.toISOString();
      details.docPath=this.imageName;
      details.companyId=this.comservice.mainTableId;
		if(details.id===0 && this.selectedId!==0 || this.selectedId!==null){
			details.id=this.selectedId;
		}
		this.comservice.updateGuarantee(details).subscribe(result => {
		  if(result.id!==0)
		   this.selectedId=result.id;
		   this.toastr.info("معلومات موفقانه تغیر یافت ");
		
		});
  }
  addData():void{
    const details = this.gauranteeForm.value as Guarantee;
    const PropertyDocumentDate = new Date(
      Date.UTC(
      this.selectedDate.year,
      this.selectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
      this.selectedDate.day
      )
      );

      const SenderMaktobDate = new Date(
        Date.UTC(
        this.senderselectedDate.year,
        this.senderselectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
        this.senderselectedDate.day
        )
        );

        const DateofGuarantee = new Date(
          Date.UTC(
          this.dateofGuaranteeSelectedDate.year,
          this.dateofGuaranteeSelectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
          this.dateofGuaranteeSelectedDate.day
          )
          );

          const AnswerdMaktobDate = new Date(
            Date.UTC(
            this.answerselectedDate.year,
            this.answerselectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
            this.answerselectedDate.day
            )
            );

            const GuaranteeDate = new Date(
              Date.UTC(
              this.guaranteeDateSelectedDate.year,
              this.guaranteeDateSelectedDate.month - 1, // Adjust month value (JavaScript months are 0-based)
              this.guaranteeDateSelectedDate.day
              )
              );
          
        
      details.senderMaktobDate=SenderMaktobDate.toISOString();
      details.propertyDocumentDate=PropertyDocumentDate.toISOString();
      details.dateofGuarantee=DateofGuarantee.toISOString();
      details.answerdMaktobDate=AnswerdMaktobDate.toISOString();
      details.guaranteeDate=GuaranteeDate.toISOString();
      details.docPath=this.imageName;
      details.companyId=this.comservice.mainTableId;
     if(details.id===null){
      details.id=0;
    }
    this.comservice.addGuarantee(details).subscribe(result => {
      if(result.id!==0)
       this.toastr.success("معلومات موفقانه ثبت شد");
       this.selectedId=result.id;
    });
  }
  resetForms():void{
    if (this.childComponent) {
      // Child component is available, reset it
      this.childComponent.reset();
      }
   this.selectedId=0;
   this.gauranteeForm.reset();
  }
  uploadFinished = (event:string) => { 
    this.imageName="Resources\\Images\\"+event;
  }
  downloadFiles() {
    const filePath = this.gauranteeForm.get('docPath')?.value;
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
  get propertyDocumentNumber() { return this.gauranteeForm.get('propertyDocumentNumber'); }
  get guaranteeTypeId() { return this.gauranteeForm.get('guaranteeTypeId'); }
  get senderMaktobNumber() { return this.gauranteeForm.get('senderMaktobNumber'); }
  get propertyDocumentDate() { return this.gauranteeForm.get('propertyDocumentDate'); }
  get senderMaktobDate() { return this.gauranteeForm.get('senderMaktobDate'); }
  get answerdMaktobNumber() { return this.gauranteeForm.get('answerdMaktobNumber'); }
  get answerdMaktobDate() { return this.gauranteeForm.get('answerdMaktobDate'); }
  get guaranteeDocNumber() { return this.gauranteeForm.get('guaranteeDocNumber'); }
  get dateofGuarantee() { return this.gauranteeForm.get('dateofGuarantee'); }
  get guaranteeDate() { return this.gauranteeForm.get('guaranteeDate'); }
  get docPath() { return this.gauranteeForm.get('docPath'); }
}
