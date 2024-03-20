import { Component, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { LoginRequest } from '../../types/aukcionas.types';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  public resetPasswordEmail!: string;
  public isValidEmail!: boolean;
  isLoginFailed = false;
  errorMessage = '';
  emailSent = false;
  emailResetInvalid = false;
  displayMessageEmail = '';
  constructor(private userSerive: UserService, private router: Router) {}
  public form = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });
  ngOnInit(): void {}

  onSubmit() {
    try {
      this.userSerive.login(this.form.value as LoginRequest).subscribe(
        () => {
          this.isLoginFailed = false;
          this.router.navigate(['home']).then(() => {
            window.location.reload();
          });
        },
        (error) => {
          this.isLoginFailed = true;
          if (error.error === 'Invalid username or pasword.') {
            this.errorMessage = 'Username or password was incorrect. Try again';
          } else {
            this.handleError(error);
          }
        }
      );
    } catch {}
  }
  private handleError(error: any): void {
    if (error.error instanceof ErrorEvent) {
      this.errorMessage = `An error occurred: ${error.error.message}`;
    } else {
      this.errorMessage = error.error ? error.error : 'Server error';
    }
  }
  checkValidEmail(event: string) {
    const value = event;
    const pattern = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,3}$/;
    this.isValidEmail = pattern.test(value);
    return this.isValidEmail;
  }
  resetPasswordSend() {
    if (this.checkValidEmail(this.resetPasswordEmail)) {
      this.userSerive.sendResetPasswordLink(this.resetPasswordEmail).subscribe(
        () => {
          this.emailSent = true;
          this.emailResetInvalid = false;
          this.displayMessageEmail =
            'Password reset link was sent to :' + this.resetPasswordEmail;
        },
        (error) => {
          this.emailResetInvalid = true;
          this.emailSent = true;
          if (error.error === 'Email does not exist') {
            this.displayMessageEmail =
              'There is no registered user with this email. Try again.';
          }
          if (error.error === 'Confirm your email address before logging in.') {
            this.displayMessageEmail =
              'Please confirm your email address before trying to log in into your account.';
          } else {
            this.handleError(error);
          }
        }
      );
    }
  }
}
