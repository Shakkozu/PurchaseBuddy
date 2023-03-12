import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthModule } from './auth/auth.module';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './shared/material.module';
import { EmployeeModule } from './employee/employee.module';
import { DepartmentModule } from './department/department.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { WeatherComponent } from './weather/weather.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpAuthorizationInterceptor } from './auth/http/http-authorization.interceptor';
import { ServerErrorInterceptor } from './shared/error-handling/server-error-interceptor';
import { NgxsModule } from '@ngxs/store';
import { ProductsModule } from './products/products.module';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    WeatherComponent,
  ],
  imports: [
    NgxsModule.forRoot(),
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    MaterialModule,
    AuthModule,
    ProductsModule,
    EmployeeModule,
    DepartmentModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
  ],
  exports: [
    CommonModule,
    WeatherComponent,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: HttpAuthorizationInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ServerErrorInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
