import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserService } from '../services/user.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private userService: UserService) {}
  /**public intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const newReq = req.clone({
      setHeaders: { Authorization: `Bearer ${this.userService.getToken()}` },
    });
    //this.userService.isExp();
    return next.handle(newReq);
  }*/
  public intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const token = this.userService.getToken();

    if (token) {
      const newReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      });
      return next.handle(newReq);
    } else {
      return next.handle(req);
    }
  }
}
