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
import { companyowner, companyOwnerAddressHistory } from 'src/app/models/companyowner';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { SellerService } from 'src/app/shared/seller.service';
import { ProfileImageCropperComponent } from 'src/app/shared/profile-image-cropper/profile-image-cropper.component';
import { environment } from 'src/environments/environment';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarService } from 'src/app/shared/calendar.service';


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

	// Address related properties
	province: any;
	ownerDistrict: any;
	permanentDistrict: any;
	temporaryDistrict: any;

	// Address change mode
	isAddressChangeMode: boolean = false;
	addressHistory: companyOwnerAddressHistory[] = [];
	showAddressHistory: boolean = false;

	// Store current address for display during change mode
	currentAddressDisplay: {
		ownerProvinceName?: string;
		ownerDistrictName?: string;
		ownerVillage?: string;
		permanentProvinceName?: string;
		permanentDistrictName?: string;
		permanentVillage?: string;
		temporaryProvinceName?: string;
		temporaryDistrictName?: string;
		temporaryVillage?: string;
	} = {};

	// New address districts (for address change mode)
	newOwnerDistrict: any;
	newPermanentDistrict: any;
	newTemporaryDistrict: any;

	@Input() id: number = 0;
	@Output() next = new EventEmitter<void>();
	onNextClick() {
		this.next.emit();
	}
	@ViewChild('childComponent') childComponent!: ProfileImageCropperComponent;
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
			pothoPath: [''],
			// Contact Information
			phoneNumber: [''],
			whatsAppNumber: [''],
			// Owner's Own Address Fields (آدرس اصلی مالک)
			ownerProvinceId: ['', Validators.required],
			ownerDistrictId: ['', Validators.required],
			ownerVillage: ['', Validators.required],
			// Permanent Address Fields (آدرس دایمی) - Current Residence
			permanentProvinceId: ['', Validators.required],
			permanentDistrictId: ['', Validators.required],
			permanentVillage: ['', Validators.required],
			// Temporary Address Fields (آدرس موقت)
			temporaryProvinceId: [''],
			temporaryDistrictId: [''],
			temporaryVillage: [''],
			// New Address Fields (for address change mode)
			newOwnerProvinceId: [''],
			newOwnerDistrictId: [''],
			newOwnerVillage: [''],
			newPermanentProvinceId: [''],
			newPermanentDistrictId: [''],
			newPermanentVillage: [''],
			newTemporaryProvinceId: [''],
			newTemporaryDistrictId: [''],
			newTemporaryVillage: ['']
		});

		// Add cross-field validation for phone numbers
		this.ownerForm.get('whatsAppNumber')?.valueChanges.subscribe(() => {
			this.validatePhoneNumbers();
		});
		this.ownerForm.get('phoneNumber')?.valueChanges.subscribe(() => {
			this.validatePhoneNumbers();
		});
	}

	// Validate that phone and WhatsApp numbers are not identical
	validatePhoneNumbers(): void {
		const phoneNumber = this.ownerForm.get('phoneNumber')?.value?.trim();
		const whatsAppNumber = this.ownerForm.get('whatsAppNumber')?.value?.trim();
		
		if (phoneNumber && whatsAppNumber && phoneNumber === whatsAppNumber) {
			this.ownerForm.get('whatsAppNumber')?.setErrors({ duplicatePhone: true });
		} else {
			const errors = this.ownerForm.get('whatsAppNumber')?.errors;
			if (errors) {
				delete errors['duplicatePhone'];
				if (Object.keys(errors).length === 0) {
					this.ownerForm.get('whatsAppNumber')?.setErrors(null);
				}
			}
		}
	}

	// Check if phone numbers are duplicate (for template use)
	get isPhoneDuplicate(): boolean {
		const phoneNumber = this.ownerForm.get('phoneNumber')?.value?.trim();
		const whatsAppNumber = this.ownerForm.get('whatsAppNumber')?.value?.trim();
		return !!(phoneNumber && whatsAppNumber && phoneNumber === whatsAppNumber);
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

		this.selerService.getprovince().subscribe(res => {
			this.province = res;
		});

		this.comservice.getOwnerById(this.id)
			.subscribe({
				next: (detail) => {
					if (detail && detail.length > 0) {
						this.ownerDetails = detail;
						this.ownerForm.patchValue({
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
							phoneNumber: detail[0].phoneNumber || '',
							whatsAppNumber: detail[0].whatsAppNumber || '',
							ownerProvinceId: detail[0].ownerProvinceId || '',
							ownerDistrictId: detail[0].ownerDistrictId || '',
							ownerVillage: detail[0].ownerVillage || '',
							permanentProvinceId: detail[0].permanentProvinceId || '',
							permanentDistrictId: detail[0].permanentDistrictId || '',
							permanentVillage: detail[0].permanentVillage || '',
							temporaryProvinceId: detail[0].temporaryProvinceId || '',
							temporaryDistrictId: detail[0].temporaryDistrictId || '',
							temporaryVillage: detail[0].temporaryVillage || ''
						});

						// Store current address for display
						this.currentAddressDisplay = {
							ownerProvinceName: detail[0].ownerProvinceName,
							ownerDistrictName: detail[0].ownerDistrictName,
							ownerVillage: detail[0].ownerVillage,
							permanentProvinceName: detail[0].permanentProvinceName,
							permanentDistrictName: detail[0].permanentDistrictName,
							permanentVillage: detail[0].permanentVillage,
							temporaryProvinceName: detail[0].temporaryProvinceName,
							temporaryDistrictName: detail[0].temporaryDistrictName,
							temporaryVillage: detail[0].temporaryVillage
						};

						this.comservice.ownerId = detail[0].id;
						this.selectedId = detail[0].id;
						const dateString = detail[0].dateofBirth;

						if (detail[0].pothoPath) {
							this.imagePath = this.baseUrl + detail[0].pothoPath;
							this.imageName = detail[0].pothoPath;
							if (this.childComponent) {
								this.childComponent.setExistingImage(this.imagePath);
							} else {
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

						if (detail[0].ownerProvinceId) {
							this.selerService.getdistrict(detail[0].ownerProvinceId).subscribe(res => {
								this.ownerDistrict = res;
							});
						}
						if (detail[0].permanentProvinceId) {
							this.selerService.getdistrict(detail[0].permanentProvinceId).subscribe(res => {
								this.permanentDistrict = res;
							});
						}
						if (detail[0].temporaryProvinceId) {
							this.selerService.getdistrict(detail[0].temporaryProvinceId).subscribe(res => {
								this.temporaryDistrict = res;
							});
						}

						// Load address history
						this.loadAddressHistory();
					}
				},
				error: (error) => {
					console.error('Error loading owner details:', error);
				}
			});
	}

	loadAddressHistory(): void {
		if (this.id > 0) {
			this.comservice.getOwnerAddressHistory(this.id).subscribe({
				next: (history) => {
					this.addressHistory = history;
				},
				error: (error) => {
					console.error('Error loading address history:', error);
				}
			});
		}
	}

	toggleAddressHistory(): void {
		this.showAddressHistory = !this.showAddressHistory;
	}

	// Enable address change mode
	enableAddressChangeMode(): void {
		this.isAddressChangeMode = true;
		// Clear new address fields
		this.ownerForm.patchValue({
			newOwnerProvinceId: '',
			newOwnerDistrictId: '',
			newOwnerVillage: '',
			newPermanentProvinceId: '',
			newPermanentDistrictId: '',
			newPermanentVillage: '',
			newTemporaryProvinceId: '',
			newTemporaryDistrictId: '',
			newTemporaryVillage: ''
		});
		this.newOwnerDistrict = [];
		this.newPermanentDistrict = [];
		this.newTemporaryDistrict = [];
	}

	// Cancel address change mode
	cancelAddressChange(): void {
		this.isAddressChangeMode = false;
		this.ownerForm.patchValue({
			newOwnerProvinceId: '',
			newOwnerDistrictId: '',
			newOwnerVillage: '',
			newPermanentProvinceId: '',
			newPermanentDistrictId: '',
			newPermanentVillage: '',
			newTemporaryProvinceId: '',
			newTemporaryDistrictId: '',
			newTemporaryVillage: ''
		});
	}

	// Filter districts for new owner address
	filterNewOwnerDistricts(event: any): void {
		if (event && event.id) {
			this.selerService.getdistrict(event.id).subscribe(res => {
				this.newOwnerDistrict = res;
				this.ownerForm.patchValue({ newOwnerDistrictId: '' });
			});
		} else {
			this.newOwnerDistrict = [];
			this.ownerForm.patchValue({ newOwnerDistrictId: '' });
		}
	}

	// Filter districts for new permanent address
	filterNewPermanentDistricts(event: any): void {
		if (event && event.id) {
			this.selerService.getdistrict(event.id).subscribe(res => {
				this.newPermanentDistrict = res;
				this.ownerForm.patchValue({ newPermanentDistrictId: '' });
			});
		} else {
			this.newPermanentDistrict = [];
			this.ownerForm.patchValue({ newPermanentDistrictId: '' });
		}
	}

	// Filter districts for new temporary address
	filterNewTemporaryDistricts(event: any): void {
		if (event && event.id) {
			this.selerService.getdistrict(event.id).subscribe(res => {
				this.newTemporaryDistrict = res;
				this.ownerForm.patchValue({ newTemporaryDistrictId: '' });
			});
		} else {
			this.newTemporaryDistrict = [];
			this.ownerForm.patchValue({ newTemporaryDistrictId: '' });
		}
	}

	filterPermanentDistricts(event: any) {
		if (event && event.id) {
			this.selerService.getdistrict(event.id).subscribe(res => {
				this.permanentDistrict = res;
				this.ownerForm.patchValue({ permanentDistrictId: '' });
			});
		} else {
			this.permanentDistrict = [];
			this.ownerForm.patchValue({ permanentDistrictId: '' });
		}
	}

	filterOwnerDistricts(event: any) {
		if (event && event.id) {
			this.selerService.getdistrict(event.id).subscribe(res => {
				this.ownerDistrict = res;
				this.ownerForm.patchValue({ ownerDistrictId: '' });
			});
		} else {
			this.ownerDistrict = [];
			this.ownerForm.patchValue({ ownerDistrictId: '' });
		}
	}

	filterTemporaryDistricts(event: any) {
		if (event && event.id) {
			this.selerService.getdistrict(event.id).subscribe(res => {
				this.temporaryDistrict = res;
				this.ownerForm.patchValue({ temporaryDistrictId: '' });
			});
		} else {
			this.temporaryDistrict = [];
			this.ownerForm.patchValue({ temporaryDistrictId: '' });
		}
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
		// Check for duplicate phone numbers before submission
		if (this.isPhoneDuplicate) {
			this.toastr.error("شماره تماس و شماره واتساپ نباید یکسان باشد");
			return;
		}

		const details = this.ownerForm.value as companyowner;
		const dateValue = this.ownerForm.get('dateofBirth')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		if (dateValue) {
			details.dateofBirth = this.formatDateForBackend(dateValue);
		} else {
			this.toastr.error("لطفا تاریخ تولد را انتخاب کنید");
			return;
		}

		if (details.indentityCardNumber != null) {
			details.indentityCardNumber = String(details.indentityCardNumber);
		}

		details.calendarType = currentCalendar;
		details.pothoPath = this.imageName;
		details.companyId = this.comservice.mainTableId;
		if (details.id === null) {
			details.id = 0;
		}

		// Contact Information
		details.phoneNumber = this.ownerForm.get('phoneNumber')?.value?.trim() || null;
		details.whatsAppNumber = this.ownerForm.get('whatsAppNumber')?.value?.trim() || null;

		// Owner's Own Address Fields (آدرس اصلی مالک)
		details.ownerProvinceId = this.ownerForm.get('ownerProvinceId')?.value || null;
		details.ownerDistrictId = this.ownerForm.get('ownerDistrictId')?.value || null;
		details.ownerVillage = this.ownerForm.get('ownerVillage')?.value || null;
		// Permanent Address Fields (آدرس دایمی) - Current Residence
		details.permanentProvinceId = this.ownerForm.get('permanentProvinceId')?.value || null;
		details.permanentDistrictId = this.ownerForm.get('permanentDistrictId')?.value || null;
		details.permanentVillage = this.ownerForm.get('permanentVillage')?.value || null;
		// Temporary Address Fields (آدرس موقت)
		details.temporaryProvinceId = this.ownerForm.get('temporaryProvinceId')?.value || null;
		details.temporaryDistrictId = this.ownerForm.get('temporaryDistrictId')?.value || null;
		details.temporaryVillage = this.ownerForm.get('temporaryVillage')?.value || null;

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
				this.toastr.error("خرابی در ثبت معلومات: " + (error.error || error.message || 'نامعلوم'));
			}
		);
	}

	updateOwner(): void {
		// Check for duplicate phone numbers before submission
		if (this.isPhoneDuplicate) {
			this.toastr.error("شماره تماس و شماره واتساپ نباید یکسان باشد");
			return;
		}

		const details = this.ownerForm.value as companyowner;
		const dateValue = this.ownerForm.get('dateofBirth')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		if (dateValue) {
			details.dateofBirth = this.formatDateForBackend(dateValue);
		} else {
			this.toastr.error("لطفا تاریخ تولد را انتخاب کنید");
			return;
		}

		if (details.indentityCardNumber != null) {
			details.indentityCardNumber = String(details.indentityCardNumber);
		}

		details.calendarType = currentCalendar;
		details.companyId = this.comservice.mainTableId;
		details.pothoPath = this.imageName;
		if (details.id === 0 && this.selectedId !== 0 || this.selectedId !== null) {
			details.id = this.selectedId;
		}

		// Contact Information
		details.phoneNumber = this.ownerForm.get('phoneNumber')?.value?.trim() || null;
		details.whatsAppNumber = this.ownerForm.get('whatsAppNumber')?.value?.trim() || null;

		// Handle address change mode
		if (this.isAddressChangeMode) {
			// Use new address values and set flag
			details.ownerProvinceId = this.ownerForm.get('newOwnerProvinceId')?.value || null;
			details.ownerDistrictId = this.ownerForm.get('newOwnerDistrictId')?.value || null;
			details.ownerVillage = this.ownerForm.get('newOwnerVillage')?.value || null;
			details.permanentProvinceId = this.ownerForm.get('newPermanentProvinceId')?.value || null;
			details.permanentDistrictId = this.ownerForm.get('newPermanentDistrictId')?.value || null;
			details.permanentVillage = this.ownerForm.get('newPermanentVillage')?.value || null;
			details.temporaryProvinceId = this.ownerForm.get('newTemporaryProvinceId')?.value || null;
			details.temporaryDistrictId = this.ownerForm.get('newTemporaryDistrictId')?.value || null;
			details.temporaryVillage = this.ownerForm.get('newTemporaryVillage')?.value || null;
			details.isAddressChange = true;
		} else {
			details.ownerProvinceId = this.ownerForm.get('ownerProvinceId')?.value || null;
			details.ownerDistrictId = this.ownerForm.get('ownerDistrictId')?.value || null;
			details.ownerVillage = this.ownerForm.get('ownerVillage')?.value || null;
			details.permanentProvinceId = this.ownerForm.get('permanentProvinceId')?.value || null;
			details.permanentDistrictId = this.ownerForm.get('permanentDistrictId')?.value || null;
			details.permanentVillage = this.ownerForm.get('permanentVillage')?.value || null;
			details.temporaryProvinceId = this.ownerForm.get('temporaryProvinceId')?.value || null;
			details.temporaryDistrictId = this.ownerForm.get('temporaryDistrictId')?.value || null;
			details.temporaryVillage = this.ownerForm.get('temporaryVillage')?.value || null;
			details.isAddressChange = false;
		}

		this.comservice.updateowner(details).subscribe(
			result => {
				if (result.id !== 0) {
					this.selectedId = result.id;
					if (this.isAddressChangeMode) {
						this.toastr.success("آدرس موفقانه تغییر یافت");
						this.isAddressChangeMode = false;
						// Reload data to refresh address display
						this.ngOnInit();
					} else {
						this.toastr.info("معلومات موفقانه تغیر یافت");
					}
					this.onNextClick();
				}
			},
			error => {
				console.error('Error updating owner:', error);
				this.toastr.error("خرابی در تغیر معلومات: " + (error.error || error.message || 'نامعلوم'));
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
		this.ownerDistrict = [];
		this.permanentDistrict = [];
		this.temporaryDistrict = [];
		this.isAddressChangeMode = false;
		this.addressHistory = [];
		this.showAddressHistory = false;
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
	get phoneNumber() { return this.ownerForm.get('phoneNumber'); }
	get whatsAppNumber() { return this.ownerForm.get('whatsAppNumber'); }
	// Owner's Own Address getters (آدرس اصلی مالک)
	get ownerProvinceId() { return this.ownerForm.get('ownerProvinceId'); }
	get ownerDistrictId() { return this.ownerForm.get('ownerDistrictId'); }
	get ownerVillage() { return this.ownerForm.get('ownerVillage'); }
	// Permanent Address getters (آدرس دایمی) - Current Residence
	get permanentProvinceId() { return this.ownerForm.get('permanentProvinceId'); }
	get permanentDistrictId() { return this.ownerForm.get('permanentDistrictId'); }
	get permanentVillage() { return this.ownerForm.get('permanentVillage'); }
	// Temporary Address getters (آدرس موقت)
	get temporaryProvinceId() { return this.ownerForm.get('temporaryProvinceId'); }
	get temporaryDistrictId() { return this.ownerForm.get('temporaryDistrictId'); }
	get temporaryVillage() { return this.ownerForm.get('temporaryVillage'); }

	isElectricIdSelected(): boolean {
		const identityCardTypeId = this.ownerForm.get('identityCardTypeId')?.value;
		const selectedType = (this.filteredIdTypes || []).find((item: any) => item.id === identityCardTypeId);
		const selectedName = selectedType?.name || '';
		return selectedName === 'الکترونیکی';
	}
}
