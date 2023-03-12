import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserProductsListComponent } from './components/user-products-list.component';
import { MaterialModule } from '../shared/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxsModule } from '@ngxs/store';
import { ProductsRoutingModule } from './products-routing.module';
import { UserProductsState } from './store/user-products.state';



@NgModule({
  declarations: [
    UserProductsListComponent
  ],
  imports: [
    ProductsRoutingModule,
    CommonModule,
    MaterialModule,
    ReactiveFormsModule,
    FormsModule,
    NgxsModule.forFeature([UserProductsState]),
  ]
})
export class ProductsModule { }
