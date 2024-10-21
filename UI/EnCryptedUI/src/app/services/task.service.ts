import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { catchError, map, Observable, of, tap, throwError } from 'rxjs';
import { Servers } from '../pages/users/users.component';

export interface EncryptionJob {
  id: string;
  userID: string;
  taskID: string;
  dataEncrypted: boolean;
  encryptedData: string;
  createdAt: Date;
  passPhrase: string;
}

export interface TaskStartDto {
  message: string;
  taskID: string;
}

export interface TaskCreateDto {
  title: string;
  description: string;
}

export interface LogicTaskCreateDto {
  taskID: string;
  allTextData: string;
  isEncrypted: boolean;
  passPhrase: string;
}

export interface TaskResponse {
  taskID: string;
  taskName: string;
  isCompleted: boolean;
  progress: number;
}

export interface Task {
  taskID: string;
  taskName: string;
  description: string;
  createdAt: string;
  isCompleted: boolean;
  progress: number;
}

export interface Tasks {
  $values: Task[];
  length: number;
}

export interface EncryptionJobResponse {
  $values: EncryptionJob[];
}

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  baseUrl: string = environment.apiBaseUrl;
  constructor(private http: HttpClient) { }

  createTask(data: TaskCreateDto): Observable<TaskResponse> {
    return this.http.post<TaskResponse>(
      `${this.baseUrl}Task/create`,
      data
    );
  }

  createEncryptTask(data: LogicTaskCreateDto): Observable<TaskResponse> {
    return this.http.post<TaskResponse>(
      `${this.baseUrl}Logic/createtasklogic`,
      data
    );
  }

  getEncryptionJobs(taskId: string): Observable<EncryptionJob[]> {
    return this.http.get<EncryptionJobResponse>(
        `${this.baseUrl}EncryptionJobs/task/${taskId}`
    ).pipe(
        map(response => response.$values) // Extract the $values array
    );
  }


  public encryptOrDecryptTask(taskID: string): Observable<TaskStartDto> {
    console.log('Initiating task with ID:', taskID);  // Log the task initiation
    return this.http.get<TaskStartDto>(`${this.baseUrl}Logic/dotask/${taskID}`);
  }

  getTaskProgress(taskId: string): Observable<number> {
    return this.http.get<number>(
      `${this.baseUrl}Logic/task/${taskId}/progress`
    ).pipe(
      catchError((error) => {
        console.error('Error fetching task progress:', error);
        return of(0);
      })
    );
  }

  getTasks(): Observable<Tasks> {
    return this.http.get<Tasks>(`${this.baseUrl}Task`);
  }

  getTask(taskId: string): Observable<Task> {
    return this.http.get<Task>(`${this.baseUrl}Task/${taskId}`);
  }

  public cancelTask(taskID: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}Logic/cancel/${taskID}`, {});
  }

  public getServerLoadingData(): Observable<Servers> {
    return this.http.get<Servers>(`${this.baseUrl}Logic/tasksinprogressonallservers`).pipe(
      catchError((error) => {
        if (error.status === 400) {
          console.error('Bad request:', error.message);
          return of({ $id: '', tasksInProgress: 0 } as unknown as Servers);
        } else {
          console.error('Error fetching server loading data:', error);
          return of({ $id: '', tasksInProgress: [] } as Servers);
        }
      })
    );
  }

}
