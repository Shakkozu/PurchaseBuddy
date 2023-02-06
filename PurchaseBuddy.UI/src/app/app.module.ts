import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatSlideToggleModule } from '@angular/material/slide-toggle'
import { MaterialLoaderModule } from './app-material-loader.module';

// BrowserModule,
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent
  ],
  imports: [
    AppRoutingModule,
    BrowserAnimationsModule,
    // BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    // ApiAuthorizationModule,
    AppRoutingModule,
    MatSlideToggleModule,
    BrowserAnimationsModule,
    AuthModule,
    MaterialLoaderModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
