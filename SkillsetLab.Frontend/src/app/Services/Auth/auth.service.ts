import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";

import { TokenResponse } from '../../models/Auth/TokenResponse';
import { LoginForm } from '../../models/Auth/LoginForm';
import { RegisterForm } from '../../models/Auth/RegisterForm';
import {Observable} from "rxjs";

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json', 'accept': 'application/json'})
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private loginUrl = 'http://localhost:5241/accounts/login';
  private signupUrl = 'http://localhost:5241/accounts/register';

  Login(loginForm: LoginForm): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(this.loginUrl, loginForm, httpOptions);
  }

  Register(registerForm: RegisterForm): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(this.signupUrl, registerForm, httpOptions);
  }

  Test(): Observable<string> {
    return this.http.post<string>('http://localhost:5241/send', httpOptions);
  }

  constructor(private http: HttpClient) { }
}
