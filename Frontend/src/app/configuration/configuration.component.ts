import { Component } from '@angular/core';
import { Router } from '@angular/router';

interface ConfigCard {
  title: string;
  description: string;
  icon: string;
  route: string;
  iconBg: string;
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
      icon: 'fas fa-users',
      route: '/users',
      iconBg: 'bg-[#004532]'
    },
    {
      title: 'مدیریت صلاحیت‌ها',
      description: 'تعیین و مدیریت صلاحیت‌های کاربران',
      icon: 'fas fa-shield-halved',
      route: '/users/permissions',
      iconBg: 'bg-[#065f46]'
    },
    {
      title: 'صلاحیت‌های نقش‌ها',
      description: 'مدیریت صلاحیت‌های نقش‌های مختلف سیستم',
      icon: 'fas fa-user-gear',
      route: '/users/role-permissions',
      iconBg: 'bg-[#44546a]'
    },
    {
      title: 'مدیریت ولسوالی‌ها',
      description: 'فعال‌سازی و مدیریت ولسوالی‌ها',
      icon: 'fas fa-location-dot',
      route: '/district-management',
      iconBg: 'bg-[#92400e]'
    },
    {
      title: 'لاگ فعالیت‌های سیستم',
      description: 'مشاهده و بررسی تمام فعالیت‌های سیستم',
      icon: 'fas fa-clock-rotate-left',
      route: '/audit-log',
      iconBg: 'bg-[#ba1a1a]'
    },
    {
      title: 'محل فعالیت عریضه‌نویس',
      description: 'مدیریت محل فعالیت‌های عریضه‌نویسان',
      icon: 'fas fa-gavel',
      route: '/petition-writer-activity-location-management',
      iconBg: 'bg-[#55615a]'
    }
  ];

  constructor(private router: Router) {}

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}