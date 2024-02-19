import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuctionService } from '../../services/auction.service';
import { Auction } from '../../models/auction.model';

@Component({
  selector: 'app-auction-form',
  templateUrl: './auction-form.component.html',
  styleUrl: './auction-form.component.scss',
})
export class AuctionFormComponent {
  auctionForm: FormGroup;
  constructor(private fb: FormBuilder, private auctionService: AuctionService) {
    this.auctionForm = this.fb.group({
      name: ['', Validators.required],
      country: ['', Validators.required],
      city: ['', Validators.required],
      starting_price: [
        '',
        [Validators.required, Validators.pattern('^[0-9]*$')],
      ],
      bid_ammount: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
      min_buy_price: [
        '',
        [Validators.required, Validators.pattern('^[0-9]*$')],
      ],
      auction_start_time: ['', Validators.required],
      auction_end_time: ['', Validators.required],
      buy_now_price: [
        '',
        [Validators.required, Validators.pattern('^[0-9]*$')],
      ],
      category: ['', Validators.required],
      description: ['', Validators.required],
      item_build_year: [
        '',
        [Validators.required, Validators.pattern('^[0-9]*$')],
      ],
      item_mass: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
      condition: ['', Validators.required],
      material: ['', Validators.required],
    });
  }

  onSubmit() {
    if (this.auctionForm.invalid) {
      return;
    }

    const auctionData: Auction = this.auctionForm.getRawValue() as Auction;

    console.log('Auction Data:', auctionData);

    this.auctionService.createAuction(auctionData).subscribe((response) => {
      console.log('Auction created successfully', response);
    });
  }
}
