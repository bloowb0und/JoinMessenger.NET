import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { User } from 'src/app/data/user';
import { AlertifyService } from 'src/app/services/alertify.service';
import { AuthapiService } from 'src/app/services/authapi.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-signup',
  templateUrl: './user-signup.component.html',
  styleUrls: ['./user-signup.component.css']
})
export class UserSignupComponent implements OnInit {
  signUpForm!: FormGroup;
  user!: User;
  userSubmitted: boolean = false;
  data: any[];
  constructor(private fb: FormBuilder,
    private userService: UserService,
    private alertify: AlertifyService,
    private router: Router,
    private api: AuthapiService) { }

  ngOnInit(): void {
    this.createSignUpForm();
  }

  createSignUpForm(){
    this.signUpForm = this.fb.group({
      name: [null,[Validators.required,Validators.minLength(5),Validators.maxLength(50)]],
      email: [null,[Validators.required,Validators.email,Validators.maxLength(150)]],
      login: [null,[Validators.required,Validators.minLength(4),Validators.maxLength(30),]],
      password: [null,[Validators.required,Validators.minLength(8),Validators.maxLength(30),
      this.patternValidator(/\d/, { hasNumber: true }),
      this.patternValidator(/[A-Z]/, { hasCapitalCase: true }),
      this.patternValidator(/[a-z]/, { hasSmallCase: true }),
      this.patternValidator(/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/,{ hasSpecialCharacters: true })]],
      confirmPassword: [null,[Validators.required,Validators.minLength(4)]]
    }, {validators: this.customPasswordValidator});
  }

  get name(){
    return this.signUpForm.get('name') as FormControl;
  }

  get email(){
    return this.signUpForm.get('email') as FormControl;
  }

  get login(){
    return this.signUpForm.get('login') as FormControl;
  }

  get password(){
    return this.signUpForm.get('password') as FormControl;
  }

  get confirmPassword(){
    return this.signUpForm.get('confirmPassword') as FormControl;
  }

  customPasswordValidator(fg: AbstractControl): ValidationErrors | null{
    return fg.get('password')?.value === fg?.get('confirmPassword')?.value ? null :
    {notmatched: true};
  }

  patternValidator(regex: RegExp, error: ValidationErrors): ValidationErrors | null{
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        return null;
      }

      const valid = regex.test(control.value);

      return valid ? null : {notstrong: true};
    };
  }

  userData() : User {
    return this.user = {
      Name : this.name.value,
      Email : this.email.value,
      Login: this.login.value,
      Password: this.password.value,
      ConfirmPassword : this.confirmPassword.value
    }
  }

  // onSubmit(){
  //   this.userSubmitted = true;
  //   if(this.signUpForm.valid){
  //     this.userService.addUser(this.userData());
  //     this.signUpForm.reset();
  //     this.userSubmitted = false;
  //     this.alertify.success('You have successfuly signed up!')
  //     this.router.navigate(['/login']);
  //   }else{
  //     this.alertify.error('Form input is not valid! Try again!');
  //   }
  // }

  onSubmit(){
    const user = this.signUpForm.value;
    this.createUser(user).subscribe(res =>{
      this.alertify.success('You have successfuly signed up!');
      this.signUpForm.reset();
      this.userSubmitted = false;
      this.router.navigate(['/login']);

    },error => {
      this.alertify.error('Try again');
    });
  }

  createUser(user: any){
    return this.api.createUser(user);
  }
}
