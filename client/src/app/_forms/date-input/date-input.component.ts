import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {
  colorTheme = 'theme-green';
  @Input() label: string;
  @Input() maxDate: Date;
  bsConfig: Partial<BsDatepickerConfig>;
  constructor(@Self() public ngControl: NgControl) { 
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: this.colorTheme , 
      dateInputFormat: "DD MMMM YYYY"
    }
  }

  writeValue(obj: any): void {
   // throw new Error('Method not implemented.');
  }
  registerOnChange(fn: any): void {
   // throw new Error('Method not implemented.');
  }
  registerOnTouched(fn: any): void {
   // throw new Error('Method not implemented.');
  }
  // setDisabledState?(isDisabled: boolean): void {
  //   throw new Error('Method not implemented.');
  // }

  ngOnInit(): void {
  }

}
