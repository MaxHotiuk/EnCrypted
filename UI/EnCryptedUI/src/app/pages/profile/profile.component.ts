import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../services/auth.service';
import { UpdatePasswordRequest } from '../../interfaces/update-password-request';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  passwordForm: FormGroup;
  userDetails: any;
  showPasswordForm = false;

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.passwordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  ngOnInit() {
    this.userDetails = this.authService.getUserDetails();
  }

  togglePasswordForm() {
    this.showPasswordForm = !this.showPasswordForm;
  }

  changePassword() {
    if (this.passwordForm.valid) {
      const updatePasswordRequest: UpdatePasswordRequest = {
        CurrentPassword: this.passwordForm.value.currentPassword,
        NewPassword: this.passwordForm.value.newPassword,
      };

      this.authService.updatePassword(updatePasswordRequest).subscribe({
        next: () => {
          alert('Password changed successfully.');
          this.passwordForm.reset();
          this.showPasswordForm = false;
        },
        error: (error) => {
          console.error('Password change failed:', error);
          alert('Failed to change password.');
        },
      });
    }
  }
}
