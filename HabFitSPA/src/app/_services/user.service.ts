import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiURL;

  constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'user');
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'user/' + id);
  }

  updateUser(id: string, user: User) {
    return this.http.put(this.baseUrl + 'user/' + id, user);
  }

  setMainPhoto(userID: string, ID: string) {
    return this.http.post(this.baseUrl + 'user/' + userID + '/photos/' + ID + '/setMain', {});
  }

  deletePhoto(userID: string, ID: string) {
    return this.http.delete(this.baseUrl + 'user/' + userID + '/photos/' + ID);
  }
}
