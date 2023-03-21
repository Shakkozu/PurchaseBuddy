import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl, NestedTreeControl } from '@angular/cdk/tree';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatTreeFlatDataSource, MatTreeFlattener, MatTreeNestedDataSource } from '@angular/material/tree';
import { Store } from '@ngxs/store';
import { ExampleFlatNode, ProductCategoryFlatNode, ProductCategoryNode } from 'src/app/product-categories/product-categories.model';
import { ProductCategoriesService } from 'src/app/product-categories/services/product-categories.service';
import { FormErrorHandler } from 'src/app/shared/error-handling/form-error-handler';
import { AddNewUserProduct } from '../../store/user-products.actions';



/**
 * @title Tree with flat nodes
 */
@Component({
  selector: 'app-product-details',
  templateUrl: 'product-details.component.html',
  styleUrls: ['product-details.component.scss'],
})
export class UserProductDetailsComponent {
  
  checklistSelection = new SelectionModel<ProductCategoryNode>(true);
  dataForm!: FormGroup;
  public selectedNode!: ProductCategoryNode;
  public selectedNodeGuid: string = '';
  public saveOccured: boolean = false;


  public dataSource!: MatTreeFlatDataSource<ProductCategoryNode, ProductCategoryFlatNode>;

  constructor (private productCategoriesService: ProductCategoriesService,
    private formBuilder: FormBuilder,
    private formErrorHandler: FormErrorHandler,
    private store: Store) {
    
    this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);
    this.productCategoriesService.getCategories().subscribe(categories => {
      this.dataSource.data = categories;
    });
    this.initForm();
  }

  private initForm(): void {
    this.dataForm = this.formBuilder.group({
      productName: new FormControl('', [Validators.required, Validators.minLength(3)]),
      productCategory: new FormControl({value: '', disabled: true}, []),
    });
  }

  onNodeSelect(node: ProductCategoryNode) {
    console.log(node);
    this.selectedNodeGuid = node.guid;
    this.dataForm.get('productCategory')?.setValue(node.name);
    this.treeControl.collapseAll();
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

  hasChild = (_: number, node: ExampleFlatNode) => node.expandable;
  
  save() {
    if (this.dataForm.invalid || this.saveOccured) {
      return;
    }

    this.saveOccured = true;
    const productName = this.dataForm.get('productName')?.value;
    this.store.dispatch(new AddNewUserProduct(productName, this.selectedNodeGuid));
  }

  public getErrorMessage(formControlName: string): string {
    return this.formErrorHandler.handleError(this.dataForm, formControlName);
  }

}
