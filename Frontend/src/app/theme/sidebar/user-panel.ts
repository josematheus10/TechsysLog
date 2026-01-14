import { Component, OnInit, ViewEncapsulation, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { AuthService, User } from '@core/authentication';
import { GravatarService } from '@core/services/gravatar.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-user-panel',
  template: `
    <div class="matero-user-panel" routerLink="/profile/overview">
      <img class="matero-user-panel-avatar" [src]="avatarUrl" alt="avatar" width="64" />
      <div class="matero-user-panel-info">
        <h4>{{ user.name || user.fullName || user.userName }}</h4>
        <h5>{{ user.email }}</h5>
      </div>
    </div>
  `,
  styleUrl: './user-panel.scss',
  encapsulation: ViewEncapsulation.None,
  imports: [RouterLink, MatButtonModule, MatIconModule, MatTooltipModule, TranslateModule],
})
export class UserPanel implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly gravatarService = inject(GravatarService);

  user!: User;
  avatarUrl = '';

  ngOnInit(): void {
    this.auth.user().subscribe(user => {
      this.user = user;
      this.avatarUrl = this.gravatarService.getGravatarUrl(user.email, { size: 128 });
    });
  }
}
