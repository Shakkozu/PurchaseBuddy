import { Component } from '@angular/core';
import { Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { UserSessionState } from '../auth/store/session.state';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {
  isExpanded = false;
  public isUserLoggedIn$: Observable<boolean | undefined>;
  public userSessionId$: Observable<string | undefined>;

  constructor (private userSessionState: UserSessionState,
  private store: Store) {
    this.isUserLoggedIn$ = this.store.select(UserSessionState.isAuthenticated);
    this.userSessionId$ = this.store.select(UserSessionState.userSessionId);
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
