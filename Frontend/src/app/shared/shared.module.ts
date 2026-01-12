import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NgSelectModule } from '@ng-select/ng-select';

import { DocumentViewerComponent } from './document-viewer/document-viewer.component';
import { ProfileImageCropperComponent } from './profile-image-cropper/profile-image-cropper.component';
import { ProfileImageCropperDialogComponent } from './profile-image-cropper/profile-image-cropper-dialog.component';
import { MultiCalendarDatepickerComponent } from './multi-calendar-datepicker/multi-calendar-datepicker.component';
import { CalendarSelectorComponent } from './calendar-selector/calendar-selector.component';
import { CalendarDatePipe } from './calendar-date.pipe';

// RBAC Directives
import {
  HasPermissionDirective,
  HasRoleDirective,
  HasAnyRoleDirective,
  CanAccessModuleDirective,
  IsViewOnlyDirective,
  CanEditDirective,
  CanEditRecordDirective,
  IsAdminDirective
} from './directives/rbac.directive';

// Numeric Input Directive for Dari/Pashto number support
import { NumericInputDirective } from './directives/numeric-input.directive';

// Dialog components moved from AuthModule for global availability
import { ChangepasswordComponent } from '../auth/changepassword/changepassword.component';
import { ResetpasswordComponent } from '../auth/resetpassword/resetpassword.component';
import { LockuserComponent } from '../auth/lockuser/lockuser.component';

@NgModule({
  declarations: [
    DocumentViewerComponent,
    ProfileImageCropperComponent,
    ProfileImageCropperDialogComponent,
    MultiCalendarDatepickerComponent,
    CalendarSelectorComponent,
    CalendarDatePipe,
    // RBAC Directives
    HasPermissionDirective,
    HasRoleDirective,
    HasAnyRoleDirective,
    CanAccessModuleDirective,
    IsViewOnlyDirective,
    CanEditDirective,
    CanEditRecordDirective,
    IsAdminDirective,
    // Numeric Input Directive
    NumericInputDirective,
    // Dialog components
    ChangepasswordComponent,
    ResetpasswordComponent,
    LockuserComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    ImageCropperModule,
    NgSelectModule
  ],
  exports: [
    DocumentViewerComponent,
    ProfileImageCropperComponent,
    MultiCalendarDatepickerComponent,
    CalendarSelectorComponent,
    CalendarDatePipe,
    // RBAC Directives
    HasPermissionDirective,
    HasRoleDirective,
    HasAnyRoleDirective,
    CanAccessModuleDirective,
    IsViewOnlyDirective,
    CanEditDirective,
    CanEditRecordDirective,
    IsAdminDirective,
    // Numeric Input Directive
    NumericInputDirective,
    // Export dialog components so they can be used anywhere
    ChangepasswordComponent,
    ResetpasswordComponent,
    LockuserComponent
  ]
})
export class SharedModule {}
