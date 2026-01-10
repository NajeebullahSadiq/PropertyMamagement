import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { RbacService } from 'src/app/shared/rbac.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private router: Router, private rbacService: RbacService) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    // Check if user is logged in
    if (!localStorage.getItem('token')) {
      this.router.navigate(['/Auth']);
      return false;
    }

    // Check for required roles
    const requiredRoles = route.data['roles'] as string[];
    if (requiredRoles && requiredRoles.length > 0) {
      if (!this.rbacService.hasAnyRole(requiredRoles)) {
        this.router.navigate(['/forbidden']);
        return false;
      }
    }

    // Check for required permissions
    const requiredPermissions = route.data['permissions'] as string[];
    if (requiredPermissions && requiredPermissions.length > 0) {
      if (!this.rbacService.hasAnyPermission(requiredPermissions)) {
        this.router.navigate(['/forbidden']);
        return false;
      }
    }

    // Check for module access
    const requiredModule = route.data['module'] as string;
    if (requiredModule) {
      if (!this.rbacService.canAccessModule(requiredModule)) {
        this.router.navigate(['/forbidden']);
        return false;
      }
    }

    return true;
  }
}

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private router: Router, private rbacService: RbacService) {}

  canActivate(): boolean {
    if (!localStorage.getItem('token')) {
      this.router.navigate(['/Auth']);
      return false;
    }

    if (!this.rbacService.isAdmin()) {
      this.router.navigate(['/forbidden']);
      return false;
    }

    return true;
  }
}

@Injectable({
  providedIn: 'root'
})
export class CompanyModuleGuard implements CanActivate {
  constructor(private router: Router, private rbacService: RbacService) {}

  canActivate(): boolean {
    if (!localStorage.getItem('token')) {
      this.router.navigate(['/Auth']);
      return false;
    }

    if (!this.rbacService.canAccessModule('company')) {
      this.router.navigate(['/forbidden']);
      return false;
    }

    return true;
  }
}

@Injectable({
  providedIn: 'root'
})
export class PropertyModuleGuard implements CanActivate {
  constructor(private router: Router, private rbacService: RbacService) {}

  canActivate(): boolean {
    if (!localStorage.getItem('token')) {
      this.router.navigate(['/Auth']);
      return false;
    }

    if (!this.rbacService.canAccessModule('property')) {
      this.router.navigate(['/forbidden']);
      return false;
    }

    return true;
  }
}

@Injectable({
  providedIn: 'root'
})
export class VehicleModuleGuard implements CanActivate {
  constructor(private router: Router, private rbacService: RbacService) {}

  canActivate(): boolean {
    if (!localStorage.getItem('token')) {
      this.router.navigate(['/Auth']);
      return false;
    }

    if (!this.rbacService.canAccessModule('vehicle')) {
      this.router.navigate(['/forbidden']);
      return false;
    }

    return true;
  }
}
