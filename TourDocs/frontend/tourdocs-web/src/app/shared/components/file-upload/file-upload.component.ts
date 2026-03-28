import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { DragDropDirective } from '../../directives/drag-drop.directive';
import { FileSizePipe } from '../../pipes/file-size.pipe';

export interface SelectedFile {
  file: File;
  preview?: string;
  progress: number;
  status: 'pending' | 'uploading' | 'complete' | 'error';
}

@Component({
  selector: 'td-file-upload',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule, MatProgressBarModule, DragDropDirective, FileSizePipe],
  template: `
    <div class="file-upload">
      <div class="file-upload__zone"
           tdDragDrop
           (fileDropped)="onFilesDropped($event)"
           (click)="fileInput.click()">
        <mat-icon class="file-upload__icon">cloud_upload</mat-icon>
        <p class="file-upload__text">Drag and drop files here</p>
        <p class="file-upload__subtext">or</p>
        <button mat-stroked-button color="primary" type="button" (click)="$event.stopPropagation()">
          <mat-icon>attach_file</mat-icon>
          Browse Files
        </button>
        @if (accept) {
          <p class="file-upload__hint">Accepted formats: {{ accept }}</p>
        }
        @if (maxSizeMB) {
          <p class="file-upload__hint">Maximum file size: {{ maxSizeMB }}MB</p>
        }
      </div>

      <input #fileInput
             type="file"
             [accept]="accept"
             [multiple]="multiple"
             (change)="onFileSelected($event)"
             class="file-upload__input">

      @if (selectedFiles.length > 0) {
        <div class="file-upload__list">
          @for (item of selectedFiles; track item.file.name) {
            <div class="file-upload__item">
              <div class="file-upload__item-info">
                <mat-icon class="file-upload__item-icon">{{ getFileIcon(item.file.name) }}</mat-icon>
                <div class="file-upload__item-details">
                  <span class="file-upload__item-name">{{ item.file.name }}</span>
                  <span class="file-upload__item-size">{{ item.file.size | fileSize }}</span>
                </div>
              </div>
              @if (item.status === 'uploading') {
                <mat-progress-bar mode="determinate" [value]="item.progress"></mat-progress-bar>
              }
              @if (item.status === 'complete') {
                <mat-icon class="file-upload__item-status file-upload__item-status--success">check_circle</mat-icon>
              }
              @if (item.status === 'error') {
                <mat-icon class="file-upload__item-status file-upload__item-status--error">error</mat-icon>
              }
              <button mat-icon-button (click)="removeFile(item)" type="button">
                <mat-icon>close</mat-icon>
              </button>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .file-upload__zone {
      border: 2px dashed #B0BEC5;
      border-radius: 12px;
      padding: 40px 24px;
      text-align: center;
      cursor: pointer;
      transition: border-color 0.2s, background-color 0.2s;
      background: #FAFAFA;

      &:hover, &.drag-over {
        border-color: #1565C0;
        background: rgba(21, 101, 192, 0.04);
      }
    }

    .file-upload__icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      color: #90A4AE;
      margin-bottom: 12px;
    }

    .file-upload__text {
      font-size: 16px;
      font-weight: 500;
      color: #546E7A;
      margin: 0 0 4px;
    }

    .file-upload__subtext {
      font-size: 13px;
      color: #90A4AE;
      margin: 0 0 16px;
    }

    .file-upload__hint {
      font-size: 12px;
      color: #90A4AE;
      margin: 8px 0 0;
    }

    .file-upload__input {
      display: none;
    }

    .file-upload__list {
      margin-top: 16px;
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .file-upload__item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 16px;
      background: #FFFFFF;
      border: 1px solid #E0E0E0;
      border-radius: 8px;
    }

    .file-upload__item-info {
      display: flex;
      align-items: center;
      gap: 12px;
      flex: 1;
      min-width: 0;
    }

    .file-upload__item-icon {
      color: #1565C0;
      flex-shrink: 0;
    }

    .file-upload__item-details {
      display: flex;
      flex-direction: column;
      min-width: 0;
    }

    .file-upload__item-name {
      font-size: 14px;
      font-weight: 500;
      color: #263238;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .file-upload__item-size {
      font-size: 12px;
      color: #78909C;
    }

    .file-upload__item-status--success {
      color: #66BB6A;
    }

    .file-upload__item-status--error {
      color: #EF5350;
    }

    mat-progress-bar {
      flex: 1;
      max-width: 120px;
    }
  `]
})
export class FileUploadComponent {
  @Input() accept = '';
  @Input() multiple = false;
  @Input() maxSizeMB = 10;
  @Output() filesSelected = new EventEmitter<SelectedFile[]>();

  selectedFiles: SelectedFile[] = [];

  onFilesDropped(fileList: FileList): void {
    this.addFiles(fileList);
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.addFiles(input.files);
      input.value = '';
    }
  }

  removeFile(item: SelectedFile): void {
    this.selectedFiles = this.selectedFiles.filter(f => f !== item);
    this.filesSelected.emit(this.selectedFiles);
  }

  getFileIcon(filename: string): string {
    const ext = filename.split('.').pop()?.toLowerCase();
    switch (ext) {
      case 'pdf': return 'picture_as_pdf';
      case 'jpg': case 'jpeg': case 'png': case 'gif': return 'image';
      case 'doc': case 'docx': return 'description';
      case 'xls': case 'xlsx': return 'table_chart';
      default: return 'insert_drive_file';
    }
  }

  private addFiles(fileList: FileList): void {
    const files = Array.from(fileList);
    const newItems: SelectedFile[] = files
      .filter(file => {
        const sizeMB = file.size / (1024 * 1024);
        return sizeMB <= this.maxSizeMB;
      })
      .map(file => ({
        file,
        progress: 0,
        status: 'pending' as const
      }));

    if (!this.multiple) {
      this.selectedFiles = newItems.slice(0, 1);
    } else {
      this.selectedFiles = [...this.selectedFiles, ...newItems];
    }

    this.filesSelected.emit(this.selectedFiles);
  }
}
