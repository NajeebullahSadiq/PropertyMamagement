import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { companydetails, companydetailsList } from '../models/companydetails';
import { companyowner } from '../models/companyowner';
import { companyOwnerAddress, companyOwnerAddressData } from '../models/companyOwnerAddress';
import { Guarantor } from '../models/Guarantor';
import { LicenseDetail } from '../models/LicenseDetail';
import { AccountInfo, AccountInfoData } from '../models/AccountInfo';
import { CancellationInfo, CancellationInfoData } from '../models/CancellationInfo';
import { environment } from 'src/environments/environment';

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
  getEducationLevel(){
    return this.http.get(this.baseUrldropbdownlist+'/getEducationLevel');
  }
  getAddressType(){
    return this.http.get(this.baseUrldropbdownlist+'/getAddressType');
  }
  getArea(){
    return this.http.get(this.baseUrldropbdownlist+'/getArea');
  }
  getProvinces(){
    return this.http.get(this.baseUrldropbdownlist+'/getProvinces');
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

  deleteGuarantor(companyId: number, guarantorId: number): Observable<any> {
    const url = `${this.baseUrlowner}Guaranator/${guarantorId}`;
    return this.http.delete(url);
  }

  addLicenseDetails(details: LicenseDetail): Observable<LicenseDetail> {
    return this.http.post<LicenseDetail>(this.baseUrlowner+'LicenseDetail', details);
  }
  updateLicenseDetails(propertyDetails: LicenseDetail): Observable<LicenseDetail> {
    const url = `${this.baseUrlowner+'LicenseDetail'}/${propertyDetails.id}`;
    return this.http.put<LicenseDetail>(url, propertyDetails);
  }

  getComapaniesList(search?: string): Observable<companydetailsList[]> {
    let url = this.baseUrl;
    if (search && search.trim()) {
      url += `?search=${encodeURIComponent(search.trim())}`;
    }
    return this.http.get<companydetailsList[]>(url);
  }
  getexpiredList(): Observable<companydetailsList[]> {
    return this.http.get<companydetailsList[]>(this.baseUrl+'/getexpired');
  }
  getCompanyById(id: number): Observable<companydetails[]> {
    return this.http.get<companydetails[]>(this.baseUrl+'/'+ id);
  }

  getCompanyViewById(id: number): Observable<any> {
    return this.http.get<any>(this.baseUrl + '/GetView/' + id);
  }

  getOwnerById(id: number): Observable<companyowner[]> {
    return this.http.get<companyowner[]>(this.baseUrlowner+'CompanyOwner/'+ id);
  }

  getOwnerAddressHistory(companyId: number): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrlowner+'CompanyOwner/addressHistory/'+ companyId);
  }

  getGuaranatorById(id: number): Observable<Guarantor[]> {
    return this.http.get<Guarantor[]>(this.baseUrlowner+'Guaranator/'+ id);
  }
  getLicenseById(id: number): Observable<LicenseDetail[]> {
    return this.http.get<LicenseDetail[]>(this.baseUrlowner+'LicenseDetail/'+ id);
  }

  // Account Info (مالیه) methods
  getAccountInfoByCompanyId(companyId: number, calendarType?: string): Observable<AccountInfo> {
    let url = `${this.baseUrlowner}CompanyAccountInfo/${companyId}`;
    if (calendarType) {
      url += `?calendarType=${calendarType}`;
    }
    return this.http.get<AccountInfo>(url);
  }

  createAccountInfo(data: AccountInfoData): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.baseUrlowner}CompanyAccountInfo`, data);
  }

  updateAccountInfo(id: number, data: AccountInfoData): Observable<{ id: number }> {
    return this.http.put<{ id: number }>(`${this.baseUrlowner}CompanyAccountInfo/${id}`, data);
  }

  // Cancellation Info (فسخ / لغوه) methods
  getCancellationInfoByCompanyId(companyId: number, calendarType?: string): Observable<CancellationInfo> {
    let url = `${this.baseUrlowner}CompanyCancellationInfo/${companyId}`;
    if (calendarType) {
      url += `?calendarType=${calendarType}`;
    }
    return this.http.get<CancellationInfo>(url);
  }

  createCancellationInfo(data: CancellationInfoData): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.baseUrlowner}CompanyCancellationInfo`, data);
  }

  updateCancellationInfo(id: number, data: CancellationInfoData): Observable<{ id: number }> {
    return this.http.put<{ id: number }>(`${this.baseUrlowner}CompanyCancellationInfo/${id}`, data);
  }

  deleteCompany(id: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/${id}`);
  }

  searchCompanyByLicense(licenseNumber: string, provinceId?: number): Observable<any[]> {
    let url = `${this.baseUrl}/searchByLicense?licenseNumber=${encodeURIComponent(licenseNumber)}`;
    if (provinceId && provinceId > 0) {
      url += `&provinceId=${provinceId}`;
    }
    return this.http.get<any[]>(url);
  }
}
