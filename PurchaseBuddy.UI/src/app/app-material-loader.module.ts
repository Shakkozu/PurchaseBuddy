import { NgModule } from "@angular/core";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';

@NgModule({
	imports: [
		BrowserAnimationsModule,
		MatSlideToggleModule,
		MatToolbarModule,
		MatSidenavModule,
		MatMenuModule,
		MatIconModule,
		MatListModule,
	],
	exports: [
		BrowserAnimationsModule,
		MatSlideToggleModule,
		MatToolbarModule,
		MatSidenavModule,	
		MatIconModule,
		MatListModule,
	],
	providers: [],
	bootstrap: [MaterialLoaderModule]
})
export class MaterialLoaderModule { }
