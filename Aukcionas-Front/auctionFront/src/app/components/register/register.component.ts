import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { RegisterRequest } from '../../types/aukcionas.types';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  isSuccessful = false;
  isSignUpFailed = false;
  errorMessage = '';

  constructor(private userSerive: UserService, private router: Router) {}

  public form = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
  });
  ngOnInit(): void {}

  onSubmit() {
    this.userSerive.register(this.form.value as RegisterRequest).subscribe(
      () => {
        this.isSuccessful = true;
        this.isSignUpFailed = false;
        this.router.navigate(['/login']);
        alert(
          'Confirmation email has been sent to your email adress: ' +
            this.form.value.email
        );
      },
      (error) => {
        this.isSuccessful = false;
        this.isSignUpFailed = true;
        if (error.error === 'User already exists') {
          this.errorMessage =
            'The provided username already exists. Please choose a different one.';
        }
        if (error.error === 'Email already taken') {
          this.errorMessage = 'Email already taken. Try another email.';
        } else {
          this.handleError(error);
        }
      }
    );
  }
  private handleError(error: any): void {
    if (error.error instanceof ErrorEvent) {
      this.errorMessage = `An error occurred: ${error.error.message}`;
    } else {
      this.errorMessage = error.error ? error.error : 'Server error';
    }
  }
}
