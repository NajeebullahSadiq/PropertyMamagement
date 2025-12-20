import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { ImageCropperModule } from 'ngx-image-cropper';

import { DocumentViewerComponent } from './document-viewer/document-viewer.component';
import { ProfileImageCropperComponent } from './profile-image-cropper/profile-image-cropper.component';
import { ProfileImageCropperDialogComponent } from './profile-image-cropper/profile-image-cropper-dialog.component';

@NgModule({
  declarations: [
    DocumentViewerComponent,
    ProfileImageCropperComponent,
    ProfileImageCropperDialogComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    ImageCropperModule
  ],
  exports: [
    DocumentViewerComponent,
    ProfileImageCropperComponent
  ]
})
export class SharedModule {}
