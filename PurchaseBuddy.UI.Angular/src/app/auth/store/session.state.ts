import { Action, State, StateContext, Select, Selector } from "@ngxs/store";
import { Observable } from "rxjs";

export class CreateUserSession {
	static readonly type = '[UserSession] Create session';
	constructor(public username: string, public sessionId: string) { }
}

export interface UserSessionStateModel {
	username?: string;
	sessionId?: string;
}

const defaultState: UserSessionStateModel = {
	username: undefined,
	sessionId: undefined
}


@State<UserSessionStateModel>({
	name: 'userSession',
	defaults: defaultState
})

export class UserSessionState {
	@Action(CreateUserSession)
	public createUserSession({ getState, patchState }: StateContext<UserSessionStateModel>, { username, sessionId }: CreateUserSession) {
		const state = getState();
		patchState({
			username: username,
			sessionId: sessionId,
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
}