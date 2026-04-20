import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { PetitionWriterActivityLocationService } from 'src/app/shared/petition-writer-activity-location.service';
import { DeleteConfirmationDialogComponent } from 'src/app/shared/delete-confirmation-dialog/delete-confirmation-dialog.component';

@Component({
  selector: 'app-petition-writer-activity-location-management',
  templateUrl: './petition-writer-activity-location-management.component.html',
  styleUrls: ['./petition-writer-activity-location-management.component.scss']
})
export class PetitionWriterActivityLocationManagementComponent implements OnInit {
  locations: any[] = [];
  locationForm: FormGroup;
  isEditMode: boolean = false;
  editingId: number | null = null;
  showForm: boolean = false;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private locationService: PetitionWriterActivityLocationService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) {
    this.locationForm = this.fb.group({
      dariName: ['', Validators.required],
      name: ['']
    });
  }

  ngOnInit(): void {
    this.loadLocations();
  }

  loadLocations(): void {
    this.isLoading = true;
    this.locationService.getAllForManagement().subscribe({
      next: (data: any) => {
        this.locations = data;
        this.isLoading = false;
      },
      error: (error: any) => {
        this.toastr.error('خطا در بارگذاری محل فعالیت‌ها');
        this.isLoading = false;
      }
    });
  }

  showAddForm(): void {
    this.showForm = true;
    this.isEditMode = false;
    this.resetForm();
  }

  editLocation(location: any): void {
    this.showForm = true;
    this.isEditMode = true;
    this.editingId = location.id;
    this.locationForm.patchValue({
      dariName: location.dariName,
      name: location.name
    });
  }

  saveLocation(): void {
    if (this.locationForm.invalid) {
      this.toastr.warning('لطفاً نام محل فعالیت را وارد کنید');
      return;
    }

    const formData = this.locationForm.value;

    if (this.isEditMode && this.editingId) {
      this.locationService.update(this.editingId, formData).subscribe({
        next: (response: any) => {
          this.toastr.success(response.message || 'محل فعالیت با موفقیت تغییر یافت');
          this.loadLocations();
          this.cancelForm();
        },
        error: (error: any) => {
          this.toastr.error(error.error?.message || 'خطا در تغییر محل فعالیت');
        }
      });
    } else {
      this.locationService.create(formData).subscribe({
        next: (response: any) => {
          this.toastr.success(response.message || 'محل فعالیت با موفقیت ثبت شد');
          this.loadLocations();
          this.cancelForm();
        },
        error: (error: any) => {
          this.toastr.error(error.error?.message || 'خطا در ثبت محل فعالیت');
        }
      });
    }
  }

  deleteLocation(location: any): void {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        title: 'تأیید حذف محل فعالیت',
        message: `آیا مطمئن هستید که می‌خواهید "${location.dariName}" را حذف کنید؟`,
        itemName: location.dariName
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.locationService.delete(location.id).subscribe({
          next: (response: any) => {
            this.toastr.success(response.message || 'محل فعالیت با موفقیت حذف شد');
            this.loadLocations();
          },
          error: (error: any) => {
            this.toastr.error(error.error?.message || 'خطا در حذف محل فعالیت');
          }
        });
      }
    });
  }

  activateLocation(location: any): void {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        title: 'تأیید فعال سازی',
        message: `آیا مطمئن هستید که می‌خواهید "${location.dariName}" را فعال کنید؟`,
        itemName: location.dariName,
        confirmButtonText: 'فعال سازی',
        confirmButtonColor: 'primary'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.locationService.activate(location.id).subscribe({
          next: (response: any) => {
            this.toastr.success(response.message || 'محل فعالیت با موفقیت فعال شد');
            this.loadLocations();
          },
          error: (error: any) => {
            this.toastr.error(error.error?.message || 'خطا در فعال سازی محل فعالیت');
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
    this.locationForm.reset();
    this.isEditMode = false;
    this.editingId = null;
  }
}
