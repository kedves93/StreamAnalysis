import { NgModule, Component } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { PasswordModule } from 'primeng/password';
import { TabMenuModule } from 'primeng/tabmenu';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { StepsModule } from 'primeng/steps';

import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginFormComponent } from './directives/login-form/login-form.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { RegisterFormComponent } from './directives/register-form/register-form.component';
import { RegisterComponent } from './pages/register/register.component';
import { MenuComponent } from './directives/menu/menu.component';
import { ContainerComponent } from './pages/container/container.component';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    LoginFormComponent,
    DashboardComponent,
    RegisterFormComponent,
    RegisterComponent,
    MenuComponent,
    ContainerComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ButtonModule,
    CheckboxModule,
    PasswordModule,
    TabMenuModule,
    HttpClientModule,
    ReactiveFormsModule,
    InputTextModule,
    StepsModule,
    RouterModule.forRoot([
      { path: 'home', component: HomeComponent },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'container', component: ContainerComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '**', redirectTo: 'home', pathMatch: 'full' }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
