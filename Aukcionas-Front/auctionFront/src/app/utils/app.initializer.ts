import { Injectable } from '@angular/core';
import { UserService } from '../services/user.service';

declare let require: any;

@Injectable({ providedIn: 'root' })
export class AppInitializer {
  constructor(private userService: UserService) {}

  public async initialize(): Promise<void> {
    this.userService.validateSession();
    console.log('Session validation');
  }
}
