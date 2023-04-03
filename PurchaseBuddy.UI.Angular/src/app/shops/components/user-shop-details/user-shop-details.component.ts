import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { Subject, takeUntil } from 'rxjs';
import { ProgressService } from 'src/app/product-categories/services/product-categories.service';
import { FormErrorHandler } from 'src/app/shared/error-handling/form-error-handler';
import { UserShop, UserShopDto } from '../../model';
import { AddNewUserShop, DeleteUserShop, UpdateUserShop } from '../../store/user-shops.actions';
import { UserShopsState } from '../../store/user-shops.state';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-user-shop-details',
  templateUrl: './user-shop-details.component.html',
  styleUrls: ['./user-shop-details.component.scss'],
  providers: [ProgressService]
})
export class UserShopDetailsComponent implements OnInit, OnDestroy {
  public header: string = 'New Shop';
  public dataForm!: FormGroup;
  private destroy$ = new Subject();
  private shop: UserShop | undefined;
  private shopId: string | null = null;

  constructor (private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private formErrorHandler: FormErrorHandler,
    private store: Store,
    public progressService: ProgressService) { }

  public ngOnDestroy(): void {
    this.destroy$.next(0);
    this.destroy$.complete();
  }

  public getErrorMessage(formControlName: string): string {
    return this.formErrorHandler.handleError(this.dataForm, formControlName);
  }

  public deleteShop() {
    if (this.shopId) {
      this.store.dispatch(new DeleteUserShop(this.shopId)).pipe(takeUntil(this.destroy$))
        .subscribe(() => this.router.navigate(['/user-shops']));
    }
  }

  public save() {
    this.dataForm.markAllAsTouched();
    if (!this.dataForm.valid)
      return;

    const request: UserShopDto = {
      name: this.dataForm.get('name')?.value,
      description: this.dataForm.get('description')?.value,
      city: this.dataForm.get('city')?.value,
      street: this.dataForm.get('street')?.value,
      localNumber: this.dataForm.get('localNumber')?.value,
    }

    if (this.shopId) {
      this.store.dispatch(new UpdateUserShop(this.shopId, request)).pipe(takeUntil(this.destroy$)).subscribe(() => {
      });

    } else {
      this.store.dispatch(new AddNewUserShop(request)).pipe(takeUntil(this.destroy$)).subscribe(() => {
      });
    }
  }

  public ngOnInit(): void {
    this.initForm();
    this.shopId = this.route.snapshot.paramMap.get('id');
    if (this.shopId) {
      this.header = 'Edit Shop';
      this.shop = this.store
        .selectSnapshot(UserShopsState.shops)
        .find((shop) => shop.guid === this.shopId);
      this.refreshForm();
    }
  }

  private refreshForm() {
    if (!this.shop)
      return;

    this.dataForm.get('name')?.setValue(this.shop.name);
    this.dataForm.get('description')?.setValue(this.shop.description);
    this.dataForm.get('city')?.setValue(this.shop.address.city);
    this.dataForm.get('street')?.setValue(this.shop.address.street);
    this.dataForm.get('localNumber')?.setValue(this.shop.address.localNumber);
  }

  private initForm() {
    this.dataForm = this.formBuilder.group({
      name: ['', Validators.required],
      description: [''],
      city: [''],
      street: [''],
      localNumber: [''],
    });

    this.dataForm.get('city')?.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((value) => {
      if (value) {
        this.dataForm.get('street')?.setValidators(Validators.required);
      } else {
        this.dataForm.get('street')?.clearValidators();
      }
      this.dataForm.get('street')?.updateValueAndValidity();
    });
  }
}
