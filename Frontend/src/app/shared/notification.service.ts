import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

/**
 * Global notification service wrapping ngx-toastr with RTL-friendly Dari messages.
 * Provides consistent success, error, warning, and info toast notifications
 * across the application.
 */
@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private toastr: ToastrService) {
    // Configure default toastr options for RTL Dari layout
    this.toastr.toastrConfig.positionClass = 'toast-top-left';
    this.toastr.toastrConfig.timeOut = 4000;
    this.toastr.toastrConfig.extendedTimeOut = 2000;
    this.toastr.toastrConfig.closeButton = true;
    this.toastr.toastrConfig.progressBar = true;
    this.toastr.toastrConfig.preventDuplicates = true;
    this.toastr.toastrConfig.maxOpened = 3;
  }

  /** Show a success toast */
  success(message: string, title: string = 'موفق'): void {
    this.toastr.success(message, title);
  }

  /** Show an error toast */
  error(message: string, title: string = 'خطا'): void {
    this.toastr.error(message, title);
  }

  /** Show a warning toast */
  warning(message: string, title: string = 'هشدار'): void {
    this.toastr.warning(message, title);
  }

  /** Show an info toast */
  info(message: string, title: string = 'اطلاعات'): void {
    this.toastr.info(message, title);
  }

  /** Show generic HTTP error message based on status code */
  showHttpError(err: any): void {
    if (!err) {
      this.error('خطای ناشناخته در ارتباط با سرور');
      return;
    }
    const status = err.status;
    switch (status) {
      case 0:
        this.error('سرور در دسترس نیست. لطفاً اتصال اینترنت خود را بررسی کنید');
        break;
      case 400:
        this.error(err.error?.message || 'درخواست نامعتبر است');
        break;
      case 401:
        this.error('نشست شما منقضی شده است. لطفاً دوباره وارد شوید');
        break;
      case 403:
        this.error('شما دسترسی لازم برای این عملیات را ندارید');
        break;
      case 404:
        this.error('اطلاعات مورد نظر یافت نشد');
        break;
      case 409:
        this.error('این اطلاعات قبلاً ثبت شده است');
        break;
      case 422:
        this.error(err.error?.message || 'اطلاعات وارد شده نامعتبر است');
        break;
      case 500:
        this.error('خطای داخلی سرور. لطفاً دوباره تلاش کنید');
        break;
      default:
        this.error('خطا در ارتباط با سرور. لطفاً دوباره تلاش کنید');
    }
  }

  /** Show success for CRUD operations */
  showSaveSuccess(entityName: string = 'اطلاعات'): void {
    this.success(`${entityName} با موفقیت ذخیره شد`);
  }

  showUpdateSuccess(entityName: string = 'اطلاعات'): void {
    this.success(`${entityName} با موفقیت به‌روزرسانی شد`);
  }

  showDeleteSuccess(entityName: string = 'مورد'): void {
    this.success(`${entityName} با موفقیت حذف شد`);
  }

  showExportSuccess(fileName?: string): void {
    this.success(fileName ? `فایل ${fileName} با موفقیت دانلود شد` : 'فایل با موفقیت دانلود شد');
  }

  /** Show loading error with retry option callback */
  showLoadError(retryCallback?: () => void): void {
    if (retryCallback) {
      // Note: toastr doesn't support action buttons natively; use warning instead
      this.warning('بارگذاری اطلاعات با خطا مواجه شد. از دکمه تلاش مجدد استفاده کنید');
    } else {
      this.error('بارگذاری اطلاعات با خطا مواجه شد');
    }
  }
}
