import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'login',
    loadChildren: () => import('./account/account.module').then((m) => m.AccountModule)
  },
  //{
  //  path: '',
  //  component: LayoutComponent,
  //  canActivate: [AuthGuard],
  //  children: [],
  //}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
