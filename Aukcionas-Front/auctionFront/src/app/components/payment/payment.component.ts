import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuctionService } from '../../services/auction.service';
import { Auction } from '../../models/auction.model';
import { MatTableDataSource } from '@angular/material/table';
import { render } from 'creditcardpayments/creditCardPayments';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.scss',
})
export class PaymentComponent implements OnInit {
  auction!: Auction;
  auctionId!: number;
  isLoading: boolean = false;
  isError: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private auctionService: AuctionService,
    private router: Router
  ) {
    render({
      id: '#PaypalButton',
      currency: 'EUR',
      value: '10',
      onApprove: (details) => {
        alert('Transaction Successfull');
      },
    });
  }
  ngOnInit(): void {
    this.route.queryParams.subscribe((params: any) => {
      const token = params['token'];
      this.auctionId = +params['auctionId'];
      console.log(this.auctionId);
      if (token) {
        this.loadAuctionDetails(token);
      }
      if (this.auctionId) {
        this.auctionService.getAuction(this.auctionId).subscribe((auction) => {
          this.auction = auction;
          console.log(this.auction);
        });
      }
    });
  }

  loadAuctionDetails(token: string): void {
    this.isLoading = true;
    this.auctionService.getAuctionDetailsByToken(token).subscribe(
      (auction) => {
        this.auction = auction;
        this.isLoading = false;
      },
      (error) => {
        if (error.status === 200) {
          const redirectUrl = error.url;
          window.location.href = redirectUrl;
        } else {
          console.error(error);
          this.isLoading = false;
          this.isError = true;
        }
      }
    );
  }
}
