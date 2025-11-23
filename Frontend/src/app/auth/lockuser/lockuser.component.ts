import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/shared/auth.service';

@Component({
  selector: 'app-lockuser',
  templateUrl: './lockuser.component.html',
  styleUrls: ['./lockuser.component.scss']
})
export class LockuserComponent {
  userName: string='';
  isLocked: boolean=true;
  users:any;
  constructor(private service:AuthService,private toastr: ToastrService,public dialogRef: MatDialogRef<LockuserComponent>) {}
  ngOnInit(): void {
    this.service.getUsers().subscribe(res => {
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
