import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, Input, Output, ChangeDetectorRef, OnChanges, SimpleChanges } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { environment } from 'src/environments/environment';
import { ProfileImageCropperDialogComponent, ProfileImageCropperDialogResult } from './profile-image-cropper-dialog.component';

@Component({
  selector: 'app-profile-image-cropper',
  templateUrl: './profile-image-cropper.component.html',
  styleUrls: ['./profile-image-cropper.component.scss']
})
export class ProfileImageCropperComponent implements OnChanges {
  @Input() initialImageUrl: string = '';
  @Input() documentType: string = 'profile';
  @Input() roundCropper: boolean = true;
  @Input() aspectRatio: number = 1; // Default 1:1 (square), use 4/3, 16/9, or 0 for free-form
  @Input() maintainAspectRatio: boolean = true; // Set to false for free-form cropping

  @Output() imageUploaded = new EventEmitter<string>();
  @Output() imageChanged = new EventEmitter<string>();

  croppedImageUrl: string = '';
  croppedBlob: Blob | null = null;

  private localPreviewObjectUrl: string = '';

  isUploading: boolean = false;
  progress: number = 0;
  message: string = '';
  error: string = '';

  constructor(
    private http: HttpClient,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialImageUrl'] && changes['initialImageUrl'].currentValue) {
      // When initialImageUrl changes from parent, clear croppedImageUrl to show the new image
      if (!this.croppedImageUrl) {
        this.message = 'عکس موجود';
      }
    }
  }

  onFileSelected(event: Event): void {
    this.error = '';
    this.message = '';
    this.progress = 0;

    const input = event.target as HTMLInputElement | null;
    const file = input?.files?.[0];
    if (!file) {
      return;
    }

    const dialogRef = this.dialog.open(ProfileImageCropperDialogComponent, {
      width: '720px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      panelClass: 'profile-cropper-dialog-panel',
      disableClose: true,
      hasBackdrop: true,
      backdropClass: 'cropper-dialog-backdrop',
      data: {
        file,
        roundCropper: this.roundCropper,
        resizeToWidth: 512,
        aspectRatio: this.aspectRatio,
        maintainAspectRatio: this.maintainAspectRatio
      }
    });

    dialogRef.afterClosed().subscribe((result: ProfileImageCropperDialogResult | null | undefined) => {
      if (!result) {
        return;
      }

      this.croppedBlob = result.blob;

      if (this.localPreviewObjectUrl) {
        URL.revokeObjectURL(this.localPreviewObjectUrl);
        this.localPreviewObjectUrl = '';
      }

      this.localPreviewObjectUrl = URL.createObjectURL(this.croppedBlob);
      this.croppedImageUrl = this.localPreviewObjectUrl;
      this.imageChanged.emit(this.croppedImageUrl);

      this.uploadCroppedImage();
    });

    if (input) {
      input.value = '';
    }
  }

  uploadCroppedImage(): void {
    this.error = '';
    this.message = '';

    if (!this.croppedBlob) {
      this.error = 'ابتدا تصویر را انتخاب و برش کنید';
      return;
    }

    const file = new File([this.croppedBlob], 'profile.jpg', { type: this.croppedBlob.type || 'image/jpeg' });
    const formData = new FormData();
    formData.append('file', file);

    this.isUploading = true;
    this.progress = 0;

    this.http.post(`${environment.apiURL}/upload?documentType=${encodeURIComponent(this.documentType)}`, formData, {
      reportProgress: true,
      observe: 'events'
    }).subscribe({
      next: (event) => {
        if (event.type === HttpEventType.UploadProgress) {
          this.progress = Math.round(100 * (event.loaded || 1) / (event.total || 1));
        }
        if (event.type === HttpEventType.Response) {
          const response: any = event.body;
          this.isUploading = false;
          this.message = 'تصویر موفقانه ذخیره شد';
          this.imageUploaded.emit(response.dbPath);
        }
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this.isUploading = false;
        this.progress = 0;
        this.error = 'خطا در آپلود تصویر';
      }
    });
  }

  reset(): void {
    if (this.localPreviewObjectUrl) {
      URL.revokeObjectURL(this.localPreviewObjectUrl);
      this.localPreviewObjectUrl = '';
    }

    this.croppedImageUrl = '';
    this.croppedBlob = null;
    this.progress = 0;
    this.message = '';
    this.error = '';
    this.isUploading = false;
    this.imageChanged.emit('');
  }

  setExistingImage(imagePath: string): void {
    if (imagePath) {
      this.croppedImageUrl = '';
      this.initialImageUrl = imagePath;
      this.message = 'عکس موجود';
      this.cdr.detectChanges();
    }
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    console.error('Image failed to load:', img.src);
    // Fallback to avatar if image fails to load
    if (img.src !== 'assets/img/avatar.png' && !img.src.endsWith('assets/img/avatar.png')) {
      this.initialImageUrl = '';
      this.croppedImageUrl = '';
      this.error = 'خطا در بارگذاری تصویر';
    }
  }

  getImageUrl(path: string): string {
    if (!path) return 'assets/img/avatar.png';
    
    // If path already starts with http/https or is a blob URL, return as is
    if (path.startsWith('http://') || path.startsWith('https://') || path.startsWith('blob:')) {
      return path;
    }
    
    // If it's an assets path, return as is
    if (path.startsWith('assets/')) {
      return path;
    }
    
    // If path starts with Resources/, it's a full path from DB - use static file serving
    // The backend serves these files at /api/Resources/...
    if (path.startsWith('Resources/') || path.startsWith('/Resources/')) {
      const cleanPath = path.startsWith('/') ? path.substring(1) : path;
      return `${environment.apiURL}/${cleanPath}`;
    }
    
    // Otherwise, use Upload/view endpoint
    return `${environment.apiURL}/Upload/view/${path}`;
  }
}
