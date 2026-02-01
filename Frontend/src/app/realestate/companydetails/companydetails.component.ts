import { Component, EventEmitter, Injectable, Input, Output, ViewChild } from '@angular/core';
import '@angular/localize/init';
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
import { RbacService } from 'src/app/shared/rbac.service';
@Component({
  selector: 'app-companydetails',
  templateUrl: './companydetails.component.html',
  styleUrls: ['./companydetails.component.scss'],
})
export class CompanydetailsComponent {

  imageName:string=''
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
  constructor(private fb: FormBuilder,private toastr: ToastrService, private comservice:CompnaydetailService,
	private propertyDetailsService: PropertyService, private parentComponent: RealestateComponent,
	private calendarConversionService: CalendarConversionService, private calendarService: CalendarService,
	private rbacService: RbacService){
	this.companyForm = this.fb.group({
		id: [0],
		title: ['', Validators.required],
		tin:['',Validators.required],
		docPath: [''],
	  });
	  this.comservice.mainTableId=0;
	  this.comservice.ownerId=0;
	}
	
	ngOnInit() {
		if (this.id && this.id > 0) {
			this.comservice.getCompanyById(this.id)
			.subscribe({
			  next: (detail: any) => {
			    if (detail && detail.length > 0) {
			      this.companyDetails = detail;
			      
			      this.companyForm.patchValue({
				    id: detail[0].id,
				    title: detail[0].title,
				    tin: detail[0].tin,
				    docPath: detail[0].docPath
			      });
			      
			      // Set imageName from existing docPath
			      this.imageName = detail[0].docPath || '';
			      
			      this.comservice.mainTableId=detail[0].id;
			      this.selectedId=detail[0].id;
			    }
			  },
			  error: (error: any) => {
			    console.error('Error loading company details:', error);
			  }
			});
		}
	}

	addCompanyDetails(): void {
		const companyDetail = this.companyForm.value as companydetails;
		
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
 get tin() { return this.companyForm.get('tin'); }
 get docPath() { return this.companyForm.get('docPath'); }
}
