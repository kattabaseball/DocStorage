import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { SearchInputComponent } from '@shared/components/search-input/search-input.component';
import { Router } from '@angular/router';

@Component({
  selector: 'td-search-bar',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule, SearchInputComponent],
  template: `
    @if (expanded) {
      <div class="search-bar__expanded">
        <td-search-input placeholder="Search documents, members, cases..."
                         (searchChange)="onSearch($event)">
        </td-search-input>
        <button mat-icon-button (click)="collapse()">
          <mat-icon>close</mat-icon>
        </button>
      </div>
    } @else {
      <button mat-icon-button (click)="expand()">
        <mat-icon>search</mat-icon>
      </button>
    }
  `,
  styles: [`
    :host {
      display: flex;
      align-items: center;
    }

    .search-bar__expanded {
      display: flex;
      align-items: center;
      gap: 4px;
      animation: slideIn 0.2s ease;
    }

    @keyframes slideIn {
      from {
        opacity: 0;
        transform: translateX(20px);
      }
      to {
        opacity: 1;
        transform: translateX(0);
      }
    }
  `]
})
export class SearchBarComponent {
  expanded = false;

  constructor(private router: Router) {}

  expand(): void {
    this.expanded = true;
  }

  collapse(): void {
    this.expanded = false;
  }

  onSearch(query: string): void {
    if (query.trim()) {
      this.router.navigate(['/search'], { queryParams: { q: query } });
    }
  }
}
