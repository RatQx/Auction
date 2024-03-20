import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BidService {
  private bidConnection?: HubConnection;
  private newBidSubject: BehaviorSubject<{
    auctionId: number;
    bidAmount: number;
  }> = new BehaviorSubject({ auctionId: 0, bidAmount: 0 });

  constructor(private http: HttpClient) {}

  createBidConnection() {
    this.bidConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/hubs/bid`)
      .withAutomaticReconnect()
      .build();

    this.bidConnection.start().catch((error) => {
      console.log(error);
    });

    this.bidConnection.on('UserConnected', () => {
      console.log('User connected');
    });

    this.bidConnection.on(
      'UpdateBid',
      (auctionId: number, bidAmount: number) => {
        console.log('Received bid update:', auctionId, bidAmount);
        this.newBidSubject.next({ auctionId, bidAmount });
      }
    );
  }

  placeBid(auctionId: number, bidAmount: number, username: string) {
    this.bidConnection
      ?.invoke('PlaceBid', auctionId, bidAmount, username)
      .catch((err) => console.error(err));
  }

  stopBidConnection() {
    this.bidConnection?.stop().catch((error) => console.error());
  }

  getNewBidObservable() {
    return this.newBidSubject.asObservable();
  }
}
