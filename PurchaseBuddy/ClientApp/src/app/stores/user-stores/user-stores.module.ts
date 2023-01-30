import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserStoresListComponent } from './user-stores-list/user-stores-list.component';
import { UserStoresRoutingModule } from '../user-stores-routing.module';


@NgModule({
  declarations: [
    UserStoresListComponent
  ],
  imports: [
    CommonModule,
    UserStoresRoutingModule,
  ]
})
export class UserStoresModule { }

