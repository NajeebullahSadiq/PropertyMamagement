import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Chart, registerables, ChartConfiguration } from 'chart.js';
import { AuthService } from '../shared/auth.service';
import { DashboardService } from '../shared/dashboard.service';
Chart.register(...registerables);
interface PropertyTypeData {
  propertyType: string;
  data: PropertyData[];
}

interface PropertyData {
  month: string;
  totalPriceOfProperties: number;
}

interface ChartDataset {
  label: string;
  data: (number | null)[]; // Modified to allow null values for missing data
  backgroundColor: string;
  borderColor: string;
  fill: boolean;
}
interface BackendData {
  propertyType: string;
  data: {
    month: string;
    totalPriceOfProperties: number;
  }[];
}
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
  chart!: Chart<any>;
  chartData: any[] = [];
  userDetails:any=[];
  dashboardData: any=[];
  vehicleDashboardData:any=[];
  PropertyTypesByMonthData:any=[];
  TransactionTypesByMonthData:any=[];
  totalCompany:any=[];
  totalexpiredLicense:any=[];
  constructor(private router: Router, private service: AuthService,private dashService:DashboardService,private http: HttpClient) { }
  ngOnInit(): void {
    this.dashboardData.totalRecord=[];
    this.vehicleDashboardData.totalRecord=[];
    this.totalCompany.totalCompanyRegisterd=0;
    this.dashService.getDashboardData().subscribe(data => {
      this.dashboardData = data;
      this.createPieChart();
      this.createColumnChart();
     
    });
    this.dashService.GetCompanyDashboardData().subscribe(data => {
      this.totalCompany = data;
    });
    this.dashService.GetExpiredLicenseDashboardData().subscribe(data => {
      this.totalexpiredLicense = data;
    });
    this.dashService.getVehicleDashboardData().subscribe(data => {
      this.vehicleDashboardData = data;
    
    });
    this.loadChartData();
    this.loadTransactionChartData();
    this.getDataFromAPI();
  }
  createPieChart() {
    const TotalTransaction = this.dashboardData.transactionDataByTypeTotal;
    const TotalTransactionCompleted = this.dashboardData.transactionDataByTypeCompleted;
    const TotalTransactionNotCompleted=this.dashboardData.transactionDataByTypeNotCompleted;
    const zoneChartElement = document.getElementById('totalchart') as HTMLCanvasElement;
    const zoneChart = new Chart(zoneChartElement, {
      type: 'pie',
      data: {
        labels: TotalTransaction.map((x: { name: any; }) => x.name),
        datasets: [{
          data: TotalTransaction.map((x: { amount: any; }) => x.amount),
        //  backgroundColor: this.generateRandomColors(TotalTransaction.length),
        }]
      }
    });
  
    const serviceTypeChartElement = document.getElementById('total-completed') as HTMLCanvasElement;
    const serviceTypeChart = new Chart(serviceTypeChartElement, {
      type: 'pie',
      data: {
        labels: TotalTransactionCompleted.map((x: { name: any; }) => x.name),
        datasets: [{
          data: TotalTransactionCompleted.map((x: { amount: any; }) => x.amount),
        // backgroundColor: this.generateRandomColors(TotalTransactionCompleted.length),
        }]
      }
    });


    const notcompletedChartElement = document.getElementById('total-notcompleted') as HTMLCanvasElement;
    const notcompletedChart = new Chart(notcompletedChartElement, {
      type: 'pie',
      data: {
        labels: TotalTransactionNotCompleted.map((x: { name: any; }) => x.name),
        datasets: [{
          data: TotalTransactionNotCompleted.map((x: { amount: any; }) => x.amount),
        //  backgroundColor: this.generateRandomColors(TotalTransactionNotCompleted.length),
        }]
      }
    });
  }
  createColumnChart() {
    const TotalTransaction = this.dashboardData.transactionDataByTransactionTypeTotal;
    const zoneChartElement = document.getElementById('totalchart2') as HTMLCanvasElement;
    const zoneChart = new Chart(zoneChartElement, {
      type: 'bar',
      data: {
        labels: TotalTransaction.map((x: { name: any }) => x.name),
        datasets: [{
          label: 'معاملات انجام شده',
          data: TotalTransaction.map((x: { amount: any }) => x.amount),
          backgroundColor: this.generateRandomColors(TotalTransaction.length),
        }]
      }
    });
    //Completed
    const TotalTransactionCompleted = this.dashboardData.transactionDataByTransactionTypeCompleted;
    const completedChartElement = document.getElementById('totalchart3') as HTMLCanvasElement;
    const completedChart = new Chart(completedChartElement, {
      type: 'bar',
      data: {
        labels: TotalTransactionCompleted.map((x: { name: any }) => x.name),
        datasets: [{
          label: 'معاملات تکمیل شده',
          data: TotalTransactionCompleted.map((x: { amount: any }) => x.amount),
       backgroundColor: this.generateRandomColors(TotalTransactionCompleted.length),
        }]
      }
    });
    //not Completed
     //Completed
     const TotalTransactionNotCompleted = this.dashboardData.transactionDataByTransactionTypeNotCompleted;
     const notcompletedChartElement = document.getElementById('totalchart4') as HTMLCanvasElement;
     const notcompletedChart = new Chart(notcompletedChartElement, {
       type: 'bar',
       data: {
         labels: TotalTransactionNotCompleted.map((x: { name: any }) => x.name),
         datasets: [{
           label: 'معاملات تکمیل ناشده',
           data: TotalTransactionNotCompleted.map((x: { amount: any }) => x.amount),
           backgroundColor: this.generateRandomColors(TotalTransactionNotCompleted.length),
         }]
       }
     });
  }

  displayChart(data: BackendData[]): void {
    const ctx = document.getElementById('propertyChart') as HTMLCanvasElement;
  
    // Extract unique property types
    const propertyTypes = data.map(d => d.propertyType);
  
    // Extract all months
    const months = data.reduce<string[]>((acc, d) => {
      d.data.forEach(entry => {
        if (!acc.includes(entry.month)) {
          acc.push(entry.month);
        }
      });
      return acc;
    }, []);
  
    // Prepare the chart datasets
    const datasets: ChartDataset[] = propertyTypes.map(propertyType => {
      const propertyTypeData = data.find(d => d.propertyType === propertyType);
      const dataPoints: (number | null)[] = months.map(month => {
        const propertyData = propertyTypeData?.data.find(d => d.month === month);
        return propertyData ? propertyData.totalPriceOfProperties : null;
      });
  
      return {
        label: propertyType,
        data: dataPoints,
        backgroundColor: '',
        borderColor: '',
        fill: false,
      };
    });
  
    this.chart = new Chart(ctx, {
      type: 'bar', // You can choose a different chart type here, e.g., line, pie, etc.
      data: {
        labels: months,
        datasets,
      },
      options: {
        responsive: true,
        scales: {
          x: {
            stacked: true,
          },
          y: {
            stacked: true,
          },
        },
      },
    });
  }
  displayTransactionChart(data: BackendData[]): void {
    const ctx = document.getElementById('transactionChart') as HTMLCanvasElement;
  
    // Extract unique property types
    const propertyTypes = data.map(d => d.propertyType);
  
    // Extract all months
    const months = data.reduce<string[]>((acc, d) => {
      d.data.forEach(entry => {
        if (!acc.includes(entry.month)) {
          acc.push(entry.month);
        }
      });
      return acc;
    }, []);
  
    // Prepare the chart datasets
    const datasets: ChartDataset[] = propertyTypes.map(propertyType => {
      const propertyTypeData = data.find(d => d.propertyType === propertyType);
      const dataPoints: (number | null)[] = months.map(month => {
        const propertyData = propertyTypeData?.data.find(d => d.month === month);
        return propertyData ? propertyData.totalPriceOfProperties : null;
      });
  
      return {
        label: propertyType,
        data: dataPoints,
        backgroundColor: '',
         borderColor: '',
        fill: false,
      };
    });
  
    this.chart = new Chart(ctx, {
      type: 'line', // You can choose a different chart type here, e.g., line, pie, etc.
      data: {
        labels: months,
        datasets,
      },
      options: {
        responsive: true,
        scales: {
          x: {
            stacked: true,
          },
          y: {
            stacked: true,
          },
        },
      },
    });
  }
  createVehicleReportChart() {
    const months = this.chartData.map((item) => item.month);
    const prices = this.chartData.map((item) => item.totalPriceOfProperties);

    const ctx = document.getElementById('myChart') as HTMLCanvasElement;
    const myChart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: months,
        datasets: [
          {
            label: 'خرید و فروش موتر در مقطع زمانی مختلف',
            data: prices,
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1,
          },
        ],
      },
      options: {
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    });
  }
  createVehicleReportbarChart() {
    const months = this.chartData.map((item) => item.month);
    const prices = this.chartData.map((item) => item.totalPriceOfProperties);

    const ctx = document.getElementById('vChart') as HTMLCanvasElement;
    const myChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: months,
        datasets: [
          {
            label: 'خرید و فروش موتر در مقطع زمانی مختلف',
            data: prices,
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1,
          },
        ],
      },
      options: {
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    });
  }
  getDataFromAPI() {
    this.http
      .get<any[]>('http://localhost:5143/api/Dashboard/GetVehicleReportByMonth') // Replace with your API endpoint
      .subscribe((data) => {
        this.chartData = data;
        this.createVehicleReportChart();
        this.createVehicleReportbarChart()
      });
  }
  loadChartData(): void {
    // Make an HTTP GET request to your ASP.NET Core Web API endpoint
    this.http.get<BackendData[]>('http://localhost:5143/api/Dashboard/GetPropertyTypesByMonth').subscribe((data: BackendData[]) => {
      this.displayChart(data);
    });
  }
  loadTransactionChartData(): void {
    // Make an HTTP GET request to your ASP.NET Core Web API endpoint
    this.http.get<BackendData[]>('http://localhost:5143/api/Dashboard/GetTransactionTypesByMonth').subscribe((data: BackendData[]) => {
      this.displayTransactionChart(data);
    });
  }
  generateRandomColors(count: number): string[] {
    const colors = [];
    for (let i = 0; i < count; i++) {
      const color = this.getRandomColor();
      colors.push(color);
    }
    return colors;
  }
 getRandomColor(): string {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }
}
