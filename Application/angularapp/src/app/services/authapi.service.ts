import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { User } from '../data/user';

@Injectable({
  providedIn: 'root'
})
export class AuthapiService {
  url: string;
  header: any;
  constructor(private http: HttpClient) {
    const headerSettings: {[name: string]: string | string[]; } = {};
    this.header = new HttpHeaders(headerSettings);
   }

   createUser(user : User){
    const headers = {
      headers: new HttpHeaders({
        'Content-Type':  'application/json'
      })
    }
     return this.http.post("https://localhost:5001/api/auth/register",user,headers);
   }

   loginUser(user: User){
    const headers = {
      headers: new HttpHeaders({
        'Content-Type':  'application/json'
      })
    }
     return this.http.post<User[]>("https://localhost:5001/api/auth/login",user,headers);
   }


}
