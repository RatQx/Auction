import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuctionService } from '../../services/auction.service';
import { Auction } from '../../models/auction.model';
import { MatTableDataSource } from '@angular/material/table';
import { render } from 'creditcardpayments/creditCardPayments';
import { Payment } from '../../models/payment.model';
import { PaymentService } from '../../services/payment.service';
import { UserService } from '../../services/user.service';
import { DecodedTokenResult } from '../../models/decoded-token-result.model';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.scss',
})
export class PaymentComponent implements OnInit {
  auction!: Auction;
  AuctionId!: number;
  UserId!: string;
  isLoading: boolean = false;
  isError: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private auctionService: AuctionService,
    private router: Router,
    private paymentService: PaymentService,
    private userService: UserService
  ) {}
  ngOnInit(): void {
    this.route.queryParams.subscribe((params: any) => {
      const token = params['token'];
      if (token) {
        this.decodeToken(token);
      }
    });
  }
  decodeToken(token: string): void {
    this.paymentService.decodeToken(token).subscribe(
      (result: DecodedTokenResult) => {
        console.log('Decoded token:', result);
        this.AuctionId = result.auctionId;
        this.GetAuctionDetails(this.AuctionId);
      },
      (error) => {
        console.error('Error decoding token:', error);
      }
    );
  }
  GetAuctionDetails(auctionId: number): void {
    this.auctionService.getAuction(auctionId).subscribe(
      (auction: Auction) => {
        this.auction = auction;
        this.isLoading = false;
        this.configurePayment();
      },
      (error: any) => {
        console.error('Error getting auction details:', error);
        this.isLoading = false;
        this.isError = true;
      }
    );
  }

  configurePayment(): void {
    if (
      this.auction &&
      this.auction.bidding_amount_history &&
      this.auction.bidding_amount_history.length > 0
    ) {
      render({
        id: '#PaypalButton',
        currency: 'EUR',
        value:
          this.auction.bidding_amount_history[
            this.auction.bidding_amount_history.length - 1
          ].toString(),
        onApprove: (details) => {
          alert('Transaction Successful');
          this.handlePaymentSuccess(details);
        },
      });
    } else {
      alert('Error loading payment information. Try again');
    }
  }
  handlePaymentSuccess(details: any): void {
    const paymentId = details.id;
    const paymentTime = new Date(details.create_time);
    const address = details.purchase_units[0]?.shipping?.address;
    const email = '';

    const payment: Payment = {
      Id: 0,
      Payment_Id: paymentId,
      Payment_Time: paymentTime,
      Payment_Amount:
        this.auction?.bidding_amount_history![
          this.auction.bidding_amount_history!.length - 1
        ] || 0,
      Payment_Successful: true,
      Address_Line1: address.address_line_1,
      Address_Line2: address.address_line_2 || '',
      Country: address.country_code,
      Postal_Code: address.postal_code,
      Buyer_Id: '',
      Auction_Id: this.AuctionId.toString(),
      Buyer_Email: '',
      Auction_Owner_Email: email.toString(),
    };

    this.paymentService.createPayment(payment).subscribe(
      (response: any) => {
        console.log('Payment created successfully:', response);
      },
      (error: any) => {
        alert('Failed to create payment');
      }
    );
  }
}
