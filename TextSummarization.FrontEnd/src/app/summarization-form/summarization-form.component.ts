import { Component } from '@angular/core';
import { SummarizationService } from '../summarization.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-summarization-form',
  standalone: true,
  templateUrl: './summarization-form.component.html',
  styleUrls: ['./summarization-form.component.css'],
  imports: [FormsModule, CommonModule]
})
export class SummarizationFormComponent {
  text: string = '';
  method: string = 'TextRank';
  capacity: number = 0.5;
  summary: string = '';
  errorMessage: string = '';

  methods = ['TextRank', 'LexRank', 'Luhn', 'LSA'];

  constructor(private summarizationService: SummarizationService) {}

  onSubmit() {
    if (this.capacity < 0 || this.capacity > 1) {
      alert('Capacity must be between 0 and 1');
      this.capacity = 0;
      return;
    }
    if(this.text == ""){
      alert('Empty text cant be summarized');
    }
    this.errorMessage = '';
    this.summarizationService.summarize(this.text, this.method, this.capacity).subscribe(
      (response: string) => this.summary = response
    );
  }
}
