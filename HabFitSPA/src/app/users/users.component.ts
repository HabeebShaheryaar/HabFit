import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  users: any;
  constructor(private http: HttpClient) { }

  ngOnInit() {
    // this.getUsers();
  }

  // getUsers() {
  //   this.http.get('https://localhost:44311/api/Account').subscribe(response => {
  //     this.users = response;
  //   }, error => {
  //     console.log(error);
  //   });
  // }
}
