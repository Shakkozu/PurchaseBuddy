import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Form, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { map, Subject, takeUntil } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { ActivatedRoute, Router } from '@angular/router';

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

  public onSubmit(): void {
    console.log(this.loginForm.value);
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
        this.cookieService.set('auth', res.toString());
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