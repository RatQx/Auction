import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RessetPasword } from '../../models/reset-password.model';
import { ConfirmPasswordValidator } from '../../utils/confirm-password.validator';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-reset',
  templateUrl: './reset.component.html',
  styleUrl: './reset.component.scss',
})
export class ResetComponent implements OnInit {
  resetPasswordForm!: FormGroup;
  emailToReset!: string;
  emailToken!: string;
  resetPasswordObj = new RessetPasword();
  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private userService: UserService,
    public router: Router
  ) {}
  ngOnInit(): void {
    this.resetPasswordForm = this.fb.group(
      {
        password: [null, Validators.required],
        confirmPassword: [null, [Validators.required, Validators.minLength(8)]],
      },
      {
        validators: ConfirmPasswordValidator('password', 'confirmPassword'),
      }
    );
    this.activatedRoute.queryParams.subscribe((val) => {
      this.emailToReset = val['email'];
      let urlToken = val['code'];
      this.emailToken = urlToken.replace(/ /g, '+');
      console.log(this.emailToken);
      console.log(this.emailToReset);
    });
  }

  onSubmit() {
    if (this.resetPasswordForm.invalid) {
      return;
    } else {
      this.resetPasswordObj.email = this.emailToReset;
      this.resetPasswordObj.newPassword = this.resetPasswordForm.value.password;
      this.resetPasswordObj.confirmPassword =
        this.resetPasswordForm.value.confirmPassword;
      this.resetPasswordObj.emailtoken = this.emailToken;
      console.log(this.resetPasswordObj.email);
      console.log(this.resetPasswordObj.newPassword);
      console.log(this.resetPasswordObj.confirmPassword);
      console.log(this.resetPasswordObj.emailtoken);
      this.userService.resetPassword(this.resetPasswordObj).subscribe({
        next: (res) => {
          console.log('Password reset succesfully');
          this.router.navigate(['/']);
        },
        error: (err) => {
          console.log('Password not reset');
        },
      });
    }
  }
}
