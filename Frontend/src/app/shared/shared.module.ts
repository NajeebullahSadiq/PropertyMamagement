import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { ImageCropperModule } from 'ngx-image-cropper';

import { DocumentViewerComponent } from './document-viewer/document-viewer.component';
import { ProfileImageCropperComponent } from './profile-image-cropper/profile-image-cropper.component';
import { ProfileImageCropperDialogComponent } from './profile-image-cropper/profile-image-cropper-dialog.component';
import { MultiCalendarDatepickerComponent } from './multi-calendar-datepicker/multi-calendar-datepicker.component';
import { CalendarSelectorComponent } from './calendar-selector/calendar-selector.component';
import { CalendarDatePipe } from './calendar-date.pipe';

@NgModule({
  declarations: [
    DocumentViewerComponent,
    ProfileImageCropperComponent,
    ProfileImageCropperDialogComponent,
    MultiCalendarDatepickerComponent,
    CalendarSelectorComponent,
    CalendarDatePipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    ImageCropperModule
  ],
  exports: [
    DocumentViewerComponent,
    ProfileImageCropperComponent,
    MultiCalendarDatepickerComponent,
    CalendarSelectorComponent,
    CalendarDatePipe
  ]
})
export class SharedModule {}
