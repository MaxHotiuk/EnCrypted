import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { interval, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatProgressBar } from '@angular/material/progress-bar';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Tasks, TaskService } from '../../services/task.service';

interface Task {
  taskID: string;
  taskName: string;
  description: string;
  createdAt: string;
  isCompleted: boolean;
  progress: number;
}

@Component({
  selector: 'app-task-list',
  standalone: true,
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css'],
  imports: [
    MatProgressBar,
    NgClass,
    NgFor,
    NgIf,
    RouterLink
  ]
})
export class TaskListComponent implements OnInit, OnDestroy {
  tasks: Tasks = { $values: [], length: 0 };
  destroy$ = new Subject<void>();

  constructor(
    private http: HttpClient,
    private router: Router,
    private taskService: TaskService
  ) {}

  ngOnInit(): void {
    this.monitorTaskProgress();
    this.taskService = new TaskService(this.http);
  }

  fetchTasks(): void {
    this.taskService.getTasks().subscribe((tasks) => {
      this.tasks.$values = tasks.$values.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    });
  }

  monitorTaskProgress(): void {
    this.fetchTasks();
    interval(5000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.fetchTasks();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
