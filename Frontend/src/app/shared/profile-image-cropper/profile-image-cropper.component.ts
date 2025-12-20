import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-profile-image-cropper',
  templateUrl: './profile-image-cropper.component.html',
  styleUrls: ['./profile-image-cropper.component.scss']
})
export class ProfileImageCropperComponent {
  @Input() initialImageUrl: string = '';
  @Input() documentType: string = 'profile';
  @Input() roundCropper: boolean = true;

  @Output() imageUploaded = new EventEmitter<string>();
  @Output() imageChanged = new EventEmitter<string>();

  imageChangedEvent: Event | null = null;
  croppedImageUrl: string = '';
  croppedBlob: Blob | null = null;

  zoom: number = 1;

  isUploading: boolean = false;
  progress: number = 0;
  message: string = '';
  error: string = '';

  constructor(
    private http: HttpClient
  ) {}

  onFileSelected(event: Event): void {
    this.error = '';
    this.message = '';
    this.progress = 0;
    this.croppedBlob = null;
    this.croppedImageUrl = '';
    this.zoom = 1;
    this.imageChangedEvent = event;
  }

  imageCropped(event: ImageCroppedEvent): void {
    this.croppedImageUrl = event.objectUrl || '';
    this.croppedBlob = event.blob || null;

    this.imageChanged.emit(this.croppedImageUrl);
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
    this.imageChangedEvent = null;
    this.croppedImageUrl = '';
    this.croppedBlob = null;
    this.zoom = 1;
    this.progress = 0;
    this.message = '';
    this.error = '';
    this.isUploading = false;
    this.imageChanged.emit('');
  }
}
