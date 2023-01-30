export interface UserShop {
	id: string,
	name: string;
	description: string;
	categories: Array<UserShopConfigurationEntry>;
}

export interface UserShopConfigurationEntry {
	index: number;
	categoryName: string;
	test: string;
}