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

	addNewUserProductCategory(request: CreateProductCategoryRequest): Observable<string> {
		var body: CreateProductCategoryRequest = {
			name: request.name,
			description: request.description,
		};
		if (request.parentId) {
			body['parentId'] = request.parentId;
		}

		return this.http.post<string>(this.urlBase, body);
	}

	removeUserProductCategory(guid: string): Observable<void> {
		return this.http.delete<any>(this.urlBase + guid);
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


export interface CreateProductCategoryRequest {
	name: string;
	description: string;
	parentId?: string;
}


import { BehaviorSubject } from 'rxjs';

@Injectable()
export class ProgressService {
	private progressBarSource = new BehaviorSubject<boolean | null>(null);
	public progressBar$ = this.progressBarSource.asObservable();
	public visible = false;
	
	resetProgressBar() {
		this.progressBarSource.next(false);
		this.visible = false;
	}

	showProgressBar() {
		this.progressBarSource.next(true);
		this.visible = true;
	}

	hideProgressBar() {
		this.progressBarSource.next(false);
		this.visible = false;
	}

	executeWithProgress<T>(observableFn: () => Observable<T>): any {
		this.showProgressBar();
		// wait 3s before showing the progress bar
		setTimeout(() => {
			const observable = observableFn();
			observable.subscribe(() => this.hideProgressBar(), () => this.hideProgressBar());
			return observable;
		}, 3000);
	}
}
