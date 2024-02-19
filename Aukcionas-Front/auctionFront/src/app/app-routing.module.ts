import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuctionComponent } from './components/auction/auction.component';
import { AuctionFormComponent } from './components/auction-form/auction-form.component';

const routes: Routes = [
  { path: 'auction', component: AuctionComponent },
  { path: 'create-auction', component: AuctionFormComponent },
  { path: 'auction/:id', component: AuctionFormComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
