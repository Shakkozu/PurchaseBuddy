import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { AuthorizeGuard } from "src/api-authorization/authorize.guard";
import { UserStoresListComponent } from "./user-stores/user-stores-list/user-stores-list.component";

const routes: Routes = [
	{ path: 'user-stores', component: UserStoresListComponent, pathMatch: 'full', canActivate: [AuthorizeGuard] },
]
@NgModule({
	declarations: [],
	imports: [
		CommonModule,
		RouterModule.forChild(routes),
	],
	exports: [RouterModule],
})
export class UserStoresRoutingModule { }
