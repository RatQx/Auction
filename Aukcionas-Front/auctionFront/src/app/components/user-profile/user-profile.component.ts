// user-profile.component.ts
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.model';
import { NgForm } from '@angular/forms';
import { AuctionService } from '../../services/auction.service';

declare var paypal: any;

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
})
export class UserProfileComponent implements OnInit {
  userInfo: any = {};
  showEditForm: boolean = false;
  showAuctionsWonTable: boolean = false;
  showLikedAuctionsTable: boolean = false;

  constructor(
    private userService: UserService,
    private auctionService: AuctionService
  ) {}
  ngOnInit(): void {
    this.getUserInfo();
  }

  getUserInfo(): void {
    this.userService.getUserInfo().subscribe((data) => {
      this.userInfo = data;
    });
  }
  toggleEditForm(): void {
    this.showEditForm = !this.showEditForm;
  }

  unlikeAuction(id: number) {
    this.auctionService.unlikeAuction(id).subscribe((data) => {
      this.getUserInfo();
    });
  }

  submitForm(): void {
    const confirmed = window.confirm(
      'Are you sure you want to save changes on your user information?'
    );
    if (confirmed) {
      this.userService.updateUserInfo(this.userInfo).subscribe(
        (response) => {
          console.log(response);
          this.toggleEditForm();
        },
        (error) => {
          console.error(error);
          this.toggleEditForm();
        }
      );
    }
  }
  toggleAuctionsWonTable() {
    this.showAuctionsWonTable = !this.showAuctionsWonTable;
  }

  toggleLikedAuctionsTable() {
    this.showLikedAuctionsTable = !this.showLikedAuctionsTable;
  }
}
