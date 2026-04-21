import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { AuthService } from 'src/app/shared/auth.service';


@Component({
  selector: 'app-changepassword',
  templateUrl: './changepassword.component.html',
  styleUrls: ['./changepassword.component.scss']
})
export class ChangepasswordComponent extends BaseComponent implements OnInit {

  model: any = {};
  showCurrentPassword = false;
  showNewPassword = false;

  constructor(private service: AuthService, private toastr: ToastrService, public dialogRef: MatDialogRef<ChangepasswordComponent>) {
    super();
  }

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
