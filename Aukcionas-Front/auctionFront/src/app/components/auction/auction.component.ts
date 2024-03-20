import { Component } from '@angular/core';
import { Auction } from '../../models/auction.model';
import { AuctionService } from '../../services/auction.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { SingleAuctionComponent } from '../single-auction/single-auction.component';

@Component({
  selector: 'app-auction',
  templateUrl: './auction.component.html',
  styleUrl: './auction.component.scss',
})
export class AuctionComponent {
  auctions: any[] = [];
  auctionForModal: any[] = [];
  auctionToEdit?: Auction;
  displayedColumns: string[] = [
    'id',
    'name',
    'country',
    'starting_price',
    'auction_end_time',
    'update',
  ];
  constructor(
    private auctionService: AuctionService,
    private router: Router,
    private dialog: MatDialog,
    private route: ActivatedRoute
  ) {}
  ngOnInit(): void {
    this.getData('');
    console.log(window.navigator.language);
    this.route.queryParams.subscribe((params) => {
      const searchName = params['name'] || '';
      this.getData(searchName);
    });
  }
  updateAuctionList(auction: Auction[]) {
    this.auctions = auction;
  }
  initNewAuction() {
    this.router.navigate(['create-auction']);
  }

  getData(name: string) {
    this.auctionService.getAuctions({ name }).subscribe((result) => {
      this.auctions = result;
    });
  }

  openDetailsModal(product: any): void {
    console.log(product);
    this.dialog.open(SingleAuctionComponent, {
      width: '400px',
      data: { product },
    });
  }
}
