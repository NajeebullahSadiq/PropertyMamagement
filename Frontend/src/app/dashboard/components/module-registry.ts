/**
 * Module Registry
 * Centralized configuration for all system modules
 * This registry defines all 11+ modules with their configurations
 */

import { ModuleConfig, ModuleColor } from './dashboard.types';

// ============================================
// COLOR PRESETS
// ============================================

const COLORS = {
  indigo: {
    primary: '#6366f1',
    secondary: '#8b5cf6',
    gradient: 'bg-gradient-to-br from-indigo-500 to-purple-600',
    gradientHover: 'hover:from-indigo-600 hover:to-purple-700',
    bgLight: 'bg-indigo-50',
    text: 'text-indigo-600',
    shadow: 'shadow-indigo-500/20'
  },
  emerald: {
    primary: '#10b981',
    secondary: '#14b8a6',
    gradient: 'bg-gradient-to-br from-emerald-500 to-teal-600',
    gradientHover: 'hover:from-emerald-600 hover:to-teal-700',
    bgLight: 'bg-emerald-50',
    text: 'text-emerald-600',
    shadow: 'shadow-emerald-500/20'
  },
  violet: {
    primary: '#8b5cf6',
    secondary: '#a855f7',
    gradient: 'bg-gradient-to-br from-violet-500 to-purple-600',
    gradientHover: 'hover:from-violet-600 hover:to-purple-700',
    bgLight: 'bg-violet-50',
    text: 'text-violet-600',
    shadow: 'shadow-violet-500/20'
  },
  amber: {
    primary: '#f59e0b',
    secondary: '#f97316',
    gradient: 'bg-gradient-to-br from-amber-500 to-orange-600',
    gradientHover: 'hover:from-amber-600 hover:to-orange-700',
    bgLight: 'bg-amber-50',
    text: 'text-amber-600',
    shadow: 'shadow-amber-500/20'
  },
  cyan: {
    primary: '#06b6d4',
    secondary: '#0ea5e9',
    gradient: 'bg-gradient-to-br from-cyan-500 to-sky-600',
    gradientHover: 'hover:from-cyan-600 hover:to-sky-700',
    bgLight: 'bg-cyan-50',
    text: 'text-cyan-600',
    shadow: 'shadow-cyan-500/20'
  },
  rose: {
    primary: '#f43f5e',
    secondary: '#e11d48',
    gradient: 'bg-gradient-to-br from-rose-500 to-red-600',
    gradientHover: 'hover:from-rose-600 hover:to-red-700',
    bgLight: 'bg-rose-50',
    text: 'text-rose-600',
    shadow: 'shadow-rose-500/20'
  },
  pink: {
    primary: '#ec4899',
    secondary: '#db2777',
    gradient: 'bg-gradient-to-br from-pink-500 to-rose-600',
    gradientHover: 'hover:from-pink-600 hover:to-rose-700',
    bgLight: 'bg-pink-50',
    text: 'text-pink-600',
    shadow: 'shadow-pink-500/20'
  },
  lime: {
    primary: '#84cc16',
    secondary: '#22c55e',
    gradient: 'bg-gradient-to-br from-lime-500 to-green-600',
    gradientHover: 'hover:from-lime-600 hover:to-green-700',
    bgLight: 'bg-lime-50',
    text: 'text-lime-600',
    shadow: 'shadow-lime-500/20'
  },
  slate: {
    primary: '#64748b',
    secondary: '#475569',
    gradient: 'bg-gradient-to-br from-slate-500 to-slate-600',
    gradientHover: 'hover:from-slate-600 hover:to-slate-700',
    bgLight: 'bg-slate-50',
    text: 'text-slate-600',
    shadow: 'shadow-slate-500/20'
  },
  blue: {
    primary: '#3b82f6',
    secondary: '#2563eb',
    gradient: 'bg-gradient-to-br from-blue-500 to-indigo-600',
    gradientHover: 'hover:from-blue-600 hover:to-indigo-700',
    bgLight: 'bg-blue-50',
    text: 'text-blue-600',
    shadow: 'shadow-blue-500/20'
  },
  teal: {
    primary: '#14b8a6',
    secondary: '#0d9488',
    gradient: 'bg-gradient-to-br from-teal-500 to-cyan-600',
    gradientHover: 'hover:from-teal-600 hover:to-cyan-700',
    bgLight: 'bg-teal-50',
    text: 'text-teal-600',
    shadow: 'shadow-teal-500/20'
  },
  orange: {
    primary: '#f97316',
    secondary: '#ea580c',
    gradient: 'bg-gradient-to-br from-orange-500 to-red-600',
    gradientHover: 'hover:from-orange-600 hover:to-red-700',
    bgLight: 'bg-orange-50',
    text: 'text-orange-600',
    shadow: 'shadow-orange-500/20'
  },
  red: {
    primary: '#ef4444',
    secondary: '#dc2626',
    gradient: 'bg-gradient-to-br from-red-500 to-rose-600',
    gradientHover: 'hover:from-red-600 hover:to-rose-700',
    bgLight: 'bg-red-50',
    text: 'text-red-600',
    shadow: 'shadow-red-500/20'
  }
};

// ============================================
// MODULE REGISTRY
// ============================================

export const MODULE_REGISTRY: ModuleConfig[] = [
  // ============================================
  // 1. ESTATE / PROPERTY TRANSACTIONS
  // ============================================
  {
    id: 'estate',
    name: 'معاملات ملکی',
    nameEn: 'Property Transactions',
    alias: 'Estate',
    icon: 'fa-building',
    iconType: 'fas',
    color: COLORS.emerald,
    route: '/estate',
    description: 'ثبت و مدیریت معاملات املاک و اراضی',
    category: 'transactions',
    priority: 1,
    features: ['registration', 'transfer', 'cancellation', 'verification']
  },

  // ============================================
  // 2. VEHICLE TRANSACTIONS
  // ============================================
  {
    id: 'vehicle',
    name: 'معاملات وسایط نقلیه',
    nameEn: 'Vehicle Transactions',
    alias: 'Vehicles',
    icon: 'fa-car',
    iconType: 'fas',
    color: COLORS.violet,
    route: '/vehicle',
    description: 'ثبت و مدیریت معاملات وسایط نقلیه',
    category: 'transactions',
    priority: 2,
    features: ['registration', 'transfer', 'verification']
  },

  // ============================================
  // 3. COMPANIES / RAHNMAYAN
  // ============================================
  {
    id: 'company',
    name: 'رهنمایان',
    nameEn: 'Real Estate Agents',
    alias: 'Agents',
    icon: 'fa-user-tie',
    iconType: 'fas',
    color: COLORS.amber,
    route: '/realestate',
    description: 'مدیریت رهنمایان و شرکت‌های املاک',
    category: 'licenses',
    priority: 3,
    features: ['registration', 'license', 'guarantee', 'monitoring']
  },

  // ============================================
  // 4. SECURITIES (اسناد بهادار)
  // ============================================
  {
    id: 'securities',
    name: 'اسناد بهادار',
    nameEn: 'Securities',
    alias: 'Securities',
    icon: 'fa-file-contract',
    iconType: 'fas',
    color: COLORS.cyan,
    route: '/securities',
    description: 'مدیریت اسناد بهادار و توزیع',
    category: 'securities',
    priority: 4,
    features: ['distribution', 'control', 'reporting']
  },

  // ============================================
  // 5. PETITION WRITER SECURITIES
  // ============================================
  {
    id: 'petitionWriterSecurities',
    name: 'سند بهادار عریضه‌نویسان',
    nameEn: 'Petition Writer Securities',
    alias: 'PW Securities',
    icon: 'fa-file-signature',
    iconType: 'fas',
    color: COLORS.pink,
    route: '/petition-writer-securities',
    description: 'اسناد بهادار عریضه‌نویسان',
    category: 'securities',
    priority: 5,
    features: ['registration', 'distribution', 'reporting']
  },

  // ============================================
  // 6. PETITION WRITER LICENSE
  // ============================================
  {
    id: 'petitionWriterLicense',
    name: 'جواز عریضه‌نویسان',
    nameEn: 'Petition Writer License',
    alias: 'PW License',
    icon: 'fa-id-card-alt',
    iconType: 'fas',
    color: COLORS.orange,
    route: '/petition-writer-license',
    description: 'صدور و مدیریت جواز عریضه‌نویسان',
    category: 'licenses',
    priority: 6,
    features: ['registration', 'renewal', 'tracking']
  },

  // ============================================
  // 7. LICENSE APPLICATIONS
  // ============================================
  {
    id: 'licenseApplication',
    name: 'درخواست جواز',
    nameEn: 'License Applications',
    alias: 'Applications',
    icon: 'fa-file-alt',
    iconType: 'fas',
    color: COLORS.blue,
    route: '/license-applications',
    description: 'مدیریت درخواست‌های جواز جدید',
    category: 'licenses',
    priority: 7,
    features: ['application', 'review', 'approval']
  },

  // ============================================
  // 8. ACTIVITY MONITORING
  // ============================================
  {
    id: 'activityMonitoring',
    name: 'نظارت بر فعالیت‌ها',
    nameEn: 'Activity Monitoring',
    alias: 'Monitoring',
    icon: 'fa-chart-line',
    iconType: 'fas',
    color: COLORS.lime,
    route: '/activity-monitoring',
    description: 'نظارت بر فعالیت‌های شرکت‌ها و رهنمایان',
    category: 'monitoring',
    priority: 8,
    features: ['tracking', 'reports', 'alerts']
  },

  // ============================================
  // 9. PETITION WRITER MONITORING
  // ============================================
  {
    id: 'petitionWriterMonitoring',
    name: 'نظارت بر عریضه‌نویسان',
    nameEn: 'Petition Writer Monitoring',
    alias: 'PW Monitor',
    icon: 'fa-eye',
    iconType: 'fas',
    color: COLORS.teal,
    route: '/petition-writer-monitoring',
    description: 'نظارت بر فعالیت عریضه‌نویسان',
    category: 'monitoring',
    priority: 9,
    features: ['tracking', 'reports', 'compliance']
  },

  // ============================================
  // 10. VERIFICATION
  // ============================================
  {
    id: 'verification',
    name: 'تاییدیه اسناد',
    nameEn: 'Document Verification',
    alias: 'Verify',
    icon: 'fa-check-double',
    iconType: 'fas',
    color: COLORS.indigo,
    route: '/verify',
    description: 'تایید و استعلام اسناد و مدارک',
    category: 'transactions',
    priority: 10,
    features: ['search', 'verify', 'report']
  },

  // ============================================
  // 11. USER MANAGEMENT
  // ============================================
  {
    id: 'users',
    name: 'مدیریت کاربران',
    nameEn: 'User Management',
    alias: 'Users',
    icon: 'fa-users-cog',
    iconType: 'fas',
    color: COLORS.slate,
    route: '/Auth/Register',
    description: 'مدیریت کاربران و دسترسی‌ها',
    category: 'administration',
    priority: 11,
    features: ['users', 'roles', 'permissions']
  },

  // ============================================
  // 12. AUDIT LOG
  // ============================================
  {
    id: 'auditLog',
    name: 'گزارش فعالیت‌ها',
    nameEn: 'Audit Log',
    alias: 'Audit',
    icon: 'fa-history',
    iconType: 'fas',
    color: COLORS.rose,
    route: '/audit-log',
    description: 'تاریخچه فعالیت‌های سیستم',
    category: 'administration',
    priority: 12,
    features: ['logs', 'tracking', 'reports']
  },

  // ============================================
  // 13. DISTRICT MANAGEMENT
  // ============================================
  {
    id: 'districtManagement',
    name: 'مدیریت ولسوالی‌ها',
    nameEn: 'District Management',
    alias: 'Districts',
    icon: 'fa-map-marked-alt',
    iconType: 'fas',
    color: COLORS.cyan,
    route: '/district-management',
    description: 'مدیریت ولسوالی‌ها و موقعیت‌ها',
    category: 'administration',
    priority: 13,
    features: ['districts', 'locations', 'mapping']
  },

  // ============================================
  // 14. EXPIRED LICENSES (Special Alert Module)
  // ============================================
  {
    id: 'expiredLicense',
    name: 'جوازهای منقضی',
    nameEn: 'Expired Licenses',
    alias: 'Expired',
    icon: 'fa-exclamation-triangle',
    iconType: 'fas',
    color: COLORS.red,
    route: '/realestate/exlist',
    description: 'جوازهای منقضی شده نیاز به تمدید',
    category: 'monitoring',
    priority: 14,
    features: ['alerts', 'renewal', 'tracking']
  }
];

// ============================================
// HELPER FUNCTIONS
// ============================================

export function getModuleById(id: string): ModuleConfig | undefined {
  return MODULE_REGISTRY.find(m => m.id === id);
}

export function getModulesByCategory(category: string): ModuleConfig[] {
  return MODULE_REGISTRY.filter(m => m.category === category);
}

export function getActiveModules(): ModuleConfig[] {
  return MODULE_REGISTRY.filter(m => m.priority <= 11);
}

export function getModuleCategories(): string[] {
  return [...new Set(MODULE_REGISTRY.map(m => m.category))];
}

export function getQuickAccessModules(): ModuleConfig[] {
  return MODULE_REGISTRY.filter(m => 
    ['estate', 'vehicle', 'company', 'securities', 'verification'].includes(m.id)
  );
}

export function getAlertModules(): ModuleConfig[] {
  return MODULE_REGISTRY.filter(m => 
    ['expiredLicense', 'activityMonitoring', 'petitionWriterMonitoring'].includes(m.id)
  );
}
