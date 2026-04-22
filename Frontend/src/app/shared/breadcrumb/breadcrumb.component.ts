import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter } from 'rxjs/operators';

interface BreadcrumbItem {
  label: string;
  route?: string;
}

/**
 * Simple breadcrumb component that maps route paths to Dari labels.
 */
@Component({
  selector: 'app-breadcrumb',
  template: `
    <nav *ngIf="breadcrumbs.length > 0" class="px-4 py-2 bg-white/80 border-b border-gray-100 backdrop-blur-sm">
      <ol class="flex items-center gap-2 text-sm text-gray-500">
        <li class="flex items-center gap-2">
          <a routerLink="/dashboard" class="hover:text-indigo-600 transition-colors flex items-center gap-1">
            <mat-icon class="text-base text-gray-400">home</mat-icon>
            <span>داشبورد</span>
          </a>
        </li>
        <li *ngFor="let crumb of breadcrumbs; let last = last" class="flex items-center gap-2">
          <mat-icon class="text-base text-gray-300">chevron_left</mat-icon>
          <span *ngIf="last" class="text-gray-800 font-medium">{{ crumb.label }}</span>
          <a *ngIf="!last && crumb.route" [routerLink]="crumb.route" class="hover:text-indigo-600 transition-colors">{{ crumb.label }}</a>
          <span *ngIf="!last && !crumb.route">{{ crumb.label }}</span>
        </li>
      </ol>
    </nav>
  `,
  styles: [``]
})
export class BreadcrumbComponent implements OnInit {
  breadcrumbs: BreadcrumbItem[] = [];

  private routeLabels: { [key: string]: string } = {
    'dashboard': 'داشبورد',
    'estate': 'ملکیت‌ها',
    'realestate': 'رهنمایان',
    'vehicle': 'وسایط نقلیه',
    'securities': 'اسناد بهادار',
    'license-applications': 'درخواست جواز',
    'users': 'کاربران',
    'configuration': 'تنظیمات',
    'audit-log': 'لاگ فعالیت‌ها',
    'activity-monitoring': 'نظارت فعالیت‌ها',
    'petition-writer-securities': 'سند بهادار عریضه‌نویسان',
    'petition-writer-license': 'جواز عریضه‌نویسان',
    'petition-writer-monitoring': 'نظارت عریضه‌نویسان',
    'securities-report': 'گزارش اسناد بهادار',
    'petition-writer-report': 'گزارش عریضه‌نویسان',
    'district-management': 'مدیریت ولسوالی‌ها',
    'report': 'گزارشات',
    'userreport': 'گزارش کاربران',
    'securities-control': 'کنترل اسناد بهادار',
  };

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe(() => {
      this.breadcrumbs = this.buildBreadcrumbs(this.activatedRoute.root);
    });
    this.breadcrumbs = this.buildBreadcrumbs(this.activatedRoute.root);
  }

  private buildBreadcrumbs(route: ActivatedRoute, url: string = ''): BreadcrumbItem[] {
    const breadcrumbs: BreadcrumbItem[] = [];
    const children: ActivatedRoute[] = route.children;

    if (children.length === 0) {
      return breadcrumbs;
    }

    for (const child of children) {
      const routeURL: string = child.snapshot.url.map(s => s.path).join('/');
      if (routeURL !== '') {
        url += `/${routeURL}`;
        const label = this.routeLabels[routeURL] || routeURL;
        breadcrumbs.push({ label, route: url });
      }
      breadcrumbs.push(...this.buildBreadcrumbs(child, url));
    }

    return breadcrumbs;
  }
}
