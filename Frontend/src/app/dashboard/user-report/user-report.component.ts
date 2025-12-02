import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { environment } from 'src/app/environments/environment';

@Component({
  selector: 'app-user-report',
  templateUrl: './user-report.component.html',
  styleUrls: ['./user-report.component.scss']
})
export class UserReportComponent {
  topUsers!: any[];
  vehicleTopUsers!:any[];
  constructor(private http: HttpClient) {}
  ngOnInit() {
    this.http.get<any>(environment.apiURL + '/Dashboard/GetTopUsersSummary').subscribe(data => {
      this.topUsers = data.topUsers;
    });

    this.http.get<any>(environment.apiURL + '/Dashboard/GetVehicleTopUsersSummary').subscribe(data => {
      this.vehicleTopUsers = data.topUsers;
    });
  }
}
