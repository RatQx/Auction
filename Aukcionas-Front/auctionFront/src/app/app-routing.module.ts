import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuctionComponent } from './components/auction/auction.component';
import { AuctionFormComponent } from './components/auction-form/auction-form.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { ResetComponent } from './components/reset/reset.component';
import { AuctionBidComponent } from './components/auction-bid/auction-bid.component';
import { ConfirmEmailComponent } from './components/confirm-email/confirm-email.component';
import { AdminGuard } from './guard/admin.guard';
import { AdminComponent } from './components/admin/admin.component';
import { PaymentComponent } from './components/payment/payment.component';

const routes: Routes = [
  { path: 'auction', component: AuctionComponent },
  { path: 'create-auction', component: AuctionFormComponent },
  { path: 'auction/:id', component: AuctionFormComponent }, //for update
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'profile', component: UserProfileComponent },
  { path: 'reset', component: ResetComponent },
  { path: 'auction-bid/:id', component: AuctionBidComponent },
  { path: 'confirm-email', component: ConfirmEmailComponent },
  { path: 'payment', component: PaymentComponent },
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'admin', component: AdminComponent, canActivate: [AdminGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
