import { Injectable } from '@angular/core';
import { Auction, GetAuctionReq } from '../models/auction.model';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { objectToParams } from '../utils/utils';
import { request } from 'http';

@Injectable({
  providedIn: 'root',
})
export class AuctionService {
  private url = 'Auction';
  constructor(private http: HttpClient) {}

  public getAuctions(req?: GetAuctionReq): Observable<Auction[]> {
    return this.http.get<Auction[]>(`${environment.apiUrl}/${this.url}`, {
      params: objectToParams(req),
    });
  }

  public updateAuction(id: number): Observable<Auction[]> {
    const url = `${environment.apiUrl}/${this.url}/Update`;
    return this.http.put<Auction[]>(url, { id });
    //return this.http.put<Auction[]>(`${environment.apiUrl}/${this.url}`,auction);
  }

  public createAuction(auction: Auction): Observable<Auction[]> {
    return this.http.post<Auction[]>(
      `${environment.apiUrl}/${this.url}`,
      auction
    );
  }

  public deleteAuction(auction: Auction): Observable<Auction[]> {
    return this.http.delete<Auction[]>(
      `${environment.apiUrl}/${this.url}/${auction.id}`
    );
  }
}
