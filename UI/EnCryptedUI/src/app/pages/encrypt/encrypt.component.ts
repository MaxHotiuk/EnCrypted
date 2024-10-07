import { NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';

@Component({
  selector: 'app-encrypt',
  standalone: true,
  imports: [ReactiveFormsModule, MatButton, MatLabel, MatFormField, NgIf],
  templateUrl: './encrypt.component.html',
  styleUrls: ['./encrypt.component.css']
})
export class EncryptComponent implements OnInit {
  form!: FormGroup;
  result: string = '';

  fb = inject(FormBuilder);

  ngOnInit(): void {
    this.form = this.fb.group({
      jobName: ['', Validators.required],
      text: ['', Validators.required]
    });
  }

  encrypt(): void {
    const text = this.form.value.text;
    this.result = text
      .split('')
      .map((char: string) => String.fromCharCode(char.charCodeAt(0) + 1))
      .join('');
    console.log('Encrypted:', this.result);
  }

  decrypt(): void {
    const text = this.form.value.text;
    this.result = text
      .split('')
      .map((char: string) => String.fromCharCode(char.charCodeAt(0) - 1))
      .join('');
    console.log('Decrypted:', this.result);
  }

  submitForm() {
    if (this.form.valid) {
      console.log('Form submitted:', this.form.value);
    }
  }
}
