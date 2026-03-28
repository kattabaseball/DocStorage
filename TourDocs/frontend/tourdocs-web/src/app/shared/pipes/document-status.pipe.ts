import { Pipe, PipeTransform } from '@angular/core';

export enum DocumentStatus {
  Uploaded = 'Uploaded',
  Reviewing = 'Reviewing',
  Verified = 'Verified',
  Rejected = 'Rejected',
  Expired = 'Expired',
  PendingUpload = 'PendingUpload',
  ReUploadRequired = 'ReUploadRequired'
}

const STATUS_LABELS: Record<string, string> = {
  [DocumentStatus.Uploaded]: 'Uploaded',
  [DocumentStatus.Reviewing]: 'Under Review',
  [DocumentStatus.Verified]: 'Verified',
  [DocumentStatus.Rejected]: 'Rejected',
  [DocumentStatus.Expired]: 'Expired',
  [DocumentStatus.PendingUpload]: 'Pending Upload',
  [DocumentStatus.ReUploadRequired]: 'Re-upload Required'
};

@Pipe({
  name: 'documentStatus',
  standalone: true
})
export class DocumentStatusPipe implements PipeTransform {
  transform(value: string): string {
    return STATUS_LABELS[value] || value;
  }
}
