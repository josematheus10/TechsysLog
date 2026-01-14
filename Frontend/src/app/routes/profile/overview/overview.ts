import { Component, inject, OnInit } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { AuthService } from '@core/authentication/auth.service';
import { GravatarService } from '@core/services/gravatar.service';
import { User } from '@core/authentication/interface';

@Component({
  selector: 'app-profile-overview',
  templateUrl: './overview.html',
  styleUrl: './overview.scss',
  imports: [CommonModule, MatCardModule, MatTabsModule],
})
export class ProfileOverview implements OnInit {
  private readonly authService = inject(AuthService);
  readonly gravatarService = inject(GravatarService);

  user: User | null = null;
  avatarUrl = '';

  ngOnInit(): void {
    this.authService.user().subscribe(user => {
      this.user = user;
      this.avatarUrl = this.gravatarService.getGravatarUrl(user.email, { size: 200 });
    });
  }
}
