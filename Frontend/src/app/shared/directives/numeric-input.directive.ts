import { Directive, HostListener, Input, ElementRef } from '@angular/core';

/**
 * Directive that allows only numeric input (both Eastern and Western Arabic numerals).
 * Eastern Arabic: ۰۱۲۳۴۵۶۷۸۹
 * Western Arabic: 0123456789
 * 
 * Usage:
 * <input type="text" appNumericInput>
 * <input type="text" appNumericInput [allowDecimal]="true">
 * <input type="text" appNumericInput [allowNegative]="true">
 */
@Directive({
  selector: '[appNumericInput]'
})
export class NumericInputDirective {
  @Input() allowDecimal: boolean = false;
  @Input() allowNegative: boolean = false;

  // Eastern Arabic numerals: ۰۱۲۳۴۵۶۷۸۹
  private readonly easternArabicNumerals = '۰۱۲۳۴۵۶۷۸۹';
  // Western Arabic numerals: 0123456789
  private readonly westernArabicNumerals = '0123456789';
  // Decimal separators (Western and Persian)
  private readonly decimalPoints = '.٫';
  private readonly negativeSign = '-';

  constructor(private el: ElementRef<HTMLInputElement>) {}

  @HostListener('keypress', ['$event'])
  onKeyPress(event: KeyboardEvent): void {
    const char = event.key;
    
    // Allow control keys (Backspace, Delete, Arrow keys, Tab, Enter, etc.)
    if (this.isControlKey(event)) {
      return;
    }
    
    if (!this.isValidChar(char)) {
      event.preventDefault();
    }
  }

  @HostListener('paste', ['$event'])
  onPaste(event: ClipboardEvent): void {
    const pastedText = event.clipboardData?.getData('text') || '';
    if (!this.isValidString(pastedText)) {
      event.preventDefault();
    }
  }

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    // Additional validation after input (handles edge cases like autocomplete)
    const input = this.el.nativeElement;
    const value = input.value;
    
    if (value && !this.isValidString(value)) {
      // Remove invalid characters
      let cleanValue = '';
      let hasDecimal = false;
      let hasNegative = false;
      
      for (let i = 0; i < value.length; i++) {
        const char = value[i];
        
        if (this.isNumeral(char)) {
          cleanValue += char;
        } else if (this.allowDecimal && this.isDecimalPoint(char) && !hasDecimal) {
          cleanValue += char;
          hasDecimal = true;
        } else if (this.allowNegative && char === this.negativeSign && i === 0 && !hasNegative) {
          cleanValue += char;
          hasNegative = true;
        }
      }
      
      input.value = cleanValue;
    }
  }

  private isControlKey(event: KeyboardEvent): boolean {
    return event.ctrlKey || event.metaKey || event.altKey ||
           ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown', 
            'Tab', 'Enter', 'Home', 'End'].includes(event.key);
  }

  private isNumeral(char: string): boolean {
    return this.easternArabicNumerals.includes(char) || 
           this.westernArabicNumerals.includes(char);
  }

  private isDecimalPoint(char: string): boolean {
    return this.decimalPoints.includes(char);
  }

  private isValidChar(char: string): boolean {
    // Allow numerals (both Eastern and Western Arabic)
    if (this.isNumeral(char)) {
      return true;
    }
    
    // Allow decimal point if enabled
    if (this.allowDecimal && this.isDecimalPoint(char)) {
      const currentValue = this.el.nativeElement.value;
      // Only allow one decimal point
      return !this.hasDecimalPoint(currentValue);
    }
    
    // Allow negative sign if enabled (only at start)
    if (this.allowNegative && char === this.negativeSign) {
      const currentValue = this.el.nativeElement.value;
      const selectionStart = this.el.nativeElement.selectionStart;
      return selectionStart === 0 && !currentValue.includes('-');
    }
    
    return false;
  }

  private hasDecimalPoint(str: string): boolean {
    for (const char of str) {
      if (this.isDecimalPoint(char)) {
        return true;
      }
    }
    return false;
  }

  private isValidString(str: string): boolean {
    let hasDecimal = false;
    let hasNegative = false;
    
    for (let i = 0; i < str.length; i++) {
      const char = str[i];
      
      if (this.isNumeral(char)) {
        continue;
      }
      
      if (this.allowDecimal && this.isDecimalPoint(char)) {
        if (hasDecimal) return false; // Multiple decimal points
        hasDecimal = true;
        continue;
      }
      
      if (this.allowNegative && char === this.negativeSign) {
        if (i !== 0 || hasNegative) return false; // Negative sign not at start or multiple
        hasNegative = true;
        continue;
      }
      
      return false; // Invalid character
    }
    
    return true;
  }
}
