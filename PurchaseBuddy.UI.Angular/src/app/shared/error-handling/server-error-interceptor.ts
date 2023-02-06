import { Injectable } from '@angular/core';
import {
	HttpEvent, HttpRequest, HttpHandler,
	HttpInterceptor, HttpErrorResponse
} from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { NotificationService } from './notification.service';
import { ActivatedRoute, Router } from '@angular/router';

@Injectable()
export class ServerErrorInterceptor implements HttpInterceptor {

	constructor (private notificationService: NotificationService, private router: Router,
	private route: ActivatedRoute) { }

	intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		this.notificationService.closeSnackBar();
		console.log(this.route.snapshot.root.firstChild?.url[0].path);

		return next.handle(request).pipe(
			retry(1),
			catchError((error: HttpErrorResponse) => {
				const errorResponse = error.error as string;
				if (errorResponse) {
					console.log(error);
					this.notificationService.showError(errorResponse);
				}

				if (error.status === 401) {
					this.notificationService.showError('User not authorized');
					this.router.navigate(['/login'],
						{ queryParams: { returnUrl: this.route.snapshot.root.firstChild?.url[0].path } });
				}

				return throwError(error);
			})
		);
	}
}