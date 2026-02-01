import { AfterViewInit, ChangeDetectorRef, Component, QueryList, ViewChild, ViewChildren, ViewEncapsulation } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Router } from '@angular/router';
import { MatSidenav } from '@angular/material/sidenav';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { ChangepasswordComponent } from 'src/app/auth/changepassword/changepassword.component';
import { ResetpasswordComponent } from 'src/app/auth/resetpassword/resetpassword.component';
import { LockuserComponent } from 'src/app/auth/lockuser/lockuser.component';
import { AuthService } from 'src/app/shared/auth.service';
import { RbacService, UserRoles } from 'src/app/shared/rbac.service';
import { PropertydetailsComponent } from 'src/app/estate/propertydetails/propertydetails.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-masterlayout',
  templateUrl: './masterlayout.component.html',
  styleUrls: ['./masterlayout.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class MasterlayoutComponent implements AfterViewInit {
  filePath: string = 'assets/img/avatar.png';
  userDetails: any = [];
  baseUrl = environment.apiURL + '/';
  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;
  userRole: string = '';
  userRoleDari: string = '';
  
  // Module access flags
  canAccessDashboard = false;
  canAccessCompany = false;
  canAccessProperty = false;
  canAccessVehicle = false;
  canAccessReports = false;
  canAccessUsers = false;
  canAccessSecurities = false;
  canAccessPetitionWriter = false;
  canAccessActivityMonitoring = false;
  canAccessVerification = false;
  isViewOnly = false;
  
  // Module create/edit flags
  canCreateCompany = false;
  canCreateProperty = false;
  canCreateVehicle = false;

  // Dropdown menu states
  isUserMenuOpen = false;
  isLangMenuOpen = false;

  ngAfterViewInit() {
    this.observer.observe(['(max-width: 800px)']).subscribe((res) => {
      if (res.matches) {
        this.sidenav.mode = 'over';
        this.sidenav.close();
      } else {
        this.sidenav.mode = 'side';
        this.sidenav.open();
      }
      this.cdr.detectChanges();
    });
  }

  constructor(
    private observer: BreakpointObserver,
    private router: Router,
    public translate: TranslateService,
    public dialog: MatDialog,
    public service: AuthService,
    public rbacService: RbacService,
    private cdr: ChangeDetectorRef
  ) {
    translate.addLangs(['English', 'دری']);
    translate.setDefaultLang('دری');
  }

  ngOnInit(): void {
    this.service.getCurrentUserProfile().subscribe(
      res => {
        this.userDetails = res;
        if (this.userDetails.photoPath != '') {
          this.filePath = this.baseUrl + this.userDetails.photoPath;
        }
      },
      err => {
        console.log(err);
      },
    );

    // Load RBAC permissions
    this.loadUserPermissions();
  }

  private loadUserPermissions(): void {
    // Get role from token
    this.userRole = this.rbacService.getCurrentRole();
    this.userRoleDari = this.rbacService.getRoleDari(this.userRole);
    
    // Set module access flags
    this.canAccessDashboard = this.rbacService.canAccessModule('dashboard');
    this.canAccessCompany = this.rbacService.canAccessModule('company');
    this.canAccessProperty = this.rbacService.canAccessModule('property');
    this.canAccessVehicle = this.rbacService.canAccessModule('vehicle');
    this.canAccessReports = this.rbacService.canAccessModule('reports');
    this.canAccessUsers = this.rbacService.isAdmin();
    this.canAccessSecurities = this.rbacService.canAccessModule('securities');
    this.canAccessPetitionWriter = this.rbacService.canAccessModule('petitionWriter');
    this.canAccessActivityMonitoring = this.rbacService.canAccessModule('activityMonitoring');
    this.canAccessVerification = this.rbacService.canAccessModule('verification');
    this.isViewOnly = this.rbacService.isViewOnly();
    
    // Set module create/edit flags
    this.canCreateCompany = this.rbacService.canCreateCompany();
    this.canCreateProperty = this.rbacService.canCreateProperty();
    this.canCreateVehicle = this.rbacService.canCreateVehicle();
  }

  onLogout() {
    this.closeAllMenus();
    localStorage.removeItem('token');
    this.rbacService.clearUser();
    this.router.navigate(['/Auth']);
  }

  navigateToRegister() {
    this.closeAllMenus();
    this.router.navigate(['/Auth/Register']);
  }

  setlang() {
    this.closeAllMenus();
    this.translate.use('دری');
  }

  setlangen() {
    this.closeAllMenus();
    this.translate.use('English');
  }

  // Dropdown menu methods
  toggleUserMenu(event: Event) {
    event.stopPropagation();
    this.isUserMenuOpen = !this.isUserMenuOpen;
    this.isLangMenuOpen = false;
  }

  toggleLangMenu(event: Event) {
    event.stopPropagation();
    this.isLangMenuOpen = !this.isLangMenuOpen;
    this.isUserMenuOpen = false;
  }

  closeAllMenus() {
    this.isUserMenuOpen = false;
    this.isLangMenuOpen = false;
  }

  handleImageError() {
    this.filePath = 'assets/img/avatar.png';
  }

  openDialog(): void {
    this.closeAllMenus();
    this.dialog.open(ChangepasswordComponent, {
      width: '500px',
      maxWidth: '95vw',
      hasBackdrop: true,
      disableClose: false
    });
  }

  openReset(): void {
    this.closeAllMenus();
    this.dialog.open(ResetpasswordComponent, {
      width: '500px',
      maxWidth: '95vw',
      hasBackdrop: true,
      disableClose: false
    });
  }

  openLockuser(): void {
    this.closeAllMenus();
    this.dialog.open(LockuserComponent, {
      width: '500px',
      maxWidth: '95vw',
      hasBackdrop: true,
      disableClose: false
    });
  }
}
