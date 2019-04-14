import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { UsersComponent } from './members/users/users.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { UserDetailComponent } from './members/user-detail/user-detail.component';
import { UserDetailResolver } from './_resolvers/user-detail.resolver';
import { UserListResolver } from './_resolvers/user-list.resolver';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent},
    { path: 'home', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'users', component: UsersComponent, resolve: {users: UserListResolver}},
            { path: 'users/:id', component: UserDetailComponent, resolve: {user: UserDetailResolver}},
            { path: 'messages', component: MessagesComponent },
        ]
    },
    { path: '**', redirectTo: 'home', pathMatch: 'full'},
];
