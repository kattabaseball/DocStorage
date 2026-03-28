import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData } from 'chart.js';

@Component({
  selector: 'td-document-health-widget',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  template: `
    <div class="widget-card">
      <div class="widget-card__header">
        <h3 class="widget-card__title">Document Health</h3>
      </div>
      <div class="widget-card__body">
        <div class="chart-container">
          <canvas baseChart
            [data]="doughnutData"
            [options]="doughnutOptions"
            type="doughnut">
          </canvas>
        </div>
        <div class="legend">
          @for (item of legendItems; track item.label) {
            <div class="legend__item">
              <span class="legend__dot" [style.backgroundColor]="item.color"></span>
              <span class="legend__label">{{ item.label }}</span>
              <span class="legend__value">{{ item.value }}</span>
            </div>
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    .chart-container {
      max-width: 240px;
      margin: 0 auto 24px;
    }

    .legend {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .legend__item {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .legend__dot {
      width: 12px;
      height: 12px;
      border-radius: 50%;
      flex-shrink: 0;
    }

    .legend__label {
      flex: 1;
      font-size: 13px;
      color: #546E7A;
    }

    .legend__value {
      font-size: 14px;
      font-weight: 600;
      color: #263238;
    }
  `]
})
export class DocumentHealthWidgetComponent implements OnChanges {
  @Input() summary: any = {};

  doughnutData: ChartData<'doughnut'> = {
    labels: ['Verified', 'Reviewing', 'Uploaded', 'Rejected', 'Expired'],
    datasets: [{
      data: [0, 0, 0, 0, 0],
      backgroundColor: ['#66BB6A', '#FFA726', '#42A5F5', '#EF5350', '#BDBDBD'],
      borderWidth: 0,
      hoverOffset: 8
    }]
  };

  doughnutOptions: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true,
    maintainAspectRatio: true,
    cutout: '70%',
    plugins: {
      legend: {
        display: false
      }
    }
  };

  legendItems: { label: string; value: number; color: string }[] = [];

  ngOnChanges(): void {
    const verified = this.summary?.verifiedDocuments ?? 0;
    const reviewing = this.summary?.pendingVerifications ?? 0;
    const uploaded = this.summary?.uploadedDocuments ?? 0;
    const rejected = this.summary?.rejectedDocuments ?? 0;
    const expired = this.summary?.expiredDocuments ?? 0;

    this.doughnutData = {
      labels: ['Verified', 'Reviewing', 'Uploaded', 'Rejected', 'Expired'],
      datasets: [{
        data: [verified, reviewing, uploaded, rejected, expired],
        backgroundColor: ['#66BB6A', '#FFA726', '#42A5F5', '#EF5350', '#BDBDBD'],
        borderWidth: 0,
        hoverOffset: 8
      }]
    };

    this.legendItems = [
      { label: 'Verified', value: verified, color: '#66BB6A' },
      { label: 'Reviewing', value: reviewing, color: '#FFA726' },
      { label: 'Uploaded', value: uploaded, color: '#42A5F5' },
      { label: 'Rejected', value: rejected, color: '#EF5350' },
      { label: 'Expired', value: expired, color: '#BDBDBD' }
    ];
  }
}
