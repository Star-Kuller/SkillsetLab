import { Component } from '@angular/core';
import {LoginForm} from "../models/Auth/LoginForm";
import {AuthService} from "../Services/Auth/auth.service";
import {TokenStorageService} from "../Services/Auth/token-storage.service";
import {FormsModule} from "@angular/forms";
import {NgIf} from "@angular/common";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    NgIf
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  form: any = {};
  isLoggedIn = false;
  isLoginFailed = false;
  errorMessage = '';
  private loginForm: LoginForm | null = null;

  constructor(private authService: AuthService, private tokenStorage: TokenStorageService) { }

  ngOnInit() {
    if (this.tokenStorage.getToken()) {
      this.isLoggedIn = true;
    }
  }

  onSubmit() {
    console.log('test this form');
    console.log(this.form);

    this.loginForm = new LoginForm(
      this.form.email,
      this.form.password);

    this.authService.Login(this.loginForm).subscribe(
      data => {
        console.log('------------');
        console.log(data);
        this.tokenStorage.saveToken(data.accessToken, data.role);

        this.isLoginFailed = false;
        this.isLoggedIn = true;
      },
      error => {
        console.log(error);
        this.errorMessage = error.error.message;
        this.isLoginFailed = true;
      }
    );
  }
}
