import { Component, Input, Output, EventEmitter, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { EmptyStateComponent } from '../empty-state/empty-state.component';
import { SearchInputComponent } from '../search-input/search-input.component';

export interface ColumnConfig {
  key: string;
  label: string;
  sortable?: boolean;
  type?: 'text' | 'date' | 'status' | 'avatar' | 'actions' | 'number' | 'template';
  width?: string;
}

@Component({
  selector: 'td-data-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressSpinnerModule,
    MatIconModule,
    EmptyStateComponent,
    SearchInputComponent
  ],
  template: `
    <div class="td-table-container">
      @if (showSearch) {
        <div class="td-table-header">
          <h3 class="td-table-title">{{ tableTitle }}</h3>
          <div class="td-table-actions">
            <td-search-input (searchChange)="onSearch($event)" [placeholder]="searchPlaceholder"></td-search-input>
            <ng-content select="[tableActions]"></ng-content>
          </div>
        </div>
      }

      <div class="table-wrapper" [class.table-loading]="loading">
        @if (loading) {
          <div class="table-loading-shade">
            <mat-spinner diameter="40"></mat-spinner>
          </div>
        }

        @if (!loading && dataSource.data.length === 0) {
          <td-empty-state
            [icon]="emptyIcon"
            [title]="emptyTitle"
            [subtitle]="emptySubtitle">
          </td-empty-state>
        } @else {
          <table mat-table [dataSource]="dataSource" matSort (matSortChange)="onSortChange($event)">
            @for (col of columns; track col.key) {
              <ng-container [matColumnDef]="col.key">
                <th mat-header-cell *matHeaderCellDef
                    [mat-sort-header]="col.sortable !== false ? col.key : ''"
                    [disabled]="col.sortable === false"
                    [style.width]="col.width || 'auto'">
                  {{ col.label }}
                </th>
                <td mat-cell *matCellDef="let row" [style.width]="col.width || 'auto'">
                  <ng-content [select]="'[column-' + col.key + ']'"></ng-content>
                  {{ row[col.key] }}
                </td>
              </ng-container>
            }

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"
                (click)="rowClick.emit(row)"
                class="clickable"></tr>
          </table>

          <mat-paginator
            [length]="totalCount"
            [pageSize]="pageSize"
            [pageSizeOptions]="[10, 25, 50, 100]"
            (page)="onPageChange($event)"
            showFirstLastButtons>
          </mat-paginator>
        }
      </div>
    </div>
  `,
  styles: [`
    .table-wrapper {
      position: relative;
      min-height: 200px;
    }

    .table-loading {
      opacity: 0.6;
      pointer-events: none;
    }

    table {
      width: 100%;
    }

    .clickable {
      cursor: pointer;
    }
  `]
})
export class DataTableComponent implements OnChanges {
  @Input() columns: ColumnConfig[] = [];
  @Input() data: unknown[] = [];
  @Input() totalCount = 0;
  @Input() pageSize = 25;
  @Input() loading = false;
  @Input() showSearch = true;
  @Input() tableTitle = '';
  @Input() searchPlaceholder = 'Search...';
  @Input() emptyIcon = 'search_off';
  @Input() emptyTitle = 'No results found';
  @Input() emptySubtitle = 'Try adjusting your search or filters';

  @Output() pageChange = new EventEmitter<PageEvent>();
  @Output() sortChange = new EventEmitter<Sort>();
  @Output() rowClick = new EventEmitter<unknown>();
  @Output() searchChange = new EventEmitter<string>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  dataSource = new MatTableDataSource<unknown>();
  displayedColumns: string[] = [];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.dataSource.data = this.data;
    }
    if (changes['columns']) {
      this.displayedColumns = this.columns.map(c => c.key);
    }
  }

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }

  onSortChange(sort: Sort): void {
    this.sortChange.emit(sort);
  }

  onSearch(query: string): void {
    this.searchChange.emit(query);
  }
}
