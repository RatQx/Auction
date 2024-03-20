import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { GroupedReport } from '../models/grouped-report.model';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private baseApiUrlAuth: string = environment.apiUrl + '/Auth';
  private baseApiUrlReport: string = environment.apiUrl + '/Report';
  private baseApiUrlAuction: string = environment.apiUrl + '/Auction';
  private baseApiUrlAdmin: string = environment.apiUrl + '/Admin';

  constructor(private http: HttpClient) {}

  public GetAllReports(): Observable<Report[]> {
    return this.http.get<Report[]>(`${this.baseApiUrlReport}/reports`);
  }

  public GetAllGroupedReports(): Observable<GroupedReport[]> {
    return this.http.get<GroupedReport[]>(
      `${this.baseApiUrlReport}/grouped-reports`
    );
  }

  public GetAuctionReports(auctioId: number): Observable<Report[]> {
    return this.http.get<Report[]>(
      `${this.baseApiUrlReport}/reports/${auctioId}`
    );
  }

  public DeleteReport(reportId: number): Observable<Report[]> {
    return this.http.delete<Report[]>(
      `${this.baseApiUrlReport}/delete-report/${reportId}`
    );
  }
}
