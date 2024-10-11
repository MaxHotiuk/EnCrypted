import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { LoginRequest } from '../interfaces/login-request';
import { RegisterRequest } from '../interfaces/register-request';
import { map, Observable } from 'rxjs';
import { AuthResponse } from '../interfaces/auth-response';
import { jwtDecode } from 'jwt-decode';
import { UserDetail } from '../interfaces/user-detail';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiBaseUrl: string = environment.apiBaseUrl;
  private tokenKey = 'token';

  constructor(private http: HttpClient) { }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiBaseUrl}User/login`, data)
      .pipe(
        map((response) => {
          if (response.isSucces) {
            localStorage.setItem(this.tokenKey, response.token);
            return response;
          }
          return response;
        })
      );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiBaseUrl}User/register`, data)
      .pipe(
        map((response) => {
          if (response.isSucces) {
            return response;
          }
          return response;
        })
      );
  }

  isLoggedIn = (): boolean => {
    const token = this.getToken();
    if (!token) {
      return false;
    }
    return !this.isTokenExpired(token);
  }

  isTokenExpired = (token: string): boolean => {
    try {
      const decodedToken: { exp?: number } = jwtDecode(token);
      if (decodedToken.exp === undefined) {
        this.logout();
        return true;
      }
      const isExpired = Date.now() > decodedToken.exp * 1000;
      if (isExpired) {
        this.logout();
      }
      return isExpired;
    } catch (error) {
      console.error('Failed to decode token:', error);
      this.logout();
      return true;
    }
  }

  logout = (): void => {
    localStorage.removeItem(this.tokenKey);
  }

  private getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getUserDetails = () => {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    const decodedToken: { name: string, email: string, role: string } = jwtDecode(token);
    const userDetails = {
      username: decodedToken.name,
      email: decodedToken.email,
      role: decodedToken.role || []
    }
    return userDetails;
  }

  getAllUsers = () : Observable<UserDetail[]> => {
    return this.http.get<UserDetail[]>(`${this.apiBaseUrl}User`);
  }

  getUserRoles = () : string[] | null => {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    const decodedToken:any = jwtDecode(token);
    return decodedToken.role || null;
  }
}
