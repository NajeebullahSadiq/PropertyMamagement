import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { propertyAddress } from '../models/propertyAddress';
import { SellerDetail } from '../models/SellerDetail';
import { witnessDetail } from '../models/witnessDetail';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SellerService {
  private baseUrl = environment.apiUrl + '/SellerDetails';
  sellerId:number=0;
  buyerId:number=0;
  withnessId:number=0;
  constructor(private http: HttpClient) { }

  private normalizeBuyerPayload<T extends any>(payload: T): T {
    if (!payload) {
      return payload;
    }

    const clone: any = { ...(payload as any) };
    if (clone.rentStartDate === '') {
      clone.rentStartDate = null;
    }
    if (clone.rentEndDate === '') {
      clone.rentEndDate = null;
    }
    return clone as T;
  }

  addSellerdetails(sellerdetails: SellerDetail): Observable<SellerDetail> {
    return this.http.post<SellerDetail>(this.baseUrl, sellerdetails);
  }
  updateSellerdetails(sellerdetails: SellerDetail): Observable<SellerDetail> {
    const url = `${this.baseUrl}/${sellerdetails.id}`;
    return this.http.put<SellerDetail>(url, sellerdetails);
  }
  addBuyerdetails(sellerdetails: SellerDetail): Observable<SellerDetail> {
    const payload = this.normalizeBuyerPayload(sellerdetails);
    return this.http.post<SellerDetail>(this.baseUrl+'/addBuyerDetails', payload);
  }
  updateBuyerdetails(sellerdetails: SellerDetail): Observable<SellerDetail> {
    const url = `${this.baseUrl+"/UpdateBuyer"}/${sellerdetails.id}`;
    const payload = this.normalizeBuyerPayload(sellerdetails);
    return this.http.put<SellerDetail>(url, payload);
  }
  addWitnessdetails(sellerdetails: witnessDetail): Observable<witnessDetail> {
    return this.http.post<witnessDetail>(this.baseUrl+'/addWitnessdetails', sellerdetails);
  }
  updateWitnessDetails(wdetails: witnessDetail): Observable<witnessDetail> {
    const url = `${this.baseUrl+"/Updatewitness"}/${wdetails.id}`;
    return this.http.put<witnessDetail>(url, wdetails);
  }
  addPaddress(sellerdetails: propertyAddress): Observable<propertyAddress> {
    return this.http.post<propertyAddress>(this.baseUrl+'/addPaddress', sellerdetails);
  }
  updatePaddress(add: propertyAddress): Observable<propertyAddress> {
    const url = `${this.baseUrl+"/updatePaddress"}/${add.id}`;
    return this.http.put<propertyAddress>(url, add);
  }
  udateSellerId(id: number) {
    this.sellerId = id;
  }
  udateBuyerId(id: number) {
    this.buyerId = id;
  }
  udateWithnessId(id: number) {
    this.withnessId = id;
  }
  getprovince(){
    return this.http.get(this.baseUrl+'/getProvince');
  }
  getdistrict(id: number){
    return this.http.get(this.baseUrl+'/'+id);
  }
  getdistrict2(id: number){
    return this.http.get(this.baseUrl+'/'+id);
  }
  
  getSellerById(id: number): Observable<SellerDetail[]> {
    return this.http.get<SellerDetail[]>(this.baseUrl+'/Action' +'/'+ id);
  }
  getBuyerById(id: number): Observable<SellerDetail[]> {
    return this.http.get<SellerDetail[]>(this.baseUrl+'/Buyer' +'/'+ id);
  }
  getWitnessById(id: number): Observable<witnessDetail[]> {
    return this.http.get<witnessDetail[]>(this.baseUrl+'/Witness' +'/'+ id);
  }
  getPaddressById(id: number): Observable<propertyAddress[]> {
    return this.http.get<propertyAddress[]>(this.baseUrl+'/paddress' +'/'+ id);
  }
  deleteSeller(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + '/' + id);
  }
  deleteBuyer(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + '/Buyer/' + id);
  }
}
