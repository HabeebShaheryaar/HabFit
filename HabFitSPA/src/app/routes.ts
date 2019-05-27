import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { UsersComponent } from './members/users/users.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { UserDetailComponent } from './members/user-detail/user-detail.component';
import { UserDetailResolver } from './_resolvers/user-detail.resolver';
import { UserListResolver } from './_resolvers/user-list.resolver';
import { UserEditComponent } from './members/user-edit/user-edit.component';
import { UserEditResolver } from './_resolvers/user-edit.resolver.';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { FanismComponent } from './fanism/fanism.component';
import { FanismResolver } from './_resolvers/fanism.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver';

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
            { path: 'user/edit', component: UserEditComponent, resolve: {user: UserEditResolver}, canDeactivate: [PreventUnsavedChanges]},
            { path: 'messages', component: MessagesComponent, resolve: {messages: MessagesResolver} },
            { path: 'fanism', component: FanismComponent, resolve: {users: FanismResolver}},
        ]
    },
    { path: '**', redirectTo: 'home', pathMatch: 'full'},
];
