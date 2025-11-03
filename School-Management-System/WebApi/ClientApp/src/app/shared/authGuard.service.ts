import { Injectable } from "@angular/core";
import { AuthService } from "./auth.service";
import { Router } from "@angular/router";

@Injectable({
    providedIn: 'root'
})
export class AuthGuardService {
    constructor(private _authService: AuthService, private _router: Router) { }

    canActivate(): boolean {
        const token = window.localStorage.getItem('token')
        if (token) {
            if (this._authService.isTokenExpired()) {
                this._authService.logout();
                return false;
            }
            return true;
        }
        this._router.navigate(['/login']);
        return false;
    }
}