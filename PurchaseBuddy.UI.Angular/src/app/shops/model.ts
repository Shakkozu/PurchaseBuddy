export interface UserShop {
	guid?: string;
	name: string;
	description: string;
	address: ShopAddressDto;
}

export interface ShopAddressDto {
	city: string;
	street: string;
	localNumber: string;
}
export interface UserShopDto {
	name: string;
	description: string;
	city: string;
	street: string;
	localNumber: string;
}

export interface UpdateUserShopRequest {
	name: string;
	categoryId?: string;
}