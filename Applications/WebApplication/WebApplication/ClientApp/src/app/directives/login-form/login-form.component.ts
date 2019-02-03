import { AuthService } from './../../services/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css']
})
export class LoginFormComponent implements OnInit {

  public validAuth: boolean;

  public loginForm = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
    rememberMe: new FormControl('')
  });

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit() {
  }

  onSubmit() {
    this.auth.signInUser(this.loginForm.value.username, this.loginForm.value.password).subscribe(
      result => {
        this.validAuth = result;
        if (this.validAuth === true) {
          this.router.navigate(['/dashboard']);
        }
      },
      error => console.error(error)
    );
  }

}
