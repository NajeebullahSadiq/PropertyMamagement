import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

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
    this.http.get<any>('http://localhost:5143/api/Dashboard/GetTopUsersSummary').subscribe(data => {
      this.topUsers = data.topUsers;
    });

    this.http.get<any>('http://localhost:5143/api/Dashboard/GetVehicleTopUsersSummary').subscribe(data => {
      this.vehicleTopUsers = data.topUsers;
    });
  }
}
