import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './shared/service/auth.service';
import { LoginComponent } from './account/login/login.component';
interface IUser{
  name: string
  role:string,
  id:string
}
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet,LoginComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  ngOnInit(): void {
    this.currentUser= this.auth.currentUser();
  }
  logout(){
    this.auth.logout();
  }
  testAuth(){
    this. auth.test().subscribe({
      next:(res:any)=>{
        console.log(JSON.stringify(res));
      },
      error:(err)=>console.log(err)
    })
  }
  auth = inject(AuthService);
  currentUser ;
}

