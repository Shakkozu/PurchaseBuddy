import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import { MaterialModule } from '../shared/material.module';
import { LoginComponent } from './components/login/login.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RegisterComponent } from './components/register/register.component';
import { CookieService } from 'ngx-cookie-service';
import { UserSessionState } from './store/session.state';
import { NgxsModule } from '@ngxs/store';


@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    MaterialModule,
    ReactiveFormsModule,
    FormsModule,
    NgxsModule.forFeature([
      UserSessionState,
    ]),
  ],
  exports: [
    LoginComponent,
    RegisterComponent
  ],
  providers: [
    CookieService,
  ]
})
export class AuthModule { }
