import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUser: any = null;
  private userData: any = null;
  errorMessage: string | null = null;

  constructor(@Inject(PLATFORM_ID) private platformId: Object, private http: HttpClient) {}

  saveToken(token: string): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('token', token);
    }
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('token');
    }
    return null;
  }

  setCurrentUser(user: any, token: string): void {
    this.currentUser = user;
    localStorage.setItem('currentUser', JSON.stringify(user));
    localStorage.setItem('token', token); // Store the token for future requests
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
    this.currentUser = null;
  }

  isAuthenticated(): boolean {
    return this.currentUser !== null;
  }

  getCurrentUser() {
    return this.currentUser;
  }

  getUserProfile(): void {
    const token = localStorage.getItem('jwt'); // Retrieve the JWT from local storage
    if (!token) {
        this.errorMessage = 'No token found. Please log in.';
        return; // Handle the absence of a token
    }

    const headers = { 'Authorization': `Bearer ${token}` }; // Set the authorization header

    this.http.get('http://localhost:5069/api/Users/profile', { headers }).subscribe({
        next: (data: any) => {
            this.userData = data; // Process user data
        },
        error: (error: any) => {
            console.log('Failed to fetch user data', error);
            this.errorMessage = 'Error fetching user data';
        }
    });
  }
}
