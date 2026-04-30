import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from './auth.service';

@Directive({
    selector: '[appHasPermission]'
})
export class HasPermissionDirective {
    private permission = '';
    private hasView = false;

    constructor(
        private templateRef: TemplateRef<unknown>,
        private viewContainer: ViewContainerRef,
        private authService: AuthService
    ) { }

    @Input()
    set appHasPermission(permission: string) {
        this.permission = permission;
        this.updateView();
    }

    private updateView(): void {
        const allowed = this.authService.hasPermission(this.permission);
        if (allowed && !this.hasView) {
            this.viewContainer.createEmbeddedView(this.templateRef);
            this.hasView = true;
        } else if (!allowed && this.hasView) {
            this.viewContainer.clear();
            this.hasView = false;
        }
    }
}
