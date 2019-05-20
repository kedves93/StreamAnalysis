import { Component, OnInit, Input, Output, EventEmitter, AfterViewInit } from '@angular/core';
import * as am4core from '@amcharts/amcharts4/core';
import * as am4charts from '@amcharts/amcharts4/charts';
import am4themes_animated from '@amcharts/amcharts4/themes/animated';
import am4themes_material from '@amcharts/amcharts4/themes/material';

@Component({
  selector: 'app-zoom-chart',
  templateUrl: './zoom-chart.component.html',
  styleUrls: ['./zoom-chart.component.css']
})
export class ZoomChartComponent implements OnInit, AfterViewInit {

  @Input() id: string;

  @Input() header: string;

  @Input() data: any[];

  @Input() measurement: string;

  @Output() delete = new EventEmitter<string>();

  private chart: am4charts.XYChart;

  constructor() {
    am4core.useTheme(am4themes_animated);
    am4core.useTheme(am4themes_material);
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.chart = this.createChart();

    const chartData = [];
    for (let i = 0; i < this.data.length; i++) {
      chartData.push({
        date: this.data[i].date,
        values: this.data[i].value
      });
    }
    this.chart.data = chartData;
  }

  public onDelete(): void {
    this.delete.emit(this.id);
  }

  private createChart(): am4charts.XYChart {
    const chart = am4core.create(this.id, am4charts.XYChart);

    //
    // X Axis
    //
    const dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.title.text = 'Time';
    dateAxis.tooltipDateFormat = 'HH:mm, d MMMM';
    dateAxis.dateFormats.setKey('minute', 'hh:mm:ss a');
    dateAxis.dateFormats.setKey('hour', 'hh:mm a');
    dateAxis.dateFormats.setKey('day', 'd');
    dateAxis.periodChangeDateFormats.setKey('second', '[bold]h:mm a');
    dateAxis.periodChangeDateFormats.setKey('minute', '[bold]h:mm a');
    dateAxis.periodChangeDateFormats.setKey('hour', '[bold]h:mm a');
    dateAxis.periodChangeDateFormats.setKey('day', '[bold]d');
    chart.events.on('datavalidated', function () {
      dateAxis.zoom({ start: 0.8, end: 1 });
    });

    //
    // Y Axis
    //
    const valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.tooltip.disabled = true;

    //
    // Series
    //
    const series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.dateX = 'date';
    series.dataFields.valueY = 'values';
    series.tooltipText = 'Value: [bold]{valueY}[/]';
    series.fillOpacity = 0.3;
    chart.cursor = new am4charts.XYCursor();
    chart.cursor.lineY.opacity = 0;

    //
    // ScrollBar
    //
    const scrollBar = new am4charts.XYChartScrollbar();
    scrollBar.series.push(series);
    chart.scrollbarX = scrollBar;

    return chart;
  }
}
