import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { UserShop } from '../../user-shops.model';
import { UserShopService } from '../user-shop.service';

@Component({
  selector: 'app-user-stores-list',
  templateUrl: './user-stores-list.component.html',
  styleUrls: ['./user-stores-list.component.css']
})
export class UserStoresListComponent implements OnInit {

  public returnMessage: string = '';
  public userShops: UserShop[] = [];
  public error?: string;
  displayedColumns: string[] = ['position', 'name', 'weight', 'symbol'];
  dataSource = ELEMENT_DATA;

  constructor (private userShopService: UserShopService) { }

  ngOnInit(): void {
    this.userShopService.getAll().subscribe(
      result => this.userShops = result,
      error => {
        console.log('im here');
        console.log(error);
        this.error = error.message;
      });
  }

}

export interface PeriodicElement {
  name: string;
  position: number;
  weight: number;
  symbol: string;
}

const ELEMENT_DATA: PeriodicElement[] = [
  { position: 1, name: 'Hydrogen', weight: 1.0079, symbol: 'H' },
  { position: 2, name: 'Helium', weight: 4.0026, symbol: 'He' },
  { position: 3, name: 'Lithium', weight: 6.941, symbol: 'Li' },
  { position: 4, name: 'Beryllium', weight: 9.0122, symbol: 'Be' },
  { position: 5, name: 'Boron', weight: 10.811, symbol: 'B' },
  { position: 6, name: 'Carbon', weight: 12.0107, symbol: 'C' },
  { position: 7, name: 'Nitrogen', weight: 14.0067, symbol: 'N' },
  { position: 8, name: 'Oxygen', weight: 15.9994, symbol: 'O' },
  { position: 9, name: 'Fluorine', weight: 18.9984, symbol: 'F' },
  { position: 10, name: 'Neon', weight: 20.1797, symbol: 'Ne' },
];
