import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

/**
 * Unwraps System.Text.Json ReferenceHandler.Preserve metadata ($id, $values)
 * from API responses so Angular components receive plain objects/arrays.
 *
 * Backend uses ReferenceHandler.Preserve which wraps arrays as { $id, $values: [...] }
 * and adds $id to objects. This interceptor strips those metadata properties
 * and replaces $values-wrapped objects with their plain arrays.
 */
@Injectable()
export class ReferenceHandlerInterceptor implements HttpInterceptor {

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Skip processing for blob responses (file downloads/views)
    if (req.responseType === 'blob') {
      return next.handle(req);
    }

    return next.handle(req).pipe(
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse && event.body) {
          // Also skip if the body is a Blob (safety check)
          if (event.body instanceof Blob) {
            return event;
          }
          return event.clone({ body: this.unwrapPreserve(event.body) });
        }
        return event;
      })
    );
  }

  /**
   * Recursively unwrap $values arrays and remove $id metadata
   */
  private unwrapPreserve(obj: any): any {
    if (obj === null || obj === undefined) {
      return obj;
    }

    if (Array.isArray(obj)) {
      return obj.map(item => this.unwrapPreserve(item));
    }

    if (typeof obj === 'object') {
      // If this object has $values, replace it with the unwrapped array
      if (obj.$values !== undefined && Array.isArray(obj.$values)) {
        return this.unwrapPreserve(obj.$values);
      }

      // Otherwise, remove $id and recursively process all properties
      const result: any = {};
      for (const key of Object.keys(obj)) {
        if (key === '$id') continue; // skip $id metadata
        result[key] = this.unwrapPreserve(obj[key]);
      }
      return result;
    }

    return obj;
  }
}
