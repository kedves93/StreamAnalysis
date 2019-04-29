import { FormGroup, FormControl } from '@angular/forms';
import { Component, OnInit, Input } from '@angular/core';
import { AuthService } from './../../services/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.css']
})
export class RegisterFormComponent implements OnInit {

  @Input() userType: string;

  public registerForm = new FormGroup({
    email: new FormControl(''),
    username: new FormControl(''),
    password: new FormControl(''),
  });

  constructor(private auth: AuthService, private router: Router) { }

  ngOnInit() {
  }

  onSubmit() {
    if (this.userType === 'container') {
      this.auth.registerUser(
        this.registerForm.value.email,
        this.registerForm.value.username,
        this.registerForm.value.password,
        true)
        .subscribe(
        result => {
          localStorage.setItem('currentUser', this.registerForm.value.username);
          this.router.navigate(['/container/home']);
        },
        error => console.error(error)
      );
    }

    if (this.userType === 'dashboard') {
      this.auth.registerUser(
        this.registerForm.value.email,
        this.registerForm.value.username,
        this.registerForm.value.password,
        false)
        .subscribe(
        result => {
          localStorage.setItem('currentUser', this.registerForm.value.username);
          this.router.navigate(['/home']);
        },
        error => console.error(error)
      );
    }
  }

}
