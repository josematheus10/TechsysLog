import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

export interface CepResponse {
  cep: string;
  state: string;
  city: string;
  neighborhood: string;
  street: string;
  service?: string;
}

@Injectable({
  providedIn: 'root',
})
export class CepService {
  protected readonly http = inject(HttpClient);

  consultarCep(cep: string): Observable<CepResponse> {
    // Remove caracteres não numéricos
    const cepLimpo = cep.replace(/\D/g, '');
    return this.http.get<CepResponse>(`https://brasilapi.com.br/api/cep/v1/${cepLimpo}`);
  }
}
