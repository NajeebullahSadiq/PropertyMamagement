import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { RbacService, UserRoles } from '../rbac.service';
import { Subscription } from 'rxjs';

/**
 * Directive to show/hide elements based on user permissions
 * Usage: *hasPermission="'company.create'"
 */
@Directive({
  selector: '[hasPermission]'
})
export class HasPermissionDirective implements OnInit {
  private permission: string = '';

  @Input() set hasPermission(permission: string) {
    this.permission = permission;
    this.updateView();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private rbacService: RbacService
  ) {}

  ngOnInit() {
    this.updateView();
  }

  private updateView() {
    this.viewContainer.clear();
    if (this.rbacService.hasPermission(this.permission)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}

/**
 * Directive to show/hide elements based on user roles
 * Usage: *hasRole="'ADMIN'"
 */
@Directive({
  selector: '[hasRole]'
})
export class HasRoleDirective implements OnInit {
  private role: string = '';

  @Input() set hasRole(role: string) {
    this.role = role;
    this.updateView();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private rbacService: RbacService
  ) {}

  ngOnInit() {
    this.updateView();
  }

  private updateView() {
    this.viewContainer.clear();
    if (this.rbacService.hasRole(this.role)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}

/**
 * Directive to show/hide elements based on any of the specified roles
 * Usage: *hasAnyRole="['ADMIN', 'COMPANY_REGISTRAR']"
 */
@Directive({
  selector: '[hasAnyRole]'
})
export class HasAnyRoleDirective implements OnInit {
  private roles: string[] = [];

  @Input() set hasAnyRole(roles: string[]) {
    this.roles = roles;
    this.updateView();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private rbacService: RbacService
  ) {}

  ngOnInit() {
    this.updateView();
  }

  private updateView() {
    this.viewContainer.clear();
    if (this.rbacService.hasAnyRole(this.roles)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}

/**
 * Directive to show/hide elements based on module access
 * Usage: *canAccessModule="'company'"
 */
@Directive({
  selector: '[canAccessModule]'
})
export class CanAccessModuleDirective implements OnInit {
  private module: string = '';

  @Input() set canAccessModule(module: string) {
    this.module = module;
    this.updateView();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private rbacService: RbacService
  ) {}

  ngOnInit() {
    this.updateView();
  }

  private updateView() {
    this.viewContainer.clear();
    if (this.rbacService.canAccessModule(this.module)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}

/**
 * Directive to show elements only for view-only users (Authority, License Reviewer)
 * Usage: *isViewOnly
 */
@Directive({
  selector: '[isViewOnly]'
})
export class IsViewOnlyDirective implements OnInit {
  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private rbacService: RbacService
  ) {}

  ngOnInit() {
    this.viewContainer.clear();
    if (this.rbacService.isViewOnly()) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}

/**
 * Directive to hide elements for view-only users
 * Usage: *canEdit
 */
@Directive({
  selector: '[canEdit]'
})
export class CanEditDirective implements OnInit {
  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private rbacService: RbacService
  ) {}

  ngOnInit() {
    this.viewContainer.clear();
    if (!this.rbacService.isViewOnly()) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}

/**
 * Directive to check if user can edit a specific record (ownership check)
 * Usage: *canEditRecord="record.createdBy"
 */
@Directive({
  selector: '[canEditRecord]'
})
export class CanEditRecordDirective implements OnInit {
  private createdBy: string = '';

  @Input() set canEditRecord(createdBy: string) {
    this.createdBy = createdBy;
    this.updateView();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private rbacService: RbacService
  ) {}

  ngOnInit() {
    this.updateView();
  }

  private updateView() {
    this.viewContainer.clear();
    if (this.rbacService.canEditRecord(this.createdBy)) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}

/**
 * Directive to show elements only for admin users
 * Usage: *isAdmin
 */
@Directive({
  selector: '[isAdmin]'
})
export class IsAdminDirective implements OnInit {
  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private rbacService: RbacService
  ) {}

  ngOnInit() {
    this.viewContainer.clear();
    if (this.rbacService.isAdmin()) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}
