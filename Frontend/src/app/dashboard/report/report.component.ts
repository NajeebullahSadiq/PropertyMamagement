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
    console.log(this.startDate,'---------------', this.endDate)
    
    // Extract year, month, and day from startDate object
    const syear = this.startDate.year;
    const smonth = this.startDate.month;
    const sday = this.startDate.day;

    // Extract year, month, and day from startDate object
    const eyear = this.endDate.year;
    const emonth = this.endDate.month;
    const eday = this.endDate.day;
  
    // Create a new Date object with year, month, and day
    const sDate = new Date(syear, smonth - 1, sday);
    // Create a new Date object with year, month, and day
    const eDate = new Date(eyear, emonth - 1, eday);
  
    // Format the new date as "DD-MM-YYYY"
    const formattdsDate = sDate.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).split('/').join('-');

     // Format the new date as "DD-MM-YYYY"
     const formattedeDate = eDate.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).split('/').join('-');
    this.dashService.getDashboardDataByDate(formattdsDate, formattedeDate)
      .subscribe((data) => {
        this.dashboardData = data;
      });
      this.dashService.getTotalUserDataByDate(formattdsDate, formattedeDate)
      .subscribe((data) => {
        console.log(data); // Check the response from the server in the browser console
        
        if (data && data.topUsers && Array.isArray(data.topUsers)) {
          this.topUsers = data.topUsers; // Extract the array from the 'topUsers' property
        } else {
          console.error("Invalid 'topUsers' data format:", data);
        }
      });
      this.dashService.getVehicleTotalUserDataByDate(formattdsDate, formattedeDate)
      .subscribe((data) => {
        console.log(data); // Check the response from the server in the browser console
        
        if (data && data.topUsers && Array.isArray(data.topUsers)) {
          this.vehicleTopUsers = data.topUsers; // Extract the array from the 'topUsers' property
        } else {
          console.error("Invalid 'topUsers' data format:", data);
        }
      });
  }
  
}
