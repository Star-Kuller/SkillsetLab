import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {LoginComponent} from "./login/login.component";
import {FormsModule} from "@angular/forms";
import {HttpClientModule} from "@angular/common/http";
import {AuthService} from "./Services/Auth/auth.service";

@Component({
  selector: 'app-root',
  standalone: true,
  imports:
    [
      RouterOutlet,
      LoginComponent,
      FormsModule,
      HttpClientModule
    ],
  providers:
    [
      AuthService
    ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'SkillsetLab.Frontend';
}
