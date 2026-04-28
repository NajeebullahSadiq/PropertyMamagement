import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { companydetailsList } from 'src/app/models/companydetails';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { RbacService } from 'src/app/shared/rbac.service';
import { DeleteConfirmationDialogComponent } from 'src/app/shared/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CalendarType } from 'src/app/models/calendar-type';
import { CalendarConversionService } from 'src/app/shared/calendar-conversion.service';

@Component({
  selector: 'app-realestatelist',
  templateUrl: './realestatelist.component.html',
  styleUrls: ['./realestatelist.component.scss']
})
export class RealestatelistComponent extends BaseComponent implements OnInit, OnDestroy {
  properties!: companydetailsList[];
  filteredProperties!: companydetailsList[];
  
  // Force Hijri Shamsi calendar for company module
  readonly hijriShamsi = CalendarType.HIJRI_SHAMSI;
  
  searchTerm: string = '';
  page: number = 1;
  count: number = 0;
  tableSize: number = 20;
  tableSizes: any = [10,20,50,100];
  isViewOnly: boolean = false;
  isAdmin: boolean = false;
  canEdit: boolean = false;
  canPrint: boolean = false;
  private searchSubject = new Subject<string>();

  // Report fields
  showReports = false;
  reportStartDate: any = '';
  reportEndDate: any = '';
  reportProvinceId: number = 0;
  reportDistrictId: number = 0;
  reportData: any = null;
  isLoadingReport = false;

  // Filter lists
  activeFilterTab: string = 'summary';

  faskhStartDate: any = '';
  faskhEndDate: any = '';
  faskhList: any[] = [];
  isLoadingFaskh = false;

  laghwaStartDate: any = '';
  laghwaEndDate: any = '';
  laghwaList: any[] = [];
  isLoadingLaghwa = false;

  transferStartDate: any = '';
  transferEndDate: any = '';
  transferList: any[] = [];
  isLoadingTransfer = false;

  inactiveStartDate: any = '';
  inactiveEndDate: any = '';
  inactiveList: any[] = [];
  isLoadingInactive = false;

  // Dropdowns for reports
  provinces: any[] = [];
  reportDistricts: any[] = [];

  constructor(
    private http: HttpClient, 
    private comservice: CompnaydetailService, 
    private toastr: ToastrService, 
    private router: Router,
    private rbacService: RbacService,
    private dialog: MatDialog,
    private calendarConversion: CalendarConversionService
  ) {
    super();
    this.isViewOnly = this.rbacService.isViewOnly();
    this.isAdmin = this.rbacService.isAdmin();
    this.canEdit = this.rbacService.hasPermission('company.create') || this.rbacService.hasPermission('company.edit');
    this.canPrint = this.rbacService.hasPermission('company.view');
    
    // Setup debounced search
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(searchTerm => {
      this.searchTerm = searchTerm;
      this.page = 1;
      this.loadData();
    });
  }

  ngOnInit() {
    this.loadData();
    this.loadProvinces();
  }

  override ngOnDestroy() {
    this.searchSubject.complete();
    super.ngOnDestroy();
  }

  loadData(){
    this.comservice.getComapaniesList(this.page, this.tableSize, this.searchTerm).pipe(takeUntil(this.destroy$)).subscribe(response => {
      this.properties = response?.items || [];
      this.filteredProperties = this.properties;
      this.count = response?.totalCount || 0;
    });
  }

  loadProvinces(): void {
    this.comservice.getProvinces().pipe(takeUntil(this.destroy$)).subscribe((res: any) => {
      this.provinces = res as any[];
    });
  }

  onReportProvinceChange(event: any): void {
    this.reportDistrictId = 0;
    this.reportDistricts = [];
    
    const provinceId = event?.id || event;
    
    if (provinceId && provinceId > 0) {
      this.http.get(`${environment.apiURL}/SellerDetails/${provinceId}`).subscribe({
        next: (res: any) => {
          this.reportDistricts = res as any[];
          console.log('Districts loaded:', this.reportDistricts);
        },
        error: (err) => {
          console.error('Error loading districts:', err);
          this.toastr.error('خطا در بارگذاری ولسوالی‌ها');
        }
      });
    }
  }

  onEdit(propertyId: number) {
    this.router.navigate(['/realestate'], { queryParams: { id: propertyId } });
  }

  onView(propertyId: number) {
    this.router.navigate(['/realestate/view', propertyId]);
  }

  onPrint(propertyId: number) {
    const url = this.router.createUrlTree(['/print/license', propertyId]).toString();
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  onTableDataChange(event: any) {
    this.page = event;
    this.loadData();
  }

  onTableSizeChange(event: any): void {
    this.tableSize = event.target.value;
    this.page = 1;
    this.loadData();
  }

  onSearch() {
    this.searchSubject.next(this.searchTerm);
  }

  onDelete(propertyId: number, title: string) {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        title: 'تأیید حذف دفتر',
        message: 'آیا مطمئن هستید که می‌خواهید این دفتر را حذف کنید؟',
        itemName: title
      },
      disableClose: true,
      panelClass: 'delete-dialog'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.comservice.deleteCompany(propertyId).subscribe({
          next: (response) => {
            this.toastr.success('دفتر با موفقیت حذف شد', 'موفق');
            this.loadData();
          },
          error: (error) => {
            console.error('Error deleting company:', error);
            if (error.status === 403) {
              this.toastr.error('شما اجازه حذف دفتر را ندارید', 'خطا');
            } else {
              this.toastr.error('خطا در حذف دفتر', 'خطا');
            }
          }
        });
      }
    });
  }

  // ==================== Reports ====================

  toggleReports(): void {
    this.showReports = !this.showReports;
    if (!this.showReports) {
      this.reportData = null;
    }
  }

  setFilterTab(tab: string): void {
    this.activeFilterTab = tab;
  }

  isTab(tab: string): boolean {
    return this.activeFilterTab === tab;
  }

  /**
   * Convert Gregorian Date object to Hijri Shamsi string format (YYYY/MM/DD)
   */
  private convertGregorianToHijriShamsiString(gregorianDate: Date): string {
    const hijriDate = this.calendarConversion.fromGregorian(gregorianDate, CalendarType.HIJRI_SHAMSI);
    const yearStr = hijriDate.year.toString().padStart(4, '0');
    const monthStr = hijriDate.month.toString().padStart(2, '0');
    const dayStr = hijriDate.day.toString().padStart(2, '0');
    return `${yearStr}/${monthStr}/${dayStr}`;
  }

  private toHijriString(dateValue: any): string {
    if (!dateValue || dateValue === '') return '';
    if (dateValue instanceof Date) return this.convertGregorianToHijriShamsiString(dateValue);
    return String(dateValue).replace(/\//g, '/');
  }

  generateReport(): void {
    if (!this.reportStartDate || !this.reportEndDate ||
        this.reportStartDate === '' || this.reportEndDate === '') {
      this.toastr.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
      return;
    }

    this.isLoadingReport = true;
    const startDate = this.toHijriString(this.reportStartDate);
    const endDate = this.toHijriString(this.reportEndDate);

    this.comservice.getComprehensiveReport(
      startDate, endDate, 'hijriShamsi',
      this.reportProvinceId > 0 ? this.reportProvinceId : undefined,
      this.reportDistrictId > 0 ? this.reportDistrictId : undefined
    ).subscribe({
      next: (data) => { this.reportData = data; this.isLoadingReport = false; },
      error: (err) => { this.toastr.error('خطا در تولید گزارش'); this.isLoadingReport = false; }
    });
  }

  // ---- فسخ list ----
  loadFaskhList(): void {
    if (!this.faskhStartDate || !this.faskhEndDate) {
      this.toastr.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
      return;
    }
    this.isLoadingFaskh = true;
    this.comservice.getFaskhList(this.toHijriString(this.faskhStartDate), this.toHijriString(this.faskhEndDate))
      .subscribe({
        next: (data) => { this.faskhList = data.items || []; this.isLoadingFaskh = false; },
        error: () => { this.toastr.error('خطا در بارگذاری لیست فسخ'); this.isLoadingFaskh = false; }
      });
  }

  exportFaskhToExcel(): void {
    if (!this.faskhList.length) { this.toastr.warning('داده‌ای برای صادرات وجود ندارد'); return; }
    const rows: any[] = [
      ['لیست فسخ'],
      ['#', 'عنوان رهنمایی معاملات', 'نام مالک', 'نام پدر مالک', 'نمبر جواز', 'تاریخ صدور جواز', 'تضمین کننده', 'نمبر مکتوب فسخ جواز', 'نمبر مکتوب فسخ عواید', 'تاریخ مکتوب فسخ', 'ملاحظات']
    ];
    this.faskhList.forEach((r, i) => rows.push([
      i + 1, r.companyTitle, r.ownerFullName, r.ownerFatherName,
      r.licenseNumber, r.issueDate, r.guarantor,
      r.licenseCancellationLetterNumber, r.revenueCancellationLetterNumber,
      r.licenseCancellationLetterDate, r.remarks
    ]));
    this.downloadCsv(rows, 'faskh-list');
  }

  // ---- لغوه list ----
  loadLaghwaList(): void {
    if (!this.laghwaStartDate || !this.laghwaEndDate) {
      this.toastr.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
      return;
    }
    this.isLoadingLaghwa = true;
    this.comservice.getLaghwaList(this.toHijriString(this.laghwaStartDate), this.toHijriString(this.laghwaEndDate))
      .subscribe({
        next: (data) => { this.laghwaList = data.items || []; this.isLoadingLaghwa = false; },
        error: () => { this.toastr.error('خطا در بارگذاری لیست لغوه'); this.isLoadingLaghwa = false; }
      });
  }

  exportLaghwaToExcel(): void {
    if (!this.laghwaList.length) { this.toastr.warning('داده‌ای برای صادرات وجود ندارد'); return; }
    const rows: any[] = [
      ['لیست لغوه'],
      ['#', 'عنوان رهنمایی معاملات', 'نام مالک', 'نام پدر مالک', 'نمبر جواز', 'تاریخ صدور جواز', 'تضمین کننده', 'نمبر مکتوب لغوه جواز', 'نمبر مکتوب لغوه عواید', 'تاریخ مکتوب لغوه', 'ملاحظات']
    ];
    this.laghwaList.forEach((r, i) => rows.push([
      i + 1, r.companyTitle, r.ownerFullName, r.ownerFatherName,
      r.licenseNumber, r.issueDate, r.guarantor,
      r.revocationLetterNumber, r.revocationRevenueLetterNumber,
      r.revocationLetterDate, r.remarks
    ]));
    this.downloadCsv(rows, 'laghwa-list');
  }

  // ---- محل انتقال list ----
  loadTransferList(): void {
    if (!this.transferStartDate || !this.transferEndDate) {
      this.toastr.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
      return;
    }
    this.isLoadingTransfer = true;
    this.comservice.getTransferLocationList(this.toHijriString(this.transferStartDate), this.toHijriString(this.transferEndDate))
      .subscribe({
        next: (data) => { this.transferList = data.items || []; this.isLoadingTransfer = false; },
        error: () => { this.toastr.error('خطا در بارگذاری لیست محل انتقال'); this.isLoadingTransfer = false; }
      });
  }

  exportTransferToExcel(): void {
    if (!this.transferList.length) { this.toastr.warning('داده‌ای برای صادرات وجود ندارد'); return; }
    const rows: any[] = [
      ['لیست محل انتقال'],
      ['#', 'عنوان رهنمایی معاملات', 'نام مالک', 'نام پدر مالک', 'نمبر جواز', 'تاریخ صدور جواز', 'تضمین کننده', 'نوع جواز', 'نوعیت جواز', 'تاریخ ختم', 'محل انتقال', 'مبلغ حق‌الامتیاز', 'مبلغ جریمه']
    ];
    this.transferList.forEach((r, i) => rows.push([
      i + 1, r.companyTitle, r.ownerFullName, r.ownerFatherName,
      r.licenseNumber, r.issueDate, r.guarantor,
      r.licenseType, r.licenseCategory, r.expireDate,
      r.transferLocation, r.royaltyAmount, r.penaltyAmount
    ]));
    this.downloadCsv(rows, 'transfer-location-list');
  }

  // ---- دفتر‌های غیرفعال list ----
  loadInactiveList(): void {
    if (!this.inactiveStartDate || !this.inactiveEndDate) {
      this.toastr.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
      return;
    }
    this.isLoadingInactive = true;
    this.comservice.getInactiveCompaniesList(this.toHijriString(this.inactiveStartDate), this.toHijriString(this.inactiveEndDate))
      .subscribe({
        next: (data) => { this.inactiveList = data.items || []; this.isLoadingInactive = false; },
        error: () => { this.toastr.error('خطا در بارگذاری لیست دفتر‌های غیرفعال'); this.isLoadingInactive = false; }
      });
  }

  exportInactiveToExcel(): void {
    if (!this.inactiveList.length) { this.toastr.warning('داده‌ای برای صادرات وجود ندارد'); return; }
    const rows: any[] = [
      ['لیست دفتر‌های غیرفعال'],
      ['#', 'عنوان رهنمایی معاملات', 'نام مالک', 'نام پدر مالک', 'نمبر جواز', 'تاریخ صدور جواز', 'تضمین کننده', 'نوع جواز', 'تاریخ ختم جواز']
    ];
    this.inactiveList.forEach((r, i) => rows.push([
      i + 1, r.title, r.ownerFullName, r.ownerFatherName,
      r.licenseNumber, r.issueDate, r.guarantor,
      r.licenseType, r.expireDate
    ]));
    this.downloadCsv(rows, 'inactive-companies-list');
  }

  // ---- shared CSV helper ----
  private downloadCsv(rows: any[][], filename: string): void {
    const csv = rows.map(r => r.map(c => `"${(c ?? '').toString().replace(/"/g, '""')}"`).join(',')).join('\n');
    const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.setAttribute('href', URL.createObjectURL(blob));
    link.setAttribute('download', `${filename}-${Date.now()}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    this.toastr.success('گزارش با موفقیت صادر شد');
  }

}
