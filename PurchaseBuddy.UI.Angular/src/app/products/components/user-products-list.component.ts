import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Store } from '@ngxs/store';
import { GetUserProducts } from '../store/user-products.actions';
import { Product, UserProductsState } from '../store/user-products.state';

@Component({
  selector: 'app-user-products-list',
  templateUrl: './user-products-list.component.html',
  styleUrls: ['./user-products-list.component.scss']
})
export class UserProductsListComponent {  
  @ViewChild(MatPaginator)
  public paginator!: MatPaginator;

  public displayedColumns: string[] = ['name', 'category'];
  public products: Product[] = [];

  public productsDataSource: MatTableDataSource<Product> = new MatTableDataSource(this.products);
  constructor (private store: Store) {
    this.store.dispatch(new GetUserProducts());
    this.store.select(UserProductsState.products).subscribe(products => {
      this.products = products;
      this.refreshMatTable();
    });
    
    this.refreshMatTable();
  }
  
  applyFilter(event: Event) {
    if (!this.productsDataSource || this.products?.length === 0)
      return;
    const filterValue = (event.target as HTMLInputElement).value;
    this.productsDataSource.filter = filterValue.trim().toLowerCase();
  }

  public refreshMatTable(): void {
    this.productsDataSource = new MatTableDataSource(this.products);
    this.productsDataSource.paginator = this.paginator;
    this.productsDataSource.filterPredicate = (data, filter) => {
      return data?.name?.toLowerCase().includes(filter);
    }
  }
}
