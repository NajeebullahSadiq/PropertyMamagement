import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';
import { environment } from 'src/app/environments/environment';

import { UploadComponent } from './upload/upload.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  filePath:string='assets/img/avatar.png';
   baseUrl=environment.apiURL+'/';
   imageName:string='';
   companylist:any;
   servicesDetails:any;
   roles:any;

   userDetails:any;
   userRole:any;

   @ViewChild(UploadComponent) childComponent!: UploadComponent;
   @ViewChild('secondDiv', { static: false }) secondDivRef!: ElementRef;
  constructor(public service: AuthService,private toastr: ToastrService) { }

  ngOnInit(): void {
    this.service.formModel.reset();
    this.service.getCompanies().subscribe(res => {
      this.companylist = res;
     });
     this.service.getRoles().subscribe(res => {
      this.roles = res;
     });
     //UserProfile
     this.service.getUserProfile().subscribe(
      res => {
        this.userDetails = res; 
      },
      err => {
        console.log(err);
      },
    );
    const token = localStorage.getItem('token');
    let userRole = '';
    
    if (token) {
      const payLoad = JSON.parse(window.atob(token.split('.')[1]));
      userRole = payLoad?.role || '';
    }
    this.userRole=userRole;

    //End
  }
  onSubmit() {
    this.service.photoPath=this.imageName;

    this.service.register().subscribe(
      (res: any) => {
        if (res.succeeded) {
          this.service.formModel.reset();
          this.toastr.success('New user created!', 'معلومات موفقانه ثبت سیستم گردید');
          this.callChildMethod();
          this.filePath='assets/img/avatar.png';
          
        } else {
          res.errors.forEach((element: { code: any; description: string | undefined; }) => {
            switch (element.code) {
              case 'DuplicateUserName':
                this.toastr.error('Username is already taken','Registration failed.');
                break;

              default:
              this.toastr.error(element.description,'Registration failed.');
                break;
            }
          });
        }
      },
      err => {
        console.log(err);
      }
    );
  }
 
  uploadFinished = (event:string) => { 
    this.imageName="Resources\\Images\\"+event;
    this.filePath=this.baseUrl+this.imageName;
   
  }
  callChildMethod() {
    // Call the method in the child component
    this.childComponent.childMethod();
  }
  onPropertyTypeChange() {
    if (this.service.formModel) {
      const roleControl = this.service.formModel.get('Role');
      const companyIdControl = this.service.formModel.get('CompanyId');
  
      if (roleControl && companyIdControl) {
        const selectedRole = roleControl.value;
  
        if (selectedRole === 'ADMIN') {
          companyIdControl.setValue(0);
          this.hideSecondDiv();
        } else if (selectedRole === 'REALESTATE') {
          this.showSecondDiv();
        }
      }
    }
  }

  private hideSecondDiv() {
    if (this.secondDivRef) {
      this.secondDivRef.nativeElement.style.display = 'none';
    }
  }

  private showSecondDiv() {
    if (this.secondDivRef) {
      this.secondDivRef.nativeElement.style.display = 'block';
    }
  }
}
