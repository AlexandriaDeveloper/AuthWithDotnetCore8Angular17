import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environment';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  http =inject(HttpClient);
  router = inject(Router);
  apiUrl=environment.apiUrl;
  currentUserSig = signal<any | undefined | null>(undefined);

  constructor() { }

  login(model) {
     this.http.post('http://localhost:5118/api/account/login',model).subscribe({
      next:(res:any)=>{
        localStorage.setItem('token',res.token);
        localStorage.setItem('user',JSON.stringify(res));
        this.currentUserSig.set(res);
        console.log(res)
        this.router.navigateByUrl('/');
      },
      error:(err)=>console.log(err)
    });
  }
  signup(model){

  }
  logout(){
    return this.http.get('http://localhost:5118/api/account/logout').subscribe({
      next:(res:any)=>{
        console.log(res);
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        this.currentUserSig.set(null);
      },
      error:(err)=>console.log(err)
    });
  }
  currentUser(){
    if(localStorage.getItem("user")){
      this.currentUserSig.set(JSON.parse(localStorage.getItem("user")));
    }
    return this.currentUserSig;
  }

  test()
  {
    return this.http.get('http://localhost:5118/api/secure/secure');
  }
}
