import { Component, OnInit } from '@angular/core';
import { GeolocationService } from '../authSetting/geolocation.service';

@Component({
  selector: 'app-access-denied',
  templateUrl: './access-denied.component.html',
  styleUrls: ['./access-denied.component.scss']
})
export class AccessDeniedComponent implements OnInit {
  userCountry: string = '';
  userCountryCode: string = '';
  isLoading = true;

  constructor(private geolocationService: GeolocationService) {}

  ngOnInit(): void {
    this.geolocationService.getUserCountry().subscribe(
      (data) => {
        if (data) {
          this.userCountry = data.country_name || 'Unknown';
          this.userCountryCode = data.country_code || 'XX';
        }
        this.isLoading = false;
      },
      (error) => {
        console.error('Error fetching country info:', error);
        this.isLoading = false;
      }
    );
  }
}
