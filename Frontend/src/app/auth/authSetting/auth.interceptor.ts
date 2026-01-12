import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { Router } from "@angular/router";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private router: Router) {

    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // Skip interceptor logic for login requests - let the component handle errors
        const isLoginRequest = req.url.includes('/Login');
        
        if (localStorage.getItem('token') != null) {
            const clonedReq = req.clone({
                headers: req.headers.set('Authorization', 'Bearer ' + localStorage.getItem('token'))
            });
            return next.handle(clonedReq).pipe(
                tap(
                    succ => { },
                    err => {
                        // Don't redirect for login requests - let the login component handle errors
                        if (isLoginRequest) {
                            return;
                        }
                        
                        if (err.status == 401) {
                            localStorage.removeItem('token');
                            this.router.navigateByUrl('/Auth');
                        }
                        else if (err.status == 403) {
                            this.router.navigateByUrl('/forbidden');
                        }
                    }
                )
            )
        }
        else
            return next.handle(req.clone());
    }
}
