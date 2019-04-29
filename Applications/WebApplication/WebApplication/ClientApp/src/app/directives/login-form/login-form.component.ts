import { AuthService } from './../../services/auth/auth.service';
import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css']
})
export class LoginFormComponent implements OnInit {

  @Input() userType: string;

  public validAuth: boolean;

  public loginForm = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
    rememberMe: new FormControl(false)
  });

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit() {
  }

  onSubmit() {
    if (this.userType === 'container') {
      this.auth.signInUser(
        this.loginForm.value.username,
        this.loginForm.value.password,
        this.loginForm.value.rememberMe,
        true)
        .subscribe(result => {
          this.validAuth = result;
          if (this.validAuth === true) {
            localStorage.setItem('currentUser', this.loginForm.value.username);
            this.router.navigate(['/container/home']);
          }
        },
        error => console.error(error)
      );
    }

    if (this.userType === 'dashboard') {
      this.auth.signInUser(
        this.loginForm.value.username,
        this.loginForm.value.password,
        this.loginForm.value.rememberMe,
        false)
        .subscribe(result => {
          this.validAuth = result;
          if (this.validAuth) {
            localStorage.setItem('currentUser', this.loginForm.value.username);
            this.router.navigate(['/home']);
          }
        },
        error => console.error(error)
      );
    }
  }

}
