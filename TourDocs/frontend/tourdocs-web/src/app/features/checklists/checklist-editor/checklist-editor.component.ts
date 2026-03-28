import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { NotificationService } from '@core/services/notification.service';
import { ApiService } from '@core/services/api.service';

@Component({
  selector: 'td-checklist-editor',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterModule,
    MatFormFieldModule, MatInputModule, MatSelectModule,
    MatCheckboxModule, MatButtonModule, MatIconModule,
    MatCardModule, MatProgressSpinnerModule, PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header [title]="isEdit ? 'Edit Checklist' : 'New Checklist'"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Checklists', link: '/checklists'}, {label: isEdit ? 'Edit' : 'New'}]">
      </td-page-header>

      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form-container">
        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Checklist Information</h3>
            <mat-form-field appearance="outline">
              <mat-label>Name</mat-label>
              <input matInput formControlName="name" placeholder="e.g., Schengen Visa Checklist">
            </mat-form-field>
            <div class="form-row">
              <mat-form-field appearance="outline">
                <mat-label>Country</mat-label>
                <input matInput formControlName="country">
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Country Code</mat-label>
                <input matInput formControlName="countryCode" placeholder="e.g., DE, US, GB">
              </mat-form-field>
            </div>
            <mat-form-field appearance="outline">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" rows="3"></textarea>
            </mat-form-field>
          </div>
        </mat-card>

        <mat-card class="form-card">
          <div class="form-section">
            <h3 class="form-section__title">Checklist Items</h3>
            <div formArrayName="items">
              @for (item of itemsArray.controls; track $index; let i = $index) {
                <div [formGroupName]="i" class="checklist-item-row">
                  <mat-form-field appearance="outline" class="item-name-field">
                    <mat-label>Item Name</mat-label>
                    <input matInput formControlName="name">
                  </mat-form-field>
                  <mat-form-field appearance="outline" class="item-category-field">
                    <mat-label>Category</mat-label>
                    <mat-select formControlName="category">
                      <mat-option value="Identity">Identity</mat-option>
                      <mat-option value="Travel">Travel</mat-option>
                      <mat-option value="Medical">Medical</mat-option>
                      <mat-option value="Financial">Financial</mat-option>
                      <mat-option value="Legal">Legal</mat-option>
                    </mat-select>
                  </mat-form-field>
                  <mat-checkbox formControlName="required" color="primary">Required</mat-checkbox>
                  <button mat-icon-button color="warn" type="button" (click)="removeItem(i)">
                    <mat-icon>delete</mat-icon>
                  </button>
                </div>
              }
            </div>
            <button mat-stroked-button color="primary" type="button" (click)="addItem()">
              <mat-icon>add</mat-icon> Add Item
            </button>
          </div>
        </mat-card>

        <div class="form-actions">
          <button mat-button type="button" routerLink="/checklists">Cancel</button>
          <button mat-flat-button color="primary" type="submit" [disabled]="form.invalid || saving">
            @if (saving) {
              <mat-spinner diameter="20"></mat-spinner>
            } @else {
              {{ isEdit ? 'Update' : 'Create' }}
            }
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .checklist-item-row {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 8px;
    }
    .item-name-field { flex: 2; }
    .item-category-field { flex: 1; }
  `]
})
export class ChecklistEditorComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ApiService);
  private readonly notification = inject(NotificationService);

  form!: FormGroup;
  isEdit = false;
  saving = false;

  get itemsArray(): FormArray {
    return this.form.get('items') as FormArray;
  }

  ngOnInit(): void {
    this.isEdit = this.route.snapshot.data['mode'] === 'edit';

    this.form = this.fb.group({
      name: ['', Validators.required],
      country: ['', Validators.required],
      countryCode: [''],
      description: [''],
      items: this.fb.array([])
    });

    if (this.isEdit) {
      const id = this.route.snapshot.paramMap.get('id');
      if (id) {
        this.api.get<any>(`checklists/${id}`).subscribe({
          next: (r) => {
            if (r.success) {
              this.form.patchValue(r.data);
              r.data.items?.forEach((item: any) => this.addItem(item));
            }
          }
        });
      }
    } else {
      this.addItem();
    }
  }

  addItem(data?: { name: string; category: string; required: boolean; description: string }): void {
    this.itemsArray.push(this.fb.group({
      name: [data?.name || '', Validators.required],
      category: [data?.category || 'Identity'],
      required: [data?.required ?? true],
      description: [data?.description || '']
    }));
  }

  removeItem(index: number): void {
    this.itemsArray.removeAt(index);
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.saving = true;
      const data = this.form.value;
      const id = this.route.snapshot.paramMap.get('id');
      const req = this.isEdit && id
        ? this.api.put(`checklists/${id}`, data)
        : this.api.post('checklists', data);

      req.subscribe({
        next: () => {
          this.saving = false;
          this.notification.showSuccess(this.isEdit ? 'Checklist updated' : 'Checklist created');
          this.router.navigate(['/checklists']);
        },
        error: () => { this.saving = false; }
      });
    }
  }
}
