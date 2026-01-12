import { Injectable } from '@angular/core';

/**
 * Service for converting between Eastern Arabic (Dari/Pashto/Persian) and Western Arabic numerals.
 * Eastern Arabic: ۰۱۲۳۴۵۶۷۸۹
 * Western Arabic: 0123456789
 */
@Injectable({
  providedIn: 'root'
})
export class NumeralService {
  private readonly easternToWesternMap: Map<string, string> = new Map([
    ['۰', '0'], ['۱', '1'], ['۲', '2'], ['۳', '3'], ['۴', '4'],
    ['۵', '5'], ['۶', '6'], ['۷', '7'], ['۸', '8'], ['۹', '9'],
    ['٫', '.']  // Persian decimal separator
  ]);

  private readonly westernToEasternMap: Map<string, string> = new Map([
    ['0', '۰'], ['1', '۱'], ['2', '۲'], ['3', '۳'], ['4', '۴'],
    ['5', '۵'], ['6', '۶'], ['7', '۷'], ['8', '۸'], ['9', '۹'],
    ['.', '٫']
  ]);

  private readonly easternNumerals = '۰۱۲۳۴۵۶۷۸۹';
  private readonly westernNumerals = '0123456789';

  /**
   * Converts Eastern Arabic numerals to Western Arabic numerals
   * @param input String potentially containing Eastern Arabic numerals
   * @returns String with all numerals converted to Western Arabic
   */
  toWesternArabic(input: string | number | null | undefined): string {
    if (input === null || input === undefined) return '';
    const str = String(input);
    
    let result = '';
    for (const char of str) {
      result += this.easternToWesternMap.get(char) ?? char;
    }
    return result;
  }

  /**
   * Converts Western Arabic numerals to Eastern Arabic numerals
   * @param input String containing Western Arabic numerals
   * @returns String with all numerals converted to Eastern Arabic
   */
  toEasternArabic(input: string | number | null | undefined): string {
    if (input === null || input === undefined) return '';
    const str = String(input);
    
    let result = '';
    for (const char of str) {
      result += this.westernToEasternMap.get(char) ?? char;
    }
    return result;
  }

  /**
   * Parses a numeric string that may contain Eastern or Western Arabic numerals
   * @param input String to parse
   * @returns Parsed number or NaN if invalid
   */
  parseNumber(input: string | null | undefined): number {
    if (!input) return NaN;
    const normalized = this.toWesternArabic(input);
    return parseFloat(normalized);
  }

  /**
   * Checks if a character is a valid numeral (Eastern or Western Arabic)
   * @param char Single character to check
   * @returns true if the character is a valid numeral
   */
  isNumeral(char: string): boolean {
    return this.easternNumerals.includes(char) || this.westernNumerals.includes(char);
  }

  /**
   * Checks if a character is a valid decimal separator
   * @param char Single character to check
   * @returns true if the character is a decimal separator
   */
  isDecimalSeparator(char: string): boolean {
    return char === '.' || char === '٫';
  }
}
