import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root',
})
export class GravatarService {
  private readonly gravatarBaseUrl = 'https://www.gravatar.com/avatar/';
  private readonly defaultOptions = {
    size: 200,
    defaultImage: 'identicon', // Options: 404, mp, identicon, monsterid, wavatar, retro, robohash, blank
    rating: 'g', // Options: g, pg, r, x
  };

  /**
   * Generates a Gravatar URL based on the provided email address
   * @param email - The email address to generate the Gravatar for
   * @param options - Optional parameters for the Gravatar (size, defaultImage, rating)
   * @returns The complete Gravatar URL
   */
  getGravatarUrl(
    email: string | undefined,
    options?: { size?: number; defaultImage?: string; rating?: string }
  ): string {
    if (!email) {
      return this.getDefaultAvatarUrl(options?.size);
    }

    const normalizedEmail = email.trim().toLowerCase();
    const hash = CryptoJS.MD5(normalizedEmail).toString();

    const params = new URLSearchParams({
      s: (options?.size || this.defaultOptions.size).toString(),
      d: options?.defaultImage || this.defaultOptions.defaultImage,
      r: options?.rating || this.defaultOptions.rating,
    });

    return `${this.gravatarBaseUrl}${hash}?${params.toString()}`;
  }

  /**
   * Returns a default avatar URL when no email is provided
   * @param size - The size of the avatar
   * @returns A default avatar URL
   */
  private getDefaultAvatarUrl(size?: number): string {
    const avatarSize = size || this.defaultOptions.size;
    return `${this.gravatarBaseUrl}00000000000000000000000000000000?s=${avatarSize}&d=${this.defaultOptions.defaultImage}`;
  }
}
