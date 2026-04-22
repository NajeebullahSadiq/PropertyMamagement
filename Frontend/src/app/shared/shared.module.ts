import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NgSelectModule } from '@ng-select/ng-select';

import { DocumentViewerComponent } from './document-viewer/document-viewer.component';
import { ProfileImageCropperComponent } from './profile-image-cropper/profile-image-cropper.component';
import { ProfileImageCropperDialogComponent } from './profile-image-cropper/profile-image-cropper-dialog.component';
import { MultiCalendarDatepickerComponent } from './multi-calendar-datepicker/multi-calendar-datepicker.component';
import { CalendarSelectorComponent } from './calendar-selector/calendar-selector.component';
import { CalendarDatePipe } from './calendar-date.pipe';
import { DeleteConfirmationDialogComponent } from './delete-confirmation-dialog/delete-confirmation-dialog.component';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { LoadingSkeletonComponent } from './loading-skeleton/loading-skeleton.component';
import { EmptyStateComponent } from './empty-state/empty-state.component';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';

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
    DeleteConfirmationDialogComponent,
    ConfirmationDialogComponent,
    LoadingSkeletonComponent,
    EmptyStateComponent,
    BreadcrumbComponent,
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
    MatIconModule,
    RouterModule,
    ImageCropperModule,
    NgSelectModule
  ],
  exports: [
    DocumentViewerComponent,
    ProfileImageCropperComponent,
    MultiCalendarDatepickerComponent,
    CalendarSelectorComponent,
    CalendarDatePipe,
    DeleteConfirmationDialogComponent,
    ConfirmationDialogComponent,
    LoadingSkeletonComponent,
    EmptyStateComponent,
    BreadcrumbComponent,
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
