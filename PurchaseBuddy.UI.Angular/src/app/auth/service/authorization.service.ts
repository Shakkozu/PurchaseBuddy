import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { IUserDto } from "..";

@Injectable({
	providedIn: 'root'
})
export class AuthorizationService {
	private urlBase = 'http://localhost:5133/authorization/';
	constructor (private httpClient: HttpClient) {	
	}

	public login(username: string, password: string): Observable<string> {
		const body = {
			login: username,
			password: password
		};

		return this.httpClient.post<string>(this.urlBase + 'login', body);
	}

	public register(userDto: IUserDto): Observable<string> {
		return this.httpClient.post<string>(this.urlBase + 'register', userDto);
	}
	
	public logout(sessionId: string) {
		return this.httpClient.post<string>(this.urlBase + 'logout', sessionId);
	}
}