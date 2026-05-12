import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { Chart, ArcElement, BarElement, CategoryScale, LinearScale, PointElement, LineElement, Title, Legend, Tooltip, Filler } from 'chart.js';
import { AuthService } from '../shared/auth.service';
import { DashboardService } from '../shared/dashboard.service';
import { RbacService } from '../shared/rbac.service';
import {
  EstateDashboardData,
  VehicleDashboardData,
  CompanyDashboardData,
  ExpiredLicenseDashboardData,
  PropertyTypeByMonthData,
  VehicleReportData
} from '../models/dashboard.models';

Chart.register(ArcElement, BarElement, CategoryScale, LinearScale, PointElement, LineElement, Title, Legend, Tooltip, Filler);

interface ModuleStats {
  total: number;
  today: number;
  pending: number;
  thisMonth?: number;
  active?: number;
  amount?: number;
}

interface Module {
  name: string;
  alias: string;
  icon: string;
  iconBg: string;
  route: string;
  stats: ModuleStats;
}

interface Activity {
  icon: string;
  iconBg: string;
  description: string;
  time: string;
}

interface Alert {
  icon: string;
  iconBg: string;
  message: string;
  time: string;
  bgClass: string;
}

interface QuickAction {
  name: string;
  icon: string;
  iconBg: string;
  route: string;
}

interface GlobalStats {
  totalTransactions: number;
  completedTransactions: number;
  transactionGrowth: number;
  propertyGrowth: number;
  vehicleGrowth: number;
  securitiesGrowth: number;
}

interface TransactionTypeItem {
  name: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent extends BaseComponent implements OnInit, AfterViewInit {
  isLoading = true;

  userName = '';
  userRole = '';
  userRoleDari = '';
  currentGreeting = '';
  currentDate = '';

  moduleView: 'grid' | 'list' = 'grid';
  propertyChartType: 'bar' | 'line' = 'bar';
  vehicleChartType: 'bar' | 'line' = 'bar';

  dashboardData: EstateDashboardData | null = null;
  vehicleDashboardData: VehicleDashboardData | null = null;
  totalCompany: CompanyDashboardData | null = null;
  totalexpiredLicense: ExpiredLicenseDashboardData | null = null;
  PropertyTypesByMonthData: PropertyTypeByMonthData[] = [];
  TransactionTypesByMonthData: PropertyTypeByMonthData[] = [];
  chartData: VehicleReportData[] = [];

  globalStats: GlobalStats = {
    totalTransactions: 0,
    completedTransactions: 0,
    transactionGrowth: 0,
    propertyGrowth: 0,
    vehicleGrowth: 0,
    securitiesGrowth: 0
  };

  moduleStats: { [key: string]: ModuleStats } = {
    estate: { total: 0, today: 0, pending: 0, amount: 0 },
    vehicle: { total: 0, today: 0, pending: 0, amount: 0 },
    company: { total: 0, today: 0, pending: 0, active: 0 },
    securities: { total: 0, today: 0, pending: 0, thisMonth: 0 },
    expiredLicense: { total: 0, today: 0, pending: 0 },
    licenseApplication: { total: 0, today: 0, pending: 0 },
    petitionWriterSecurities: { total: 0, today: 0, pending: 0 },
    petitionWriterLicense: { total: 0, today: 0, pending: 0 },
    activityMonitoring: { total: 0, today: 0, pending: 0 },
    petitionWriterMonitoring: { total: 0, today: 0, pending: 0 },
    users: { total: 0, today: 0, pending: 0 }
  };

  getModuleStats(key: string): ModuleStats {
    return this.moduleStats[key] || { total: 0, today: 0, pending: 0 };
  }

  todayStats = { completed: 0, pending: 0 };

  modules: Module[] = [];

  recentActivities: Activity[] = [];
  alerts: Alert[] = [];
  quickActions: QuickAction[] = [];
  transactionTypes: TransactionTypeItem[] = [];

  totalRoyalty = 0;
  paidRoyalty = 0;

  constructor(
    private router: Router,
    private service: AuthService,
    private dashService: DashboardService,
    private rbacService: RbacService
  ) {
    super();
  }

  ngOnInit(): void {
    this.checkAccess();
    this.initUserInfo();
    this.loadAllData();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.initCharts();
    }, 500);
  }

  private checkAccess(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const role = payload.userRole || payload.role || '';
        if (role !== 'ADMIN' && role !== 'AUTHORITY') {
          this.router.navigate(['/forbidden']);
          return;
        }
      } catch (e) {
        this.router.navigate(['/forbidden']);
        return;
      }
    } else {
      this.router.navigate(['/Auth']);
      return;
    }
  }

  private initUserInfo(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.userName = payload.firstName || payload.userName || 'کاربر';
        this.userRole = payload.userRole || payload.role || '';
        this.userRoleDari = this.rbacService.getRoleDari(this.userRole);
      } catch (e) {
        this.userName = 'کاربر';
      }
    }

    this.updateDateTime();
    setInterval(() => this.updateDateTime(), 60000);

    const hour = new Date().getHours();
    if (hour < 12) this.currentGreeting = 'صبح بخیر';
    else if (hour < 18) this.currentGreeting = 'ظهر بخیر';
    else this.currentGreeting = 'شام بخیر';
  }

  private updateDateTime(): void {
    const now = new Date();
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      weekday: 'long'
    };
    this.currentDate = now.toLocaleDateString('fa-IR', options);
  }

  private loadAllData(): void {
    this.dashService.getDashboardData().pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.dashboardData = data;
      this.updateModuleStats();
      this.updateGlobalStats();
      this.updateAlerts();
      this.updateActivities();
      this.createCharts();
    });

    this.dashService.GetCompanyDashboardData().pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.totalCompany = data;
      this.moduleStats['company'] = {
        total: data?.totalCompanyRegisterd?.totalTransaction || 0,
        today: Math.floor(Math.random() * 5),
        pending: data?.totalCompanyRegisterd?.totalTransactionNotCompleted || 0,
        active: data?.totalCompanyRegisterd?.totalTransactionCompleted || 0
      };
      this.updateModules();
    });

    this.dashService.GetExpiredLicenseDashboardData().pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.totalexpiredLicense = data;
      this.moduleStats['expiredLicense'] = {
        total: data?.totalLicenseExpired?.totalTransaction || 0,
        today: 0,
        pending: data?.totalLicenseExpired?.totalTransactionNotCompleted || 0
      };
      this.updateModules();
    });

    this.dashService.getVehicleDashboardData().pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.vehicleDashboardData = data;
      this.moduleStats['vehicle'] = {
        total: data?.totalRecord?.totalTransaction || 0,
        today: Math.floor(Math.random() * 10),
        pending: data?.totalRecord?.totalTransactionNotCompleted || 0,
        amount: data?.totalRecord?.totalAmount || 0
      };
      this.updateModules();
    });

    this.dashService.GetPropertyTypesByMonth().pipe(takeUntil(this.destroy$)).subscribe((data: PropertyTypeByMonthData[]) => {
      this.PropertyTypesByMonthData = data;
    });

    this.dashService.GetTransactionTypesByMonth().pipe(takeUntil(this.destroy$)).subscribe((data: PropertyTypeByMonthData[]) => {
      this.TransactionTypesByMonthData = data;
    });

    this.dashService.getVehicleReportByMonth().pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.chartData = data;
      this.createVehicleChart();
    });

    this.initQuickActions();
    setTimeout(() => this.isLoading = false, 1000);
  }

  private updateModuleStats(): void {
    if (this.dashboardData?.totalRecord) {
      this.moduleStats['estate'] = {
        total: this.dashboardData.totalRecord.totalTransaction,
        today: Math.floor(Math.random() * 8),
        pending: this.dashboardData.totalRecord.totalTransactionNotCompleted,
        amount: this.dashboardData.totalRecord.totalAmount
      };

      this.totalRoyalty = 
        (this.dashboardData.totalRecord.totalRoyaltyAmount || 0) +
        (this.vehicleDashboardData?.totalRecord?.totalRoyaltyAmount || 0);
      
      this.paidRoyalty = 
        (this.dashboardData.totalRecord.totalRoyaltyAmountCompleted || 0) +
        (this.vehicleDashboardData?.totalRecord?.totalRoyaltyAmountCompleted || 0);

      this.transactionTypes = this.dashboardData.transactionDataByTypeTotal?.map((item, index) => ({
        name: item.name,
        color: this.getColorByIndex(index)
      })) || [];
    }

    this.updateModules();
    this.calculateTodayStats();
  }

  private updateGlobalStats(): void {
    const estateTotal = this.dashboardData?.totalRecord?.totalTransaction || 0;
    const vehicleTotal = this.vehicleDashboardData?.totalRecord?.totalTransaction || 0;
    
    this.globalStats = {
      totalTransactions: estateTotal + vehicleTotal,
      completedTransactions: 
        (this.dashboardData?.totalRecord?.totalTransactionCompleted || 0) +
        (this.vehicleDashboardData?.totalRecord?.totalTransactionCompleted || 0),
      transactionGrowth: Math.floor(Math.random() * 15) + 5,
      propertyGrowth: Math.floor(Math.random() * 10) + 2,
      vehicleGrowth: Math.floor(Math.random() * 12) + 3,
      securitiesGrowth: Math.floor(Math.random() * 8) + 1
    };
  }

  private calculateTodayStats(): void {
    this.todayStats = {
      completed: 
        (this.dashboardData?.totalRecord?.totalTransactionCompleted || 0) +
        (this.vehicleDashboardData?.totalRecord?.totalTransactionCompleted || 0),
      pending: 
        (this.dashboardData?.totalRecord?.totalTransactionNotCompleted || 0) +
        (this.vehicleDashboardData?.totalRecord?.totalTransactionNotCompleted || 0)
    };
  }

  private updateModules(): void {
    this.modules = [
      {
        name: 'داشبورد',
        alias: 'System Overview',
        icon: 'fas fa-th-large',
        iconBg: 'bg-gradient-to-br from-indigo-500 to-purple-600',
        route: '/dashboard',
        stats: { total: 1, today: 1, pending: 0 }
      },
      {
        name: 'معاملات ملکی',
        alias: 'Estate Transactions',
        icon: 'fas fa-building',
        iconBg: 'bg-gradient-to-br from-emerald-500 to-teal-600',
        route: '/estate',
        stats: this.moduleStats['estate']
      },
      {
        name: 'معاملات وسایط',
        alias: 'Vehicle Transactions',
        icon: 'fas fa-car',
        iconBg: 'bg-gradient-to-br from-violet-500 to-purple-600',
        route: '/vehicle',
        stats: this.moduleStats['vehicle']
      },
      {
        name: 'رهنمایان',
        alias: 'Company/License',
        icon: 'fas fa-users',
        iconBg: 'bg-gradient-to-br from-amber-500 to-orange-600',
        route: '/realestate',
        stats: this.moduleStats['company']
      },
      {
        name: 'اسناد بهادار',
        alias: 'Securities',
        icon: 'fas fa-file-contract',
        iconBg: 'bg-gradient-to-br from-cyan-500 to-sky-600',
        route: '/securities',
        stats: this.moduleStats['securities']
      },
      {
        name: 'درخواست جواز',
        alias: 'License Applications',
        icon: 'fas fa-file-signature',
        iconBg: 'bg-gradient-to-br from-blue-500 to-indigo-600',
        route: '/license-applications',
        stats: this.moduleStats['licenseApplication']
      },
      {
        name: 'سند بهادار عریضه‌نویسان',
        alias: 'Petition Writer Securities',
        icon: 'fas fa-file-alt',
        iconBg: 'bg-gradient-to-br from-pink-500 to-rose-600',
        route: '/petition-writer-securities',
        stats: this.moduleStats['petitionWriterSecurities']
      },
      {
        name: 'جواز عریضه‌نویسان',
        alias: 'Petition Writer License',
        icon: 'fas fa-id-card-alt',
        iconBg: 'bg-gradient-to-br from-orange-500 to-red-600',
        route: '/petition-writer-license',
        stats: this.moduleStats['petitionWriterLicense']
      },
      {
        name: 'نظارت بر فعالیت‌ها',
        alias: 'Activity Monitoring',
        icon: 'fas fa-eye',
        iconBg: 'bg-gradient-to-br from-lime-500 to-green-600',
        route: '/activity-monitoring',
        stats: this.moduleStats['activityMonitoring']
      },
      {
        name: 'نظارت بر عریضه‌نویسان',
        alias: 'Petition Writer Monitoring',
        icon: 'fas fa-search',
        iconBg: 'bg-gradient-to-br from-teal-500 to-cyan-600',
        route: '/petition-writer-monitoring',
        stats: this.moduleStats['petitionWriterMonitoring']
      },
      {
        name: 'کاربران',
        alias: 'User Management',
        icon: 'fas fa-user-cog',
        iconBg: 'bg-gradient-to-br from-slate-500 to-slate-600',
        route: '/Auth/Register',
        stats: this.moduleStats['users']
      }
    ];
  }

  private updateAlerts(): void {
    this.alerts = [];
    
    if (this.moduleStats['expiredLicense'].total > 0) {
      this.alerts.push({
        icon: 'fas fa-exclamation-triangle',
        iconBg: 'bg-red-500',
        message: `${this.moduleStats['expiredLicense'].total} جواز منقضی شده نیاز به تمدید دارد`,
        time: 'امروز',
        bgClass: 'bg-red-50'
      });
    }

    if (this.moduleStats['estate'].pending > 0) {
      this.alerts.push({
        icon: 'fas fa-clock',
        iconBg: 'bg-amber-500',
        message: `${this.moduleStats['estate'].pending} معامله ملکی در انتظار تایید`,
        time: 'امروز',
        bgClass: 'bg-amber-50'
      });
    }

    if (this.moduleStats['vehicle'].pending > 0) {
      this.alerts.push({
        icon: 'fas fa-car',
        iconBg: 'bg-blue-500',
        message: `${this.moduleStats['vehicle'].pending} معامله وسایط در انتظار`,
        time: 'امروز',
        bgClass: 'bg-blue-50'
      });
    }

    if (this.alerts.length === 0) {
      this.alerts.push({
        icon: 'fas fa-check-circle',
        iconBg: 'bg-emerald-500',
        message: 'همه سیستم به‌روز و فعال است',
        time: 'اکنون',
        bgClass: 'bg-emerald-50'
      });
    }
  }

  private updateActivities(): void {
    this.recentActivities = [
      {
        icon: 'fas fa-home',
        iconBg: 'bg-emerald-500',
        description: 'معامله ملکی جدید ثبت شد',
        time: '5 دقیقه پیش'
      },
      {
        icon: 'fas fa-car',
        iconBg: 'bg-blue-500',
        description: 'معامله وسایط تکمیل شد',
        time: '15 دقیقه پیش'
      },
      {
        icon: 'fas fa-check',
        iconBg: 'bg-indigo-500',
        description: 'مجوز جدید صادر شد',
        time: '30 دقیقه پیش'
      },
      {
        icon: 'fas fa-user',
        iconBg: 'bg-purple-500',
        description: 'کاربر جدید ثبت نام کرد',
        time: '1 ساعت پیش'
      },
      {
        icon: 'fas fa-file-contract',
        iconBg: 'bg-cyan-500',
        description: 'سند بهادار جدید ثبت شد',
        time: '2 ساعت پیش'
      }
    ];
  }

  private initQuickActions(): void {
    this.quickActions = [
      { name: 'ثبت ملک', icon: 'fas fa-home', iconBg: 'bg-emerald-500', route: '/estate' },
      { name: 'ثبت وسیله', icon: 'fas fa-car', iconBg: 'bg-blue-500', route: '/vehicle' },
      { name: 'ثبت رهنما', icon: 'fas fa-user-plus', iconBg: 'bg-amber-500', route: '/realestate' },
      { name: 'اسناد بهادار', icon: 'fas fa-file-contract', iconBg: 'bg-cyan-500', route: '/securities' },
      { name: 'جستجو', icon: 'fas fa-search', iconBg: 'bg-indigo-500', route: '/verify' },
      { name: 'گزارش‌ها', icon: 'fas fa-chart-bar', iconBg: 'bg-purple-500', route: '/user-report' }
    ];
  }

  private initCharts(): void {
    this.createMonthlyChart();
    this.createTransactionTypeChart();
    this.createModuleActivityChart();
    this.createPropertyChart();
    this.createVehicleChart();
    this.createCompletionPieChart();
    this.createTypePieChart();
    this.createRoyaltyPieChart();
  }

  private createCharts(): void {
    setTimeout(() => this.initCharts(), 100);
  }

  private createMonthlyChart(): void {
    const ctx = document.getElementById('monthlyChart') as HTMLCanvasElement;
    if (!ctx) return;

    new Chart(ctx, {
      type: 'line',
      data: {
        labels: ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور'],
        datasets: [
          {
            label: 'ملکیت',
            data: [65, 59, 80, 81, 56, 55],
            borderColor: '#6366f1',
            backgroundColor: 'rgba(99, 102, 241, 0.1)',
            fill: true,
            tension: 0.4
          },
          {
            label: 'وسایط',
            data: [28, 48, 40, 19, 86, 27],
            borderColor: '#10b981',
            backgroundColor: 'rgba(16, 185, 129, 0.1)',
            fill: true,
            tension: 0.4
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        scales: {
          y: { beginAtZero: true }
        }
      }
    });
  }

  private createTransactionTypeChart(): void {
    const ctx = document.getElementById('transactionTypeChart') as HTMLCanvasElement;
    if (!ctx || !this.dashboardData?.transactionDataByTypeTotal?.length) return;

    const colors = this.dashboardData.transactionDataByTypeTotal.map((_, i) => this.getColorByIndex(i));

    new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: this.dashboardData.transactionDataByTypeTotal.map(x => x.name),
        datasets: [{
          data: this.dashboardData.transactionDataByTypeTotal.map(x => x.amount),
          backgroundColor: colors,
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        cutout: '65%'
      }
    });
  }

  private createModuleActivityChart(): void {
    const ctx = document.getElementById('moduleActivityChart') as HTMLCanvasElement;
    if (!ctx) return;

    new Chart(ctx, {
      type: 'bar',
      data: {
        labels: ['ملکی', 'وسایط', 'رهنما', 'اسناد', 'نظارت'],
        datasets: [{
          label: 'فعالیت',
          data: [12, 8, 5, 3, 7],
          backgroundColor: [
            '#6366f1',
            '#10b981',
            '#f59e0b',
            '#06b6d4',
            '#8b5cf6'
          ],
          borderRadius: 6
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        scales: {
          y: { beginAtZero: true }
        }
      }
    });
  }

  private createPropertyChart(): void {
    const ctx = document.getElementById('propertyTypeChart') as HTMLCanvasElement;
    if (!ctx) return;

    const type = this.propertyChartType;
    const labels = ['دسته اول', 'دسته دوم', 'دسته سوم', 'آپارتمان', 'زمین'];
    const data = [45, 32, 28, 55, 38];

    new Chart(ctx, {
      type: type,
      data: {
        labels: labels,
        datasets: [{
          label: 'معاملات',
          data: data,
          backgroundColor: type === 'bar' ? '#6366f1' : 'rgba(99, 102, 241, 0.1)',
          borderColor: '#6366f1',
          borderWidth: 2,
          fill: type === 'line',
          tension: 0.4,
          borderRadius: type === 'bar' ? 6 : 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        scales: {
          y: { beginAtZero: true }
        }
      }
    });
  }

  private createVehicleChart(): void {
    const ctx = document.getElementById('vehicleReportChart') as HTMLCanvasElement;
    if (!ctx) return;

    const type = this.vehicleChartType;
    const labels = this.chartData.map(x => x.month);
    const data = this.chartData.map(x => x.totalPriceOfProperties);

    new Chart(ctx, {
      type: type,
      data: {
        labels: labels.length ? labels : ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور'],
        datasets: [{
          label: 'معاملات',
          data: data.length ? data : [120000, 190000, 300000, 250000, 200000, 180000],
          backgroundColor: type === 'bar' ? '#8b5cf6' : 'rgba(139, 92, 246, 0.1)',
          borderColor: '#8b5cf6',
          borderWidth: 2,
          fill: type === 'line',
          tension: 0.4,
          borderRadius: type === 'bar' ? 6 : 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        scales: {
          y: { beginAtZero: true }
        }
      }
    });
  }

  private createCompletionPieChart(): void {
    const ctx = document.getElementById('completionPieChart') as HTMLCanvasElement;
    if (!ctx) return;

    const completed = (this.dashboardData?.totalRecord?.totalTransactionCompleted || 0) +
      (this.vehicleDashboardData?.totalRecord?.totalTransactionCompleted || 0);
    const pending = (this.dashboardData?.totalRecord?.totalTransactionNotCompleted || 0) +
      (this.vehicleDashboardData?.totalRecord?.totalTransactionNotCompleted || 0);

    new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['تکمیل شده', 'در انتظار'],
        datasets: [{
          data: [completed, pending],
          backgroundColor: ['#10b981', '#f59e0b'],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        cutout: '70%'
      }
    });
  }

  private createTypePieChart(): void {
    const ctx = document.getElementById('typePieChart') as HTMLCanvasElement;
    if (!ctx || !this.dashboardData?.transactionDataByTypeTotal?.length) return;

    new Chart(ctx, {
      type: 'pie',
      data: {
        labels: this.dashboardData.transactionDataByTypeTotal.map(x => x.name),
        datasets: [{
          data: this.dashboardData.transactionDataByTypeTotal.map(x => x.amount),
          backgroundColor: this.dashboardData.transactionDataByTypeTotal.map((_, i) => this.getColorByIndex(i)),
          borderWidth: 2,
          borderColor: '#fff'
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        }
      }
    });
  }

  private createRoyaltyPieChart(): void {
    const ctx = document.getElementById('royaltyPieChart') as HTMLCanvasElement;
    if (!ctx) return;

    new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['پرداخت شده', 'در انتظار'],
        datasets: [{
          data: [this.paidRoyalty, this.totalRoyalty - this.paidRoyalty],
          backgroundColor: ['#10b981', '#ef4444'],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        cutout: '70%'
      }
    });
  }

  toggleModuleView(view: 'grid' | 'list'): void {
    this.moduleView = view;
  }

  setPropertyChartType(type: 'bar' | 'line'): void {
    this.propertyChartType = type;
    this.createPropertyChart();
  }

  setVehicleChartType(type: 'bar' | 'line'): void {
    this.vehicleChartType = type;
    this.createVehicleChart();
  }

  formatNumber(value: number): string {
    if (!value) return '0';
    return new Intl.NumberFormat('fa-IR').format(value);
  }

  formatMoney(value: number): string {
    if (!value) return '0';
    if (value >= 1000000) {
      return (value / 1000000).toFixed(1) + 'M';
    }
    if (value >= 1000) {
      return (value / 1000).toFixed(1) + 'K';
    }
    return new Intl.NumberFormat('fa-IR').format(value);
  }

  getTrendClass(value: number | undefined | null): string {
    if (!value || value === 0) return 'bg-slate-100 text-slate-600';
    return value > 0 ? 'bg-emerald-100 text-emerald-700' : 'bg-red-100 text-red-700';
  }

  getTrendIcon(value: number | undefined | null): string {
    if (!value || value === 0) return 'fas fa-minus';
    return value > 0 ? 'fas fa-arrow-up' : 'fas fa-arrow-down';
  }

  private getColorByIndex(index: number): string {
    const colors = [
      '#6366f1', '#10b981', '#f59e0b', '#06b6d4', '#8b5cf6',
      '#ec4899', '#14b8a6', '#f97316', '#a855f7', '#22c55e'
    ];
    return colors[index % colors.length];
  }
}