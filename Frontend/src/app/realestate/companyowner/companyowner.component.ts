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
	ownerForm!: FormGroup;
	ownerDetails!: companyowner[];
	filteredIdTypes: any;

	// Address related properties
	province: any;
	ownerDistrict: any;
	permanentDistrict: any;

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
	} = {};

	// New address districts (for address change mode)
	newOwnerDistrict: any;
	newPermanentDistrict: any;

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
			electronicNationalIdNumber: ['', Validators.required],
			companyId: [''],
			pothoPath: [''],
			// Contact Information
			phoneNumber: [''],
			whatsAppNumber: [''],
			// Owner's Own Address Fields (آدرس اصلی مالک)
			ownerProvinceId: ['', Validators.required],
			ownerDistrictId: ['', Validators.required],
			ownerVillage: [''],
			// Permanent Address Fields (آدرس دایمی) - Current Residence
			permanentProvinceId: ['', Validators.required],
			permanentDistrictId: ['', Validators.required],
			permanentVillage: ['', Validators.required],
			// New Address Fields (for address change mode)
			newOwnerProvinceId: [''],
			newOwnerDistrictId: [''],
			newOwnerVillage: [''],
			newPermanentProvinceId: [''],
			newPermanentDistrictId: [''],
			newPermanentVillage: ['']
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

	// Property to track if WhatsApp should be same as phone
	sameAsPhone: boolean = false;

	// Toggle same as phone checkbox
	toggleSameAsPhone(): void {
		this.sameAsPhone = !this.sameAsPhone;
		if (this.sameAsPhone) {
			const phoneNumber = this.ownerForm.get('phoneNumber')?.value;
			this.ownerForm.patchValue({ whatsAppNumber: phoneNumber });
		}
	}

	// Sync WhatsApp when phone changes (if sameAsPhone is checked)
	onPhoneNumberChange(): void {
		if (this.sameAsPhone) {
			const phoneNumber = this.ownerForm.get('phoneNumber')?.value;
			this.ownerForm.patchValue({ whatsAppNumber: phoneNumber });
		}
	}

	ngOnInit() {
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
							electronicNationalIdNumber: detail[0].electronicNationalIdNumber || '',
							companyId: detail[0].companyId,
							pothoPath: detail[0].pothoPath,
							phoneNumber: detail[0].phoneNumber || '',
							whatsAppNumber: detail[0].whatsAppNumber || '',
							ownerProvinceId: detail[0].ownerProvinceId || '',
							ownerDistrictId: detail[0].ownerDistrictId || '',
							ownerVillage: detail[0].ownerVillage || '',
							permanentProvinceId: detail[0].permanentProvinceId || '',
							permanentDistrictId: detail[0].permanentDistrictId || '',
							permanentVillage: detail[0].permanentVillage || ''
						});

						// Store current address for display
						this.currentAddressDisplay = {
							ownerProvinceName: detail[0].ownerProvinceName,
							ownerDistrictName: detail[0].ownerDistrictName,
							ownerVillage: detail[0].ownerVillage,
							permanentProvinceName: detail[0].permanentProvinceName,
							permanentDistrictName: detail[0].permanentDistrictName,
							permanentVillage: detail[0].permanentVillage
						};

						this.comservice.ownerId = detail[0].id;
						this.selectedId = detail[0].id;
						const dateString = detail[0].dateofBirth;

						if (detail[0].pothoPath) {
							// Use the Upload controller's view endpoint for proper file serving
							this.imagePath = `${this.baseUrl}api/Upload/view/${detail[0].pothoPath}`;
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
			newPermanentVillage: ''
		});
		this.newOwnerDistrict = [];
		this.newPermanentDistrict = [];
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
			newPermanentVillage: ''
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

	uploadFinished = (event: string) => {
		this.imageName = event;
		this.imagePath = event ? `${this.baseUrl}api/Upload/view/${event}` : 'assets/img/avatar.png';
	}

	profilePreviewChanged = (localObjectUrl: string) => {
		if (localObjectUrl) {
			this.imagePath = localObjectUrl;
			return;
		}
		if (this.imageName) {
			this.imagePath = `${this.baseUrl}api/Upload/view/${this.imageName}`;
			return;
		}
		this.imagePath = 'assets/img/avatar.png';
	}

	profileImageUploaded = (dbPath: string) => {
		this.imageName = dbPath || '';
		this.ownerForm.patchValue({ pothoPath: this.imageName });
		this.imagePath = this.imageName ? `${this.baseUrl}api/Upload/view/${this.imageName}` : 'assets/img/avatar.png';
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

		if (details.electronicNationalIdNumber != null) {
			details.electronicNationalIdNumber = String(details.electronicNationalIdNumber);
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
		const details = this.ownerForm.value as companyowner;
		const dateValue = this.ownerForm.get('dateofBirth')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		if (dateValue) {
			details.dateofBirth = this.formatDateForBackend(dateValue);
		} else {
			this.toastr.error("لطفا تاریخ تولد را انتخاب کنید");
			return;
		}

		if (details.electronicNationalIdNumber != null) {
			details.electronicNationalIdNumber = String(details.electronicNationalIdNumber);
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
			details.isAddressChange = true;
		} else {
			details.ownerProvinceId = this.ownerForm.get('ownerProvinceId')?.value || null;
			details.ownerDistrictId = this.ownerForm.get('ownerDistrictId')?.value || null;
			details.ownerVillage = this.ownerForm.get('ownerVillage')?.value || null;
			details.permanentProvinceId = this.ownerForm.get('permanentProvinceId')?.value || null;
			details.permanentDistrictId = this.ownerForm.get('permanentDistrictId')?.value || null;
			details.permanentVillage = this.ownerForm.get('permanentVillage')?.value || null;
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
		this.isAddressChangeMode = false;
		this.addressHistory = [];
		this.showAddressHistory = false;
	}

	get firstName() { return this.ownerForm.get('firstName'); }
	get fatherName() { return this.ownerForm.get('fatherName'); }
	get grandFatherName() { return this.ownerForm.get('grandFatherName'); }
	get educationLevelId() { return this.ownerForm.get('educationLevelId'); }
	get dateofBirth() { return this.ownerForm.get('dateofBirth'); }
	get electronicNationalIdNumber() { return this.ownerForm.get('electronicNationalIdNumber'); }
	get companyId() { return this.ownerForm.get('companyId'); }
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
}
