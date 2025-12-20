import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ImageCroppedEvent } from 'ngx-image-cropper';

export interface ProfileImageCropperDialogData {
  file: File;
  roundCropper: boolean;
  resizeToWidth?: number;
}

export interface ProfileImageCropperDialogResult {
  blob: Blob;
}

@Component({
  selector: 'app-profile-image-cropper-dialog',
  templateUrl: './profile-image-cropper-dialog.component.html',
  styleUrls: ['./profile-image-cropper-dialog.component.scss']
})
export class ProfileImageCropperDialogComponent implements OnInit {
  imageBase64: string = '';
  zoom: number = 1;

  croppedBlob: Blob | null = null;
  croppedPreviewUrl: string = '';

  error: string = '';

  constructor(
    private dialogRef: MatDialogRef<ProfileImageCropperDialogComponent, ProfileImageCropperDialogResult | null>,
    @Inject(MAT_DIALOG_DATA) public data: ProfileImageCropperDialogData
  ) {}

  ngOnInit(): void {
    const reader = new FileReader();
    reader.onload = () => {
      this.imageBase64 = (reader.result as string) || '';
    };
    reader.onerror = () => {
      this.error = 'خطا در خواندن فایل تصویر';
    };
    reader.readAsDataURL(this.data.file);
  }

  imageCropped(event: ImageCroppedEvent): void {
    this.croppedPreviewUrl = event.objectUrl || '';
    this.croppedBlob = event.blob || null;
  }

  cancel(): void {
    this.dialogRef.close(null);
  }

  save(): void {
    this.error = '';

    if (!this.croppedBlob) {
      this.error = 'ابتدا تصویر را برش کنید';
      return;
    }

    this.dialogRef.close({
      blob: this.croppedBlob
    });
  }
}
