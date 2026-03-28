import { Component } from '@angular/core';
import { environment } from '@env/environment';

@Component({
  selector: 'td-footer',
  standalone: true,
  template: `
    <footer class="td-footer">
      <span>&copy; {{ currentYear }} TourDocs. All rights reserved.</span>
      <span class="td-footer__version">v{{ version }}</span>
    </footer>
  `,
  styles: [`
    .td-footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 12px 24px;
      font-size: 12px;
      color: #90A4AE;
      border-top: 1px solid #E0E0E0;
      background: #FAFAFA;
      flex-shrink: 0;
    }

    .td-footer__version {
      font-family: monospace;
    }
  `]
})
export class FooterComponent {
  currentYear = new Date().getFullYear();
  version = environment.version;
}
