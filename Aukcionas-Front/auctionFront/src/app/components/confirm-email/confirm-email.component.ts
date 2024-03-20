import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss'],
})
export class ConfirmEmailComponent implements OnInit {
  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) {}
  ngOnInit(): void {
    this.confirmEmail();
  }
  confirmEmail() {
    const email = this.route.snapshot.queryParams['email'];
    const code = this.route.snapshot.queryParams['code'];

    if (email && code) {
      this.userService.confirmEmail(email, code).subscribe(
        (response) => {
          console.log('Confirmation success:', response);
          alert('Email confirmed successfully. You can now log in.');
          this.router.navigate(['/login']);
        },
        (error) => {
          console.error('Confirmation error:', error);
          alert('Failed to confirm email. Please try again.');
          this.router.navigate(['/login']);
        }
      );
    } else {
      alert('Invalid confirmation link.');
      this.router.navigate(['/login']);
    }
  }
}
