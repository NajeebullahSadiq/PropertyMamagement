import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DuplicateCheckService {
  private estateSellerUrl = environment.apiUrl + '/SellerDetails';
  private vehicleSellerUrl = environment.apiUrl + '/VehiclesSub';

  constructor(private http: HttpClient) { }

  checkDuplicateSeller(firstName: string, fatherName: string, grandFather: string, propertyDetailsId: number, sellerId: number = 0): Observable<any> {
    const request = {
      firstName,
      fatherName,
      grandFather,
      propertyDetailsId,
      sellerId
    };
    return this.http.post(`${this.estateSellerUrl}/CheckDuplicateOwner`, request);
  }

  checkDuplicateBuyer(firstName: string, fatherName: string, grandFather: string, propertyDetailsId: number, buyerId: number = 0): Observable<any> {
    const request = {
      firstName,
      fatherName,
      grandFather,
      propertyDetailsId,
      sellerId: buyerId
    };
    return this.http.post(`${this.estateSellerUrl}/CheckDuplicateBuyer`, request);
  }

  checkDuplicateVehicleSeller(firstName: string, fatherName: string, grandFather: string, propertyDetailsId: number, sellerId: number = 0): Observable<any> {
    const request = {
      firstName,
      fatherName,
      grandFather,
      propertyDetailsId,
      sellerId
    };
    return this.http.post(`${this.vehicleSellerUrl}/CheckDuplicateSeller`, request);
  }

  checkDuplicateVehicleBuyer(firstName: string, fatherName: string, grandFather: string, propertyDetailsId: number, buyerId: number = 0): Observable<any> {
    const request = {
      firstName,
      fatherName,
      grandFather,
      propertyDetailsId,
      sellerId: buyerId
    };
    return this.http.post(`${this.vehicleSellerUrl}/CheckDuplicateBuyer`, request);
  }
}
