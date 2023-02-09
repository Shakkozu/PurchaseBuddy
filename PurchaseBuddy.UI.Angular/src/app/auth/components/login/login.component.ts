import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subject } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { CreateUserSession } from '../../store/session.state';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  providers: [CookieService],
})
export class LoginComponent implements OnInit {
  public hide: boolean = false;
  public loginForm!: FormGroup;
  private destroy$: Subject<any> = new Subject<any>();

  constructor (private formBuilder: FormBuilder,
    private cookieService: CookieService,
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private store: Store,
    ) {
    
    
  }

  public ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    this.loginForm = this.formBuilder.group({
      login: ["username"],
      password: ["zaq1@WSX"]
    });
  }

  public save(): void {
    const body: IUserLoginRequest = {
      login: this.loginForm.value.login,
      password: this.loginForm.value.password
    };

    const url = 'http://localhost:5133/authorization/login';
    this.http.post(url, body)
      .pipe()
      .subscribe(res => {
        console.log(res.toString());
        this.store.dispatch(new CreateUserSession(this.loginForm.value.login, res.toString()));
        this.redirect();
      });

  }

  private redirect(): void {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    if (returnUrl) {
      this.router.navigate([returnUrl]);

      return;
    }
    
    this.router.navigate(['/home']);
  }
}

export interface IUserLoginRequest {
  login: string;
  password: string;
}