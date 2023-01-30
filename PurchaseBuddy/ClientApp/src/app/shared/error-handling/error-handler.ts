import { ErrorHandler, Injectable } from '@angular/core';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

	
	constructor(private notificationService: NotificationService) {

	}
	
	handleError(error: any) {
		this.notificationService.showError(error);
		console.log(error);
	}
}