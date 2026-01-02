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
import { LocalizationService } from 'src/app/shared/localization.service';
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
	selector: 'app-guarantee',
	templateUrl: './guarantee.component.html',
	styleUrls: ['./guarantee.component.scss'],
	providers: [
		{ provide: NgbCalendar, useClass: NgbCalendarPersian },
		{ provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
	],
})
export class GuaranteeComponent {
	maxDate = { year: 1410, month: 12, day: 31 }
	minDate = { year: 1320, month: 12, day: 31 }

	gauranteeForm: FormGroup = new FormGroup({});
	selectedId: number = 0;
	selectedDate!: NgbDate;
	senderselectedDate!: NgbDate;
	answerselectedDate!: NgbDate;
	dateofGuaranteeSelectedDate!: NgbDate;
	GuaranteeTypes: any;
	localizedGuaranteeTypes: any;
	guaranteeDateSelectedDate!: NgbDate;
	imageName: string = ''
	guaranteeDetails!: Guarantee[];
	@Input() id: number = 0;
	@ViewChild('childComponent') childComponent!: FileuploadComponent;

	ngAfterViewInit(): void {
		if (this.childComponent) {
			this.childComponent.reset();
		}
	}

	constructor(
		private fb: FormBuilder,
		private toastr: ToastrService,
		private comservice: CompnaydetailService,
		private selerService: SellerService,
		private ngbDateParserFormatter: NgbDateParserFormatter,
		private propertyDetailsService: PropertyService,
		private localizationService: LocalizationService,
		private calendarConversionService: CalendarConversionService,
		private calendarService: CalendarService
	) {
		this.gauranteeForm = this.fb.group({
			id: [0],
			guaranteeTypeId: ['', Validators.required],
			propertyDocumentNumber: ['', Validators.required],
			senderMaktobNumber: ['', Validators.required],
			propertyDocumentDate: ['', Validators.required],
			senderMaktobDate: ['', Validators.required],
			answerdMaktobNumber: ['', Validators.required],
			answerdMaktobDate: ['', Validators.required],
			dateofGuarantee: ['', Validators.required],
			guaranteeDocNumber: ['', Validators.required],
			guaranteeDate: ['', Validators.required],
			companyId: [''],
			docPath: [''],
		});
	}

	ngOnInit() {
		this.comservice.getGuaranteeType().subscribe(res => {
			this.GuaranteeTypes = res;
			this.localizedGuaranteeTypes = this.mapGuaranteeTypesToLocalized(res as any[]);
		});

		if (this.id > 0) {
			this.comservice.getGuaranteeById(this.id)
				.subscribe(detail => {
					if (!detail || detail.length === 0) {
						// No guarantee data exists yet
						this.guaranteeDetails = [];
						return;
					}
					this.guaranteeDetails = detail;
					this.gauranteeForm.setValue({
						id: detail[0].id,
						guaranteeTypeId: detail[0].guaranteeTypeId,
						propertyDocumentNumber: detail[0].propertyDocumentNumber,
						senderMaktobNumber: detail[0].senderMaktobNumber,
						propertyDocumentDate: detail[0].propertyDocumentDate,
						senderMaktobDate: detail[0].senderMaktobDate,
						answerdMaktobNumber: detail[0].answerdMaktobNumber,
						answerdMaktobDate: detail[0].answerdMaktobDate,
						dateofGuarantee: detail[0].dateofGuarantee,
						guaranteeDocNumber: detail[0].guaranteeDocNumber,
						guaranteeDate: detail[0].guaranteeDate,
						companyId: detail[0].companyId,
						docPath: detail[0].docPath,
					});
					this.selectedId = detail[0].id;
					// Set imageName from existing docPath
					this.imageName = detail[0].docPath || '';
					const dateString = detail[0].propertyDocumentDate;
					const senderMaktobDateString = detail[0].senderMaktobDate;
					const answerMaktobDateString = detail[0].answerdMaktobDate;
					const dateofGuaranteeString = detail[0].dateofGuarantee;
					const guaranteeDateString = detail[0].guaranteeDate;
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
						answerMaktobDateparsedDate = new NgbDate(answerMaktobDateparsedDateStruct.year, answerMaktobDateparsedDateStruct.month, answerMaktobDateparsedDateStruct.day);
						dateofGuaranteeparsedDate = new NgbDate(dateofGuaranteeparsedDateStruct.year, dateofGuaranteeparsedDateStruct.month, dateofGuaranteeparsedDateStruct.day);
						guaranteeDateparsedDate = new NgbDate(guaranteeDateparsedDateStruct.year, guaranteeDateparsedDateStruct.month, guaranteeDateparsedDateStruct.day);
					}
					if (parsedDate && senderMaktobDateparsedDate && answerMaktobDateparsedDate && dateofGuaranteeparsedDate && guaranteeDateparsedDate) {
						this.selectedDate = parsedDate;
						this.senderselectedDate = senderMaktobDateparsedDate;
						this.answerselectedDate = answerMaktobDateparsedDate;
						this.dateofGuaranteeSelectedDate = dateofGuaranteeparsedDate;
						this.guaranteeDateSelectedDate = guaranteeDateparsedDate;
					}
				});
		}
	}

	private formatDateForBackend(dateValue: any): string {
		const currentCalendar = this.calendarService.getSelectedCalendar();

		if (dateValue instanceof Date) {
			const calendarDate = this.calendarConversionService.fromGregorian(dateValue, currentCalendar);
			const year = calendarDate.year;
			const month = String(calendarDate.month).padStart(2, '0');
			const day = String(calendarDate.day).padStart(2, '0');
			return `${year}-${month}-${day}`;
		} else if (typeof dateValue === 'object' && dateValue.year) {
			const year = dateValue.year;
			const month = String(dateValue.month).padStart(2, '0');
			const day = String(dateValue.day).padStart(2, '0');
			return `${year}-${month}-${day}`;
		} else if (typeof dateValue === 'string') {
			return dateValue.replace(/\//g, '-');
		}
		return '';
	}

	updateData(): void {
		const details = this.gauranteeForm.value as Guarantee;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		details.propertyDocumentDate = this.formatDateForBackend(this.gauranteeForm.get('propertyDocumentDate')?.value);
		details.senderMaktobDate = this.formatDateForBackend(this.gauranteeForm.get('senderMaktobDate')?.value);
		details.answerdMaktobDate = this.formatDateForBackend(this.gauranteeForm.get('answerdMaktobDate')?.value);
		details.dateofGuarantee = this.formatDateForBackend(this.gauranteeForm.get('dateofGuarantee')?.value);
		details.guaranteeDate = this.formatDateForBackend(this.gauranteeForm.get('guaranteeDate')?.value);

		// Convert string form values to numbers for backend (int? fields)
		details.propertyDocumentNumber = Number(details.propertyDocumentNumber) || 0;
		details.answerdMaktobNumber = Number(details.answerdMaktobNumber) || 0;
		details.guaranteeDocNumber = Number(details.guaranteeDocNumber) || 0;
		details.guaranteeTypeId = Number(details.guaranteeTypeId) || 0;

		details.calendarType = currentCalendar;
		details.docPath = this.imageName;
		details.companyId = this.comservice.mainTableId;
		if (details.id === 0 && this.selectedId !== 0 || this.selectedId !== null) {
			details.id = this.selectedId;
		}
		this.comservice.updateGuarantee(details).subscribe(result => {
			if (result.id !== 0)
				this.selectedId = result.id;
			this.toastr.info("معلومات موفقانه تغیر یافت ");
		});
	}

	addData(): void {
		const details = this.gauranteeForm.value as Guarantee;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		details.propertyDocumentDate = this.formatDateForBackend(this.gauranteeForm.get('propertyDocumentDate')?.value);
		details.senderMaktobDate = this.formatDateForBackend(this.gauranteeForm.get('senderMaktobDate')?.value);
		details.answerdMaktobDate = this.formatDateForBackend(this.gauranteeForm.get('answerdMaktobDate')?.value);
		details.dateofGuarantee = this.formatDateForBackend(this.gauranteeForm.get('dateofGuarantee')?.value);
		details.guaranteeDate = this.formatDateForBackend(this.gauranteeForm.get('guaranteeDate')?.value);

		// Convert string form values to numbers for backend (int? fields)
		details.propertyDocumentNumber = Number(details.propertyDocumentNumber) || 0;
		details.answerdMaktobNumber = Number(details.answerdMaktobNumber) || 0;
		details.guaranteeDocNumber = Number(details.guaranteeDocNumber) || 0;
		details.guaranteeTypeId = Number(details.guaranteeTypeId) || 0;

		details.calendarType = currentCalendar;
		details.docPath = this.imageName;
		details.companyId = this.comservice.mainTableId;
		if (details.id === null) {
			details.id = 0;
		}
		
		console.log('Sending guarantee data:', JSON.stringify(details));
		
		this.comservice.addGuarantee(details).subscribe({
			next: (result) => {
				if (result.id !== 0)
					this.toastr.success("معلومات موفقانه ثبت شد");
				this.selectedId = result.id;
			},
			error: (err) => {
				console.error('Guarantee POST error:', err);
				if (err.error) {
					console.error('Error body:', err.error);
					this.toastr.error(typeof err.error === 'string' ? err.error : JSON.stringify(err.error));
				}
			}
		});
	}

	resetForms(): void {
		if (this.childComponent) {
			this.childComponent.reset();
		}
		this.selectedId = 0;
		this.gauranteeForm.reset();
	}

	uploadFinished = (event: string) => {
		this.imageName = event;
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

	downloadFiles() {
		const filePath = this.gauranteeForm.get('docPath')?.value;
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
				} else {
					console.log('An error occurred:', error);
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
