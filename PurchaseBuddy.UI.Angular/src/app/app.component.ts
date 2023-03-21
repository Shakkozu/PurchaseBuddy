import { Component } from '@angular/core';
import { Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { Logout } from './auth/store/authorization.actions';
import { AuthorizationState } from './auth/store/authorization.state';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  public isUserLoggedIn$: Observable<boolean | undefined>;
  public userSessionId$: Observable<string | undefined>;
  public username$: Observable<string | undefined>;
  title = 'PurchaseBuddy.UI.Angular';

  constructor (private store: Store) {
    this.isUserLoggedIn$ = this.store.select(AuthorizationState.isAuthenticated);
    this.userSessionId$ = this.store.select(AuthorizationState.userSessionId);
    this.username$ = this.store.select(AuthorizationState.username);
  }

  logout() {
    this.store.dispatch(new Logout());
  }
}
