import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { calculateTimeDifference } from '../../utils/calculate-time-left';

@Component({
  selector: 'app-product-details-modal',
  templateUrl: './single-auction.component.html',
  styleUrls: ['./single-auction.component.scss'],
})
export class SingleAuctionComponent implements OnInit {
  timeDiff: string | undefined;
  constructor(
    public dialogRef: MatDialogRef<SingleAuctionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private router: Router
  ) {}
  ngOnInit(): void {
    setInterval(() => {
      this.timeDiff = this.calculateTimeDifference();
    }, 1000);
  }

  calculateTimeDifference(): string {
    return calculateTimeDifference(this.data.product.auction_end_time);
  }
  bidNow(): void {
    this.dialogRef.close();
    const auctionId = this.data.product.id;
    this.router.navigate(['/auction-bid', auctionId]);
  }
}
