import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PunitType } from '../models/PunitType';

@Injectable({
  providedIn: 'root'
})
export class PunittypeService {
  private baseUrl = 'http://localhost:5143/api/PropertyDetails/getunitType';
  constructor(private http: HttpClient) { }

  getUnitTypes(): Observable<PunitType[]> {
    return this.http.get<PunitType[]>(this.baseUrl);
  }
}
