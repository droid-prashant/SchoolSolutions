import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { jwtDecode } from "jwt-decode"
import { Router } from '@angular/router';
import { Login } from './common/models/login/login.mode';
import { JwtPayLoad } from './common/models/jwtPayload/jwtPayload.model';
import { environment } from '../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    baseUrl: string = environment.API_BASE_URL
    constructor(private _httpService: HttpClient, private _router: Router) { }

    login(login: Login): Observable<any> {
        return this._httpService.post(this.baseUrl + "api/App/Login", login);
    }

    decodeToken(): JwtPayLoad {
        const token = window.localStorage.getItem('token');
        if (token) {
            return jwtDecode(token);
        }
        else {
            return new JwtPayLoad();
        }
    }

    getTokenExpirationDate(): Date {
        const decodedToken = this.decodeToken();
        if (decodedToken) {
            const date = new Date();
            date.setUTCSeconds(decodedToken.exp)
            return date;
        }
        else {
            return new Date();
        }
    }

    isTokenExpired(): boolean {
        const tokenExpirationDate = this.getTokenExpirationDate();
        if (tokenExpirationDate) {
            return !(tokenExpirationDate.valueOf() > new Date().valueOf());
        }
        return false;
    }

    logout() {
        window.localStorage.clear();
        this._router.navigateByUrl("");
    }
}
