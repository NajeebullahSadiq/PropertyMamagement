import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'roleLabel' })
export class RoleLabelPipe implements PipeTransform {
  transform(roles: any[], roleId: string): string {
    const found = roles?.find(r => r.id === roleId);
    return found?.dari || roleId;
  }
}
