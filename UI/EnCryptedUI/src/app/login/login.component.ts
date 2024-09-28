import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterOutlet, HttpClientModule, CommonModule, FormsModule],
  providers: [AuthService],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  constructor(private http: HttpClient, private router: Router, private authService: AuthService) { }
  errorMessage: string | null = null;
  password: string = '';
  username: string = '';
  currentuserid: number = 0;

  onSubmitLogin(): void {
    this.http.post('http://localhost:5069/api/Users/login', {
      username: this.username,
      password: this.password
    }).subscribe({
      next: (response: any) => {
        console.log('Login successful', response);
        localStorage.setItem('jwt', response.token);
        // Assume the response contains a token and user data
        const user = response.user; // Adjust according to your API response
        const token = response.token; // If your API returns a token
        this.currentuserid = user.id;

        // Store user data and token
        this.authService.setCurrentUser(user, token); // Update the AuthService accordingly

        // Navigate to another page on successful login
        this.router.navigate(['../userprofile']); // Change '/dashboard' to your desired route
      },
      error: (error: any) => {
        console.log('Login failed', error);
        this.errorMessage = 'Invalid username or password';
      }
    });
  }
}
