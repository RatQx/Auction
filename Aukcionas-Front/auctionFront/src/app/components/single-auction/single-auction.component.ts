import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-product-details-modal',
  templateUrl: './single-auction.component.html',
  styleUrls: ['./single-auction.component.scss'],
})
export class SingleAuctionComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}
  demo: any;

  x = setInterval(() => {
    var now = new Date().getTime();
    //console.log(now);
    //console.log(this.data.auction_end_time);
    var distance = this.data.auction_end_time - now;
    //console.log(distance);
    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
    var hours = Math.floor(
      (distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)
    );
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);
    this.demo = days + 'd ' + hours + 'h ' + minutes + 'm ' + seconds + 's';
    if (distance < 0) {
      clearInterval(this.x);
      this.demo = 'Auction Expired';
    }
  });
}
