import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthModule } from './auth/auth.module';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './shared/material.module';
import { EmployeeModule } from './employee/employee.module';
import { DepartmentModule } from './department/department.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpAuthorizationInterceptor } from './auth/http/http-authorization.interceptor';
import { ServerErrorInterceptor } from './shared/error-handling/server-error-interceptor';
import { NgxsModule } from '@ngxs/store';
import { ProductsModule } from './products/products.module';
import { GenericDialogComponent } from './shared/generic-dialog/generic-dialog.component';
import { ProductCategoriesModule } from './product-categories/product-categories/product-categories.module';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { UserShopsModule } from './shops/user-shops.module';

@NgModule({
  declarations: [
    AppComponent,
    GenericDialogComponent,
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
    ProductCategoriesModule,
    NgxDatatableModule,
    BrowserAnimationsModule,
    UserShopsModule,
  ],
  exports: [
    CommonModule,
    GenericDialogComponent,
    NgxDatatableModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: HttpAuthorizationInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ServerErrorInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
