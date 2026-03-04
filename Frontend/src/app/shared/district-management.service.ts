import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DistrictManagementService {
  private baseUrl = `${environment.apiUrl}/DistrictManagement`;

  constructor(private http: HttpClient) { }

  getProvinces(): Observable<any> {
    return this.http.get(`${this.baseUrl}/provinces`);
  }

  getDistrictsByProvince(provinceId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/province/${provinceId}`);
  }

  getDistrict(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  createDistrict(data: any): Observable<any> {
    return this.http.post(this.baseUrl, data);
  }

  updateDistrict(id: number, data: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, data);
  }

  deleteDistrict(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  activateDistrict(id: number): Observable<any> {
    return this.http.patch(`${this.baseUrl}/${id}/activate`, {});
  }
}
