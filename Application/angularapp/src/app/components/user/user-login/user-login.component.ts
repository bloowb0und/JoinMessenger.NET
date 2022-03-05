import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { User } from 'src/app/data/user';
import { AlertifyService } from 'src/app/services/alertify.service';
import { AuthService } from 'src/app/services/auth.service';
import { AuthapiService } from 'src/app/services/authapi.service';

@Component({
  selector: 'app-user-login',
  templateUrl: './user-login.component.html',
  styleUrls: ['./user-login.component.css']
})
export class UserLoginComponent implements OnInit {

  constructor(private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService,
    private api: AuthapiService) { }

  ngOnInit(): void {

  }

  onLogin(loginForm : NgForm){
    // console.log(loginForm.value);
    // const token = this.authService.authUser(loginForm.value);
    // console.log(token);
    // if (token){
    //   let UserArray = JSON.parse(localStorage.getItem('Users') || '{}');
    //   let foundUser = UserArray.find((u: { Email: any; Password: any; }) => u.Email === loginForm.value.email && u.Password === loginForm.value.password);
    //   localStorage.setItem('token',foundUser.UserName);
    //   this.alertify.success('You have successfuly logged in!')
    //   this.router.navigate(['/']);
    // }else{
    //   this.alertify.error('Login or password is incorrect!')
    // }

    const user = loginForm.value;
    this.loginUser(user).subscribe(res =>{
      if (res['result'] == null){
        this.alertify.error('Login or password is incorrect');
      }else{

        this.alertify.success(`${res['result']['login']}, you are logged in `);
        localStorage.setItem('token',`${res['result']['login']}`);
        this.router.navigate(['/main']);
      }
    });
  }

  loginUser(user: User){
    return this.api.loginUser(user);
  }
}
