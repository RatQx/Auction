import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Payment } from '../models/payment.model';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private baseApiUrl: string = environment.apiUrl + '/Payment';

  constructor(private http: HttpClient) {}

  public createPayment(payment: Payment): Observable<Payment[]> {
    return this.http.post<Payment[]>(`${this.baseApiUrl}/create`, payment);
  }
}
