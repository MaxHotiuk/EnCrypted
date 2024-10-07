import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { HomeComponent } from './pages/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { UsersComponent } from './pages/users/users.component';
import { EncryptComponent } from './pages/encrypt/encrypt.component';
import { NonAuthGuard } from './guards/nonauth.guard.spec';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'users',
    component: UsersComponent,
    canActivate: [RoleGuard],
    data: {
      roles: ['admin']
    }
  },
  {
    path: 'encrypt',
    component: EncryptComponent,
    canActivate: [NonAuthGuard]
  }
];
