import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-weather',
  templateUrl: './weather.component.html',
  styleUrls: ['./weather.component.scss']
})
export class WeatherComponent implements OnDestroy {
  private destroy$: Subject<void> = new Subject();
  public form!: FormGroup;
  public weatherForecast: Array<IWeatherForecastDto> = [];
  constructor (private http: HttpClient) {
  }
    

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getWeather() {
    const url = `http://localhost:5133/weatherforecast/weather`;

    this.http.get(url)
      .pipe(takeUntil(this.destroy$))
      .subscribe((data: any) => {
        this.weatherForecast = data;
      },
      (error) => console.log(error));

  }
  
  public getWeatherAuthorized() {
    const url = `http://localhost:5133/weatherforecast/weather-authorized`;

    this.http.get(url)
      .pipe(takeUntil(this.destroy$))
      .subscribe((data: any) => {
        this.weatherForecast = data;
      },
      (error) => console.log(error));

  }
}

export interface IWeatherForecastDto {
  date: Date;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}