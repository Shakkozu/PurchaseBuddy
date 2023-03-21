import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material/tree';
import { Store } from '@ngxs/store';
import { Subject, takeUntil } from 'rxjs';
import { FormErrorHandler } from 'src/app/shared/error-handling/form-error-handler';
import { ExampleFlatNode, ProductCategoryNode } from '../../product-categories.model';
import { UserProductCategoriesState } from '../../store/product-categories-state';
import { AddNewUserProductCategory, CreateProductCategoryRequest, InitializeUserProductCategories } from '../../store/product-categories.actions';
import { Component, EventEmitter, OnInit } from '@angular/core';
import { ProgressService } from '../../services/product-categories.service';


@Component({
  selector: 'app-add-new-product-category',
  templateUrl: './add-new-product-category.component.html',
  styleUrls: ['./add-new-product-category.component.scss'],
  providers: [ProgressService]
})
export class AddNewProductCategoryComponent implements OnInit {

  private destroy$ = new Subject();

  checklistSelection = new SelectionModel<ProductCategoryNode>(true);
  dataForm!: FormGroup;
  public selectedNode!: ProductCategoryNode;
  public selectedNodeGuid: string = '';
  private _transformer = (node: ProductCategoryNode, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      guid: node.guid,
      level: level,
    };
  };
  treeFlattener = new MatTreeFlattener(
    this._transformer,
    node => node.level,
    node => node.expandable,
    node => node.children,
  );

  treeControl = new FlatTreeControl<ExampleFlatNode>(
    node => node.level,
    node => node.expandable,
  );

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  constructor (private formBuilder: FormBuilder,
    private formErrorHandler: FormErrorHandler,
    public progressService: ProgressService,
    private store: Store) {
    this.store.dispatch(new InitializeUserProductCategories()).pipe(takeUntil(this.destroy$));
    this.dataSource.data = this.store.selectSnapshot(UserProductCategoriesState.productCategories);
  }

  ngOnInit(): void {
    this.initForm();
    this.progressService.resetProgressBar();
  }

  public onClose: EventEmitter<void> = new EventEmitter();
  public isSubmitting: boolean = false;

  private initForm(): void {
    this.dataForm = this.formBuilder.group({
      categoryName: new FormControl('', [Validators.required, Validators.minLength(3)]),
      categoryDescription: new FormControl('', [Validators.minLength(5)]),
      parentCategory: new FormControl({ value: '', disabled: true }, []),
    });
  }

  onNodeSelect(node: ProductCategoryNode) {
    this.dataForm.get('parentCategory')?.setValue(node.name);
    this.selectedNodeGuid = node.guid;
    this.treeControl.collapseAll();
  }


  hasChild = (_: number, node: ExampleFlatNode) => node.expandable;

  save() {
    if (this.dataForm.invalid || this.isSubmitting) {
      return;
    }

    this.isSubmitting = true;
    const request : CreateProductCategoryRequest = {
      name: this.dataForm.get('categoryName')?.value,
      description: this.dataForm.get('categoryDescription')?.value,
      parentId: this.selectedNodeGuid,
    };

    this.progressService.executeWithProgress(
      () => this.store.dispatch(new AddNewUserProductCategory(request)));
  }

  public getErrorMessage(formControlName: string): string {
    return this.formErrorHandler.handleError(this.dataForm, formControlName);
  }

  hide() {
    this.progressService.hideProgressBar();
  }

}