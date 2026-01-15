import { AfterViewInit, Component, NgZone, OnDestroy, OnInit, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatListModule } from '@angular/material/list';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterLink } from '@angular/router';
import { SettingsService, SignalRService } from '@core';
import { MtxAlertModule } from '@ng-matero/extensions/alert';
import { MtxProgressModule } from '@ng-matero/extensions/progress';
import { Subscription } from 'rxjs';
import { CHARTS, ELEMENT_DATA, MESSAGES, STATS } from './data';
import { OrderFormCard } from '../orders/order-form-card/order-form-card';
import { OrdersList } from '../orders/orders-list/orders-list';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  imports: [
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatListModule,
    MatGridListModule,
    MatTableModule,
    MatTabsModule,
    MtxProgressModule,
    MtxAlertModule,
    OrderFormCard,
    OrdersList,
  ],
})
export class Dashboard implements OnInit, AfterViewInit, OnDestroy {
private readonly ngZone = inject(NgZone);
private readonly settings = inject(SettingsService);
private readonly signalRService = inject(SignalRService);

  displayedColumns: string[] = ['position', 'name', 'weight', 'symbol'];
  dataSource = ELEMENT_DATA;

  messages = MESSAGES;

  charts = CHARTS;
  chart1?: ApexCharts;

  stats = STATS;

  notifySubscription = Subscription.EMPTY;
  newOrderSubscription = Subscription.EMPTY;
  deliveredOrderSubscription = Subscription.EMPTY;

  isShowAlert = true;

  // Rastrear timestamps dos eventos (ao invés de contadores acumulados)
  newOrderTimestamps: number[] = [];
  deliveredOrderTimestamps: number[] = [];
  
  // Dados do gráfico
  newOrderData: [number, number][] = [];
  deliveredOrderData: [number, number][] = [];
  chartTimeRange = '';
  
  // Intervalo de agregação em milissegundos (10 segundos)
  private readonly aggregationInterval = 10 * 1000;
  
  // Timer para atualizar o gráfico continuamente
  private chartUpdateInterval?: number;

  introducingItems = [
    {
      name: 'Acrodata GUI',
      description: 'A JSON powered GUI for configurable panels.',
      link: 'https://github.com/acrodata/gui',
    },
    {
      name: 'Code Editor',
      description: 'The CodeMirror 6 wrapper for Angular.',
      link: 'https://github.com/acrodata/code-editor',
    },
    {
      name: 'Watermark',
      description: 'A watermark component that can prevent deletion.',
      link: 'https://github.com/acrodata/watermark',
    },
    {
      name: 'RnD Dialog',
      description: 'Resizable and draggable dialog based on CDK dialog.',
      link: 'https://github.com/acrodata/rnd-dialog',
    },
    {
      name: 'Gradient Picker',
      description: 'A powerful and beautiful gradient picker.',
      link: 'https://github.com/acrodata/gradient-picker',
    },
    {
      name: 'NG DnD',
      description: 'A toolkit for building complex drag and drop and very similar to react-dnd.',
      link: 'https://github.com/ng-dnd/ng-dnd',
    },
  ];

  introducingItem = this.introducingItems[this.getRandom(0, 4)];

  ngOnInit() {
    this.signalRService.startConnection();
    
    this.notifySubscription = this.settings.notify.subscribe(opts => {

      this.updateCharts();
    });

    // Escutar evento de novo pedido
    this.newOrderSubscription = this.signalRService.on('new-order-notify').subscribe((data) => {
      const timestamp = new Date().getTime();
      this.newOrderTimestamps.push(timestamp);
      this.updateRealtimeChart();
    });

    // Escutar evento de pedido entregue
    this.deliveredOrderSubscription = this.signalRService.on('delivered-order-notify').subscribe((data) => {
      const timestamp = new Date().getTime();
      this.deliveredOrderTimestamps.push(timestamp);
      this.updateRealtimeChart();
    });
  }

  onOrderCreated(): void {
    // Callback opcional quando um pedido é criado
  }

  ngAfterViewInit() {
    this.ngZone.runOutsideAngular(() => this.initCharts());
  }

  ngOnDestroy() {
    this.chart1?.destroy();

    this.notifySubscription.unsubscribe();
    this.newOrderSubscription.unsubscribe();
    this.deliveredOrderSubscription.unsubscribe();
    this.signalRService.stopConnection();
    
    // Limpar o interval
    if (this.chartUpdateInterval) {
      clearInterval(this.chartUpdateInterval);
    }
  }

  initCharts() {
    this.chart1 = new ApexCharts(document.querySelector('#chart1'), this.charts[0]);
    this.chart1?.render();

    this.updateCharts();
    
    // Inicializar dados do gráfico de tempo real
    this.initializeChartData();
    
    // Iniciar atualização automática
    this.startChartAutoUpdate();
  }

  updateCharts() {
    const isDark = this.settings.getThemeColor() == 'dark';

    this.chart1?.updateOptions({
      chart: {
        foreColor: isDark ? '#ccc' : '#333',
      },
      tooltip: {
        theme: isDark ? 'dark' : 'light',
      },
      grid: {
        borderColor: isDark ? '#5a5a5a' : '#e1e1e1',
      },
    });
  }

  onAlertDismiss() {
    this.isShowAlert = false;
  }

  initializeChartData() {
    // Inicializar o gráfico com a janela de 3 minutos
    // 3 minutos para trás até agora
    const now = new Date().getTime();
    const threeMinutesAgo = now - (3 * 60 * 1000);
    
    this.newOrderData = [[threeMinutesAgo, 0]];
    this.deliveredOrderData = [[threeMinutesAgo, 0]];
    
    this.updateTimeRange(threeMinutesAgo, now);
    
    this.ngZone.run(() => {
      this.chart1?.updateSeries([
        {
          name: 'Novos Pedidos',
          data: [...this.newOrderData],
        },
        {
          name: 'Pedidos Entregues',
          data: [...this.deliveredOrderData],
        },
      ]);

      this.chart1?.updateOptions({
        xaxis: {
          min: threeMinutesAgo,
          max: now,
        },
      });
    });
  }

  updateRealtimeChart() {
    if (!this.chart1) return;

    const now = new Date().getTime();
    const threeMinutesAgo = now - (3 * 60 * 1000);

    // Limpar timestamps antigos (mais de 3 minutos)
    this.newOrderTimestamps = this.newOrderTimestamps.filter(ts => ts >= threeMinutesAgo);
    this.deliveredOrderTimestamps = this.deliveredOrderTimestamps.filter(ts => ts >= threeMinutesAgo);

    // Reconstruir os dados do gráfico com base nos timestamps
    this.rebuildChartData(threeMinutesAgo, now);

    this.updateTimeRange(threeMinutesAgo, now);

    this.ngZone.run(() => {
      this.chart1?.updateSeries([
        {
          name: 'Novos Pedidos',
          data: [...this.newOrderData],
        },
        {
          name: 'Pedidos Entregues',
          data: [...this.deliveredOrderData],
        },
      ]);

      this.chart1?.updateOptions({
        xaxis: {
          min: threeMinutesAgo,
          max: now,
        },
      });
    });
  }

  rebuildChartData(startTime: number, endTime: number) {
    this.newOrderData = [];
    this.deliveredOrderData = [];

    // Criar intervalos de tempo (a cada 10 segundos)
    const intervalCount = Math.ceil((endTime - startTime) / this.aggregationInterval);
    
    for (let i = 0; i <= intervalCount; i++) {
      const intervalStart = startTime + (i * this.aggregationInterval);
      const intervalEnd = intervalStart + this.aggregationInterval;
      
      // Contar eventos neste intervalo
      const newOrdersInInterval = this.newOrderTimestamps.filter(
        ts => ts >= intervalStart && ts < intervalEnd
      ).length;
      
      const deliveredOrdersInInterval = this.deliveredOrderTimestamps.filter(
        ts => ts >= intervalStart && ts < intervalEnd
      ).length;
      
      this.newOrderData.push([intervalStart, newOrdersInInterval]);
      this.deliveredOrderData.push([intervalStart, deliveredOrdersInInterval]);
    }
  }

  updateTimeRange(start: number, end: number) {
    const startDate = new Date(start);
    const endDate = new Date(end);
    const formatTime = (date: Date) => {
      return date.toLocaleTimeString('pt-BR', { 
        hour: '2-digit', 
        minute: '2-digit',
        second: '2-digit'
      });
    };
    this.chartTimeRange = `${formatTime(startDate)} - ${formatTime(endDate)}`;
  }

  startChartAutoUpdate() {
    // Atualizar o gráfico a cada 1 segundo para dar impressão de tempo real
    this.chartUpdateInterval = window.setInterval(() => {
      this.updateChartWindow();
    }, 1000);
  }

  updateChartWindow() {
    if (!this.chart1) return;

    const now = new Date().getTime();
    const threeMinutesAgo = now - (3 * 60 * 1000);

    // Limpar timestamps antigos
    this.newOrderTimestamps = this.newOrderTimestamps.filter(ts => ts >= threeMinutesAgo);
    this.deliveredOrderTimestamps = this.deliveredOrderTimestamps.filter(ts => ts >= threeMinutesAgo);

    // Reconstruir dados do gráfico
    this.rebuildChartData(threeMinutesAgo, now);

    this.updateTimeRange(threeMinutesAgo, now);

    this.ngZone.run(() => {
      this.chart1?.updateSeries([
        {
          name: 'Novos Pedidos',
          data: [...this.newOrderData],
        },
        {
          name: 'Pedidos Entregues',
          data: [...this.deliveredOrderData],
        },
      ]);

      this.chart1?.updateOptions({
        xaxis: {
          min: threeMinutesAgo,
          max: now,
        },
      });
    });
  }

  getRandom(min: number, max: number) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }
}
