import { Component } from '@angular/core';
import { Router } from '@angular/router';

interface ConfigCard {
  title: string;
  description: string;
  icon: string;
  route: string;
  color: string;
  gradient: string;
}

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.scss']
})
export class ConfigurationComponent {
  configCards: ConfigCard[] = [
    {
      title: 'لیست کاربران',
      description: 'مشاهده و مدیریت تمام کاربران سیستم',
      icon: 'people',
      route: '/users',
      color: 'blue',
      gradient: 'from-blue-500 to-indigo-600'
    },
    {
      title: 'مدیریت صلاحیت‌ها',
      description: 'تعیین و مدیریت صلاحیت‌های کاربران',
      icon: 'admin_panel_settings',
      route: '/users/permissions',
      color: 'purple',
      gradient: 'from-purple-500 to-violet-600'
    },
    {
      title: 'صلاحیت‌های نقش‌ها',
      description: 'مدیریت صلاحیت‌های نقش‌های مختلف سیستم',
      icon: 'manage_accounts',
      route: '/users/role-permissions',
      color: 'emerald',
      gradient: 'from-emerald-500 to-teal-600'
    },
    {
      title: 'مدیریت ولسوالی‌ها',
      description: 'فعال‌سازی و مدیریت ولسوالی‌ها',
      icon: 'location_city',
      route: '/district-management',
      color: 'amber',
      gradient: 'from-amber-500 to-orange-600'
    },
    {
      title: 'لاگ فعالیت‌های سیستم',
      description: 'مشاهده و بررسی تمام فعالیت‌های سیستم',
      icon: 'history',
      route: '/audit-log',
      color: 'rose',
      gradient: 'from-rose-500 to-pink-600'
    },
    {
      title: 'محل فعالیت عریضه‌نویس',
      description: 'مدیریت محل فعالیت‌های عریضه‌نویسان',
      icon: 'gavel',
      route: '/petition-writer-activity-location-management',
      color: 'orange',
      gradient: 'from-orange-500 to-amber-600'
    }
  ];

  constructor(private router: Router) {}

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}
