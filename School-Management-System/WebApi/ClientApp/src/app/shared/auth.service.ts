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

    getCurrentAcademicYearId(): string {
        return this.decodeToken()?.academicYear ?? '';
    }

    getPermissions(): string[] {
        const decodedToken = this.decodeToken();
        return this.toArray(decodedToken?.permission);
    }

    getRoles(): string[] {
        const decodedToken: any = this.decodeToken();
        return [
            ...this.toArray(decodedToken?.roles),
            ...this.toArray(decodedToken?.role),
            ...this.toArray(decodedToken?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])
        ].filter((value, index, values) => values.indexOf(value) === index);
    }

    hasPermission(permission: string): boolean {
        if (!permission) {
            return true;
        }

        if (this.hasRole('SuperAdmin') || this.hasRole('Super Admin')) {
            return true;
        }

        return this.getPermissions().some(x => x.toLowerCase() === permission.toLowerCase());
    }

    hasAnyPermission(permissions: string[]): boolean {
        if (!permissions || permissions.length === 0) {
            return true;
        }

        return permissions.some(permission => this.hasPermission(permission));
    }

    hasRole(role: string): boolean {
        if (!role) {
            return true;
        }

        return this.getRoles().some(x => x.toLowerCase() === role.toLowerCase());
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

    private toArray(value: string | string[] | undefined | null): string[] {
        if (!value) {
            return [];
        }

        return Array.isArray(value) ? value : [value];
    }
}
