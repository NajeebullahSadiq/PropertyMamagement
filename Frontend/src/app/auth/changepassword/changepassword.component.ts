import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';


@Component({
  selector: 'app-changepassword',
  templateUrl: './changepassword.component.html',
  styleUrls: ['./changepassword.component.scss']
})
export class ChangepasswordComponent implements OnInit {

  model: any = {};
  showCurrentPassword = false;
  showNewPassword = false;

  constructor(private service: AuthService,public dialogRef: MatDialogRef<ChangepasswordComponent>,private toastr: ToastrService) { }

  changePassword() {
    this.service.changePassword(this.model)
      .subscribe(
        () => {
          this.toastr.success('رمز عبور با موفقیت تغییر یافت');
          this.dialogRef.close();
        },
        (error: any) => {
          console.error(error);
          this.toastr.error('مشکلی در تغییر رمز عبور بوجود آمد');
        }
      );
  }
  ngOnInit(): void {
  }
  close(){
    this.dialogRef.close();
  }

}
