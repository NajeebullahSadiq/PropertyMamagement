import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

import { UserListComponent } from './user-list/user-list.component';
import { UserEditDialogComponent } from './user-edit-dialog/user-edit-dialog.component';
import { PermissionManagementComponent } from './permission-management/permission-management.component';
import { RoleAssignDialogComponent } from './permission-management/role-assign-dialog/role-assign-dialog.component';
import { RoleLabelPipe } from './permission-management/role-label.pipe';
import { RolePermissionsComponent } from './role-permissions/role-permissions.component';
import { GroupCountPipe } from './role-permissions/group-count.pipe';

const routes: Routes = [
  { path: '', component: UserListComponent },
  { path: 'permissions', component: PermissionManagementComponent },
  { path: 'role-permissions', component: RolePermissionsComponent }
];

@NgModule({
  declarations: [
    UserListComponent,
    UserEditDialogComponent,
    PermissionManagementComponent,
    RoleAssignDialogComponent,
    RoleLabelPipe,
    RolePermissionsComponent,
    GroupCountPipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgxPaginationModule,
    MatDialogModule,
    MatIconModule,
    RouterModule.forChild(routes)
  ]
})
export class UsersModule { }
