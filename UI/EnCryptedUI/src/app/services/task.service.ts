// task.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';

export interface EncryptionJob {
  id: string;
  userID: string;
  taskID: string;
  dataEncrypted: boolean;
  encryptedData: string;
  createdAt: Date;
  passPhrase: string;
}

export interface LogicTaskCreateDto {
  taskName: string;
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

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  baseUrl: string = environment.apiBaseUrl;
  private tokenKey = 'token';
  constructor(private http: HttpClient) { }

  // Create a new encryption task
  createEncryptTask(data: LogicTaskCreateDto): Observable<TaskResponse> {
    return this.http.post<TaskResponse>(
      `${this.baseUrl}/Logic/createtasklogic`,
      data
    );
  }

  // Get all encryption jobs for a specific task
  getEncryptionJobs(taskId: string): Observable<EncryptionJob[]> {
    return this.http.get<EncryptionJob[]>(
      `${this.baseUrl}Logic/task/${taskId}`
    );
  }

  // Encrypt or decrypt data for a specific task
  encryptOrDecryptTask(taskId: string): Observable<any> {
    return this.http.put(
      `${this.baseUrl}Logic/dotask`,
      null,
      { params: { taskId } }
    );
  }

  // Get task progress
  getTaskProgress(taskId: string): Observable<number> {
    return this.http.get<number>(
      `${this.baseUrl}Logic/task/${taskId}/progress`
    );
  }
}
