import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { LoginRequest } from '../interfaces/login-request';
import { map, Observable, of } from 'rxjs';
import { AuthResponse } from '../interfaces/auth-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiBaseUrl: string = environment.apiBaseUrl;
  private tokenKey = 'token';

  constructor(private http: HttpClient) { }

  login(data:LoginRequest):Observable<AuthResponse>
  {
    return this.http
      .post<AuthResponse>(`${this.apiBaseUrl}/User/login`, data)
      .pipe(map((response) => {
        if(response.isSucces)
        {
          localStorage.setItem(this.tokenKey, response.token);
          return response;
        }
        return response;
      })
    );
  }
}
