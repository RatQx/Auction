import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Report } from '../models/report.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private baseApiUrl: string = environment.apiUrl + '/Report';
  constructor(private http: HttpClient) {}

  public CreateReport(report: Report): Observable<Report[]> {
    return this.http.post<Report[]>(`${this.baseApiUrl}`, report);
  }
}
