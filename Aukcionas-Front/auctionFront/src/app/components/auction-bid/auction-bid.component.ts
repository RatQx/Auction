import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Auction } from '../../models/auction.model';
import { AuctionService } from '../../services/auction.service';
import { UserService } from '../../services/user.service';
import { CommentService } from '../../services/comment.service';
import { calculateTimeDifference } from '../../utils/calculate-time-left';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Comment } from '../../models/comment.model';
import { MatDialog } from '@angular/material/dialog';
import { ReportModalComponent } from '../report-modal/report-modal.component';
import { ReportService } from '../../services/report.service';
import { BidService } from '../../services/bid.service';
import { Subscription } from 'rxjs';

declare var paypal: any;

@Component({
  selector: 'app-auction-bid',
  templateUrl: './auction-bid.component.html',
  styleUrls: ['./auction-bid.component.scss'],
})
export class AuctionBidComponent implements OnInit {
  private newBidSubscription!: Subscription;
  auctionId?: number;
  auction: Auction | undefined;
  public timeDiff: string | undefined;
  public isAuctionLiked: boolean = false;
  comments: Comment[] = [];
  commentForm!: FormGroup;
  isAuthenticated!: boolean;
  private username!: '';

  constructor(
    private route: ActivatedRoute,
    private auctionService: AuctionService,
    private userService: UserService,
    private commentService: CommentService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private reportService: ReportService,
    private bidService: BidService
  ) {
    this.userService.isAuthenticated$.subscribe((isAuthenticated) => {
      this.isAuthenticated = isAuthenticated;
    });
  }

  @ViewChild('paypal', { static: true }) paypalElement!: ElementRef;
  product = {
    price: 0.1,
    description: 'test buy',
  };
  paidFor = false;
  ngOnInit(): void {
    this.bidService.createBidConnection();
    this.newBidSubscription = this.bidService
      .getNewBidObservable()
      .subscribe((bid) => {
        this.handleNewBid(bid.auctionId, bid.bidAmount);
      });

    this.route.params.subscribe((params) => {
      this.auctionId = params['id'];
      this.getAuctionDetails();
      this.checkIfAuctionIsLiked();
      this.getComments();
      this.userService.getUserInfo().subscribe((userInfo) => {
        if (userInfo.userName) {
          this.username = userInfo.userName;
          console.log(this.username);
        }
      });
    });
    this.commentForm = this.fb.group({
      text: ['', Validators.required],
    });
    setInterval(() => {
      this.timeDiff = this.calculateTimeDifference();
    }, 1000);
    paypal
      .Buttons({
        createOrder: (data: any, actions: any) => {
          return actions.oder.create({
            purchase_units: [
              {
                desription: this.product.description,
                amount: {
                  currency_code: 'EUR',
                  value: this.product.price,
                },
              },
            ],
          });
        },
        onApprove: async (data: any, actions: any) => {
          const order = await actions.order.capture();
          this.paidFor = true;
        },
        onError: (err: any) => {
          console.log(err);
        },
      })
      .render(this.paypalElement.nativeElement);
  }

  ngOnDestroy(): void {
    this.newBidSubscription.unsubscribe();
    this.bidService.stopBidConnection();
  }

  handleNewBid(auctionId: number, bidAmount: number) {
    if (this.auction && this.auction.id === auctionId) {
      this.updateBiddingHistory(bidAmount);
    }
  }

  updateBiddingHistory(bidAmount: number) {
    if (this.auction) {
      this.auction.auction_biders_list!.push(this.username); // update this code. create normal update not based on current user info
      this.auction.bidding_amount_history!.push(bidAmount); //
      this.auction.bidding_times_history!.push(new Date()); //
    }
  }

  calculateTimeDifference(): string {
    if (this.auction && this.auction.auction_end_time) {
      let auctionEndTime: Date;
      if (typeof this.auction.auction_end_time === 'string') {
        auctionEndTime = new Date(this.auction.auction_end_time);
      } else if (this.auction.auction_end_time instanceof Date) {
        auctionEndTime = this.auction.auction_end_time;
      } else {
        return 'Invalid date';
      }

      return calculateTimeDifference(auctionEndTime.toString());
    }

    return '';
  }
  getAuctionDetails(): void {
    if (this.auctionId) {
      this.auctionService.getAuction(this.auctionId).subscribe((auction) => {
        this.auction = auction;
      });
    }
  }

  checkIfAuctionIsLiked(): void {
    this.userService.getUserInfo().subscribe((userInfo) => {
      if (userInfo.id && this.auctionId && this.auction?.auction_likes_list) {
        this.isAuctionLiked = this.auction?.auction_likes_list.includes(
          userInfo.id
        );
        console.log(this.isAuctionLiked);
      }
    });
  }
  likeOrUnlikeAuction(): void {
    const auctionId = this.auctionId;
    if (auctionId) {
      if (this.isAuctionLiked) {
        this.auctionService.unlikeAuction(auctionId).subscribe(() => {
          this.isAuctionLiked = false;
        });
      } else {
        this.auctionService.likeAuction(auctionId).subscribe(() => {
          this.isAuctionLiked = true;
        });
      }
    }
  }
  getComments(): void {
    if (this.auctionId) {
      this.commentService.getCommentsForAuction(this.auctionId).subscribe(
        (comments) => {
          this.comments = comments;
        },
        (error) => {
          console.error('Error fetching comments:', error);
        }
      );
    }
  }
  onSubmitComment(): void {
    if (this.commentForm.valid && this.auctionId) {
      const username = this.commentForm.value.username;
      console.log(username);
      console.log('dasdsd');
      const newComment: Comment = {
        date: new Date(),
        username: this.username,
        text: this.commentForm.value.text,
      };

      this.commentService.addComment(this.auctionId, newComment).subscribe(
        (comment) => {
          this.comments.push(comment);
          this.commentForm.reset();
        },
        (error) => {
          console.error('Error adding comment:', error);
        }
      );
    }
  }
  calculateNextBid(): number {
    if (this.auction) {
      if (
        this.auction.bidding_amount_history &&
        this.auction.bidding_amount_history.length > 0
      ) {
        const lastBidAmount =
          this.auction.bidding_amount_history[
            this.auction.bidding_amount_history.length - 1
          ];

        if (typeof lastBidAmount === 'number') {
          const nextBidAmount = lastBidAmount + this.auction.bid_ammount!;
          return nextBidAmount;
        }
      }
      return this.auction.starting_price!;
    }
    return 0;
  }
  PlaceBid(auctionId: number, bidAmount: number) {
    this.bidService.placeBid(auctionId, bidAmount, this.username);
  }
  buyNow(): void {}
  openReportModal(auctionId: number) {
    const dialogRef = this.dialog.open(ReportModalComponent, {
      data: { auctionId: auctionId },
    });
    dialogRef.afterClosed().subscribe((reportData) => {
      if (reportData) {
        this.reportService.CreateReport(reportData).subscribe(
          (response) => {
            console.log('Report submitted successfully');
            dialogRef.close();
          },
          (error) => {
            console.error('Error submitting report:', error);
            dialogRef.close();
          }
        );
      }
    });
  }

  reverseIndexArray(arr: any[]): number[] {
    return Array.from({ length: arr.length }, (_, i) => i).reverse();
  }
}
