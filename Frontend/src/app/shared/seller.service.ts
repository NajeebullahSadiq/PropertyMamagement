import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { propertyAddress } from '../models/propertyAddress';
import { SellerDetail } from '../models/SellerDetail';
import { witnessDetail } from '../models/witnessDetail';

@Injectable({
  providedIn: 'root'
})
export class SellerService {
  private baseUrl = 'http://localhost:5143/api/SellerDetails';
  sellerId:number=0;
  buyerId:number=0;
  withnessId:number=0;
  constructor(private http: HttpClient) { }

  addSellerdetails(sellerdetails: SellerDetail): Observable<SellerDetail> {
    return this.http.post<SellerDetail>(this.baseUrl, sellerdetails);
  }
  updateSellerdetails(sellerdetails: SellerDetail): Observable<SellerDetail> {
    const url = `${this.baseUrl}/${sellerdetails.id}`;
    return this.http.put<SellerDetail>(url, sellerdetails);
  }
  addBuyerdetails(sellerdetails: SellerDetail): Observable<SellerDetail> {
    return this.http.post<SellerDetail>(this.baseUrl+'/addBuyerDetails', sellerdetails);
  }
  updateBuyerdetails(sellerdetails: SellerDetail): Observable<SellerDetail> {
    const url = `${this.baseUrl+"/UpdateBuyer"}/${sellerdetails.id}`;
    return this.http.put<SellerDetail>(url, sellerdetails);
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
}
