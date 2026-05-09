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
  private companyOwnerUrl = environment.apiUrl + '/CompanyOwner';
  private guarantorUrl = environment.apiUrl + '/Guaranator';

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

  checkDuplicateCompanyOwner(firstName: string, fatherName: string, grandFatherName: string, electronicNationalIdNumber: string, ownerId: number = 0): Observable<any> {
    const request = {
      firstName,
      fatherName,
      grandFatherName,
      electronicNationalIdNumber,
      ownerId
    };
    return this.http.post(`${this.companyOwnerUrl}/CheckDuplicateOwner`, request);
  }

  checkDuplicateGuarantor(electronicNationalIdNumber: string, guarantorId: number = 0): Observable<any> {
    const request = {
      electronicNationalIdNumber,
      guarantorId
    };
    return this.http.post(`${this.guarantorUrl}/CheckDuplicateGuarantor`, request);
  }

  checkDuplicateSetSerialNumber(setSerialNumber: string, guarantorId: number = 0): Observable<any> {
    const request = {
      setSerialNumber,
      guarantorId
    };
    return this.http.post(`${this.guarantorUrl}/CheckDuplicateSetSerialNumber`, request);
  }

  checkDuplicatePropertyDocumentNumber(propertyDocumentNumber: number, guarantorId: number = 0): Observable<any> {
    const request = {
      propertyDocumentNumber,
      guarantorId
    };
    return this.http.post(`${this.guarantorUrl}/CheckDuplicatePropertyDocumentNumber`, request);
  }
}
