import { Injectable } from '@angular/core';
declare let alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  success(message: string){
     alertify.success(message);
  }

  warn(message: string){
    alertify.warn(message);
  }

  error(message: string){
    alertify.error(message);
  }
}
