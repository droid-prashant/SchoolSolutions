import { HttpInterceptorFn } from '@angular/common/http';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const token = window.localStorage.getItem('token');
  if (token) {
    const reqWithToken = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(reqWithToken);
  }
  else {
    return next(req);
  }
};
