import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({ providedIn: 'root'})
export class ProductCategoriesService {
	private urlBase = 'http://localhost:5133/categories/';
	constructor (private http: HttpClient) { }

	getCategories(): Observable<ProductCategory[]> {
		return this.http.get<ProductCategory[]>(this.urlBase);
	}
}

export interface ProductCategory {
	guid: string;
	name: string;
	description: string;
	parent: ProductCategory;
	children: ProductCategory[];
	isRoot: boolean;
	parentId: string;
}


