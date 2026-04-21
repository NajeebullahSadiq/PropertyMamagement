import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-user-report',
  templateUrl: './user-report.component.html',
  styleUrls: ['./user-report.component.scss']
})
export class UserReportComponent extends BaseComponent {
  topUsers!: any[];
  vehicleTopUsers!:any[];
  constructor(private http: HttpClient) {
    super();
  }
  ngOnInit() {
    this.http.get<any>(environment.apiURL + '/Dashboard/GetTopUsersSummary').pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.topUsers = data.topUsers;
    });

    this.http.get<any>(environment.apiURL + '/Dashboard/GetVehicleTopUsersSummary').pipe(takeUntil(this.destroy$)).subscribe(data => {
      this.vehicleTopUsers = data.topUsers;
    });
  }
}
