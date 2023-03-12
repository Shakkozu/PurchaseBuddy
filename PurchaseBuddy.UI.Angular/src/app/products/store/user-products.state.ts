import { Injectable } from "@angular/core";
import { Action, Selector, State, StateContext } from "@ngxs/store";
import { Observable } from "rxjs";
import { ProductsService } from "../services/products-service";
import { GetUserProducts } from "./user-products.actions";

export interface UserProductsStateModel {
	  products: Product[];
}

export interface Product {
	guid: string;
	name: string;
	categoryId?: string;
	categoryName?: string;
}

const defaultState: UserProductsStateModel = {
	products: []
}

@Injectable()
@State<UserProductsStateModel>({
	name: 'userProducts',
	defaults: defaultState
})
export class UserProductsState {
	constructor (private productsService: ProductsService) {
	}

	@Selector()
	public static products(state: UserProductsStateModel): Product[] {
		return state.products;
	}

	@Action(GetUserProducts)
	public getUserProducts(ctx: StateContext<UserProductsStateModel>) {
		return this.productsService.getUserProducts().subscribe(products => {
			ctx.patchState({
				products: products
			});
		});
	}
}