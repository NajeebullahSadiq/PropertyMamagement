import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PetitionWriterActivityLocationService {
  private baseUrl = `${environment.apiURL}/PetitionWriterActivityLocation`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<any> {
    return this.http.get(this.baseUrl);
  }

  getAllForManagement(): Observable<any> {
    return this.http.get(`${this.baseUrl}/manage`);
  }

  create(data: any): Observable<any> {
    return this.http.post(this.baseUrl, data);
  }

  update(id: number, data: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, data);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  activate(id: number): Observable<any> {
    return this.http.patch(`${this.baseUrl}/${id}/activate`, {});
  }
}
