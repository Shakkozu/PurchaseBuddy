import { ProductCategory } from "../services/product-categories.service";

export class InitializeUserProductCategories {
	static readonly type = '[UserProductCategories] InitializeUserProductCategories';
	constructor () { }
}

export class AddNewUserProductCategory {
	static readonly type = '[UserProductCategories] AddNewUserProductCategory';
	constructor (public request: CreateProductCategoryRequest) { }
}

export class AddNewUserProductCategorySuccess {
	static readonly type = '[UserProductCategories] AddNewUserProductCategorySuccess';
	constructor () { }
}

export interface CreateProductCategoryRequest {
	name: string;
	description: string;
	parentId?: string;
}
