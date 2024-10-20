import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TaskService, LogicTaskCreateDto, TaskCreateDto, TaskStartDto } from '../../services/task.service';
import { Subject } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';

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
  userRoles: string[] | null = null;
  router = inject(Router);
  private destroy$ = new Subject<void>();
  private snackBar = inject(MatSnackBar);
  private authService = inject(AuthService);

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      jobName: ['', Validators.required],
      text: ['', [Validators.required, this.maxLengthValidator(10000)]],
      passPhrase: ['', Validators.required],
      operation: ['encrypt', Validators.required],
      description: ['', Validators.required]
    });
    this.userRoles = this.authService.getUserRoles();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  maxLengthValidator(max: number) {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value as string;
      if (value && value.length > max) {
        return { 'maxLength': { requiredLength: max, actualLength: value.length } };
      }
      return null;
    };
  }

  async submitForm() {
    try {
      const taskCreate: TaskCreateDto = {
        title: this.form.value.jobName,
        description: this.form.value.description
      };

      const createdTaskResponse = await this.taskService.createTask(taskCreate).toPromise();
      if (createdTaskResponse && createdTaskResponse.taskID) {
        const taskData: LogicTaskCreateDto = {
          taskID: createdTaskResponse.taskID,
          allTextData: this.form.value.text,
          isEncrypted: this.form.value.operation !== 'encrypt',
          passPhrase: this.form.value.passPhrase
        };
        const tasksInProgress = await this.taskService.getTasks().toPromise().then((tasks) => {
          if (tasks && tasks.$values) {
            return tasks.$values.filter((task) => !task.isCompleted).length;
          }
          return 0;
        });
        if (tasksInProgress >= 5 && !(this.userRoles?.includes('admin') && tasksInProgress <= 10)) {
          this.snackBar.open('Maximum number of tasks in progress. Please wait for some tasks to complete.', 'Close', {
            duration: 5000,
            horizontalPosition: 'center',
            verticalPosition: 'top'
          });
          throw new Error('Maximum number of tasks in progress');
        }
        const encryptTaskResponse = await this.taskService.createEncryptTask(taskData).toPromise();
        if (encryptTaskResponse) {
          console.log('Task created:', encryptTaskResponse);
          this.monitorProgress(encryptTaskResponse.taskID);
          this.taskService.encryptOrDecryptTask(encryptTaskResponse.taskID).subscribe({
            next: (response: TaskStartDto) => {
                console.log('Task initiation response:', response);
            },
            error: (err) => {
                console.error('Error during task initiation:', err);
            }
        });
        } else {
          throw new Error('Invalid response from server');
        }
      } else {
        throw new Error('Task ID is null');
      }
    } catch (error) {
      if (error instanceof ErrorEvent) {
        console.error('Client-side error:', error.message);
      } else if (error instanceof HttpErrorResponse) {
        console.error(`Server-side error: Status ${error.status}, Error: ${JSON.stringify(error.error)}`);
      } else {
        console.error('Unexpected error:', error);
      }

      this.error = 'Failed to process task. Please try again.';
      this.isProcessing = false;
    }
  }



  private monitorProgress(taskID: string): void {
    if (this.router) {
      this.router.navigate(['/task-progress', taskID]);
    } else {
      console.error('Router is not initialized');
    }
  }
}
