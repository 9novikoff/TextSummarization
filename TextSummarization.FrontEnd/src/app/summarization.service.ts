import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpClientModule } from '@angular/common/http';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SummarizationService {
  private apiUrl = 'https://localhost:7228/summary/';

  constructor(private http: HttpClient) { }

  summarize(text: string, method: string, capacity: number): Observable<string> {
    const body = { text, capacity };
    return this.http.post(this.apiUrl+method.toLowerCase(), body, {responseType: 'text'});
  }
}