import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { AuthService } from 'src/app/shared/auth.service';

@Component({
  selector: 'app-lockuser',
  templateUrl: './lockuser.component.html',
  styleUrls: ['./lockuser.component.scss']
})
export class LockuserComponent extends BaseComponent {
  userName: string='';
  isLocked: boolean=true;
  users:any;
  constructor(private service:AuthService,private toastr: ToastrService,public dialogRef: MatDialogRef<LockuserComponent>) {
    super();
  }
  ngOnInit(): void {
    this.service.getUsers().pipe(takeUntil(this.destroy$)).subscribe(res => {
      this.users = res;
     });
  }
  lockUser() {
    this.service.lockUser(this.userName, this.isLocked).subscribe(
      () => {
       this.toastr.success("عملیه موفقانه اجرا گردید")
      },
      (error) => {
        console.error('Error locking/unlocking user:', error);
        this.toastr.error('Error locking/unlocking user:', error);
        // Handle error cases or show error messages
      }
    );
  }
  close(){
    this.dialogRef.close();
  }
}
