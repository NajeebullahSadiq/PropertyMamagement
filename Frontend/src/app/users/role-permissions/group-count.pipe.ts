import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'groupCount', pure: false })
export class GroupCountPipe implements PipeTransform {
  transform(keys: string[], activeMap: Record<string, boolean>): number {
    if (!activeMap) return 0;
    return keys.filter(k => activeMap[k]).length;
  }
}
