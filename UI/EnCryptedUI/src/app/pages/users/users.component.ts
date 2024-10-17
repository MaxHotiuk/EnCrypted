import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Observable } from 'rxjs';
import { UserDetail } from '../../interfaces/user-detail';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [AsyncPipe, NgFor, NgIf],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit {
  private authService = inject(AuthService);
  users$!: Observable<UserDetail[]>;

  ngOnInit() {
    this.users$ = this.authService.getAllUsers();
  }
}
