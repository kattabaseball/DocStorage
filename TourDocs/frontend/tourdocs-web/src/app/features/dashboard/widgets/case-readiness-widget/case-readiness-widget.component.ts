import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData } from 'chart.js';

interface CaseReadiness {
  caseId: string;
  caseName: string;
  readyPercent: number;
  pendingPercent: number;
}

@Component({
  selector: 'td-case-readiness-widget',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  template: `
    <div class="widget-card">
      <div class="widget-card__header">
        <h3 class="widget-card__title">Case Readiness</h3>
      </div>
      <div class="widget-card__body">
        @if (cases.length === 0) {
          <p class="text-muted text-center">No active cases.</p>
        } @else {
          <canvas baseChart
            [data]="barData"
            [options]="barOptions"
            type="bar">
          </canvas>
        }
      </div>
    </div>
  `,
  styles: [`
    .widget-card__body {
      height: 300px;
    }

    .text-muted {
      color: #90A4AE;
      font-size: 14px;
    }

    .text-center {
      text-align: center;
      padding-top: 120px;
    }
  `]
})
export class CaseReadinessWidgetComponent implements OnChanges {
  @Input() cases: CaseReadiness[] = [];

  barData: ChartData<'bar'> = { labels: [], datasets: [] };

  barOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    indexAxis: 'y',
    scales: {
      x: {
        stacked: true,
        max: 100,
        ticks: {
          callback: (value) => value + '%'
        },
        grid: {
          display: false
        }
      },
      y: {
        stacked: true,
        grid: {
          display: false
        }
      }
    },
    plugins: {
      legend: {
        position: 'bottom'
      }
    }
  };

  ngOnChanges(): void {
    this.barData = {
      labels: this.cases.map(c => c.caseName),
      datasets: [
        {
          label: 'Ready',
          data: this.cases.map(c => c.readyPercent),
          backgroundColor: '#66BB6A',
          borderRadius: 4
        },
        {
          label: 'Pending',
          data: this.cases.map(c => c.pendingPercent),
          backgroundColor: '#FFA726',
          borderRadius: 4
        }
      ]
    };
  }
}
