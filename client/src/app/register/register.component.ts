import { ToastrService } from 'ngx-toastr';
import { Component, inject, OnInit, output } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { JsonPipe, NgIf } from '@angular/common';
import { NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, JsonPipe, NgIf, NgbDatepicker],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  private accountService = inject(AccountService);
  private toastr = inject(ToastrService)
  private router = inject(Router)
  cancelRegister = output<boolean>();
  model: any = {};
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date();
  validationErrors: string[] | undefined;

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18)
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      gender: new FormControl(['male']), 
      username: new FormControl('', Validators.required),
      knownAs: new FormControl('', Validators.required), 
      dateOfBirth: new FormControl('', Validators.required), 
      city: new FormControl('', Validators.required), 
      country: new FormControl('', Validators.required), 
      password: new FormControl('', [Validators.required, Validators.minLength(4), 
        Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo: string) : ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: true}
    }
  }

  register() {
    this.accountService.register(this.model).subscribe({
      next: _ => this.router.navigateByUrl('/members'),
      error: error => this.validationErrors = error
    })
  }

  cancel() {
    this.cancelRegister.emit(false)
  }
}
