import { AfterViewInit, Component, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Router } from '@angular/router';
import { MatSidenav } from '@angular/material/sidenav';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { ChangepasswordComponent } from 'src/app/auth/changepassword/changepassword.component';
import { ResetpasswordComponent } from 'src/app/auth/resetpassword/resetpassword.component';
import { LockuserComponent } from 'src/app/auth/lockuser/lockuser.component';
import { AuthService } from 'src/app/shared/auth.service';
import { PropertydetailsComponent } from 'src/app/estate/propertydetails/propertydetails.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-masterlayout',
  templateUrl: './masterlayout.component.html',
  styleUrls: ['./masterlayout.component.scss']
})
export class MasterlayoutComponent implements AfterViewInit {
  filePath:string='assets/img/avatar.png';
  userDetails:any=[];
  baseUrl=environment.apiURL+'/';
  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;
  userRole:any;

  ngAfterViewInit() {
    this.observer.observe(['(max-width: 800px)']).subscribe((res) => {
      if (res.matches) {
        this.sidenav.mode = 'over';
        this.sidenav.close();
      } else {
        this.sidenav.mode = 'side';
        this.sidenav.open();
      }
    });
  
  }
  constructor(private observer: BreakpointObserver,private router: Router,public translate: TranslateService,public dialog: MatDialog,
    public service: AuthService) { 
    translate.addLangs(['English', 'دری']);
    translate.setDefaultLang('دری');
  }
  ngOnInit(): void {
    this.service.getCurrentUserProfile().subscribe(
      res => {
        this.userDetails = res;
        if(this.userDetails.photoPath!=''){
          this.filePath=this.baseUrl+this.userDetails.photoPath;
          console.log(this.baseUrl+this.userDetails.photoPath);
        }
      },
      err => {
        console.log(err);
      },
    );
    //Get Role of Users
    const token = localStorage.getItem('token');
    let userRole = '';
    
    if (token) {
      const payLoad = JSON.parse(window.atob(token.split('.')[1]));
      userRole = payLoad?.role || '';
    }
    this.userRole=userRole;
    //End
    
  }
  onLogout() {
    localStorage.removeItem('token');
    this.router.navigate(['/Auth']);
   
  }
  setlang(){
    this.translate.use('دری');
    // document.body.classList.add('rtl');
    // document.body.classList.remove('ltr');
    // document.body.dir = 'rtl';
  }
  setlangen(){
    this.translate.use('English');
   
  }
  handleImageError() {
    this.filePath = 'assets/img/avatar.png';
  }
  openDialog(): void {
    this.dialog.open(ChangepasswordComponent, {
     // width: '250px'
    });
  
  }
  openReset(): void {
    this.dialog.open(ResetpasswordComponent, {
    });
  }
  openLockuser(): void {
    this.dialog.open(LockuserComponent, {
       width: '300px'
    });
  }
 
}
