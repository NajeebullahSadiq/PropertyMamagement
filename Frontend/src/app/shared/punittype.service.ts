import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PunitType } from '../models/PunitType';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PunittypeService {
  private baseUrl = environment.apiUrl + '/PropertyDetails/getunitType';
  constructor(private http: HttpClient) { }

  getUnitTypes(): Observable<PunitType[]> {
    return this.http.get<PunitType[]>(this.baseUrl);
  }
}
