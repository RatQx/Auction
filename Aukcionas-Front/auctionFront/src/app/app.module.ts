import { APP_INITIALIZER, NgModule } from '@angular/core';
import {
  BrowserModule,
  provideClientHydration,
} from '@angular/platform-browser';
import { MatFormFieldModule } from '@angular/material/form-field';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { AuctionComponent } from './components/auction/auction.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuctionFormComponent } from './components/auction-form/auction-form.component';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { SingleAuctionComponent } from './components/single-auction/single-auction.component';
import { DatePipe } from '@angular/common';
import { RegisterComponent } from '../app/components/register/register.component';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { LoadingInterceptor } from './interceptors/loading';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { LoginComponent } from '../app/components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { CommonModule } from '@angular/common';
import { AppInitializer } from './utils/app.initializer';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { ResetComponent } from './components/reset/reset.component';
import { AuctionBidComponent } from './components/auction-bid/auction-bid.component';
import { ConfirmEmailComponent } from './components/confirm-email/confirm-email.component';
import { ReportModalComponent } from './components/report-modal/report-modal.component';
import { MatRadioModule } from '@angular/material/radio';
import { AdminComponent } from './components/admin/admin.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatPaginator } from '@angular/material/paginator';

function initApp(initializer: AppInitializer) {
  return () => initializer.initialize();
}

@NgModule({
  declarations: [
    AppComponent,
    AuctionComponent,
    AuctionFormComponent,
    SingleAuctionComponent,
    RegisterComponent,
    LoginComponent,
    HomeComponent,
    UserProfileComponent,
    ResetComponent,
    AuctionBidComponent,
    ConfirmEmailComponent,
    ReportModalComponent,
    AdminComponent,
  ],
  imports: [
    MatRadioModule,
    BrowserModule,
    CommonModule,
    CollapseModule,
    RouterModule,
    MatFormFieldModule,
    MatSlideToggleModule,
    ReactiveFormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatTableModule,
    HttpClientModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule,
    MatPaginator,
  ],
  providers: [
    AppInitializer,
    {
      provide: APP_INITIALIZER,
      useFactory: initApp,
      deps: [AppInitializer],
      multi: true,
    },
    provideClientHydration(),
    provideAnimationsAsync(),
    DatePipe,

    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
