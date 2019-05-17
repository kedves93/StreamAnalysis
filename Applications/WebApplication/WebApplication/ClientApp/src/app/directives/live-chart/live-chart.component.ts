import { Component, OnInit, AfterViewInit, Input, OnChanges, EventEmitter, Output } from '@angular/core';
import * as am4core from '@amcharts/amcharts4/core';
import * as am4charts from '@amcharts/amcharts4/charts';
import am4themes_animated from '@amcharts/amcharts4/themes/animated';
import am4themes_material from '@amcharts/amcharts4/themes/material';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-live-chart',
  templateUrl: './live-chart.component.html',
  styleUrls: ['./live-chart.component.css']
})
export class LiveChartComponent implements OnInit, AfterViewInit {

  @Input() id: string;

  @Input() header: string;

  @Input() dataStream: Subject<string>;

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
    this.dataStream.subscribe(x => this.chart.addData({ date: new Date(), value: +x}, 1));
  }

  private createChart(): am4charts.XYChart {
    const chart = am4core.create(this.id, am4charts.XYChart);
    chart.padding(0, 0, 0, 0);
    chart.zoomOutButton.disabled = true;

    // initial data
    const data = [];
    let visits = 10;
    let i = 0;

    for (i = 0; i <= 3; i++) {
        visits -= Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 10);
        data.push({ date: new Date().setSeconds(i - 30), value: visits });
    }

    chart.data = data;


    //
    // X Axis
    //
    const dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.title.text = 'Time';
    dateAxis.renderer.grid.template.location = 0;
    dateAxis.renderer.minGridDistance = 30;
    dateAxis.dateFormats.setKey('second', 'ss');
    dateAxis.periodChangeDateFormats.setKey('second', '[bold]h:mm a');
    dateAxis.periodChangeDateFormats.setKey('minute', '[bold]h:mm a');
    dateAxis.periodChangeDateFormats.setKey('hour', '[bold]h:mm a');
    dateAxis.renderer.inside = false;
    dateAxis.renderer.axisFills.template.disabled = true;
    dateAxis.renderer.ticks.template.disabled = true;
    dateAxis.interpolationDuration = 500;
    dateAxis.rangeChangeDuration = 500;
    // zoom
    chart.events.on('datavalidated', function () {
      dateAxis.zoom({ start: 1 / 2, end: 1.2 }, false, true);
    });
    // this makes date axis labels to fade out
    dateAxis.renderer.labels.template.adapter.add('fillOpacity', function (fillOpacity, target) {
      const dataItem = target.dataItem;
      return dataItem.position;
    });
    // need to set this, otherwise fillOpacity is not changed and not set
    dateAxis.events.on('validated', function () {
      am4core.iter.each(dateAxis.renderer.labels.iterator(), function (label) {
        label.fillOpacity = label.fillOpacity;
      });
    });

    //
    // Y Axis
    //
    const valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.renderer.inside = true;
    valueAxis.renderer.minLabelPosition = 0.05;
    valueAxis.renderer.maxLabelPosition = 0.95;
    valueAxis.renderer.axisFills.template.disabled = true;
    valueAxis.renderer.ticks.template.disabled = true;
    valueAxis.tooltip.disabled = true;
    valueAxis.interpolationDuration = 500;
    valueAxis.rangeChangeDuration = 500;

    //
    // Series
    //
    const series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.dateX = 'date';
    series.dataFields.valueY = 'value';
    series.interpolationDuration = 500;
    series.defaultState.transitionDuration = 0;
    series.tensionX = 0.8;
    series.fillOpacity = 1;
    const gradient = new am4core.LinearGradient();
    gradient.addColor(chart.colors.getIndex(0), 0.2);
    gradient.addColor(chart.colors.getIndex(0), 0);
    series.fill = gradient;
    // bullet at the front of the line
    const bullet = series.createChild(am4charts.CircleBullet);
    bullet.circle.radius = 5;
    bullet.fillOpacity = 1;
    bullet.fill = chart.colors.getIndex(0);
    bullet.isMeasured = false;
    // moves bullet
    series.events.on('validated', function() {
      if (series.dataItems.length > 0) {
        bullet.moveTo(series.dataItems.last.point);
        bullet.validatePosition();
      }
    });

    return chart;
  }

  public onDelete(): void {
    this.delete.emit(this.id);
  }

}
