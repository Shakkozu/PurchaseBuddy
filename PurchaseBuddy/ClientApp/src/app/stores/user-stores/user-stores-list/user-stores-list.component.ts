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
