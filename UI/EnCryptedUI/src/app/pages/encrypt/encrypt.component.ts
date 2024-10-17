// encrypt.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TaskService, LogicTaskCreateDto } from '../../services/task.service';
import { Subject, interval } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-encrypt',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressBarModule
  ],
  templateUrl: './encrypt.component.html',
  styleUrl: './encrypt.component.css'
})
export class EncryptComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  progress: number = 0;
  result: string = '';
  currentTaskId: string | null = null;
  isProcessing: boolean = false;
  error: string | null = null;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      jobName: ['', Validators.required],
      text: ['', Validators.required],
      passPhrase: ['', Validators.required],
      operation: ['encrypt', Validators.required]
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  async submitForm(): Promise<void> {
    if (this.form.invalid || this.isProcessing) return;

    this.isProcessing = true;
    this.progress = 0;
    this.result = '';
    this.error = null;

    try {
      const taskData: LogicTaskCreateDto = {
        taskName: this.form.value.jobName,
        allTextData: this.form.value.text,
        isEncrypted: this.form.value.operation === 'encrypt',
        passPhrase: this.form.value.passPhrase
      };

      const response = await this.taskService.createEncryptTask(taskData).toPromise();

      if (response && response.taskID) {
        this.currentTaskId = response.taskID;

        // Start the encryption/decryption process
        await this.taskService.encryptOrDecryptTask(this.currentTaskId).toPromise();

        // Start monitoring progress
        this.monitorProgress(this.currentTaskId);
      } else {
        throw new Error('Invalid response from server');
      }
    } catch (error) {
      console.error('Error processing task:', error);
      this.error = 'Failed to process task. Please try again.';
      this.isProcessing = false;
    }
  }

  private monitorProgress(taskId: string): void {
    interval(1000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(async () => {
        try {
          const progress = await this.taskService.getTaskProgress(taskId).toPromise() ?? 0;
          this.progress = progress;

          if (progress === 100) {
            this.isProcessing = false;
            const jobs = await this.taskService.getEncryptionJobs(taskId).toPromise();
            if (jobs) {
              this.result = jobs
                .map(job => job.encryptedData)
                .join(' ');
            } else {
              this.error = 'No jobs found for the task';
            }
            this.destroy$.next(); // Stop monitoring
          }
        } catch (error) {
          console.error('Error monitoring progress:', error);
          this.error = 'Error monitoring task progress';
          this.isProcessing = false;
          this.destroy$.next(); // Stop monitoring on error
        }
      });
  }
}
