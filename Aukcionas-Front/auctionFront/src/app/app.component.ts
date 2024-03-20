import { Router } from '@angular/router';
import { UserService } from './services/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  title = 'Aukcionai';
  searchTerm: string = '';
  showDropdown: boolean = false;
  public isCollapsed = true;
  public isLoged: boolean = false;
  public isAdmin = false;
  constructor(private userService: UserService, private router: Router) {
    this.isLoged = this.userService.isAuthenticated;
    this.isAdmin = this.userService.isAdmin;
  }
  ngOnInit(): void {
    this.userService.checkAuthentication().subscribe((isAuthenticated) => {
      this.userService.updateAuthenticationState(isAuthenticated);
    });
  }

  logOut() {
    this.userService.logout();
    this.router.navigate(['/home']).then(() => {
      window.location.reload();
    });
  }

  onSearch(event: any) {
    const searchTerm = event?.target?.value || '';
    this.router.navigate(['/auction'], { queryParams: { name: searchTerm } });
  }

  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }
}
