import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Angular Material
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatStepperModule } from '@angular/material/stepper';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';

// Shared Components
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { FileUploadComponent } from './components/file-upload/file-upload.component';
import { StatusBadgeComponent } from './components/status-badge/status-badge.component';
import { DocumentCardComponent } from './components/document-card/document-card.component';
import { EmptyStateComponent } from './components/empty-state/empty-state.component';
import { TimelineComponent } from './components/timeline/timeline.component';
import { StatCardComponent } from './components/stat-card/stat-card.component';
import { DataTableComponent } from './components/data-table/data-table.component';
import { SearchInputComponent } from './components/search-input/search-input.component';
import { AvatarComponent } from './components/avatar/avatar.component';

// Directives
import { HasRoleDirective } from './directives/has-role.directive';
import { DragDropDirective } from './directives/drag-drop.directive';

// Pipes
import { TimeAgoPipe } from './pipes/time-ago.pipe';
import { FileSizePipe } from './pipes/file-size.pipe';
import { DocumentStatusPipe } from './pipes/document-status.pipe';

const MATERIAL_MODULES = [
  MatButtonModule,
  MatIconModule,
  MatCardModule,
  MatFormFieldModule,
  MatInputModule,
  MatSelectModule,
  MatCheckboxModule,
  MatRadioModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatTableModule,
  MatPaginatorModule,
  MatSortModule,
  MatDialogModule,
  MatSnackBarModule,
  MatMenuModule,
  MatTooltipModule,
  MatChipsModule,
  MatTabsModule,
  MatProgressBarModule,
  MatProgressSpinnerModule,
  MatBadgeModule,
  MatDividerModule,
  MatListModule,
  MatExpansionModule,
  MatAutocompleteModule,
  MatSlideToggleModule,
  MatStepperModule,
  MatToolbarModule,
  MatSidenavModule,
];

const SHARED_COMPONENTS = [
  PageHeaderComponent,
  ConfirmDialogComponent,
  FileUploadComponent,
  StatusBadgeComponent,
  DocumentCardComponent,
  EmptyStateComponent,
  TimelineComponent,
  StatCardComponent,
  DataTableComponent,
  SearchInputComponent,
  AvatarComponent,
];

const SHARED_DIRECTIVES = [
  HasRoleDirective,
  DragDropDirective,
];

const SHARED_PIPES = [
  TimeAgoPipe,
  FileSizePipe,
  DocumentStatusPipe,
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    ...MATERIAL_MODULES,
    ...SHARED_COMPONENTS,
    ...SHARED_DIRECTIVES,
    ...SHARED_PIPES,
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    ...MATERIAL_MODULES,
    ...SHARED_COMPONENTS,
    ...SHARED_DIRECTIVES,
    ...SHARED_PIPES,
  ]
})
export class SharedModule {}
