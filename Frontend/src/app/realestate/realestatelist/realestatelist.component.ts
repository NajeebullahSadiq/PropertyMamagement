import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { companydetailsList } from 'src/app/models/companydetails';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { RbacService } from 'src/app/shared/rbac.service';
import { DeleteConfirmationDialogComponent } from 'src/app/shared/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-realestatelist',
  templateUrl: './realestatelist.component.html',
  styleUrls: ['./realestatelist.component.scss']
})
export class RealestatelistComponent implements OnInit, OnDestroy {
  properties!: companydetailsList[];
  filteredProperties!: companydetailsList[];
  searchTerm: string = '';
  page: number = 1;
  count: number = 0;
  tableSize: number = 10;
  tableSizes: any = [10,50,100];
  isViewOnly: boolean = false;
  isAdmin: boolean = false;
  private searchSubject = new Subject<string>();

  constructor(
    private http: HttpClient, 
    private comservice: CompnaydetailService, 
    private toastr: ToastrService, 
    private router: Router,
    private rbacService: RbacService,
    private dialog: MatDialog
  ) {
    this.isViewOnly = this.rbacService.isViewOnly();
    this.isAdmin = this.rbacService.isAdmin();
    
    // Setup debounced search
    this.searchSubject.pipe(
      debounceTime(500), // Wait 500ms after user stops typing
      distinctUntilChanged() // Only emit if value is different from previous
    ).subscribe(searchTerm => {
      this.searchTerm = searchTerm;
      this.page = 1; // Reset page when search changes
      this.loadData();
    });
  }

  ngOnInit() {
    this.loadData();
  }

  ngOnDestroy() {
    this.searchSubject.complete();
  }

  loadData(){
    this.comservice.getComapaniesList(this.searchTerm).subscribe(properties => {
      this.properties = properties;
      this.filteredProperties = properties;
      this.count = properties.length;
    });
  }

  onEdit(propertyId: number) {
    this.router.navigate(['/realestate'], { queryParams: { id: propertyId } });
  }

  onView(propertyId: number) {
    this.router.navigate(['/realestate/view', propertyId]);
  }

  onPrint(propertyId: number) {
    const url = this.router.createUrlTree(['/printLicense', propertyId]).toString();
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  onTableDataChange(event: any) {
    this.page = event;
  }

  onTableSizeChange(event: any): void {
    this.tableSize = event.target.value;
    this.page = 1;
  }

  onSearch() {
    this.searchSubject.next(this.searchTerm);
  }

  onDelete(propertyId: number, title: string) {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        title: 'تأیید حذف دفتر',
        message: 'آیا مطمئن هستید که می‌خواهید این دفتر را حذف کنید؟',
        itemName: title
      },
      disableClose: true,
      panelClass: 'delete-dialog'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.comservice.deleteCompany(propertyId).subscribe({
          next: (response) => {
            this.toastr.success('دفتر با موفقیت حذف شد', 'موفق');
            this.loadData();
          },
          error: (error) => {
            console.error('Error deleting company:', error);
            if (error.status === 403) {
              this.toastr.error('شما اجازه حذف دفتر را ندارید', 'خطا');
            } else {
              this.toastr.error('خطا در حذف دفتر', 'خطا');
            }
          }
        });
      }
    });
  }
}
