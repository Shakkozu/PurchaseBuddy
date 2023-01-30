import { Injectable } from '@angular/core';
import {
	HttpEvent, HttpRequest, HttpHandler,
	HttpInterceptor, HttpErrorResponse
} from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class ServerErrorInterceptor implements HttpInterceptor {

	constructor (private notificationService: NotificationService) { }

	intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

		return next.handle(request).pipe(
			retry(1),
			catchError((error: HttpErrorResponse) => {
				this.notificationService.showError(error.error);
				return throwError(error);
			})
		);
	}
}