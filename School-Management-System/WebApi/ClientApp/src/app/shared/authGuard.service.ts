import { Injectable } from "@angular/core";
import { AuthService } from "./auth.service";
import { CanActivateChild, CanMatch, Route, Router, UrlSegment } from "@angular/router";

@Injectable({
    providedIn: 'root'
})
export class AuthGuardService implements CanActivateChild, CanMatch {
    constructor(private _authService: AuthService, private _router: Router) { }

    canActivate(): boolean {
        return this.validateAccess();
    }

    canActivateChild(): boolean {
        return this.validateAccess();
    }

    canMatch(_route: Route, _segments: UrlSegment[]): boolean {
        return this.validateAccess();
    }

    private validateAccess(): boolean {
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
