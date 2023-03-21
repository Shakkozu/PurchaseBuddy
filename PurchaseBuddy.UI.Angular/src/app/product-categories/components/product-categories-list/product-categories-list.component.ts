import { Component } from '@angular/core';
import { FlatTreeControl } from '@angular/cdk/tree';
import { FormGroup } from '@angular/forms';
import { MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material/tree';
import { Store } from '@ngxs/store';
import { Subject, takeUntil } from 'rxjs';
import { ProductCategoryNode, ExampleFlatNode, ProductCategoryFlatNode } from '../../product-categories.model';
import { UserProductCategoriesState } from '../../store/product-categories-state';
import { InitializeUserProductCategories, RemoveUserProductCategory } from '../../store/product-categories.actions';
import { OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-product-categories-list',
  templateUrl: './product-categories-list.component.html',
  styleUrls: ['./product-categories-list.component.scss']
})
export class ProductCategoriesListComponent implements OnInit {

  public dataForm!: FormGroup;
  public dataSource!: MatTreeFlatDataSource<ProductCategoryNode, ProductCategoryFlatNode>;
  private destroy$ = new Subject();

  constructor (private store: Store,
  private router: Router) {
    this.ngOnInit();
  }

  public ngOnInit():void {
    this.store.dispatch(new InitializeUserProductCategories()).pipe(takeUntil(this.destroy$));

    this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
    this.store.select(UserProductCategoriesState.productCategories).subscribe((data) => {
      this.dataSource.data = data;
    });
  }

  private _transformer = (node: ProductCategoryNode, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      guid: node.guid,
      level: level,
    };
  };

  private treeFlattener = new MatTreeFlattener(
    this._transformer,
    node => node.level,
    node => node.expandable,
    node => node.children,
  );

  public treeControl = new FlatTreeControl<ProductCategoryFlatNode>(
    node => node.level,
    node => node.expandable,
  );

  public hasChild = (_: number, node: ExampleFlatNode) => node.expandable;

  public addNewCategory(): void {
    this.router.navigate(['user-product-categories/add']);
  }

  remove(node: ProductCategoryNode) {
   this.store.dispatch(new RemoveUserProductCategory(node.guid)) 
  }

}

