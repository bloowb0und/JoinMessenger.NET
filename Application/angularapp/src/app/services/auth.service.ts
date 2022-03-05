import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() { }

  authUser(user:any){
    let UserArray = [];
    if (localStorage.getItem('Users')){
      UserArray = JSON.parse(localStorage.getItem('Users') || '{}');
    }

    return UserArray.find((u: { Email: any; Password: any; }) => u.Email === user.email && u.Password === user.password);
  }
}
