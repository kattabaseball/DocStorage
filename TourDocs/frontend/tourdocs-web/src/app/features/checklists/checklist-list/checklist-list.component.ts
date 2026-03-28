import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '@shared/components/page-header/page-header.component';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';
import { ApiService } from '@core/services/api.service';

interface Checklist {
  id: string;
  name: string;
  country: string;
  countryCode: string;
  itemsCount: number;
  description: string;
  updatedAt: string;
}

@Component({
  selector: 'td-checklist-list',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatCardModule, MatIconModule,
    MatButtonModule, MatChipsModule, MatProgressSpinnerModule,
    PageHeaderComponent, EmptyStateComponent
  ],
  template: `
    <div class="page-container">
      <td-page-header title="Checklists"
                      [breadcrumbs]="[{label: 'Home', link: '/', icon: 'home'}, {label: 'Checklists'}]">
        <button mat-flat-button color="primary" routerLink="/checklists/new">
          <mat-icon>add</mat-icon>
          New Checklist
        </button>
      </td-page-header>

      @if (loading) {
        <div class="flex-center" style="height: 300px;"><mat-spinner diameter="40"></mat-spinner></div>
      } @else if (checklists.length === 0) {
        <td-empty-state icon="checklist" title="No checklists yet"
                        subtitle="Create document checklists for different countries and trip types"
                        actionText="Create Checklist" actionLink="/checklists/new">
        </td-empty-state>
      } @else {
        <div class="grid-3col">
          @for (cl of checklists; track cl.id) {
            <mat-card class="checklist-card clickable" [routerLink]="['/checklists', cl.id]">
              <div class="checklist-card__header">
                <span class="checklist-card__flag">{{ cl.countryCode }}</span>
                <h3 class="checklist-card__name">{{ cl.name }}</h3>
              </div>
              <p class="checklist-card__desc">{{ cl.description }}</p>
              <div class="checklist-card__footer">
                <mat-chip-set>
                  <mat-chip>
                    <mat-icon matChipAvatar>list</mat-icon>
                    {{ cl.itemsCount }} items
                  </mat-chip>
                </mat-chip-set>
                <span class="text-muted">{{ cl.country }}</span>
              </div>
            </mat-card>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .checklist-card {
      padding: 24px;
      cursor: pointer;
      transition: box-shadow 0.2s, transform 0.2s;
    }
    .checklist-card:hover {
      box-shadow: 0 4px 16px rgba(0,0,0,0.12);
      transform: translateY(-2px);
    }
    .checklist-card__header {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 12px;
    }
    .checklist-card__flag {
      font-size: 28px;
      line-height: 1;
    }
    .checklist-card__name {
      font-size: 16px;
      font-weight: 600;
      color: #263238;
      margin: 0;
    }
    .checklist-card__desc {
      font-size: 13px;
      color: #546E7A;
      margin: 0 0 16px;
      line-height: 1.5;
    }
    .checklist-card__footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
  `]
})
export class ChecklistListComponent implements OnInit {
  private readonly api = inject(ApiService);
  checklists: Checklist[] = [];
  loading = false;

  ngOnInit(): void {
    this.loading = true;
    this.api.get<Checklist[]>('checklists').subscribe({
      next: (r) => { if (r.success) this.checklists = r.data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
