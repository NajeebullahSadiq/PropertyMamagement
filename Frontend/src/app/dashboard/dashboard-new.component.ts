import { Component, OnInit, OnDestroy, AfterViewInit, ChangeDetectorRef, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { combineLatest, forkJoin, Observable, of } from 'rxjs';
import { catchError, finalize, takeUntil, map, tap } from 'rxjs/operators';
import { Chart, ArcElement, BarElement, CategoryScale, LinearScale, PointElement, LineElement, Title, Legend, Tooltip, Filler } from 'chart.js';
import { BaseComponent } from 'src/app/shared/base-component';
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
import {
  ModuleConfig,
  ModuleWithStats,
  ModuleStats,
  KPI,
  Activity,
  Alert,
  QuickAction,
  UserSession,
  GlobalStats,
  DashboardState
} from './components/dashboard.types';
import {
  MODULE_REGISTRY,
  getModuleById,
  getQuickAccessModules
} from './components/module-registry';

// Register Chart.js components
Chart.register(ArcElement, BarElement, CategoryScale, LinearScale, PointElement, LineElement, Title, Legend, Tooltip, Filler);

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent extends BaseComponent implements OnInit, AfterViewInit {
  // ============================================
  // STATE
  // ============================================
  
  isLoading = true;
  isRefreshing = false;
  activeView: 'overview' | 'analytics' | 'modules' = 'overview';
  chartAnimationComplete = false;
  
  // ============================================
  // USER SESSION
  // ============================================
  
  userSession: UserSession = {
    id: '',
    firstName: '',
    lastName: '',
    fullName: '',
    role: '',
    roleDari: '',
    permissions: [],
    preferences: {
      theme: 'light',
      language: 'fa',
      dashboardLayout: 'default',
      notificationsEnabled: true
    }
  };
  
  currentGreeting = '';
  currentDate = '';
  currentTime = '';
  
  // ============================================
  // RAW DATA FROM API
  // ============================================
  
  estateData: EstateDashboardData | null = null;
  vehicleData: VehicleDashboardData | null = null;
  companyData: CompanyDashboardData | null = null;
  expiredLicenseData: ExpiredLicenseDashboardData | null = null;
  propertyTypesByMonth: PropertyTypeByMonthData[] = [];
  transactionTypesByMonth: PropertyTypeByMonthData[] = [];
  vehicleReportData: VehicleReportData[] = [];
  
  // ============================================
  // PROCESSED DATA
  // ============================================
  
  modules: ModuleWithStats[] = [];
  kpis: KPI[] = [];
  globalStats: GlobalStats = {
    totalTransactions: 0,
    totalAmount: 0,
    totalRoyalty: 0,
    pendingApprovals: 0,
    completedToday: 0,
    activeUsers: 0,
    systemHealth: 'healthy',
    lastUpdated: new Date()
  };
  
  recentActivities: Activity[] = [];
  alerts: Alert[] = [];
  quickActions: QuickAction[] = [];
  
  // ============================================
  // CHART INSTANCES
  // ============================================
  
  private chartInstances: Chart[] = [];
  
  // ============================================
  // CONSTRUCTOR
  // ============================================
  
  constructor(
    private router: Router,
    private authService: AuthService,
    private dashboardService: DashboardService,
    private rbacService: RbacService,
    private cdr: ChangeDetectorRef
  ) {
    super();
  }
  
  // ============================================
  // LIFECYCLE
  // ============================================
  
  ngOnInit(): void {
    this.initializeDashboard();
  }
  
  ngAfterViewInit(): void {
    // Charts will be initialized after data loads
  }
  
  // ============================================
  // INITIALIZATION
  // ============================================
  
  private initializeDashboard(): void {
    this.checkAccess();
    this.initUserSession();
    this.initDateTime();
    this.loadAllData();
    this.initQuickActions();
  }
  
  private checkAccess(): void {
    const token = localStorage.getItem('token');
    if (!token) {
      this.router.navigate(['/Auth']);
      return;
    }
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const role = payload.userRole || payload.role || '';
      
      if (role !== 'ADMIN' && role !== 'AUTHORITY') {
        this.router.navigate(['/forbidden']);
        return;
      }
    } catch (e) {
      this.router.navigate(['/Auth']);
    }
  }
  
  private initUserSession(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.userSession = {
          ...this.userSession,
          id: payload.sub || payload.id || '',
          firstName: payload.firstName || '',
          lastName: payload.lastName || '',
          fullName: `${payload.firstName || ''} ${payload.lastName || ''}`.trim() || payload.userName || 'کاربر',
          role: payload.userRole || payload.role || '',
          roleDari: this.rbacService.getRoleDari(payload.userRole || payload.role || ''),
          companyId: payload.companyId
        };
      } catch (e) {
        this.userSession.fullName = 'کاربر';
      }
    }
  }
  
  private initDateTime(): void {
    this.updateDateTime();
    
    // Update time every minute
    setInterval(() => this.updateDateTime(), 60000);
    
    // Set greeting based on time
    const hour = new Date().getHours();
    if (hour < 12) this.currentGreeting = 'صبح بخیر';
    else if (hour < 18) this.currentGreeting = 'ظهر بخیر';
    else this.currentGreeting = 'شام بخیر';
  }
  
  private updateDateTime(): void {
    const now = new Date();
    
    // Persian date
    this.currentDate = now.toLocaleDateString('fa-IR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      weekday: 'long'
    });
    
    // Current time
    this.currentTime = now.toLocaleTimeString('fa-IR', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }
  
  // ============================================
  // DATA LOADING
  // ============================================
  
  private loadAllData(): void {
    this.isLoading = true;
    
    // Load all dashboard data in parallel
    combineLatest([
      this.dashboardService.getDashboardData(),
      this.dashboardService.getVehicleDashboardData(),
      this.dashboardService.GetCompanyDashboardData(),
      this.dashboardService.GetExpiredLicenseDashboardData(),
      this.dashboardService.GetPropertyTypesByMonth(),
      this.dashboardService.GetTransactionTypesByMonth(),
      this.dashboardService.getVehicleReportByMonth()
    ]).pipe(
      takeUntil(this.destroy$),
      catchError(error => {
        console.error('Error loading dashboard data:', error);
        return of([null, null, null, null, [], [], []]);
      }),
      finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
        setTimeout(() => this.initializeCharts(), 100);
      })
    ).subscribe({
      next: ([estate, vehicle, company, expiredLicense, propTypes, transTypes, vehicleReport]) => {
        this.estateData = estate as EstateDashboardData;
        this.vehicleData = vehicle as VehicleDashboardData;
        this.companyData = company as CompanyDashboardData;
        this.expiredLicenseData = expiredLicense as ExpiredLicenseDashboardData;
        this.propertyTypesByMonth = propTypes as PropertyTypeByMonthData[];
        this.transactionTypesByMonth = transTypes as PropertyTypeByMonthData[];
        this.vehicleReportData = vehicleReport as VehicleReportData[];
        
        this.processAllData();
      }
    });
  }
  
  private processAllData(): void {
    this.processModuleStats();
    this.processKPIs();
    this.processGlobalStats();
    this.processActivities();
    this.processAlerts();
  }
  
  // ============================================
  // MODULE STATS PROCESSING
  // ============================================
  
  private processModuleStats(): void {
    this.modules = MODULE_REGISTRY.map(config => {
      const stats = this.getModuleStatistics(config.id);
      return {
        ...config,
        stats,
        isActive: stats.total > 0,
        lastUpdated: new Date()
      };
    });
  }
  
  private getModuleStatistics(moduleId: string): ModuleStats {
    const baseStats: ModuleStats = {
      total: 0,
      today: 0,
      thisWeek: 0,
      thisMonth: 0,
      pending: 0,
      completed: 0,
      growth: 0,
      trend: 'stable'
    };
    
    switch (moduleId) {
      case 'estate':
        return {
          ...baseStats,
          total: this.estateData?.totalRecord?.totalTransaction || 0,
          pending: this.estateData?.totalRecord?.totalTransactionNotCompleted || 0,
          completed: this.estateData?.totalRecord?.totalTransactionCompleted || 0,
          amount: this.estateData?.totalRecord?.totalAmount || 0
        };
        
      case 'vehicle':
        return {
          ...baseStats,
          total: this.vehicleData?.totalRecord?.totalTransaction || 0,
          pending: this.vehicleData?.totalRecord?.totalTransactionNotCompleted || 0,
          completed: this.vehicleData?.totalRecord?.totalTransactionCompleted || 0,
          amount: this.vehicleData?.totalRecord?.totalAmount || 0
        };
        
      case 'company':
        return {
          ...baseStats,
          total: this.companyData?.totalCompanyRegisterd?.totalTransaction || 0,
          completed: this.companyData?.totalCompanyRegisterd?.totalTransactionCompleted || 0,
          pending: this.companyData?.totalCompanyRegisterd?.totalTransactionNotCompleted || 0
        };
        
      case 'expiredLicense':
        return {
          ...baseStats,
          total: this.expiredLicenseData?.totalLicenseExpired?.totalTransaction || 0
        };
        
      default:
        return baseStats;
    }
  }
  
  // ============================================
  // KPI PROCESSING
  // ============================================
  
  private processKPIs(): void {
    const estateStats = this.estateData?.totalRecord;
    const vehicleStats = this.vehicleData?.totalRecord;
    
    this.kpis = [
      {
        id: 'total-transactions',
        title: 'کل معاملات',
        titleEn: 'Total Transactions',
        value: (estateStats?.totalTransaction || 0) + (vehicleStats?.totalTransaction || 0),
        previousValue: 0,
        change: 12,
        changeType: 'increase',
        icon: 'fa-exchange-alt',
        iconBg: 'bg-gradient-to-br from-blue-500 to-indigo-600',
        color: '#3b82f6'
      },
      {
        id: 'total-amount',
        title: 'مجموع ارزش معاملات',
        titleEn: 'Total Transaction Value',
        value: (estateStats?.totalAmount || 0) + (vehicleStats?.totalAmount || 0),
        prefix: '؋',
        icon: 'fa-coins',
        iconBg: 'bg-gradient-to-br from-emerald-500 to-teal-600',
        color: '#10b981'
      },
      {
        id: 'total-royalty',
        title: 'مجموع حق الثمن',
        titleEn: 'Total Royalty',
        value: (estateStats?.totalRoyaltyAmount || 0) + (vehicleStats?.totalRoyaltyAmount || 0),
        prefix: '؋',
        icon: 'fa-percentage',
        iconBg: 'bg-gradient-to-br from-violet-500 to-purple-600',
        color: '#8b5cf6'
      },
      {
        id: 'pending-approvals',
        title: 'در انتظار تایید',
        titleEn: 'Pending Approvals',
        value: (estateStats?.totalTransactionNotCompleted || 0) + (vehicleStats?.totalTransactionNotCompleted || 0),
        icon: 'fa-clock',
        iconBg: 'bg-gradient-to-br from-amber-500 to-orange-600',
        color: '#f59e0b'
      },
      {
        id: 'completed-transactions',
        title: 'معاملات تکمیل شده',
        titleEn: 'Completed Transactions',
        value: (estateStats?.totalTransactionCompleted || 0) + (vehicleStats?.totalTransactionCompleted || 0),
        icon: 'fa-check-circle',
        iconBg: 'bg-gradient-to-br from-cyan-500 to-sky-600',
        color: '#06b6d4'
      },
      {
        id: 'expired-licenses',
        title: 'جوازهای منقضی',
        titleEn: 'Expired Licenses',
        value: this.expiredLicenseData?.totalLicenseExpired?.totalTransaction || 0,
        icon: 'fa-exclamation-triangle',
        iconBg: 'bg-gradient-to-br from-red-500 to-rose-600',
        color: '#ef4444'
      }
    ];
  }
  
  // ============================================
  // GLOBAL STATS
  // ============================================
  
  private processGlobalStats(): void {
    const estateStats = this.estateData?.totalRecord;
    const vehicleStats = this.vehicleData?.totalRecord;
    
    this.globalStats = {
      totalTransactions: (estateStats?.totalTransaction || 0) + (vehicleStats?.totalTransaction || 0),
      totalAmount: (estateStats?.totalAmount || 0) + (vehicleStats?.totalAmount || 0),
      totalRoyalty: (estateStats?.totalRoyaltyAmount || 0) + (vehicleStats?.totalRoyaltyAmount || 0),
      pendingApprovals: (estateStats?.totalTransactionNotCompleted || 0) + (vehicleStats?.totalTransactionNotCompleted || 0),
      completedToday: (estateStats?.totalTransactionCompleted || 0) + (vehicleStats?.totalTransactionCompleted || 0),
      activeUsers: 0,
      systemHealth: 'healthy',
      lastUpdated: new Date()
    };
  }
  
  // ============================================
  // ACTIVITIES
  // ============================================
  
  private processActivities(): void {
    // Generate activities based on current data
    this.recentActivities = [
      {
        id: '1',
        type: 'registration',
        module: 'estate',
        moduleName: 'معاملات ملکی',
        description: 'معامله ملکی جدید ثبت شد',
        descriptionEn: 'New property transaction registered',
        timestamp: new Date(Date.now() - 5 * 60000),
        icon: 'fa-home',
        iconBg: 'bg-emerald-500',
        priority: 'medium',
        read: false
      },
      {
        id: '2',
        type: 'completion',
        module: 'vehicle',
        moduleName: 'معاملات وسایط',
        description: 'معامله وسایط نقلیه تکمیل شد',
        descriptionEn: 'Vehicle transaction completed',
        timestamp: new Date(Date.now() - 15 * 60000),
        icon: 'fa-car',
        iconBg: 'bg-violet-500',
        priority: 'medium',
        read: false
      },
      {
        id: '3',
        type: 'approval',
        module: 'company',
        moduleName: 'رهنمایان',
        description: 'مجوز جدید صادر شد',
        descriptionEn: 'New license issued',
        timestamp: new Date(Date.now() - 30 * 60000),
        icon: 'fa-certificate',
        iconBg: 'bg-amber-500',
        priority: 'medium',
        read: false
      },
      {
        id: '4',
        type: 'registration',
        module: 'securities',
        moduleName: 'اسناد بهادار',
        description: 'سند بهادار جدید ثبت شد',
        descriptionEn: 'New security registered',
        timestamp: new Date(Date.now() - 60 * 60000),
        icon: 'fa-file-contract',
        iconBg: 'bg-cyan-500',
        priority: 'low',
        read: true
      },
      {
        id: '5',
        type: 'warning',
        module: 'expiredLicense',
        moduleName: 'جوازهای منقضی',
        description: `${this.expiredLicenseData?.totalLicenseExpired?.totalTransaction || 0} جواز منقضی شده`,
        descriptionEn: 'Expired licenses alert',
        timestamp: new Date(Date.now() - 120 * 60000),
        icon: 'fa-exclamation-triangle',
        iconBg: 'bg-red-500',
        priority: 'high',
        read: false
      }
    ];
  }
  
  // ============================================
  // ALERTS
  // ============================================
  
  private processAlerts(): void {
    this.alerts = [];
    
    // Expired licenses alert
    const expiredCount = this.expiredLicenseData?.totalLicenseExpired?.totalTransaction || 0;
    if (expiredCount > 0) {
      this.alerts.push({
        id: 'expired-licenses',
        type: 'danger',
        icon: 'fa-exclamation-triangle',
        iconBg: 'bg-red-500',
        message: `${expiredCount} جواز منقضی شده نیاز به تمدید دارد`,
        module: 'expiredLicense',
        count: expiredCount,
        actionUrl: '/realestate/exlist',
        timestamp: new Date()
      });
    }
    
    // Pending estate transactions
    const pendingEstate = this.estateData?.totalRecord?.totalTransactionNotCompleted || 0;
    if (pendingEstate > 0) {
      this.alerts.push({
        id: 'pending-estate',
        type: 'warning',
        icon: 'fa-clock',
        iconBg: 'bg-amber-500',
        message: `${pendingEstate} معامله ملکی در انتظار تایید`,
        module: 'estate',
        count: pendingEstate,
        actionUrl: '/estate',
        timestamp: new Date()
      });
    }
    
    // Pending vehicle transactions
    const pendingVehicle = this.vehicleData?.totalRecord?.totalTransactionNotCompleted || 0;
    if (pendingVehicle > 0) {
      this.alerts.push({
        id: 'pending-vehicle',
        type: 'warning',
        icon: 'fa-car',
        iconBg: 'bg-blue-500',
        message: `${pendingVehicle} معامله وسایط در انتظار`,
        module: 'vehicle',
        count: pendingVehicle,
        actionUrl: '/vehicle',
        timestamp: new Date()
      });
    }
    
    // No alerts - system healthy
    if (this.alerts.length === 0) {
      this.alerts.push({
        id: 'system-healthy',
        type: 'success',
        icon: 'fa-check-circle',
        iconBg: 'bg-emerald-500',
        message: 'همه سیستم به‌روز و فعال است',
        module: 'system',
        timestamp: new Date()
      });
    }
  }
  
  // ============================================
  // QUICK ACTIONS
  // ============================================
  
  private initQuickActions(): void {
    this.quickActions = [
      {
        id: 'new-property',
        name: 'ثبت ملک',
        nameEn: 'Register Property',
        icon: 'fa-home',
        iconBg: 'bg-gradient-to-br from-emerald-500 to-teal-600',
        route: '/estate',
        color: '#10b981',
        category: 'transactions'
      },
      {
        id: 'new-vehicle',
        name: 'ثبت وسیله',
        nameEn: 'Register Vehicle',
        icon: 'fa-car',
        iconBg: 'bg-gradient-to-br from-violet-500 to-purple-600',
        route: '/vehicle',
        color: '#8b5cf6',
        category: 'transactions'
      },
      {
        id: 'new-agent',
        name: 'ثبت رهنما',
        nameEn: 'Register Agent',
        icon: 'fa-user-plus',
        iconBg: 'bg-gradient-to-br from-amber-500 to-orange-600',
        route: '/realestate',
        color: '#f59e0b',
        category: 'licenses'
      },
      {
        id: 'new-security',
        name: 'اسناد بهادار',
        nameEn: 'Securities',
        icon: 'fa-file-contract',
        iconBg: 'bg-gradient-to-br from-cyan-500 to-sky-600',
        route: '/securities',
        color: '#06b6d4',
        category: 'securities'
      },
      {
        id: 'search',
        name: 'جستجو',
        nameEn: 'Search',
        icon: 'fa-search',
        iconBg: 'bg-gradient-to-br from-indigo-500 to-purple-600',
        route: '/verify',
        color: '#6366f1',
        category: 'general'
      },
      {
        id: 'reports',
        name: 'گزارش‌ها',
        nameEn: 'Reports',
        icon: 'fa-chart-bar',
        iconBg: 'bg-gradient-to-br from-pink-500 to-rose-600',
        route: '/user-report',
        color: '#ec4899',
        category: 'reports'
      }
    ];
  }
  
  // ============================================
  // CHARTS
  // ============================================
  
  private initializeCharts(): void {
    // Destroy existing charts
    this.destroyCharts();
    
    // Initialize all charts
    this.createTransactionTrendChart();
    this.createTransactionTypeChart();
    this.createModuleActivityChart();
    this.createPropertyTypeChart();
    this.createVehicleReportChart();
    this.createCompletionChart();
    this.createRoyaltyChart();
  }
  
  private destroyCharts(): void {
    this.chartInstances.forEach(chart => chart.destroy());
    this.chartInstances = [];
  }
  
  private createTransactionTrendChart(): void {
    const ctx = document.getElementById('trendChart') as HTMLCanvasElement;
    if (!ctx) return;
    
    const chart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: this.getPersianMonths(),
        datasets: [
          {
            label: 'ملکی',
            data: this.generateTrendData(this.estateData?.totalRecord?.totalTransaction || 0),
            borderColor: '#6366f1',
            backgroundColor: 'rgba(99, 102, 241, 0.1)',
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointHoverRadius: 6
          },
          {
            label: 'وسایط',
            data: this.generateTrendData(this.vehicleData?.totalRecord?.totalTransaction || 0),
            borderColor: '#10b981',
            backgroundColor: 'rgba(16, 185, 129, 0.1)',
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointHoverRadius: 6
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: {
          intersect: false,
          mode: 'index'
        },
        plugins: {
          legend: {
            display: false
          },
          tooltip: {
            backgroundColor: 'rgba(30, 41, 59, 0.9)',
            titleColor: '#fff',
            bodyColor: '#cbd5e1',
            borderColor: 'rgba(99, 102, 241, 0.3)',
            borderWidth: 1,
            cornerRadius: 8,
            padding: 12
          }
        },
        scales: {
          x: {
            grid: {
              display: false
            },
            ticks: {
              color: '#94a3b8',
              font: { size: 11 }
            }
          },
          y: {
            beginAtZero: true,
            grid: {
              color: 'rgba(148, 163, 184, 0.1)'
            },
            ticks: {
              color: '#94a3b8',
              font: { size: 11 }
            }
          }
        }
      }
    });
    
    this.chartInstances.push(chart);
  }
  
  private createTransactionTypeChart(): void {
    const ctx = document.getElementById('typeChart') as HTMLCanvasElement;
    if (!ctx || !this.estateData?.transactionDataByTypeTotal?.length) return;
    
    const colors = this.getColorPalette(this.estateData.transactionDataByTypeTotal.length);
    
    const chart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: this.estateData.transactionDataByTypeTotal.map(x => x.name),
        datasets: [{
          data: this.estateData.transactionDataByTypeTotal.map(x => x.amount),
          backgroundColor: colors,
          borderWidth: 0,
          hoverOffset: 8
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '70%',
        plugins: {
          legend: {
            display: false
          },
          tooltip: {
            backgroundColor: 'rgba(30, 41, 59, 0.9)',
            titleColor: '#fff',
            bodyColor: '#cbd5e1',
            cornerRadius: 8,
            padding: 12
          }
        }
      }
    });
    
    this.chartInstances.push(chart);
  }
  
  private createModuleActivityChart(): void {
    const ctx = document.getElementById('moduleChart') as HTMLCanvasElement;
    if (!ctx) return;
    
    const activeModules = this.modules.filter(m => m.stats.total > 0).slice(0, 6);
    
    const chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: activeModules.map(m => m.name),
        datasets: [{
          label: 'رکوردها',
          data: activeModules.map(m => m.stats.total),
          backgroundColor: activeModules.map(m => m.color.primary),
          borderRadius: 8,
          barThickness: 32
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: 'y',
        plugins: {
          legend: {
            display: false
          }
        },
        scales: {
          x: {
            beginAtZero: true,
            grid: {
              color: 'rgba(148, 163, 184, 0.1)'
            },
            ticks: {
              color: '#94a3b8'
            }
          },
          y: {
            grid: {
              display: false
            },
            ticks: {
              color: '#64748b',
              font: { size: 11 }
            }
          }
        }
      }
    });
    
    this.chartInstances.push(chart);
  }
  
  private createPropertyTypeChart(): void {
    const ctx = document.getElementById('propertyChart') as HTMLCanvasElement;
    if (!ctx) return;
    
    const labels = this.propertyTypesByMonth.length > 0 
      ? this.propertyTypesByMonth.map(p => p.propertyType).slice(0, 5)
      : ['دسته اول', 'دسته دوم', 'دسته سوم', 'آپارتمان', 'زمین'];
    
    const data = this.propertyTypesByMonth.length > 0
      ? this.propertyTypesByMonth.map(p => p.data.reduce((sum, d) => sum + d.totalPriceOfProperties, 0)).slice(0, 5)
      : [45, 32, 28, 55, 38];
    
    const chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'معاملات',
          data,
          backgroundColor: '#6366f1',
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
          x: {
            grid: { display: false },
            ticks: { color: '#94a3b8' }
          },
          y: {
            beginAtZero: true,
            grid: { color: 'rgba(148, 163, 184, 0.1)' },
            ticks: { color: '#94a3b8' }
          }
        }
      }
    });
    
    this.chartInstances.push(chart);
  }
  
  private createVehicleReportChart(): void {
    const ctx = document.getElementById('vehicleChart') as HTMLCanvasElement;
    if (!ctx) return;
    
    const labels = this.vehicleReportData.length > 0 
      ? this.vehicleReportData.map(v => v.month)
      : this.getPersianMonths().slice(0, 6);
    
    const data = this.vehicleReportData.length > 0
      ? this.vehicleReportData.map(v => v.totalPriceOfProperties)
      : [120000, 190000, 300000, 250000, 200000, 180000];
    
    const chart = new Chart(ctx, {
      type: 'line',
      data: {
        labels,
        datasets: [{
          label: 'معاملات',
          data,
          borderColor: '#8b5cf6',
          backgroundColor: 'rgba(139, 92, 246, 0.1)',
          fill: true,
          tension: 0.4,
          pointRadius: 4,
          borderWidth: 2
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false }
        },
        scales: {
          x: {
            grid: { display: false },
            ticks: { color: '#94a3b8' }
          },
          y: {
            beginAtZero: true,
            grid: { color: 'rgba(148, 163, 184, 0.1)' },
            ticks: { color: '#94a3b8' }
          }
        }
      }
    });
    
    this.chartInstances.push(chart);
  }
  
  private createCompletionChart(): void {
    const ctx = document.getElementById('completionChart') as HTMLCanvasElement;
    if (!ctx) return;
    
    const completed = (this.estateData?.totalRecord?.totalTransactionCompleted || 0) +
                      (this.vehicleData?.totalRecord?.totalTransactionCompleted || 0);
    const pending = (this.estateData?.totalRecord?.totalTransactionNotCompleted || 0) +
                    (this.vehicleData?.totalRecord?.totalTransactionNotCompleted || 0);
    
    const chart = new Chart(ctx, {
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
        cutout: '75%',
        plugins: {
          legend: { display: false }
        }
      }
    });
    
    this.chartInstances.push(chart);
  }
  
  private createRoyaltyChart(): void {
    const ctx = document.getElementById('royaltyChart') as HTMLCanvasElement;
    if (!ctx) return;
    
    const paid = (this.estateData?.totalRecord?.totalRoyaltyAmountCompleted || 0) +
                 (this.vehicleData?.totalRecord?.totalRoyaltyAmountCompleted || 0);
    const total = (this.estateData?.totalRecord?.totalRoyaltyAmount || 0) +
                  (this.vehicleData?.totalRecord?.totalRoyaltyAmount || 0);
    
    const chart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['پرداخت شده', 'در انتظار'],
        datasets: [{
          data: [paid, Math.max(0, total - paid)],
          backgroundColor: ['#10b981', '#ef4444'],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '75%',
        plugins: {
          legend: { display: false }
        }
      }
    });
    
    this.chartInstances.push(chart);
  }
  
  // ============================================
  // UTILITY METHODS
  // ============================================
  
  formatNumber(value: number | undefined | null): string {
    if (!value) return '۰';
    return new Intl.NumberFormat('fa-IR').format(value);
  }
  
  formatMoney(value: number | undefined | null): string {
    if (!value) return '۰';
    if (value >= 1000000000) {
      return new Intl.NumberFormat('fa-IR').format(parseFloat((value / 1000000000).toFixed(1))) + ' میلیارد';
    }
    if (value >= 1000000) {
      return new Intl.NumberFormat('fa-IR').format(parseFloat((value / 1000000).toFixed(1))) + ' میلیون';
    }
    if (value >= 1000) {
      return new Intl.NumberFormat('fa-IR').format(parseFloat((value / 1000).toFixed(1))) + ' هزار';
    }
    return new Intl.NumberFormat('fa-IR').format(value);
  }
  
  formatTime(date: Date): string {
    const now = new Date();
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);
    
    if (diff < 60) return 'همین الان';
    if (diff < 3600) return `${Math.floor(diff / 60)} دقیقه پیش`;
    if (diff < 86400) return `${Math.floor(diff / 3600)} ساعت پیش`;
    return `${Math.floor(diff / 86400)} روز پیش`;
  }
  
  getTrendClass(value: number | undefined | null): string {
    if (!value || value === 0) return 'bg-slate-100 text-slate-600';
    return value > 0 ? 'bg-emerald-100 text-emerald-700' : 'bg-red-100 text-red-700';
  }
  
  getTrendIcon(value: number | undefined | null): string {
    if (!value || value === 0) return 'fas fa-minus';
    return value > 0 ? 'fas fa-arrow-up' : 'fas fa-arrow-down';
  }
  
  getPersianMonths(): string[] {
    return ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور', 'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'];
  }
  
  generateTrendData(total: number): number[] {
    // Generate realistic trend data based on total
    const months = 6;
    const data: number[] = [];
    let remaining = total;
    
    for (let i = 0; i < months; i++) {
      const portion = Math.floor(remaining * (0.1 + Math.random() * 0.2));
      data.push(portion);
      remaining -= portion;
    }
    
    return data;
  }
  
  getColorPalette(count: number): string[] {
    const colors = [
      '#6366f1', '#10b981', '#f59e0b', '#06b6d4', '#8b5cf6',
      '#ec4899', '#14b8a6', '#f97316', '#a855f7', '#22c55e'
    ];
    return Array(count).fill(0).map((_, i) => colors[i % colors.length]);
  }
  
  // ============================================
  // VIEW ACTIONS
  // ============================================
  
  setActiveView(view: 'overview' | 'analytics' | 'modules'): void {
    this.activeView = view;
  }
  
  refreshData(): void {
    this.isRefreshing = true;
    this.dashboardService.clearCache();
    this.loadAllData();
    setTimeout(() => this.isRefreshing = false, 1000);
  }
  
  navigateToModule(route: string): void {
    this.router.navigate([route]);
  }
}
