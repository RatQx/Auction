import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuctionService } from '../../services/auction.service';
import { Auction } from '../../models/auction.model';
import { MatTableDataSource } from '@angular/material/table';
import { render } from 'creditcardpayments/creditCardPayments';
import { Payment } from '../../models/payment.model';
import { PaymentService } from '../../services/payment.service';

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
    private router: Router,
    private paymentService: PaymentService
  ) {}
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
          this.configurePayment();
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
    const address = details.purchase_units[0].shipping.address;
    console.log(address);
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
      Address_Line1: address.address_line_2,
      Address_Line2: address.address_line_2 || '',
      Country: address.country_code,
      Postal_Code: address.postal_code,
      Buyer_Id: '',
      Auction_Id: this.auctionId.toString(),
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
