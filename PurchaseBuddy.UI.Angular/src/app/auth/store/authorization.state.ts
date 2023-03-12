import { Injectable } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { Action, State, StateContext, Selector } from "@ngxs/store";
import { AuthorizationService } from "../service/authorization.service";
import { OnLoginSuccess, Login, Register, Logout } from "./authorization.actions";


export interface UserSessionStateModel {
	username?: string;
	sessionId?: string;
}

const defaultState: UserSessionStateModel = {
	username: undefined,
	sessionId: undefined
}

@Injectable({
	providedIn: 'root'
})
export class testService {

}

@Injectable()
@State<UserSessionStateModel>({
	name: 'userSession',
	defaults: defaultState
})

export class AuthorizationState {
	constructor (private authorizationService: AuthorizationService,
		private router: Router,
		private route: ActivatedRoute) {
	}

	@Action(OnLoginSuccess)
	public createUserSession({ patchState }: StateContext<UserSessionStateModel>, { username, sessionId }: OnLoginSuccess) {
		patchState({
			username: username,
			sessionId: sessionId,
		});
		this.redirect();
	}
	
	@Action(Login)
	public login(ctx: StateContext<UserSessionStateModel>, { username, password }: Login) {
		this.authorizationService.login(username, password)
			.subscribe(sessionId => ctx.dispatch(new OnLoginSuccess(username, sessionId)));
	}
	
	@Action(Register)
	public register(ctx: StateContext<UserSessionStateModel>, { userDto }: Register) {
		this.authorizationService.register(userDto).subscribe(() =>
			ctx.dispatch(new Login(userDto.login, userDto.password))
		);
	}
	
	@Action(Logout)
	public logout({ getState, patchState }: StateContext<UserSessionStateModel>) {
		const state = getState();
		patchState({
			...defaultState
		});
	}
	
	@Selector()
	public static isAuthenticated(state: UserSessionStateModel) {
		return !!state.username;
	}

	@Selector()
	static userSessionId(state: UserSessionStateModel) {
	  return state.sessionId;
	}
	
	@Selector()
	static username(state: UserSessionStateModel) {
	  return state.username;
	}

	private redirect(): void {
		// temporary disabled redirect, because it doesn't work with user-products component.
		// Filtering on products table doesnt work.

		this.router.navigate(['/home']);
	}
}