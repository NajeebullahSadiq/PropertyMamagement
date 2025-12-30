import { Component, Injectable } from '@angular/core';
import { DashboardService } from 'src/app/shared/dashboard.service';
import {
	NgbDateStruct,
	NgbCalendar,
	NgbDatepickerI18n,
	NgbCalendarPersian,
} from '@ng-bootstrap/ng-bootstrap';
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
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.scss'],
  providers: [
		{ provide: NgbCalendar, useClass: NgbCalendarPersian },
		{ provide: NgbDatepickerI18n, useClass: NgbDatepickerI18nPersian },
	],
})
export class ReportComponent {
  dashboardData: any=[];
  startDate: any;
  endDate:any;
  topUsers: any[]=[];
  vehicleTopUsers:any[]=[];
  constructor(private dashService:DashboardService) { 

  }
  ngOnInit(): void {
    this.dashboardData.totalRecord=[];
    this.dashboardData.totalRecord.totalAmount=0;
    this.dashboardData.totalRecord.totalAmountNotCompleted=0;
    this.dashboardData.totalRecord.totalAmountCompleted=0;
    this.dashboardData.totalRecord.totalTransaction=0;
    this.dashboardData.totalRecord.totalTransactionNotCompleted=0;
    this.dashboardData.totalRecord.totalTransactionCompleted=0;
    this.dashboardData.totalRecord.totalRoyaltyAmount=0;
    this.dashboardData.totalRecord.totalRoyaltyAmountNotCompleted=0;
    this.dashboardData.totalRecord.totalRoyaltyAmountCompleted=0;
    //This Second
    this.dashboardData.totalRecord.totalAmountProperty=0;
    this.dashboardData.totalRecord.totalAmountPropertyNotCompleted=0;
    this.dashboardData.totalRecord.totalAmountPropertyCompleted=0;
    this.dashboardData.totalRecord.totalPropertyTransaction=0;
    this.dashboardData.totalRecord.totalPropertyTransactionNotCompleted=0;
    this.dashboardData.totalRecord.totalPropertyTransactionCompleted=0;
    this.dashboardData.totalRecord.totalPropertyRoyaltyAmount=0;
    this.dashboardData.totalRecord.totalPropertyRoyaltyAmountNotCompleted=0;
    this.dashboardData.totalRecord.totalPropertyRoyaltyAmountCompleted=0;
  }

  fetchDashboardData(): void {
    const sDate = this.startDate ? new Date(this.startDate) : null;
    const eDate = this.endDate ? new Date(this.endDate) : null;
    if (!sDate || !eDate || Number.isNaN(sDate.getTime()) || Number.isNaN(eDate.getTime())) {
      return;
    }

    const start = this.toYmd(sDate);
    const end = this.toYmd(eDate);

    this.dashService.getDashboardDataByDate(start, end)
      .subscribe((data) => {
        this.dashboardData = data;
      });
      this.dashService.getTotalUserDataByDate(start, end)
      .subscribe((data) => {
        console.log(data); // Check the response from the server in the browser console
        
        if (data && data.topUsers && Array.isArray(data.topUsers)) {
          this.topUsers = data.topUsers; // Extract the array from the 'topUsers' property
        } else {
          console.error("Invalid 'topUsers' data format:", data);
        }
      });
      this.dashService.getVehicleTotalUserDataByDate(start, end)
      .subscribe((data) => {
        console.log(data); // Check the response from the server in the browser console
        
        if (data && data.topUsers && Array.isArray(data.topUsers)) {
          this.vehicleTopUsers = data.topUsers; // Extract the array from the 'topUsers' property
        } else {
          console.error("Invalid 'topUsers' data format:", data);
        }
      });
  }

  private toYmd(date: Date): string {
    const y = date.getFullYear();
    const m = `${date.getMonth() + 1}`.padStart(2, '0');
    const d = `${date.getDate()}`.padStart(2, '0');
    return `${y}-${m}-${d}`;
  }
  
}
