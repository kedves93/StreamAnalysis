import { ContainerAuthGuard } from './guards/container-auth.guard';
import { HistoricalService } from './services/historical/historical.service';
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
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { TabViewModule } from 'primeng/tabview';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { DialogModule } from 'primeng/dialog';
import { ChartModule } from 'primeng/chart';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TableModule } from 'primeng/table';
import { ContextMenuModule } from 'primeng/contextmenu';

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
import { ContainerLoginComponent } from './pages/container-login/container-login.component';
import { ContainerRegisterComponent } from './pages/container-register/container-register.component';
import { DashboardService } from './services/dashboard/dashboard.service';
import { HubService } from './services/hub/hub.service';
import { TopicNamePipe } from './pipes/topicName/topic-name.pipe';
import { HistoricalComponent } from './pages/historical/historical.component';
import { QueueNamePipe } from './pipes/queueName/queue-name.pipe';
import { ContainerHomeComponent } from './pages/container-home/container-home.component';
import { ScheduleRuleIdPipe } from './pipes/scheduleRuleName/schedule-rule-id.pipe';


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
    ContainerComponent,
    ContainerLoginComponent,
    ContainerRegisterComponent,
    TopicNamePipe,
    HistoricalComponent,
    QueueNamePipe,
    ContainerHomeComponent,
    ScheduleRuleIdPipe
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
    ScrollPanelModule,
    TabViewModule,
    CardModule,
    DataViewModule,
    DialogModule,
    ChartModule,
    ConfirmDialogModule,
    TableModule,
    ContextMenuModule,
    RouterModule.forRoot([
      { path: 'home', component: HomeComponent, canActivate: [ AuthGuard ] },
      { path: 'login', component: LoginComponent },
      { path: 'container/home', component: ContainerHomeComponent, canActivate: [ ContainerAuthGuard ] },
      { path: 'container/create', component: ContainerComponent, canActivate: [ ContainerAuthGuard ] },
      { path: 'container/login', component: ContainerLoginComponent },
      { path: 'container/register', component: ContainerRegisterComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'dashboard', component: DashboardComponent, canActivate: [ AuthGuard ] },
      { path: 'historical', component: HistoricalComponent, canActivate: [ AuthGuard ] },
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '**', redirectTo: 'home', pathMatch: 'full' }
    ])
  ],
  providers: [
    AuthService,
    ContainerService,
    HubService,
    DashboardService,
    HistoricalService,
    ConfirmationService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
