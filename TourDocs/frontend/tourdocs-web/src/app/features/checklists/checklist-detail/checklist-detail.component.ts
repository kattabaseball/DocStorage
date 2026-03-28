import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { ApiService } from '@core/services/api.service';

interface ChecklistItem {
  id: string;
  name: string;
  category: string;
  required: boolean;
  description: string;
}

interface ChecklistDetail {
  id: string;
  name: string;
  country: string;
  description: string;
  items: ChecklistItem[];
}

@Component({
  selector: 'td-checklist-detail',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatCardModule, MatIconModule,
    MatButtonModule, MatListModule, MatChipsModule, MatProgressSpinnerModule,
    PageHeaderComponent
  ],
  template: `
    <div class="page-container">
      @if (loading) {
        <div class="flex-center" style="height: 300px;"><mat-spinner diameter="40"></mat-spinner></div>
      } @else if (checklist) {
        <td-page-header [title]="checklist.name"
                        [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Checklists', link: '/checklists'}, {label: checklist.name}]">
          <button mat-stroked-button [routerLink]="['/checklists', checklist.id, 'edit']">
            <mat-icon>edit</mat-icon> Edit
          </button>
        </td-page-header>

        <mat-card>
          <mat-card-header>
            <mat-card-title>{{ checklist.country }} - Document Requirements</mat-card-title>
            <mat-card-subtitle>{{ checklist.description }}</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <mat-list>
              @for (item of checklist.items; track item.id) {
                <mat-list-item class="checklist-item">
                  <mat-icon matListItemIcon [style.color]="item.required ? '#EF5350' : '#90A4AE'">
                    {{ item.required ? 'check_circle' : 'radio_button_unchecked' }}
                  </mat-icon>
                  <div matListItemTitle>
                    {{ item.name }}
                    @if (item.required) {
                      <mat-chip class="required-chip">Required</mat-chip>
                    }
                  </div>
                  <div matListItemLine class="text-muted">{{ item.description }}</div>
                  <div matListItemMeta>
                    <mat-chip>{{ item.category }}</mat-chip>
                  </div>
                </mat-list-item>
              }
            </mat-list>
          </mat-card-content>
        </mat-card>
      }
    </div>
  `,
  styles: [`
    .checklist-item { border-bottom: 1px solid #E0E0E0; }
    .required-chip { font-size: 10px; height: 20px; margin-left: 8px; background: rgba(239,83,80,0.1); color: #C62828; }
  `]
})
export class ChecklistDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  checklist: ChecklistDetail | null = null;
  loading = true;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.get<ChecklistDetail>(`checklists/${id}`).subscribe({
        next: (r) => { if (r.success) this.checklist = r.data; this.loading = false; },
        error: () => { this.loading = false; }
      });
    }
  }
}
