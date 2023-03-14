import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { GetUserProducts } from '../store/user-products.actions';
import { Product, UserProductsState } from '../store/user-products.state';
import { UserProductDetailsComponent } from './product-details/product-details.component';

@Component({
  selector: 'app-user-products-list',
  templateUrl: './user-products-list.component.html',
  styleUrls: ['./user-products-list.component.scss'],
  providers: [MatDialog],
})
export class UserProductsListComponent {
  @ViewChild(MatPaginator)
  public paginator!: MatPaginator;

  public displayedColumns: string[] = ['name', 'category'];
  public products: Product[] = [];
  public productsDataSource: MatTableDataSource<Product> = new MatTableDataSource(this.products);
  dialogRef: any;

  constructor (private store: Store,
    private matDialog: MatDialog,
    private router: Router) {
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

  navigateToProductDetails(productGuid: string): void {

    this.router.navigate(['user-product-details', productGuid]);
  }

  add() {
    this.dialogRef = this.matDialog.open(UserProductDetailsComponent, {
      width: '400px',
      height: '400px',
    });
  }
}
