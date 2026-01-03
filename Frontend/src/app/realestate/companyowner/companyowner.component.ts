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
import { ProfileImageCropperComponent } from 'src/app/shared/profile-image-cropper/profile-image-cropper.component';
import { environment } from 'src/environments/environment';
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
	selector: 'app-companyowner',
	templateUrl: './companyowner.component.html',
	styleUrls: ['./companyowner.component.scss'],
	providers: [
		{ provide: NgbCalendar, useClass: NgbCalendarPersian },
		{ provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
	],
})
export class CompanyownerComponent {
	maxDate = { year: 1410, month: 12, day: 31 }
	minDate = { year: 1320, month: 12, day: 31 }

	baseUrl: string = environment.apiURL + '/';
	imagePath: string = 'assets/img/avatar.png';
	imageName: string = '';
	selectedId: number = 0;
	selectedDate!: NgbDate;
	IdTypes: any;
	EducationLevel: any;
	ownerForm: FormGroup = new FormGroup({});
	ownerDetails!: companyowner[];
	filteredIdTypes: any;

	@Input() id: number = 0;
	@Output() next = new EventEmitter<void>();
	onNextClick() {
		this.next.emit();
	}
	@ViewChild('childComponent') childComponent!: ProfileImageCropperComponent;
	private pendingImagePath: string = '';
	
	ngAfterViewInit(): void {
		// If we have a pending image path from ngOnInit, set it now
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
		private calendarConversionService: CalendarConversionService,
		private calendarService: CalendarService
	) {
		this.ownerForm = this.fb.group({
			id: [0],
			firstName: ['', Validators.required],
			fatherName: ['', Validators.required],
			grandFatherName: ['', Validators.required],
			educationLevelId: ['', Validators.required],
			dateofBirth: ['', Validators.required],
			identityCardTypeId: ['', Validators.required],
			indentityCardNumber: ['', Validators.required],
			jild: [''],
			safha: [''],
			companyId: [''],
			sabtNumber: [''],
			pothoPath: ['']
		});
	}

	ngOnInit() {
		this.comservice.getIdentityTypes().subscribe(res => {
			this.IdTypes = res;
			this.filteredIdTypes = (res as any[]).filter((item: any) => {
				const name = item.name || '';
				return name === 'الکترونیکی' || name === 'کاغذی';
			}).map((item: any) => ({
				...item,
				displayName: item.name
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
							firstName: detail[0].firstName,
							fatherName: detail[0].fatherName,
							grandFatherName: detail[0].grandFatherName,
							educationLevelId: detail[0].educationLevelId,
							dateofBirth: detail[0].dateofBirth,
							identityCardTypeId: detail[0].identityCardTypeId,
							indentityCardNumber: detail[0].indentityCardNumber != null ? String(detail[0].indentityCardNumber) : '',
							jild: detail[0].jild,
							safha: detail[0].safha,
							companyId: detail[0].companyId,
							sabtNumber: detail[0].sabtNumber,
							pothoPath: detail[0].pothoPath,
						});
						this.comservice.ownerId = detail[0].id;
						this.selectedId = detail[0].id;
						const dateString = detail[0].dateofBirth;
						// Set existing image for profile cropper
						if (detail[0].pothoPath) {
							this.imagePath = this.baseUrl + detail[0].pothoPath;
							this.imageName = detail[0].pothoPath;
							if (this.childComponent) {
								this.childComponent.setExistingImage(this.imagePath);
							} else {
								// Store for later if child component isn't ready yet
								this.pendingImagePath = this.imagePath;
							}
						}
						const parsedDateStruct: NgbDateStruct | null = this.ngbDateParserFormatter.parse(dateString);
						let parsedDate: NgbDate | null = null;

						if (parsedDateStruct) {
							parsedDate = new NgbDate(parsedDateStruct.year, parsedDateStruct.month, parsedDateStruct.day);
						}
						if (parsedDate) {
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
		this.ownerForm.patchValue({ pothoPath: this.imageName });
		this.imagePath = this.imageName ? (this.baseUrl + this.imageName) : 'assets/img/avatar.png';
	}

	onPropertyTypeChange() {
		const identityCardTypeId = this.ownerForm.get('identityCardTypeId')?.value;
		const jild = this.ownerForm.get('jild');
		const safha = this.ownerForm.get('safha');
		const sabtNumber = this.ownerForm.get('sabtNumber');

		const selectedType = (this.filteredIdTypes || []).find((item: any) => item.id === identityCardTypeId);
		const selectedName = (selectedType?.name || '').toLowerCase();
		const isElectricId = selectedName.includes('electric') || selectedName.includes('الکترونی');

		if (isElectricId) {
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

	addOwner(): void {
		const details = this.ownerForm.value as companyowner;
		const dateValue = this.ownerForm.get('dateofBirth')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		if (dateValue) {
			details.dateofBirth = this.formatDateForBackend(dateValue);
		} else {
			this.toastr.error("لطفا تاریخ تولد را انتخاب کنید");
			return;
		}

		// Ensure indentityCardNumber is sent as string
		if (details.indentityCardNumber != null) {
			details.indentityCardNumber = String(details.indentityCardNumber);
		}

		details.calendarType = currentCalendar;
		details.pothoPath = this.imageName;
		details.companyId = this.comservice.mainTableId;
		if (details.id === null) {
			details.id = 0;
		}
		this.comservice.addcompanyOwner(details).subscribe(
			result => {
				if (result.id !== 0) {
					this.toastr.success("معلومات موفقانه ثبت شد");
					this.comservice.ownerId = result.id;
					this.selectedId = result.id;
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
		const dateValue = this.ownerForm.get('dateofBirth')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		if (dateValue) {
			details.dateofBirth = this.formatDateForBackend(dateValue);
		} else {
			this.toastr.error("لطفا تاریخ تولد را انتخاب کنید");
			return;
		}

		// Ensure indentityCardNumber is sent as string
		if (details.indentityCardNumber != null) {
			details.indentityCardNumber = String(details.indentityCardNumber);
		}

		details.calendarType = currentCalendar;
		details.companyId = this.comservice.mainTableId;
		details.pothoPath = this.imageName;
		if (details.id === 0 && this.selectedId !== 0 || this.selectedId !== null) {
			details.id = this.selectedId;
		}
		this.comservice.updateowner(details).subscribe(
			result => {
				if (result.id !== 0) {
					this.selectedId = result.id;
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

	resetForms(): void {
		if (this.childComponent) {
			this.childComponent.reset();
		}
		this.imagePath = 'assets/img/avatar.png';
		this.imageName = '';
		this.ownerForm.reset();
		this.comservice.ownerId = 0;
		this.selectedId = 0;
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
		return selectedName === 'الکترونیکی';
	}
}
