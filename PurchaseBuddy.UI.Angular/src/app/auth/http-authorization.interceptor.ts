import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Store } from '@ngxs/store';
import { UserSessionState } from './store/session.state';

@Injectable()
export class HttpAuthorizationInterceptor implements HttpInterceptor {

	constructor (private store: Store) {
	 }

	intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		const sessionId = this.store.selectSnapshot(UserSessionState.userSessionId);
		const isAuthenticated = this.store.selectSnapshot(UserSessionState.isAuthenticated);

		if (isAuthenticated) {
			req = req.clone({
				setHeaders: {
					Authorization: `${ sessionId }`
				}
			});
		}

		return next.handle(req);
	}
}
