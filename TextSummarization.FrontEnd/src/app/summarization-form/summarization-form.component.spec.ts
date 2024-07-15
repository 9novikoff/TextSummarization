import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummarizationFormComponent } from './summarization-form.component';

describe('SummarizationFormComponent', () => {
  let component: SummarizationFormComponent;
  let fixture: ComponentFixture<SummarizationFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SummarizationFormComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SummarizationFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
