import { Pipe, PipeTransform } from '@angular/core';

/**
 * Pipe to extract only dynamic data values from text containing labels
 * Example: "اسم: محمد" -> "محمد"
 * Example: "ولایت: کابل" -> "کابل"
 */
@Pipe({
  name: 'dataOnly'
})
export class DataOnlyPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) {
      return '';
    }

    const str = value.toString().trim();
    
    // If the string contains a colon, extract only the part after it
    const colonIndex = str.indexOf(':');
    if (colonIndex !== -1) {
      return str.substring(colonIndex + 1).trim();
    }

    // If no colon, return the value as is (it's probably already just data)
    return str;
  }
}
