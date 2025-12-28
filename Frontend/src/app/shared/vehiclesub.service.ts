import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VBuyerDetail } from '../models/SellerDetail';
import { VehicleDetails } from '../models/vehicle';
import { witnessDetail } from '../models/witnessDetail';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VehiclesubService {
  private baseUrl = environment.apiUrl + '/VehiclesSub';
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

  addBuyerdetails(buyerdetails: VBuyerDetail): Observable<VBuyerDetail> {

    const payload = this.normalizeBuyerPayload(buyerdetails);
    return this.http.post<VBuyerDetail>(this.baseUrl+'/addBuyerDetails', payload);
  }
  updateBuyerdetails(buyerdetails: VBuyerDetail): Observable<VBuyerDetail> {
    const url = `${this.baseUrl+"/UpdateBuyer"}/${buyerdetails.id}`;
    const payload = this.normalizeBuyerPayload(buyerdetails);
    return this.http.put<VBuyerDetail>(url, payload);
  }
  addSellerdetails(sellerdetails: VBuyerDetail): Observable<VBuyerDetail> {
    return this.http.post<VBuyerDetail>(this.baseUrl+'/addSellerDetails', sellerdetails);
  }
  updateSellerdetails(sellerdetails: VBuyerDetail): Observable<VBuyerDetail> {
    const url = `${this.baseUrl+"/UpdateSeller"}/${sellerdetails.id}`;
    return this.http.put<VBuyerDetail>(url, sellerdetails);
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

  getWitnessById(id: number): Observable<witnessDetail[]> {
    return this.http.get<witnessDetail[]>(this.baseUrl+'/Witness' +'/'+ id);
  }
  addWitnessdetails(sellerdetails: witnessDetail): Observable<witnessDetail> {
    return this.http.post<witnessDetail>(this.baseUrl+'/addWitnessdetails', sellerdetails);
  }
  updateWitnessDetails(wdetails: witnessDetail): Observable<witnessDetail> {
    const url = `${this.baseUrl+"/Updatewitness"}/${wdetails.id}`;
    return this.http.put<witnessDetail>(url, wdetails);
  }
  getBuyerById(id: number): Observable<VBuyerDetail[]> {
    return this.http.get<VBuyerDetail[]>(this.baseUrl+'/Buyer' +'/'+ id);
  }
  getSellerById(id: number): Observable<VBuyerDetail[]> {
    return this.http.get<VBuyerDetail[]>(this.baseUrl+'/Seller' +'/'+ id);
  }
  deleteSeller(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + '/Seller/' + id);
  }
  deleteBuyer(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + '/Buyer/' + id);
  }

}
