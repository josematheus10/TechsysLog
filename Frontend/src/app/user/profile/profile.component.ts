import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserProfile } from '../../models/auth.models';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  standalone: false
})
export class ProfileComponent implements OnInit {
  profile: UserProfile | null = null;
  loading = true;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.getProfile().subscribe({
      next: (data) => {
        this.profile = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load profile', err);
        this.loading = false;
      }
    });
  }
}
