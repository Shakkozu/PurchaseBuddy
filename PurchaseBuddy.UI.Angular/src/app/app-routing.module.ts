import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { AuthGuard } from './auth/auth.guard';
import { LoginComponent } from './auth/components/login/login.component';
import { DepartmentComponent } from './department/department.component';
import { HomeComponent } from './home/home.component';
import { WeatherComponent } from './weather/weather.component';

const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'login', component: LoginComponent},
  { path: 'department', component: DepartmentComponent},
  { path: 'weather', component: WeatherComponent, canActivate: [AuthGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
