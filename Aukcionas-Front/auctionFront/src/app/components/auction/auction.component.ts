import { Component } from '@angular/core';
import { Auction } from '../../models/auction.model';
import { AuctionService } from '../../services/auction.service';
import { Router } from '@angular/router';
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
    private dialog: MatDialog
  ) {}
  ngOnInit(): void {
    this.getData();
    console.log(window.navigator.language);
  }
  updateAuctionList(auction: Auction[]) {
    this.auctions = auction;
  }
  initNewAuction() {
    this.router.navigate(['create-auction']);
  }

  editAuction(id: number) {
    this.auctionService.updateAuction(id);
  }

  getData() {
    this.auctionService.getAuctions({ name: '' }).subscribe((result) => {
      //filters get auction by name
      this.auctions = result;
    });
  }

  openDetailsModal(product: any): void {
    console.log(product);
    this.dialog.open(SingleAuctionComponent, {
      width: '400px', // Adjust the width as needed
      data: { product },
    });
  }
}
