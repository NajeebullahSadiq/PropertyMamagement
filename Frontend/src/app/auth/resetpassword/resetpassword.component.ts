import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { loginModel } from 'src/app/models/loginModel';
import { AuthService } from 'src/app/shared/auth.service';


@Component({
  selector: 'app-resetpassword',
  templateUrl: './resetpassword.component.html',
  styleUrls: ['./resetpassword.component.scss']
})
export class ResetpasswordComponent implements OnInit {
  users:any;
  constructor(private service:AuthService,private toastr: ToastrService,public dialogRef: MatDialogRef<ResetpasswordComponent>) { }

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
