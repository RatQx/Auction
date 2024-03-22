import { Injectable } from '@angular/core';
import { Auction, GetAuctionReq } from '../models/auction.model';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, delay } from 'rxjs';
import { environment } from '../../environments/environment';
import { objectToParams } from '../utils/utils';

@Injectable({
  providedIn: 'root',
})
export class AuctionService {
  private baseApiUrl: string = environment.apiUrl + '/Auction';
  private populateFormSubject = new Subject<any>();
  private updateListSubject = new Subject<any>();
  constructor(private http: HttpClient) {}

  public getAuctions(req?: GetAuctionReq): Observable<Auction[]> {
    return this.http.get<Auction[]>(`${this.baseApiUrl}`, {
      params: objectToParams(req),
    });
  }

  likeAuction(auctionId: number): Observable<any> {
    return this.http.post(`${this.baseApiUrl}/${auctionId}/like`, []);
  }

  unlikeAuction(auctionId: number): Observable<any> {
    return this.http.post(`${this.baseApiUrl}/${auctionId}/unlike`, []);
  }
  getAuction(id: number): Observable<Auction> {
    const url = `${this.baseApiUrl}/${id}`;
    return this.http.get<Auction>(url);
  }

  updateAuction(auction: Auction) {
    return this.http.put(this.baseApiUrl + auction.id, auction);
  }
  public createAuction(auction: Auction): Observable<Auction[]> {
    return this.http.post<Auction[]>(`${this.baseApiUrl}`, auction);
  }

  public deleteAuction(auction: Auction): Observable<Auction[]> {
    return this.http.delete<Auction[]>(`${this.baseApiUrl}/${auction.id}`);
  }

  sendPopulateForm() {
    return this.populateFormSubject.asObservable();
  }
  populateForm(id: number) {
    this.populateFormSubject.next(id);
  }

  updateList() {
    this.updateListSubject.next(true);
  }

  sendUpdateList() {
    return this.updateListSubject.asObservable();
  }

  getAuctionDetailsByToken(token: string): Observable<any> {
    return this.http.get<any>(`${this.baseApiUrl}/payment?token=${token}`);
  }
}
