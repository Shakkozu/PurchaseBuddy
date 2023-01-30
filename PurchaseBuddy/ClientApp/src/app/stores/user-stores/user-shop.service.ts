import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { UserShop } from "../user-shops.model";

@Injectable({
	providedIn: 'root'
})
export class UserShopService {
	constructor (private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { 

	}

	public getAll(): Observable<UserShop[]> {
		return this.http.get<UserShop[]>(this.baseUrl + 'usershop');
	}
}



