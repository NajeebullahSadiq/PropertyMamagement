import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { companydetails, companydetailsList } from '../models/companydetails';
import { companyowner } from '../models/companyowner';
import { companyOwnerAddress, companyOwnerAddressData } from '../models/companyOwnerAddress';
import { Guarantee } from '../models/Guarantee';
import { Guarantor } from '../models/Guarantor';
import { LicenseDetail } from '../models/LicenseDetail';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompnaydetailService {
  mainTableId: number=0;
  ownerId: number=0;
  private baseUrlowner=environment.apiURL+'/';
  private baseUrl = environment.apiURL + '/CompanyDetails';
  private baseUrldropbdownlist = environment.apiURL + '/SellerDetails';
  constructor(private http: HttpClient) { }

  updateMainTableId(id: number) {
    this.mainTableId = id;
  }
  addcompanies(compdetails: companydetails): Observable<companydetails> {
    return this.http.post<companydetails>(this.baseUrl, compdetails);
  }
  updatecompanies(propertyDetails: companydetails): Observable<companydetails> {
    const url = `${this.baseUrl}/${propertyDetails.id}`;
    return this.http.put<companydetails>(url, propertyDetails);
  }
  getIdentityTypes(){
    return this.http.get(this.baseUrldropbdownlist+'/getIdentityCardType');
  }
  getEducationLevel(){
    return this.http.get(this.baseUrldropbdownlist+'/getEducationLevel');
  }
  getAddressType(){
    return this.http.get(this.baseUrldropbdownlist+'/getAddressType');
  }
  getArea(){
    return this.http.get(this.baseUrldropbdownlist+'/getArea');
  }
  getGuaranteeType(){
    return this.http.get(this.baseUrldropbdownlist+'/getGuaranteeType');
  }

  addcompanyOwner(details: companyowner): Observable<companyowner> {
    return this.http.post<companyowner>(this.baseUrlowner+'CompanyOwner', details);
  }
  updateowner(propertyDetails: companyowner): Observable<companyowner> {
    const url = `${this.baseUrlowner+'CompanyOwner'}/${propertyDetails.id}`;
    return this.http.put<companyowner>(url, propertyDetails);
  }
  addcompanyOwnerAddress(details: companyOwnerAddress): Observable<companyOwnerAddress> {
    return this.http.post<companyOwnerAddress>(this.baseUrlowner+'CompanyOwnerAddress', details);
  }
  updateownerAddress(propertyDetails: companyOwnerAddress): Observable<companyOwnerAddress> {
    const url = `${this.baseUrlowner+'CompanyOwnerAddress'}/${propertyDetails.id}`;
    return this.http.put<companyOwnerAddress>(url, propertyDetails);
  }

  getOwnerAddressById(id: number): Observable<companyOwnerAddressData[]> {
    return this.http.get<companyOwnerAddressData[]>(this.baseUrlowner+'CompanyOwnerAddress/ownerAddress' +'/'+ id);
  }

  addcompanyGuaranator(details: Guarantor): Observable<Guarantor> {
    return this.http.post<Guarantor>(this.baseUrlowner+'Guaranator', details);
  }
  updateGuaranator(propertyDetails: Guarantor): Observable<Guarantor> {
    const url = `${this.baseUrlowner+'Guaranator'}/${propertyDetails.id}`;
    return this.http.put<Guarantor>(url, propertyDetails);
  }

  addLicenseDetails(details: LicenseDetail): Observable<LicenseDetail> {
    return this.http.post<LicenseDetail>(this.baseUrlowner+'LicenseDetail', details);
  }
  updateLicenseDetails(propertyDetails: LicenseDetail): Observable<LicenseDetail> {
    const url = `${this.baseUrlowner+'LicenseDetail'}/${propertyDetails.id}`;
    return this.http.put<LicenseDetail>(url, propertyDetails);
  }
  addGuarantee(details: Guarantee): Observable<Guarantee> {
    return this.http.post<Guarantee>(this.baseUrlowner+'Guarantee', details);
  }
  updateGuarantee(propertyDetails: Guarantee): Observable<Guarantee> {
    const url = `${this.baseUrlowner+'Guarantee'}/${propertyDetails.id}`;
    return this.http.put<Guarantee>(url, propertyDetails);
  }

  getComapaniesList(): Observable<companydetailsList[]> {
    return this.http.get<companydetailsList[]>(this.baseUrl);
  }
  getexpiredList(): Observable<companydetailsList[]> {
    return this.http.get<companydetailsList[]>(this.baseUrl+'/getexpired');
  }
  getCompanyById(id: number): Observable<companydetails[]> {
    return this.http.get<companydetails[]>(this.baseUrl+'/'+ id);
  }

  getOwnerById(id: number): Observable<companyowner[]> {
    return this.http.get<companyowner[]>(this.baseUrlowner+'CompanyOwner/'+ id);
  }
  getGuaranatorById(id: number): Observable<Guarantor[]> {
    return this.http.get<Guarantor[]>(this.baseUrlowner+'Guaranator/'+ id);
  }
  getLicenseById(id: number): Observable<LicenseDetail[]> {
    return this.http.get<LicenseDetail[]>(this.baseUrlowner+'LicenseDetail/'+ id);
  }
  getGuaranteeById(id: number): Observable<Guarantee[]> {
    return this.http.get<Guarantee[]>(this.baseUrlowner+'Guarantee/'+ id);
  }
}
