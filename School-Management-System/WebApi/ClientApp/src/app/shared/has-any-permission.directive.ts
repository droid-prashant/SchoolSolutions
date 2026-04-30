import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from './auth.service';

@Directive({
    selector: '[appHasAnyPermission]'
})
export class HasAnyPermissionDirective {
    private permissions: string[] = [];
    private hasView = false;

    constructor(
        private templateRef: TemplateRef<unknown>,
        private viewContainer: ViewContainerRef,
        private authService: AuthService
    ) { }

    @Input()
    set appHasAnyPermission(permissions: string[]) {
        this.permissions = permissions ?? [];
        this.updateView();
    }

    private updateView(): void {
        const allowed = this.authService.hasAnyPermission(this.permissions);
        if (allowed && !this.hasView) {
            this.viewContainer.createEmbeddedView(this.templateRef);
            this.hasView = true;
        } else if (!allowed && this.hasView) {
            this.viewContainer.clear();
            this.hasView = false;
        }
    }
}
