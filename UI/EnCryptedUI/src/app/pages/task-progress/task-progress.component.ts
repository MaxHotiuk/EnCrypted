import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Task, TaskService } from '../../services/task.service';
import { Subject, interval } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatProgressBar } from '@angular/material/progress-bar';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-task-progress',
  standalone: true,
  templateUrl: './task-progress.component.html',
  styleUrls: ['./task-progress.component.css'],
  imports: [
    MatProgressBar,
    CommonModule,
    NgIf,
    RouterLink
  ]
})
export class TaskProgressComponent implements OnInit {
  taskID: string | undefined;
  progress: number = 0;
  isProcessing: boolean = true;
  result: string = '';
  destroy$ = new Subject<void>();
  error: string = '';
  message: string = '';
  taskData: Task | undefined;
  isCancelled: boolean = false;

  constructor(private route: ActivatedRoute, private taskService: TaskService) { }

  ngOnInit(): void {
    this.taskID = this.route.snapshot.paramMap.get('taskID') ?? '';
    this.monitorProgress(this.taskID);
    this.taskService.getTask(this.taskID).subscribe(task => {
      this.taskData = task;
    });
  }

  private getTaskProgress(taskID: string): void {
    this.taskService.getTaskProgress(taskID).subscribe(progress => {
      this.progress = progress;

      if (progress === 100) {
        this.isProcessing = false;
        this.fetchEncryptionJobs(taskID);
      }
    });
  }

  private monitorProgress(taskID: string): void {
    this.getTaskProgress(taskID);
    interval(1000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.getTaskProgress(taskID);
      });
  }

  private fetchEncryptionJobs(taskID: string): void {
    this.taskService.getEncryptionJobs(taskID).subscribe(
      jobsResponse => {
        console.log('Encryption jobs:', jobsResponse);
        jobsResponse.forEach(job => {
          console.log('Job details:', job);
        });

        if (jobsResponse && jobsResponse.length > 0) {
          this.result = jobsResponse.map(job => job.encryptedData).join(' ');
          console.log('Accumulated encrypted data:', this.result);
        } else {
          this.error = 'No encryption jobs found for the task';
        }
        this.destroy$.next();
      },
      error => {
        console.error('Error fetching encryption jobs:', error);
        this.isProcessing = false;
        this.error = 'Failed to retrieve encryption jobs.';
        this.destroy$.next();
      }
    );
  }

  cancelTask(): void {
    this.taskService.cancelTask(this.taskID!).subscribe(
      response => {
        console.log('Task cancellation response:', response);
        this.isProcessing = false;
        this.isCancelled = true;
        this.error = 'Task was successfully canceled.';
      },
      error => {
        console.error('Error canceling task:', error);
        this.error = 'Failed to cancel the task.';
      }
    );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
