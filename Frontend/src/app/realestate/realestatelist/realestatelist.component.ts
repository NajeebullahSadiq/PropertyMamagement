import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { companydetailsList } from 'src/app/models/companydetails';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { RbacService } from 'src/app/shared/rbac.service';
import { DeleteConfirmationDialogComponent } from 'src/app/shared/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CalendarType } from 'src/app/models/calendar-type';

@Component({
  selector: 'app-realestatelist',
  templateUrl: './realestatelist.component.html',
  styleUrls: ['./realestatelist.component.scss']
})
export class RealestatelistComponent implements OnInit, OnDestroy {
  properties!: companydetailsList[];
  filteredProperties!: companydetailsList[];
  
  // Force Hijri Shamsi calendar for company module
  readonly hijriShamsi = CalendarType.HIJRI_SHAMSI;
  
  searchTerm: string = '';
  page: number = 1;
  count: number = 0;
  tableSize: number = 10;
  tableSizes: any = [10,50,100];
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
  
  // Dropdowns for reports
  provinces: any[] = [];
  reportDistricts: any[] = [];

  constructor(
    private http: HttpClient, 
    private comservice: CompnaydetailService, 
    private toastr: ToastrService, 
    private router: Router,
    private rbacService: RbacService,
    private dialog: MatDialog
  ) {
    this.isViewOnly = this.rbacService.isViewOnly();
    this.isAdmin = this.rbacService.isAdmin();
    this.canEdit = this.rbacService.hasPermission('company.create') || this.rbacService.hasPermission('company.edit');
    this.canPrint = this.rbacService.hasPermission('company.view');
    
    // Setup debounced search
    this.searchSubject.pipe(
      debounceTime(500), // Wait 500ms after user stops typing
      distinctUntilChanged() // Only emit if value is different from previous
    ).subscribe(searchTerm => {
      this.searchTerm = searchTerm;
      this.page = 1; // Reset page when search changes
      this.loadData();
    });
  }

  ngOnInit() {
    this.loadData();
    this.loadProvinces();
  }

  ngOnDestroy() {
    this.searchSubject.complete();
  }

  loadData(){
    this.comservice.getComapaniesList(this.searchTerm).subscribe(properties => {
      this.properties = properties;
      this.filteredProperties = properties;
      this.count = properties.length;
    });
  }

  loadProvinces(): void {
    this.comservice.getProvinces().subscribe((res: any) => {
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
    const url = this.router.createUrlTree(['/printLicense', propertyId]).toString();
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  onTableDataChange(event: any) {
    this.page = event;
  }

  onTableSizeChange(event: any): void {
    this.tableSize = event.target.value;
    this.page = 1;
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

  generateReport(): void {
    console.log('=== Generate Report Called ===');
    console.log('reportStartDate:', this.reportStartDate);
    console.log('reportEndDate:', this.reportEndDate);
    console.log('reportStartDate type:', typeof this.reportStartDate);
    console.log('reportEndDate type:', typeof this.reportEndDate);
    
    if (!this.reportStartDate || !this.reportEndDate) {
      this.toastr.warning('لطفاً تاریخ شروع و پایان را وارد کنید');
      return;
    }

    // Check if dates are empty strings
    if (this.reportStartDate === '' || this.reportEndDate === '') {
      this.toastr.warning('لطفاً تاریخ شروع و پایان را انتخاب کنید');
      return;
    }

    this.isLoadingReport = true;

    // Format dates - the date picker returns dates in format like "1403/01/15"
    let startDate = this.reportStartDate;
    let endDate = this.reportEndDate;

    // If it's a Date object, convert to string
    if (typeof this.reportStartDate === 'object' && this.reportStartDate !== null) {
      // For Jalali calendar, we need to format it properly
      startDate = this.reportStartDate.toISOString ? this.reportStartDate.toISOString().split('T')[0] : String(this.reportStartDate);
    }
    if (typeof this.reportEndDate === 'object' && this.reportEndDate !== null) {
      endDate = this.reportEndDate.toISOString ? this.reportEndDate.toISOString().split('T')[0] : String(this.reportEndDate);
    }

    console.log('Sending startDate:', startDate);
    console.log('Sending endDate:', endDate);

    // The date picker returns Gregorian dates, so use 'gregorian' calendar type
    this.comservice.getComprehensiveReport(
      startDate,
      endDate,
      'gregorian',
      this.reportProvinceId > 0 ? this.reportProvinceId : undefined,
      this.reportDistrictId > 0 ? this.reportDistrictId : undefined
    ).subscribe({
      next: (data) => {
        console.log('Report data received:', data);
        this.reportData = data;
        this.isLoadingReport = false;
      },
      error: (err) => {
        this.toastr.error('خطا در تولید گزارش');
        this.isLoadingReport = false;
        console.error('Report error:', err);
      }
    });
  }

  exportToExcel(): void {
    if (!this.reportData) {
      this.toastr.warning('ابتدا گزارش را تولید کنید');
      return;
    }

    // Create Excel data
    const excelData: any[] = [];
    
    // Header
    excelData.push(['گزارش دفتر‌های رهنمایی معاملات']);
    excelData.push([`از تاریخ: ${this.reportData.startDate} تا تاریخ: ${this.reportData.endDate}`]);
    excelData.push([]);
    
    // Cancellations
    excelData.push(['تعداد فسخ/لغوه', this.reportData.totalCancellations]);
    excelData.push([]);
    
    // Companies status
    excelData.push(['وضعیت دفتر‌ها']);
    excelData.push(['دفتر‌های فعال', this.reportData.activeCompanies]);
    excelData.push(['دفتر‌های غیرفعال', this.reportData.inactiveCompanies]);
    excelData.push(['مجموع دفتر‌ها', this.reportData.totalCompanies]);
    excelData.push([]);
    
    // Licenses by category
    excelData.push(['جواز‌ها بر اساس نوعیت']);
    excelData.push(['نوعیت جواز', 'تعداد']);
    this.reportData.licensesByCategory.forEach((item: any) => {
      excelData.push([item.category, item.count]);
    });
    excelData.push(['مجموع جواز‌ها', this.reportData.totalLicenses]);
    excelData.push([]);
    
    // Guarantors by type
    excelData.push(['تضمین‌کنندگان بر اساس نوع']);
    excelData.push(['نوع تضمین', 'تعداد']);
    this.reportData.guarantorsByType.forEach((item: any) => {
      excelData.push([item.guaranteeTypeName, item.count]);
    });
    excelData.push(['مجموع تضمین‌کنندگان', this.reportData.totalGuarantors]);
    excelData.push([]);
    
    // Revenue by license type
    excelData.push(['عواید بر اساس نوع جواز']);
    excelData.push(['نوع جواز', 'تعداد', 'قیمت فی جواز', 'مجموع عواید']);
    this.reportData.licenseRevenueByType.forEach((item: any) => {
      excelData.push([item.licenseType, item.count, item.pricePerLicense, item.totalRevenue]);
    });
    excelData.push(['مجموع عواید', '', '', this.reportData.totalRevenue]);
    
    // Convert to CSV
    const csv = excelData.map(row => row.join(',')).join('\n');
    const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', `companies-report-${Date.now()}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    this.toastr.success('گزارش با موفقیت صادر شد');
  }
}
