import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VehiclesDetailsList } from '../models/PropertyDetail';
import { VehicleDetails } from '../models/vehicle';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  mainTableId: number=0;
  private baseUrl = environment.apiUrl + '/Vehicles';
  constructor(private http: HttpClient) { }

  updateMainTableId(id: number) {
    this.mainTableId = id;
  }
  addVehicles(propertyDetails: VehicleDetails): Observable<VehicleDetails> {
    return this.http.post<VehicleDetails>(this.baseUrl, propertyDetails);
  }
  updateVehicleDetails(propertyDetails: VehicleDetails): Observable<VehicleDetails> {
    const url = `${this.baseUrl}/${propertyDetails.id}`;
    return this.http.put<VehicleDetails>(url, propertyDetails);
  }
  getPropertyDetails(page: number = 1, pageSize: number = 20, search?: string): Observable<any> {
    let url = `${this.baseUrl}?page=${page}&pageSize=${pageSize}`;
    if (search && search.trim()) {
      url += `&search=${encodeURIComponent(search.trim())}`;
    }
    return this.http.get<any>(url);
  }
  getPropertyDetailsById(id: number): Observable<VehicleDetails[]> {
    return this.http.get<VehicleDetails[]>(this.baseUrl +'/'+ id);
  }
  downloadFile(file:any) {
    return this.http.get(environment.apiUrl + '/' + file, { responseType: 'blob' });
  }
  getVehiclePropertyPrintData(id: any): Observable<any> {
    const url = `${this.baseUrl}/GetPrintRecord/${id}`;
    return this.http.get(url);
  }
  
  /**
   * Get complete vehicle details for view page (read-only)
   */
  getVehicleViewById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/View/${id}`);
  }

  /**
   * Upload vehicle document (seta)
   */
  uploadVehicleDocument(formData: FormData): Observable<any> {
    return this.http.post(`${this.baseUrl}/SetaDocument/upload`, formData);
  }
}
