import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment } from '../models/comment.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  private baseApiUrl: string = environment.apiUrl + '/Auction';

  constructor(private http: HttpClient) {}

  getCommentsForAuction(auctionId: number): Observable<Comment[]> {
    const url = `${this.baseApiUrl}/${auctionId}/comments`;
    return this.http.get<Comment[]>(url);
  }

  addComment(auctionId: number, comment: Comment): Observable<Comment> {
    const url = `${this.baseApiUrl}/${auctionId}/comments`;
    return this.http.post<Comment>(url, comment);
  }
}
