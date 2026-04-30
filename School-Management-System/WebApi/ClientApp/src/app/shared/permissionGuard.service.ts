import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanMatch, Route, Router, UrlSegment } from '@angular/router';
import { AuthGuardService } from './authGuard.service';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root'
})
export class PermissionGuardService implements CanActivate, CanMatch {
    constructor(
        private authGuard: AuthGuardService,
        private authService: AuthService,
        private router: Router
    ) { }

    canActivate(route: ActivatedRouteSnapshot): boolean {
        if (!this.authGuard.canActivate()) {
            return false;
        }

        return this.validatePermission(route.data?.['permissions'] ?? []);
    }

    canMatch(route: Route, _segments: UrlSegment[]): boolean {
        if (!this.authGuard.canMatch(route, _segments)) {
            return false;
        }

        return this.validatePermission(route.data?.['permissions'] ?? []);
    }

    private validatePermission(permissions: string[]): boolean {
        if (this.authService.hasAnyPermission(permissions)) {
            return true;
        }

        this.router.navigate(['/unauthorized']);
        return false;
    }
}
