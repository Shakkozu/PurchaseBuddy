import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
	providedIn: 'root'
})
export class AuthGuard implements CanActivate {
	constructor (private router: Router) { }

	canActivate(
		next: ActivatedRouteSnapshot,
		state: RouterStateSnapshot
	): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
		const isAuthenticated = this.checkIfAuthenticated();
		if (!isAuthenticated) {
			this.router.navigate(['/login']);
			return false;
		}
		return true;
	}

	checkIfAuthenticated() {
		const cookieValue = this.getCookieValue('auth');
		return !!cookieValue;
	}

	private getCookieValue(name: string) {
		const value = `; ${ document.cookie }`;
		const parts = value.split(`; ${ name }=`);
		if (parts?.length === 2) {
			return parts.pop();
		}

		return undefined;
	}
}
