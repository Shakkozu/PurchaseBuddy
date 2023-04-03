import { UserShopDto, UserShop } from "../model";
import { Injectable } from "@angular/core";
import { Action, Selector, State, StateContext } from "@ngxs/store";
import { ShopService } from "../services/shop-service";
import { AddNewUserShop, DeleteUserShop, GetUserShops, UpdateUserShop, UserShopSavedSuccessfully } from "./user-shops.actions";
import { Router } from "@angular/router";

export interface UserShopsStateModel {
	  shops: UserShop[];
}

const defaultState: UserShopsStateModel = {
	shops: []
}

@Injectable()
@State<UserShopsStateModel>({
	name: 'userShops',
	defaults: defaultState
})
export class UserShopsState {
	constructor (private shopsService: ShopService,
	private router: Router) {
	}

	@Selector()
	public static shops(state: UserShopsStateModel): UserShop[] {
		return state.shops;
	}

	@Selector()
	public static shop(state: UserShopsStateModel) {
		return (id: string) => state.shops.find(s => s.guid === id);
	}

	@Action(GetUserShops)
	public getUserShops(ctx: StateContext<UserShopsStateModel>) {
		return this.shopsService.getUserShops().subscribe(shops => {
			ctx.patchState({
				shops: shops
			});
		});
	}

	@Action(AddNewUserShop)
	public addNewUserShop(ctx: StateContext<UserShopsStateModel>, action: AddNewUserShop) {
		return this.shopsService.addNewUserShop(action.request).subscribe(response => {
			ctx.dispatch(new UserShopSavedSuccessfully());
		});
	}

	@Action(UpdateUserShop)
	public updateUserShop(ctx: StateContext<UserShopsStateModel>, action: UpdateUserShop) {
		return this.shopsService.updateUserShop(action.id, action.request).subscribe(() => {
			ctx.dispatch(new GetUserShops());
		});
	}

	@Action(UserShopSavedSuccessfully)
	public userShopSavedSuccessfully(ctx: StateContext<UserShopsStateModel>) {
		ctx.dispatch(new GetUserShops()).subscribe(() =>
			this.router.navigate(['/user-shops'])
		);
	}
	
	
	@Action(DeleteUserShop)
	public deleteUserShop(ctx: StateContext<UserShopsStateModel>, action: DeleteUserShop) {
		return this.shopsService.deleteUserShop(action.id).subscribe(() => {
			ctx.dispatch(new GetUserShops());
			this.router.navigate(['/user-shops']);
		});
	}
}




