import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class HttpCookieInterceptor implements HttpInterceptor {

	constructor () { }

	intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		const cookieValue = this.getCookieValue('auth');

		if (cookieValue) {
			req = req.clone({
				setHeaders: {
					Authorization: `${ cookieValue }`
				}
			});
		}

		return next.handle(req);
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
