import { HttpEvent, HttpHandlerFn, HttpRequest, HttpResponse } from '@angular/common/http';
import { mergeMap, of, throwError } from 'rxjs';

export function apiInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
  if (!req.url.includes('/api/')) {
    return next(req);
  }

  return next(req).pipe(
    mergeMap((event: HttpEvent<any>) => {
      if (event instanceof HttpResponse) {
        const body: any = event.body;
        // failure: { code: **, msg: 'failure' }
        // success: { code: 0,  msg: 'success', data: {} }
        if (body && typeof body === 'object' && 'code' in body && typeof body.code === 'number' && body.code !== 0) {
          return throwError(() => new Error(body.msg || 'API Error'));
        }
      }
      // Pass down event if everything is OK
      return of(event);
    })
  );
}
