import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TaskService } from '../../services/task.service';
import { Subject, interval } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatProgressBar } from '@angular/material/progress-bar';
import { CommonModule, NgIf } from '@angular/common';

export interface EncryptionJobResponse {
  $id: string;
  $values: EncryptionJob[];
}
export interface EncryptionJob {
  jobID: string;
  userID: string;
  taskID: string;
  dataEncrypted: boolean;
  encryptedData: string;
  decryptedData: string;
  passPhrase: string;
  createdAt: string;
}
@Component({
  selector: 'app-task-progress',
  standalone: true,
  templateUrl: './task-progress.component.html',
  styleUrls: ['./task-progress.component.css'],
  imports: [
    MatProgressBar,
    CommonModule,
    NgIf
  ]
})
export class TaskProgressComponent implements OnInit {
  taskID: string | undefined;
  progress: number = 0;
  isProcessing: boolean = true;
  result: string = '';
  destroy$ = new Subject<void>();
  error: string = '';

  constructor(private route: ActivatedRoute, private taskService: TaskService) { }

  ngOnInit(): void {
    this.taskID = this.route.snapshot.paramMap.get('taskID') ?? '';
    this.monitorProgress(this.taskID);
  }

  private monitorProgress(taskID: string): void {
    interval(1000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.taskService.getTaskProgress(taskID).subscribe(progress => {
          this.progress = progress;

          if (progress === 100) {
            this.isProcessing = false;

            // Fetch all encryption jobs for the task
            this.taskService.getEncryptionJobs(taskID).subscribe(
              jobsResponse => {
                console.log('Encryption jobs:', jobsResponse); // Log the entire response
                jobsResponse.forEach(job => {
                  console.log('Job details:', job); // Log each job
                });

                // Check if jobsResponse contains jobs
                if (jobsResponse && jobsResponse.length > 0) {
                  this.result = jobsResponse
                    .filter(job => job.dataEncrypted) // Ensure we only map encrypted data
                    .map(job => job.encryptedData) // Map to the encrypted data
                    .join(' ');

                  console.log('Accumulated encrypted data:', this.result);
                } else {
                  this.error = 'No encryption jobs found for the task';
                }

                // Stop the interval
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
        }, error => {
          console.error('Error fetching task progress:', error);
        });
      });
}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
