import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { loginModel } from 'src/app/models/loginModel';
import { AuthService } from 'src/app/shared/auth.service';


@Component({
  selector: 'app-resetpassword',
  templateUrl: './resetpassword.component.html',
  styleUrls: ['./resetpassword.component.scss']
})
export class ResetpasswordComponent extends BaseComponent implements OnInit {
  users:any;
  showPassword = false;
  
  constructor(private service:AuthService,private toastr: ToastrService,public dialogRef: MatDialogRef<ResetpasswordComponent>) {
    super();
  }

  ngOnInit(): void {
    this.service.getUsers().subscribe(res => {
      this.users = res;
     });
  }
  resetPassword(model: loginModel) {
    this.service.resetPassword(model).subscribe(
      (response) => {
        // Handle success
        this.toastr.success("رمز جدید به این کاربر موفقانه تفویض گردید");
        this.close();
      },
      (error) => {
        // Handle error
        console.error(error);
      }
    );
  }
  close(){
    this.dialogRef.close();
  }
}
