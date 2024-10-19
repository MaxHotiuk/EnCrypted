import { TestBed } from '@angular/core/testing';
import { HttpRequest, HttpHandler, HttpEvent, HttpResponse } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';
import { Observable, of } from 'rxjs';

describe('AuthInterceptor', () => {
  let interceptor: AuthInterceptor;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthInterceptor]
    });

    interceptor = TestBed.inject(AuthInterceptor);
  });

  it('should be created', () => {
    expect(interceptor).toBeTruthy();
  });

  it('should add an Authorization header', () => {
    const mockRequest = new HttpRequest('GET', '/test');
    const mockHandler: HttpHandler = {
      handle: (req: HttpRequest<any>): Observable<HttpEvent<any>> => {
        return of(new HttpResponse({ body: req.body, headers: req.headers, status: 200, statusText: 'OK', url: req.url }));
      }
    };

    interceptor.intercept(mockRequest, mockHandler).subscribe((response) => {
      const httpResponse = response as HttpResponse<any>;
      expect(httpResponse.headers.has('Authorization')).toBeTrue();
      // Check if the Authorization header has the expected token or value
      expect(httpResponse.headers.get('Authorization')).toBe('Bearer some-token');
    });
  });
});
