import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Form, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { map, Subject, takeUntil } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

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
    // this.http.post(, body).pipe(
    //   takeUntil(this.destroy$)).subscribe((response: any) => {
    //     console.log('Success!  ' + response)
    //     const cookies = response.headers.get('Set-Cookie');
    //     console.log(cookies);
    //     this.cookieService.set('auth', cookies);
    //     this.cookieService.set('token', response);
    //   },
    //     (error) => console.log(error));
    this.http.post(url, body)
      // .pipe(map((res: any) => res.headers.get('headerName')))
      .pipe()
      .subscribe(res => {
        console.log(res);
        this.cookieService.set('auth', res.toString());
        
      // Do something with the header value
    });

  }
}

export interface IUserLoginRequest {
  login: string;
  password: string;
}