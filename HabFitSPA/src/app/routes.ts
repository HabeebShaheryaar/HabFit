import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { UsersComponent } from './users/users.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent},
    { path: 'home', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'users', component: UsersComponent, canActivate: [AuthGuard]},
            { path: 'messages', component: MessagesComponent, canActivate: [AuthGuard]},
        ]
    },
    { path: '**', redirectTo: 'home', pathMatch: 'full'},
];
