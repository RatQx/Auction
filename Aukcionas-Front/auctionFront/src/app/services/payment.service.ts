import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Payment } from '../models/payment.model';
import { DecodedTokenResult } from '../models/decoded-token-result.model';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private baseApiUrl: string = environment.apiUrl + '/Payment';

  constructor(private http: HttpClient) {}

  public createPayment(payment: Payment): Observable<Payment[]> {
    return this.http.post<Payment[]>(`${this.baseApiUrl}/create`, payment);
  }

  public decodeToken(token: string): Observable<DecodedTokenResult> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    // Instead of passing an object, pass the token directly
    return this.http.post<DecodedTokenResult>(
      `${this.baseApiUrl}/decode`,
      `"${token}"`, // Pass token directly as a string
      { headers }
    );
  }
}
