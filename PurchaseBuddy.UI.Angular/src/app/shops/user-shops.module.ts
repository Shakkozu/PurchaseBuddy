import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '../shared/material.module';
import { UserShopsListComponent } from './components/user-shops-list/user-shops-list.component';
import { UserShopDetailsComponent } from './components/user-shop-details/user-shop-details.component';
import { UserShopsRoutingModule } from './user-shops-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgxsModule } from '@ngxs/store';
import { UserShopsState } from './store/user-shops.state';



@NgModule({
  declarations: [
    UserShopsListComponent,
    UserShopDetailsComponent
  ],
  imports: [
    CommonModule,
    UserShopsRoutingModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    NgxDatatableModule,
    RouterModule,
    NgxsModule.forFeature([UserShopsState]),
  ]
})
export class UserShopsModule { }

