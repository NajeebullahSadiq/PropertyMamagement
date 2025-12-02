import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PropertyDetails, PropertyDetailsList } from '../models/PropertyDetail';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class PropertyService {

  private baseUrl = environment.apiUrl + '/PropertyDetails';
  private baseUrl2 = environment.apiUrl;
  mainTableId: number=0;
  constructor(private http: HttpClient) { }

  addProperty(propertyData: any): Observable<any> {
    return this.http.post(this.baseUrl+'/PropertyDetails', propertyData);
  }
  updateMainTableId(id: number) {
    this.mainTableId = id;
  }
  
  getPropertyType(){
    return this.http.get(this.baseUrl+'/getpropertyType');
  }
  getTransactionType(){
    return this.http.get(this.baseUrl+'/gettransactionType');
  }
  getUnitTpe(){
    return this.http.get(this.baseUrl+'/getunitType');
  }
  getPropertyDetails(): Observable<PropertyDetailsList[]> {
    return this.http.get<PropertyDetailsList[]>(this.baseUrl);
  }

  addPropertyDetails(propertyDetails: PropertyDetails): Observable<PropertyDetails> {
    return this.http.post<PropertyDetails>(this.baseUrl, propertyDetails);
  }

  updatePropertyDetails(propertyDetails: PropertyDetails): Observable<PropertyDetails> {
    const url = `${this.baseUrl}/${propertyDetails.id}`;
    return this.http.put<PropertyDetails>(url, propertyDetails);
  }

  deletePropertyDetails(id: number): Observable<unknown> {
    const url = `${this.baseUrl}/${id}`;
    return this.http.delete(url);
  }
  getPropertyDetailsById(id: number): Observable<PropertyDetails[]> {
    return this.http.get<PropertyDetails[]>(this.baseUrl +'/'+ id);
  }
  downloadFile(file:any) {
    return this.http.get(environment.apiUrl + '/' + file, { responseType: 'blob' });
  }
  getPropertyPrintData(id: any): Observable<any> {
    const url = `${this.baseUrl}/GetPrintRecord/${id}`;
    return this.http.get(url);
  }
  getLicensePrintData(id: any): Observable<any> {
    const url = `${this.baseUrl2}/LicenseDetail/GetLicenseView/${id}`;
    return this.http.get(url);
  }

  
}
