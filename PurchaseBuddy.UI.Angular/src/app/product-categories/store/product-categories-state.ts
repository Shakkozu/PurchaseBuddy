import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Action, Selector, State, StateContext } from "@ngxs/store";
import { ProductCategoriesService, ProductCategory } from "../services/product-categories.service";
import { AddNewUserProductCategory, AddNewUserProductCategorySuccess, InitializeUserProductCategories, RemoveUserProductCategory } from "./product-categories.actions";

export interface UserProductCategoriesStateModel { 
	productCategories: ProductCategory[];
}

const defaultState: UserProductCategoriesStateModel = {
	productCategories: []
}

@Injectable()
@State<UserProductCategoriesStateModel>({
	name: 'userProductCategories',
	defaults: defaultState,
})
export class UserProductCategoriesState {
	constructor (private productCategoriesService: ProductCategoriesService,
		private router: Router) {
	}

	@Selector()
	public static productCategories(state: UserProductCategoriesStateModel): ProductCategory[] {
		return state.productCategories;
	}

	@Action(InitializeUserProductCategories)
	public initializeUserProductCategories(ctx: StateContext<UserProductCategoriesStateModel>) {
		return this.productCategoriesService.getCategories().subscribe(productCategories => {
			ctx.patchState({
				productCategories: productCategories
			});
		});
	}
	
	@Action(AddNewUserProductCategory)
	public addNewUserProductCategory(ctx: StateContext<UserProductCategoriesStateModel>, action: AddNewUserProductCategory) {
		return this.productCategoriesService.addNewUserProductCategory(action.request)
			.subscribe(response => ctx.dispatch(new AddNewUserProductCategorySuccess()));
	}

	@Action(AddNewUserProductCategorySuccess)
	public addNewUserProductCategorySuccess(ctx: StateContext<UserProductCategoriesStateModel>) {
		ctx.dispatch(new InitializeUserProductCategories());
		this.router.navigate(['user-product-categories']);
	}

	@Action(RemoveUserProductCategory)
	public removeUserProductCategory(ctx: StateContext<UserProductCategoriesStateModel>, action: RemoveUserProductCategory) {
		return this.productCategoriesService.removeUserProductCategory(action.guid)
			.subscribe(() => ctx.dispatch(new InitializeUserProductCategories()));
	}
}




