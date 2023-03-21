import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Product } from "../store/user-products.state";

@Injectable({
	providedIn: 'root'
})
export class ProductsService {
	private urlBase = 'http://localhost:5133/products/';
	constructor (private http: HttpClient) { }
	
	addNewUserProduct(request: AddNewUserProductRequest) {
		var body: AddNewUserProductRequest = {
			name: request.name,
		}
		if (request.categoryId)
			body['categoryId'] = request.categoryId;

		console.log(body);
		return this.http.post(this.urlBase, body);
	}

	getUserProducts(): Observable<Product[]> {
		return this.http.get<Product[]>(this.urlBase);
	}

}

export interface AddNewUserProductRequest {
	name: string;
	categoryId?: string;
}