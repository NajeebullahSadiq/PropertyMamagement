import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DistrictManagementService } from 'src/app/shared/district-management.service';
import { DeleteConfirmationDialogComponent } from 'src/app/shared/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';

@Component({
  selector: 'app-district-management',
  templateUrl: './district-management.component.html',
  styleUrls: ['./district-management.component.scss']
})
export class DistrictManagementComponent extends BaseComponent implements OnInit {
  provinces: any[] = [];
  districts: any[] = [];
  selectedProvinceId: number | null = null;
  districtForm: FormGroup;
  isEditMode: boolean = false;
  editingDistrictId: number | null = null;
  showForm: boolean = false;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private districtService: DistrictManagementService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) {
    super();
    this.districtForm = this.fb.group({
      dari: ['', Validators.required],
      name: ['']
    });
  }

  ngOnInit(): void {
    this.loadProvinces();
  }

  loadProvinces(): void {
    this.isLoading = true;
    this.districtService.getProvinces().subscribe({
      next: (data: any) => {
        this.provinces = data;
        this.isLoading = false;
      },
      error: (error: any) => {
        this.toastr.error('خطا در بارگذاری ولایات');
        this.isLoading = false;
      }
    });
  }

  onProvinceChange(event: any): void {
    this.selectedProvinceId = event.target.value ? parseInt(event.target.value) : null;
    if (this.selectedProvinceId) {
      this.loadDistricts(this.selectedProvinceId);
    } else {
      this.districts = [];
    }
    this.resetForm();
  }

  loadDistricts(provinceId: number): void {
    this.isLoading = true;
    this.districtService.getDistrictsByProvince(provinceId).subscribe({
      next: (data: any) => {
        this.districts = data;
        this.isLoading = false;
      },
      error: (error: any) => {
        this.toastr.error('خطا در بارگذاری ولسوالی ها');
        this.isLoading = false;
      }
    });
  }

  showAddForm(): void {
    if (!this.selectedProvinceId) {
      this.toastr.warning('لطفاً ابتدا ولایت را انتخاب کنید');
      return;
    }
    this.showForm = true;
    this.isEditMode = false;
    this.resetForm();
  }

  editDistrict(district: any): void {
    this.showForm = true;
    this.isEditMode = true;
    this.editingDistrictId = district.id;
    this.districtForm.patchValue({
      dari: district.dari,
      name: district.name
    });
  }

  saveDistrict(): void {
    if (this.districtForm.invalid) {
      this.toastr.warning('لطفاً تمام فیلدهای ضروری را پر کنید');
      return;
    }

    if (!this.selectedProvinceId) {
      this.toastr.warning('لطفاً ولایت را انتخاب کنید');
      return;
    }

    const formData = {
      ...this.districtForm.value,
      provinceId: this.selectedProvinceId
    };

    if (this.isEditMode && this.editingDistrictId) {
      this.updateDistrict(formData);
    } else {
      this.createDistrict(formData);
    }
  }

  createDistrict(data: any): void {
    this.isLoading = true;
    this.districtService.createDistrict(data).subscribe({
      next: (response: any) => {
        this.toastr.success(response.message || 'ولسوالی با موفقیت ثبت شد');
        this.loadDistricts(this.selectedProvinceId!);
        this.resetForm();
        this.showForm = false;
        this.isLoading = false;
      },
      error: (error: any) => {
        this.toastr.error(error.error?.message || 'خطا در ثبت ولسوالی');
        this.isLoading = false;
      }
    });
  }

  updateDistrict(data: any): void {
    this.isLoading = true;
    this.districtService.updateDistrict(this.editingDistrictId!, data).subscribe({
      next: (response: any) => {
        this.toastr.success(response.message || 'ولسوالی با موفقیت ویرایش شد');
        this.loadDistricts(this.selectedProvinceId!);
        this.resetForm();
        this.showForm = false;
        this.isLoading = false;
      },
      error: (error: any) => {
        this.toastr.error(error.error?.message || 'خطا در ویرایش ولسوالی');
        this.isLoading = false;
      }
    });
  }

  deleteDistrict(district: any): void {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        title: 'تأیید حذف ولسوالی',
        message: `آیا مطمئن هستید که می‌خواهید "${district.dari}" را حذف کنید؟`,
        itemName: district.dari
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.isLoading = true;
        this.districtService.deleteDistrict(district.id).subscribe({
          next: (response: any) => {
            this.toastr.success(response.message || 'ولسوالی با موفقیت حذف شد');
            this.loadDistricts(this.selectedProvinceId!);
            this.isLoading = false;
          },
          error: (error: any) => {
            this.toastr.error(error.error?.message || 'خطا در حذف ولسوالی');
            this.isLoading = false;
          }
        });
      }
    });
  }

  activateDistrict(district: any): void {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        title: 'تأیید فعال سازی ولسوالی',
        message: `آیا مطمئن هستید که می‌خواهید "${district.dari}" را فعال کنید؟`,
        itemName: district.dari,
        confirmButtonText: 'فعال سازی',
        confirmButtonColor: 'primary'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.isLoading = true;
        this.districtService.activateDistrict(district.id).subscribe({
          next: (response: any) => {
            this.toastr.success(response.message || 'ولسوالی با موفقیت فعال شد');
            this.loadDistricts(this.selectedProvinceId!);
            this.isLoading = false;
          },
          error: (error: any) => {
            this.toastr.error(error.error?.message || 'خطا در فعال سازی ولسوالی');
            this.isLoading = false;
          }
        });
      }
    });
  }

  cancelForm(): void {
    this.showForm = false;
    this.resetForm();
  }

  resetForm(): void {
    this.districtForm.reset();
    this.isEditMode = false;
    this.editingDistrictId = null;
  }

  getProvinceName(provinceId: number): string {
    const province = this.provinces.find(p => p.id === provinceId);
    return province ? province.dari : '';
  }
}
