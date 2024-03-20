import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  User,
} from '../../app/types/aukcionas.types';
import { RessetPasword } from '../models/reset-password.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  private baseApiUrl: string = environment.apiUrl + '/Auth';
  constructor(private http: HttpClient, private router: Router) {
    this.checkAuthentication();
  }
  public isAuthenticated: boolean = false;
  public isAdmin: boolean = false;
  public isUser: boolean = false;
  public roles!: string;
  public ss: string = '';
  public user: string[] = [];
  private readonly TokenValidInMinutes: number = 60;
  readonly storage = 'auction';

  public checkAuthentication(): Observable<boolean> {
    if (typeof localStorage !== 'undefined') {
      const token = localStorage.getItem('auction');
      if (token) {
        const decodedToken = this.decodeToken(token);
        const isTokenValid = this.isTokenValid(decodedToken);
        return of(isTokenValid);
      }
    }
    return of(false);
  }
  private decodeToken(token: string): any {
    return JSON.parse(atob(token.split('.')[1]));
  }
  private isTokenValid(decodedToken: any): boolean {
    if (decodedToken && decodedToken.exp) {
      const expirationTime = decodedToken.exp * 1000;
      const currentTime = new Date().getTime();

      return expirationTime > currentTime;
    }

    return false;
  }
  updateAuthenticationState(isAuthenticated: boolean) {
    this.isAuthenticatedSubject.next(isAuthenticated);
  }
  register(req: RegisterRequest): Observable<any> {
    return this.http.post(this.baseApiUrl + '/register', req);
  }
  confirmEmail(email: string, code: string): Observable<any> {
    return this.http.get(
      `${this.baseApiUrl}/confirm-email?email=${email}&code=${code}`
    );
  }
  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseApiUrl}/user/${id}`);
  }

  login(req: LoginRequest): Observable<LoginResponse> {
    console.log(this.baseApiUrl + '/login', req);
    return this.http.post<LoginResponse>(this.baseApiUrl + '/login', req).pipe(
      tap({
        next: (response: { accessToken: string }) => {
          localStorage.setItem(this.storage, response.accessToken);
          this.isAuthenticatedSubject.next(true);
        },
      })
    );
  }
  getUserInfo(): Observable<any> {
    console.log(this.baseApiUrl);
    return this.http.get<any>(`${this.baseApiUrl}/userinfo`);
  }
  updateUserInfo(updatedUserInfo: any): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    const body = JSON.stringify(updatedUserInfo);
    return this.http.put(`${this.baseApiUrl}/update`, body, {
      headers,
    });
  }

  logout() {
    localStorage.removeItem(this.storage);
    this.isAuthenticatedSubject.next(false);
  }

  public isUserAdmin(): boolean {
    return this.isAdmin;
  }

  public isUserValid(): boolean {
    return this.isUser;
  }

  public getToken(): string | null {
    if (typeof localStorage !== 'undefined') {
      return localStorage.getItem(this.storage);
    }
    return null;
  }

  public validateSession(): void {
    const token = this.getToken();
    if (!token) {
      this.isAuthenticated = false;
    } else this.isAuthenticated = true;
    if (token != null) {
      let jwtData = token.split('.')[1];
      let decodedJwtJsonData = window.atob(jwtData);
      let decodedJwtData = JSON.parse(decodedJwtJsonData);

      const roles =
        decodedJwtData[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ];
      if (roles.includes('Admin')) {
        this.isAdmin = true;
      }
      if (roles.includes('ForumUser')) {
        this.isUser = true;
      }
    }
  }
  sendResetPasswordLink(email: string) {
    console.log(this.baseApiUrl + '/send-reset-email/' + email);
    console.log(email);
    return this.http.post(`${this.baseApiUrl}/send-reset-email/${email}`, {});
  }

  resetPassword(resetPasswordObj: RessetPasword) {
    return this.http.post<any>(
      `${this.baseApiUrl}/reset-password`,
      resetPasswordObj
    );
  }
}
