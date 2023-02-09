import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { UserSessionState } from './store/session.state';

@Injectable({
	providedIn: 'root'
})
export class AuthGuard implements CanActivate {
	constructor (private router: Router,
	private store: Store) { }

	canActivate(
		next: ActivatedRouteSnapshot,
		state: RouterStateSnapshot
	): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
		const isAuthenticated = this.store.selectSnapshot(UserSessionState.isAuthenticated);
		if (!isAuthenticated) {
			this.router.navigate(['/login']);
			return false;
		}
		return true;
	}
}
