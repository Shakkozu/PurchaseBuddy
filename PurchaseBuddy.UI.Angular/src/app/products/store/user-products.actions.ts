export class GetUserProducts {
	static readonly type = '[UserProducts] Get user products';
	constructor () { }
}

export class AddNewUserProduct {
	static readonly type = '[UserProducts] Save user product';
	constructor (public name: string, public categoryGuid: string) { }
}