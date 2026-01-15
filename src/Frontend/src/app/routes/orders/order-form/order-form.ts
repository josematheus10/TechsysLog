import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { OrdersService } from '../orders.service';
import { CepService } from '../cep.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-order-form',
  templateUrl: './order-form.html',
  styleUrl: './order-form.scss',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
})
export class OrderForm implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly ordersService = inject(OrdersService);
  private readonly cepService = inject(CepService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);

  orderForm!: FormGroup;
  isLoadingCep = false;
  isSubmitting = false;

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.orderForm = this.fb.group({
      orderNumber: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      value: ['', [Validators.required, Validators.min(0.01)]],
      deliveryAddress: this.fb.group({
        cep: ['', [Validators.required, Validators.pattern(/^\d{5}-?\d{3}$/)]],
        street: ['', [Validators.required, Validators.maxLength(200)]],
        number: ['', [Validators.required, Validators.maxLength(10)]],
        neighborhood: ['', [Validators.required, Validators.maxLength(100)]],
        city: ['', [Validators.required, Validators.maxLength(100)]],
        state: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(2)]],
      }),
    });

    // Adiciona listener para buscar CEP automaticamente
    this.orderForm.get('deliveryAddress.cep')?.valueChanges.subscribe(cep => {
      if (cep && cep.replace(/\D/g, '').length === 8) {
        this.buscarCep(cep);
      }
    });
  }

  buscarCep(cep: string): void {
    if (!cep || this.isLoadingCep) return;

    const cepLimpo = cep.replace(/\D/g, '');
    if (cepLimpo.length !== 8) {
      this.snackBar.open('CEP inválido', 'Fechar', { duration: 3000 });
      return;
    }

    this.isLoadingCep = true;
    const addressGroup = this.orderForm.get('deliveryAddress');

    this.cepService
      .consultarCep(cepLimpo)
      .pipe(finalize(() => (this.isLoadingCep = false)))
      .subscribe({
        next: response => {
          addressGroup?.patchValue({
            street: response.street || '',
            neighborhood: response.neighborhood || '',
            city: response.city || '',
            state: response.state || '',
          });

          // Foca no campo de número
          setTimeout(() => {
            document.getElementById('number')?.focus();
          }, 100);
        },
        error: error => {
          this.snackBar.open(
            'Erro ao buscar CEP. Verifique se o CEP está correto.',
            'Fechar',
            { duration: 5000 }
          );
        },
      });
  }

  onSubmit(): void {
    if (this.orderForm.invalid) {
      this.markFormGroupTouched(this.orderForm);
      this.snackBar.open('Por favor, preencha todos os campos obrigatórios', 'Fechar', {
        duration: 3000,
      });
      return;
    }

    this.isSubmitting = true;
    const formValue = this.orderForm.value;

    // Formata o CEP
    const cep = formValue.deliveryAddress.cep.replace(/\D/g, '');
    const cepFormatado = `${cep.slice(0, 5)}-${cep.slice(5)}`;

    const orderData = {
      ...formValue,
      deliveryAddress: {
        ...formValue.deliveryAddress,
        cep: cepFormatado,
        state: formValue.deliveryAddress.state.toUpperCase(),
      },
    };

    this.ordersService
      .createOrder(orderData)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: response => {
          this.snackBar.open('Pedido criado com sucesso!', 'Fechar', {
            duration: 3000,
          });
          this.router.navigate(['/dashboard']);
        },
        error: error => {
          let errorMessage = 'Erro ao criar pedido. Tente novamente.';

          if (error.status === 409) {
            errorMessage = 'Já existe um pedido com este número.';
          } else if (error.error?.message) {
            errorMessage = error.error.message;
          } else if (error.error?.errors) {
            const errors = Object.values(error.error.errors).flat();
            errorMessage = (errors as string[]).join('\n');
          }

          this.snackBar.open(errorMessage, 'Fechar', { duration: 5000 });
        },
      });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  getErrorMessage(fieldName: string): string {
    const control = this.orderForm.get(fieldName);
    if (!control || !control.errors || !control.touched) return '';

    if (control.errors['required']) return 'Este campo é obrigatório';
    if (control.errors['min']) return `Valor mínimo: ${control.errors['min'].min}`;
    if (control.errors['maxLength'])
      return `Tamanho máximo: ${control.errors['maxLength'].requiredLength} caracteres`;
    if (control.errors['minLength'])
      return `Tamanho mínimo: ${control.errors['minLength'].requiredLength} caracteres`;
    if (control.errors['pattern']) {
      if (fieldName.includes('cep')) return 'CEP inválido. Use o formato: 12345-678';
    }

    return 'Campo inválido';
  }
}
