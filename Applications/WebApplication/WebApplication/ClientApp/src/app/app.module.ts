import { NgModule, Component } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { PasswordModule } from 'primeng/password';
import { TabMenuModule } from 'primeng/tabmenu';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { StepsModule } from 'primeng/steps';
import { AccordionModule } from 'primeng/accordion';
import { RadioButtonModule } from 'primeng/radiobutton';
import { DropdownModule } from 'primeng/dropdown';
import { ClipboardModule } from 'ngx-clipboard';
import { SidebarModule } from 'primeng/sidebar';

import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginFormComponent } from './directives/login-form/login-form.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { RegisterFormComponent } from './directives/register-form/register-form.component';
import { RegisterComponent } from './pages/register/register.component';
import { MenuComponent } from './directives/menu/menu.component';
import { ContainerComponent } from './pages/container/container.component';
import { ContainerService } from './services/container/container.service';
import { AuthService } from './services/auth/auth.service';
import { AuthGuard } from './guards/auth.guard';


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
    FormsModule,
    InputTextModule,
    StepsModule,
    AccordionModule,
    RadioButtonModule,
    DropdownModule,
    ClipboardModule,
    SidebarModule,
    RouterModule.forRoot([
      { path: 'home', component: HomeComponent, canActivate: [ AuthGuard ] },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'container', component: ContainerComponent },
      { path: 'dashboard', component: DashboardComponent, canActivate: [ AuthGuard ] },
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '**', redirectTo: 'home', pathMatch: 'full' }
    ])
  ],
  providers: [
    AuthService,
    ContainerService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
