import { Component } from '@angular/core';
import { Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { Logout } from '../auth/store/authorization.actions';
import { AuthorizationState } from '../auth/store/authorization.state';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {
  isExpanded = false;
  public isUserLoggedIn$: Observable<boolean | undefined>;
  public userSessionId$: Observable<string | undefined>;
  public username$: Observable<string | undefined>;

  constructor (private store: Store) {
    this.isUserLoggedIn$ = this.store.select(AuthorizationState.isAuthenticated);
    this.userSessionId$ = this.store.select(AuthorizationState.userSessionId);
    this.username$ = this.store.select(AuthorizationState.username);
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.store.dispatch(new Logout());
  }
}
