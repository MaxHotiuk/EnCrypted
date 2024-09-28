import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../auth.service'; // Adjust the import path
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.css'],
  standalone: true,
  providers: [AuthService, CommonModule],
  imports: [CommonModule]
})
export class UserprofileComponent implements OnInit {
  userData: any = null;
  errorMessage: string = '';

  constructor(private http: HttpClient, private authService: AuthService) { }

  ngOnInit(): void {
    this.fetchUserData();
  }

  fetchUserData(): void {
    const token = localStorage.getItem('jwt');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get('http://localhost:5069/api/Users/profile', { headers }).subscribe({
      next: (response: any) => {
        this.userData = response; // Adjust according to your API response
      },
      error: (error: any) => {
        console.log('Failed to fetch user data', error);
        this.errorMessage = 'Could not retrieve user data';
      }
    });
  }
}
