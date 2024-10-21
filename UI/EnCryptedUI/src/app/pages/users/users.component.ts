import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Observable } from 'rxjs';
import { UserDetail } from '../../interfaces/user-detail';
import { TaskService } from '../../services/task.service';
import { BehaviorSubject } from 'rxjs';

export interface Server {
  $id: string;
  serverName: string;
  tasksCount: number;
}
export interface Servers {
  $id: string;
  tasksInProgress: Server[];
}
@Component({
  selector: 'app-users',
  standalone: true,
  imports: [AsyncPipe, NgFor, NgIf],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  private authService = inject(AuthService);
  users$!: Observable<UserDetail[]>;
  ngOnInit() {
    this.loadUsers();

    setInterval(() => {
      this.loadUsers();
    }, 10000);
  }

  private loadUsers() {
    this.users$ = this.authService.getAllUsers();
    this.users$.subscribe(users => {
      for (let user of users) {
        console.log(user);
      }
    });
  }
}
