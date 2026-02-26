import { Component, EventEmitter, Injectable, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { SellerService } from 'src/app/shared/seller.service';
import { LicenseDetail } from 'src/app/models/LicenseDetail';
import { PropertyService } from 'src/app/shared/property.service';
import { FileuploadComponent } from '../fileupload/fileupload.component';
import { Router } from '@angular/router';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';
import { CalendarService } from 'src/app/shared/calendar.service';
import { CalendarType } from 'src/app/models/calendar-type';
import { NumeralService } from 'src/app/shared/numeral.service';
import { RbacService } from 'src/app/shared/rbac.service';
import { AuthService } from 'src/app/shared/auth.service';

@Component({
	selector: 'app-licensedetails',
	templateUrl: './licensedetails.component.html',
	styleUrls: ['./licensedetails.component.scss'],
})
export class LicensedetailsComponent {

	licenseForm: FormGroup = new FormGroup({});
	selectedId: number = 0;
	Areas: any;
	provinces: any[] = [];  // Add provinces array
	isAdmin: boolean = false;  // Track if user is admin
	userProvinceId: number | null = null;  // Store user's province
	imageName: string = ''
	licenseDetails!: LicenseDetail[];
	licenseTypes = [
		{ id: 'realEstate', name: 'Real Estate', dari: 'املاک' },
		{ id: 'carSale', name: 'Car Sale', dari: 'موټر فروشی' }
	];

	// License Category options (نوعیت جواز)
	licenseCategoryOptions = [
		{ value: 'جدید', label: 'جدید' },
		{ value: 'تجدید', label: 'تجدید' },
		{ value: 'مثنی', label: 'مثنی' }
	];

	@Input() id: number = 0;
	@Output() next = new EventEmitter<void>();
	onNextClick() {
		this.next.emit();
	}
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
		private propertyDetailsService: PropertyService,
		private router: Router,
		private calendarConversionService: CalendarConversionService,
		private calendarService: CalendarService,
		private numeralService: NumeralService,
		private rbacService: RbacService,
		private authService: AuthService
	) {
		this.licenseForm = this.fb.group({
			id: [0],
			provinceId: ['', Validators.required],  // Add provinceId field
			licenseNumber: [''],  // Make it optional since it's auto-generated
			issueDate: ['', Validators.required],
			expireDate: ['', Validators.required],
			transferLocation: [''],
			activityLocation: [''],
			officeAddress: ['', Validators.required],
			licenseType: ['', Validators.required],
			licenseCategory: [''],
			renewalRound: [null],
			companyId: [''],
			docPath: [''],
			// Financial and Administrative Fields (جزئیات مالی و اسناد جواز)
			royaltyAmount: [null],
			royaltyDate: [''],
			tariffNumber: [''],
			penaltyAmount: [null],
			penaltyDate: [''],
			hrLetter: [''],
			hrLetterDate: [''],
		});

		this.licenseForm.get('issueDate')?.valueChanges.subscribe((value) => {
			this.applyAutoExpireDate(value);
		});

		// Watch for license category changes to show/hide renewal round field
		this.licenseForm.get('licenseCategory')?.valueChanges.subscribe(value => {
			if (value === 'تجدید') {
				this.licenseForm.get('renewalRound')?.setValidators([Validators.required]);
			} else {
				this.licenseForm.get('renewalRound')?.clearValidators();
				this.licenseForm.get('renewalRound')?.setValue(null);
			}
			this.licenseForm.get('renewalRound')?.updateValueAndValidity();
		});
	}

	private applyAutoExpireDate(issueValue: any): void {
		const expireCtrl = this.licenseForm.get('expireDate');
		if (!expireCtrl) return;

		if (!issueValue) {
			expireCtrl.setValue('');
			expireCtrl.markAsDirty();
			expireCtrl.updateValueAndValidity();
			return;
		}

		const currentCalendar = this.calendarService.getSelectedCalendar();
		const issueDate = this.tryResolveGregorianDate(issueValue, currentCalendar);
		if (!issueDate) {
			expireCtrl.setValue('');
			expireCtrl.updateValueAndValidity();
			return;
		}

		// Validate the issue date is valid
		if (isNaN(issueDate.getTime())) {
			expireCtrl.setValue('');
			expireCtrl.updateValueAndValidity();
			return;
		}

		// Add 3 years to the Gregorian date using a safer method
		const expireDate = new Date(issueDate.getTime());
		const currentYear = expireDate.getFullYear();
		const currentMonth = expireDate.getMonth();
		const currentDay = expireDate.getDate();
		
		// Set the new year first
		expireDate.setFullYear(currentYear + 3);
		
		// If the day changed (e.g., Feb 29 -> Feb 28), adjust it
		if (expireDate.getMonth() !== currentMonth) {
			expireDate.setDate(0); // Go to last day of previous month
		}
		
		// Validate the expire date is valid
		if (isNaN(expireDate.getTime())) {
			expireCtrl.setValue('');
			expireCtrl.updateValueAndValidity();
			return;
		}
		
		// Set the Date object - the datepicker will handle the conversion
		expireCtrl.setValue(expireDate);
		expireCtrl.updateValueAndValidity();
	}

	private tryResolveGregorianDate(value: any, calendar: CalendarType): Date | null {
		if (!value) return null;

		if (value instanceof Date && !isNaN(value.getTime())) {
			return value;
		}

		if (typeof value === 'object' && value.year && value.month && value.day) {
			try {
				return this.calendarConversionService.toGregorian({
					year: value.year,
					month: value.month,
					day: value.day,
					calendarType: calendar
				});
			} catch {
				return null;
			}
		}

		if (typeof value === 'string') {
			const normalized = value.trim();
			if (!normalized) return null;

			// Check if it's an ISO date format (YYYY-MM-DD) - these are always Gregorian
			if (/^\d{4}-\d{2}-\d{2}$/.test(normalized)) {
				const date = new Date(normalized + 'T00:00:00');
				return isNaN(date.getTime()) ? null : date;
			}

			const normalizedForParser = calendar === CalendarType.GREGORIAN
				? normalized
				: normalized.replace(/-/g, '/');

			return this.calendarConversionService.parseInputDate(normalizedForParser, calendar);
		}

		return null;
	}

	ngOnInit() {
		// Check if user is admin
		this.isAdmin = this.rbacService.isAdmin();
		
		// Get user's province if not admin
		if (!this.isAdmin) {
			this.authService.getCurrentUserProfile().subscribe({
				next: (profile: any) => {
					this.userProvinceId = profile.provinceId;
					// Auto-populate province for non-admin users
					if (this.userProvinceId) {
						this.licenseForm.patchValue({ provinceId: this.userProvinceId });
						// Disable the field for non-admin users
						this.licenseForm.get('provinceId')?.disable();
					}
				},
				error: (error: any) => {
					console.error('Error loading user profile:', error);
				}
			});
		}
		
		// Load provinces
		this.comservice.getProvinces().subscribe((res: any) => {
			this.provinces = res;
		});
		
		// Remove Areas loading since we don't need it anymore

		this.comservice.getLicenseById(this.id)
			.subscribe({
				next: (detail) => {
					if (detail && detail.length > 0) {
						this.licenseDetails = detail;
						this.licenseForm.setValue({
							id: detail[0].id,
							provinceId: detail[0].provinceId || '',  // Add provinceId
							licenseNumber: detail[0].licenseNumber,
							issueDate: detail[0].issueDate,
							expireDate: detail[0].expireDate,
							transferLocation: detail[0].transferLocation || '',
							activityLocation: detail[0].activityLocation || '',
							officeAddress: detail[0].officeAddress,
							licenseType: detail[0].licenseType,
							licenseCategory: detail[0].licenseCategory || '',
							renewalRound: detail[0].renewalRound || null,
							companyId: detail[0].companyId,
							docPath: detail[0].docPath,
							// Financial and Administrative Fields
							royaltyAmount: detail[0].royaltyAmount || null,
							royaltyDate: detail[0].royaltyDate || '',
							tariffNumber: detail[0].tariffNumber || '',
							penaltyAmount: detail[0].penaltyAmount || null,
							penaltyDate: detail[0].penaltyDate || '',
							hrLetter: detail[0].hrLetter || '',
							hrLetterDate: detail[0].hrLetterDate || '',
						});

						this.applyAutoExpireDate(detail[0].issueDate);
						this.selectedId = detail[0].id;
						// Set imageName from existing docPath
						this.imageName = detail[0].docPath || '';
					}
				},
				error: (error) => {
					console.error('Error loading license details:', error);
				}
			});
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
		// Get form value including disabled fields
		const details = this.licenseForm.getRawValue() as LicenseDetail;
		const issueDateValue = this.licenseForm.get('issueDate')?.value;
		const expireDateValue = this.licenseForm.get('expireDate')?.value;
		const hrLetterDateValue = this.licenseForm.get('hrLetterDate')?.value;
		const royaltyDateValue = this.licenseForm.get('royaltyDate')?.value;
		const penaltyDateValue = this.licenseForm.get('penaltyDate')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		if (!issueDateValue) {
			this.toastr.warning('تاریخ صدور جواز الزامی است');
			return;
		}

		const issueAsGregorian = this.tryResolveGregorianDate(issueDateValue, currentCalendar);
		const expireAsGregorian = this.tryResolveGregorianDate(expireDateValue, currentCalendar);
		if (!issueAsGregorian || !expireAsGregorian) {
			this.toastr.warning('تاریخ ختم جواز به صورت سیستمی محاسبه نشد');
			return;
		}

		if (issueDateValue) {
			details.issueDate = this.formatDateForBackend(issueDateValue);
		}
		if (expireDateValue) {
			details.expireDate = this.formatDateForBackend(expireDateValue);
		}
		if (hrLetterDateValue) {
			details.hrLetterDate = this.formatDateForBackend(hrLetterDateValue);
		}
		if (royaltyDateValue) {
			details.royaltyDate = this.formatDateForBackend(royaltyDateValue);
		}
		if (penaltyDateValue) {
			details.penaltyDate = this.formatDateForBackend(penaltyDateValue);
		}

		// Convert Persian/Dari numerals to Western numerals for numeric fields
		if (details.royaltyAmount) {
			const parsed = this.numeralService.parseNumber(String(details.royaltyAmount));
			details.royaltyAmount = isNaN(parsed) ? undefined : parsed;
		}
		if (details.penaltyAmount) {
			const parsed = this.numeralService.parseNumber(String(details.penaltyAmount));
			details.penaltyAmount = isNaN(parsed) ? undefined : parsed;
		}
		if (details.hrLetter) {
			details.hrLetter = this.numeralService.toWesternArabic(details.hrLetter);
		}

		details.calendarType = currentCalendar;
		details.companyId = this.comservice.mainTableId;
		details.docPath = this.imageName;
		details.tariffNumber = (this.licenseForm.get('tariffNumber')?.value ?? '').toString().trim();
		if (details.id === 0 && this.selectedId !== 0 || this.selectedId !== null) {
			details.id = this.selectedId;
		}
		this.comservice.updateLicenseDetails(details).subscribe(result => {
			if (result.id !== 0)
				this.selectedId = result.id;
			this.toastr.info("معلومات موفقانه تغیر یافت ");
		});
	}

	addData(): void {
		// Get form value including disabled fields
		const details = this.licenseForm.getRawValue() as LicenseDetail;
		const issueDateValue = this.licenseForm.get('issueDate')?.value;
		const expireDateValue = this.licenseForm.get('expireDate')?.value;
		const hrLetterDateValue = this.licenseForm.get('hrLetterDate')?.value;
		const royaltyDateValue = this.licenseForm.get('royaltyDate')?.value;
		const penaltyDateValue = this.licenseForm.get('penaltyDate')?.value;
		const currentCalendar = this.calendarService.getSelectedCalendar();

		if (!issueDateValue) {
			this.toastr.warning('تاریخ صدور جواز الزامی است');
			return;
		}

		const issueAsGregorian = this.tryResolveGregorianDate(issueDateValue, currentCalendar);
		const expireAsGregorian = this.tryResolveGregorianDate(expireDateValue, currentCalendar);
		if (!issueAsGregorian || !expireAsGregorian) {
			this.toastr.warning('تاریخ ختم جواز به صورت سیستمی محاسبه نشد');
			return;
		}

		if (issueDateValue) {
			details.issueDate = this.formatDateForBackend(issueDateValue);
		}
		if (expireDateValue) {
			details.expireDate = this.formatDateForBackend(expireDateValue);
		}
		if (hrLetterDateValue) {
			details.hrLetterDate = this.formatDateForBackend(hrLetterDateValue);
		}
		if (royaltyDateValue) {
			details.royaltyDate = this.formatDateForBackend(royaltyDateValue);
		}
		if (penaltyDateValue) {
			details.penaltyDate = this.formatDateForBackend(penaltyDateValue);
		}

		// Convert Persian/Dari numerals to Western numerals for numeric fields
		if (details.royaltyAmount) {
			const parsed = this.numeralService.parseNumber(String(details.royaltyAmount));
			details.royaltyAmount = isNaN(parsed) ? undefined : parsed;
		}
		if (details.penaltyAmount) {
			const parsed = this.numeralService.parseNumber(String(details.penaltyAmount));
			details.penaltyAmount = isNaN(parsed) ? undefined : parsed;
		}
		if (details.hrLetter) {
			details.hrLetter = this.numeralService.toWesternArabic(details.hrLetter);
		}

		details.calendarType = currentCalendar;
		details.docPath = this.imageName;
		details.companyId = this.comservice.mainTableId;
		details.tariffNumber = (this.licenseForm.get('tariffNumber')?.value ?? '').toString().trim();
		if (details.id === null) {
			details.id = 0;
		}
		this.comservice.addLicenseDetails(details).subscribe(result => {
			if (result.id !== 0)
				this.toastr.success("معلومات موفقانه ثبت شد");
			this.selectedId = result.id;
			this.onNextClick();
		});
	}

	resetForms(): void {
		if (this.childComponent) {
			this.childComponent.reset();
		}
		this.selectedId = 0;
		this.licenseForm.reset();
	}

	uploadFinished = (event: string) => {
		this.imageName = event;
	}

	downloadFiles() {
		const filePath = this.licenseForm.get('docPath')?.value;
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

	navigateToPrint() {
		let id = this.comservice.mainTableId;
		this.router.navigate(['/printLicense', id]);
	}

	get provinceId() { return this.licenseForm.get('provinceId'); }
	get licenseNumber() { return this.licenseForm.get('licenseNumber'); }
	get officeAddress() { return this.licenseForm.get('officeAddress'); }
	get issueDate() { return this.licenseForm.get('issueDate'); }
	get expireDate() { return this.licenseForm.get('expireDate'); }
	get licenseType() { return this.licenseForm.get('licenseType'); }
	get licenseCategory() { return this.licenseForm.get('licenseCategory'); }
	get renewalRound() { return this.licenseForm.get('renewalRound'); }
	get transferLocation() { return this.licenseForm.get('transferLocation'); }
	get companyId() { return this.licenseForm.get('companyId'); }
	get docPath() { return this.licenseForm.get('docPath'); }
	// Financial and Administrative Fields getters
	get royaltyAmount() { return this.licenseForm.get('royaltyAmount'); }
	get royaltyDate() { return this.licenseForm.get('royaltyDate'); }
	get penaltyAmount() { return this.licenseForm.get('penaltyAmount'); }
	get penaltyDate() { return this.licenseForm.get('penaltyDate'); }
	get hrLetter() { return this.licenseForm.get('hrLetter'); }
	get hrLetterDate() { return this.licenseForm.get('hrLetterDate'); }
}
