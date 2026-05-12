/**
 * Dashboard Types and Interfaces
 * Centralized type definitions for the enterprise dashboard
 */

// ============================================
// MODULE DEFINITIONS
// ============================================

export interface ModuleConfig {
  id: string;
  name: string;           // Dari name
  nameEn: string;         // English name
  alias: string;          // Short alias
  icon: string;           // FontAwesome icon class
  iconType: 'fas' | 'far' | 'fab';
  color: ModuleColor;
  route: string;
  description: string;
  permission?: string;
  category: ModuleCategory;
  priority: number;       // Display order priority
  features: string[];
}

export interface ModuleColor {
  primary: string;
  secondary: string;
  gradient: string;
  gradientHover: string;
  bgLight: string;
  text: string;
  shadow: string;
}

export type ModuleCategory = 
  | 'transactions'
  | 'licenses'
  | 'monitoring'
  | 'securities'
  | 'administration'
  | 'reports';

export interface ModuleStats {
  total: number;
  today: number;
  thisWeek: number;
  thisMonth: number;
  pending: number;
  completed: number;
  amount?: number;
  growth?: number;
  trend?: 'up' | 'down' | 'stable';
}

export interface ModuleWithStats extends ModuleConfig {
  stats: ModuleStats;
  isActive: boolean;
  lastUpdated?: Date;
}

// ============================================
// CHART DATA TYPES
// ============================================

export interface ChartDataPoint {
  label: string;
  value: number;
  color?: string;
}

export interface TimeSeriesData {
  date: string;
  value: number;
  category?: string;
}

export interface PieChartData {
  name: string;
  value: number;
  percentage: number;
  color: string;
}

export interface BarChartData {
  labels: string[];
  datasets: ChartDataset[];
}

export interface ChartDataset {
  label: string;
  data: number[];
  backgroundColor: string | string[];
  borderColor?: string;
  borderWidth?: number;
}

// ============================================
// KPI & STATISTICS
// ============================================

export interface KPI {
  id: string;
  title: string;
  titleEn: string;
  value: number;
  previousValue?: number;
  change?: number;
  changeType?: 'increase' | 'decrease' | 'neutral';
  unit?: string;
  prefix?: string;
  suffix?: string;
  icon: string;
  iconBg: string;
  color: string;
  trend?: number[];
  sparklineData?: number[];
}

export interface GlobalStats {
  totalTransactions: number;
  totalAmount: number;
  totalRoyalty: number;
  pendingApprovals: number;
  completedToday: number;
  activeUsers: number;
  systemHealth: 'healthy' | 'warning' | 'critical';
  lastUpdated: Date;
}

// ============================================
// ACTIVITY & NOTIFICATIONS
// ============================================

export interface Activity {
  id: string;
  type: ActivityType;
  module: string;
  moduleName: string;
  description: string;
  descriptionEn: string;
  user?: string;
  timestamp: Date;
  icon: string;
  iconBg: string;
  priority: 'low' | 'medium' | 'high';
  read: boolean;
  actionUrl?: string;
}

export type ActivityType = 
  | 'registration'
  | 'approval'
  | 'completion'
  | 'update'
  | 'deletion'
  | 'warning'
  | 'info'
  | 'error';

export interface Notification {
  id: string;
  type: 'alert' | 'warning' | 'info' | 'success';
  title: string;
  message: string;
  module?: string;
  timestamp: Date;
  read: boolean;
  actionUrl?: string;
  actionText?: string;
}

export interface Alert {
  id: string;
  type: 'danger' | 'warning' | 'info' | 'success';
  icon: string;
  iconBg: string;
  message: string;
  module: string;
  count?: number;
  actionUrl?: string;
  timestamp: Date;
}

// ============================================
// QUICK ACTIONS
// ============================================

export interface QuickAction {
  id: string;
  name: string;
  nameEn: string;
  icon: string;
  iconBg: string;
  route: string;
  color: string;
  category: string;
  permission?: string;
  shortcut?: string;
}

// ============================================
// USER & SESSION
// ============================================

export interface UserSession {
  id: string | null;
  firstName: string | null;
  lastName: string | null;
  fullName: string | null;
  role: string | null;
  roleDari: string | null;
  permissions: string[];
  company?: string;
  companyId?: string;
  avatar?: string;
  lastLogin?: Date;
  preferences: UserPreferences;
}

export interface UserPreferences {
  theme: 'light' | 'dark' | 'auto';
  language: 'fa' | 'en' | 'ps';
  dashboardLayout: 'default' | 'compact' | 'expanded';
  notificationsEnabled: boolean;
}

// ============================================
// DASHBOARD STATE
// ============================================

export interface DashboardState {
  isLoading: boolean;
  isInitialized: boolean;
  activeTab: string;
  dateRange: DateRange;
  refreshInterval: number;
  lastRefresh: Date;
  errors: string[];
}

export interface DateRange {
  start: Date;
  end: Date;
  label: string;
  type: 'today' | 'week' | 'month' | 'quarter' | 'year' | 'custom';
}

// ============================================
// SEARCH & FILTERS
// ============================================

export interface SearchFilters {
  query: string;
  modules: string[];
  dateRange?: DateRange;
  status?: 'all' | 'pending' | 'completed';
  sortBy: string;
  sortOrder: 'asc' | 'desc';
}

export interface SearchResult {
  id: string;
  type: string;
  module: string;
  title: string;
  description: string;
  url: string;
  timestamp: Date;
  relevance: number;
}

// ============================================
// SYSTEM HEALTH
// ============================================

export interface SystemHealth {
  status: 'operational' | 'degraded' | 'down';
  uptime: number;
  lastChecked: Date;
  services: ServiceStatus[];
  alerts: SystemAlert[];
}

export interface ServiceStatus {
  name: string;
  status: 'online' | 'offline' | 'degraded';
  responseTime?: number;
  lastChecked: Date;
}

export interface SystemAlert {
  id: string;
  severity: 'critical' | 'warning' | 'info';
  message: string;
  timestamp: Date;
  resolved: boolean;
}
