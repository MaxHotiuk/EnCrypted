import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskProgressComponent } from './task-progress.component';

describe('TaskProgressComponent', () => {
  let component: TaskProgressComponent;
  let fixture: ComponentFixture<TaskProgressComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskProgressComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TaskProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
