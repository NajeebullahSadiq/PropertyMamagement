import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { PropertyDetails, PropertyDetailsList } from 'src/app/models/PropertyDetail';
import { PropertyService } from 'src/app/shared/property.service';
import { LocalizationService } from 'src/app/shared/localization.service';
import { RbacService } from 'src/app/shared/rbac.service';
import { DeleteConfirmationDialogComponent } from 'src/app/shared/delete-confirmation-dialog/delete-confirmation-dialog.component';

@Component({
  selector: 'app-propertydetailslist',
  templateUrl: './propertydetailslist.component.html',
  styleUrls: ['./propertydetailslist.component.scss']
})

export class PropertydetailslistComponent {
  properties!: PropertyDetailsList[];
  filteredProperties!: PropertyDetailsList[];
  searchTerm: string = '';
  page: number = 1;
  count: number = 0;
  tableSize: number = 10;
  tableSizes: any = [10,50,100];
  
  // RBAC flags
  isViewOnly = false;
  canCreate = false;
  canEdit = false;
  isAdmin = false;
  currentUserId = '';

  constructor(
    private http: HttpClient,
    private propertyService: PropertyService,
    private toastr: ToastrService,
    private router: Router,
    private localizationService: LocalizationService,
    private rbacService: RbacService,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    // Load RBAC permissions
    this.isViewOnly = this.rbacService.isViewOnly();
    this.canCreate = this.rbacService.canCreateProperty();
    this.canEdit = this.rbacService.canCreateProperty(); // If can create, can also edit
    this.isAdmin = this.rbacService.isAdmin();
    this.currentUserId = this.rbacService.getCurrentUserId();
    
    this.loadData();
    // Subscribe to property added event to reload list immediately
    this.propertyService.propertyAdded.subscribe(() => {
      this.loadData();
    });
  }

  loadData(){
    this.propertyService.getPropertyDetails().subscribe(properties => {
      // Ensure no English is visible in UI for property type.
      const mapped = (properties || []).map(p => ({
        ...p,
        propertyTypeText: this.getDariPropertyTypeLabel(p.propertyTypeText)
      }));
      this.properties = mapped;
      this.filteredProperties = this.filterProperties(mapped, this.searchTerm);
      this.count = this.filteredProperties.length;
    });
  }

  private getDariPropertyTypeLabel(propertyTypeValue: any): string {
    const value = (propertyTypeValue ?? '').toString();
    const match = this.localizationService.propertyTypes.find(pt => pt.value === value);
    return match?.label || 'سایر';
  }

  canEditRecord(createdBy: string): boolean {
    return this.rbacService.canEditRecord(createdBy);
  }

  onEdit(propertyId: number) {
    this.router.navigate(['/estate'], { queryParams: { id: propertyId } });
  }

  onView(propertyId: number) {
    this.router.navigate(['/estate/view', propertyId]);
  }

  onTableDataChange(event: any) {
    this.page = event;
    this.loadData();
  }

  onTableSizeChange(event: any): void {
    this.tableSize = event.target.value;
    this.page = 1;
    this.loadData();
  }

  onSearch() {
    this.filteredProperties = this.filterProperties(this.properties, this.searchTerm);
    this.count = this.filteredProperties.length;
    this.page = 1;
  }

  filterProperties(properties: PropertyDetailsList[], searchTerm: string): PropertyDetailsList[] {
    const term = (searchTerm ?? '').toString().trim().toLowerCase();
    if (!term) {
      return properties;
    }

    const toText = (value: unknown) => (value ?? '').toString().toLowerCase();

    return (properties || []).filter(property => {
      const idMatch = toText(property.id).includes(term);
      const refMatch = toText(property.pnumber).includes(term);
      const propertyTypeMatch = toText(property.propertyTypeText).includes(term);
      const transactionTypeMatch = toText(property.transactionTypeText).includes(term);
      const buyerNameMatch = toText(property.buyerName).includes(term);
      const sellerNameMatch = toText(property.sellerName).includes(term);
      const buyerNationalIdMatch = toText(property.buyerElectronicNationalIdNumber).includes(term);
      const sellerNationalIdMatch = toText(property.sellerElectronicNationalIdNumber).includes(term);

      return (
        idMatch ||
        refMatch ||
        propertyTypeMatch ||
        transactionTypeMatch ||
        buyerNameMatch ||
        sellerNameMatch ||
        buyerNationalIdMatch ||
        sellerNationalIdMatch
      );
    });
  }
  onPrint(id:any):void{
    const tree = this.router.createUrlTree(['/print', id]);
    const url = tree.toString();
    const absoluteUrl = `${window.location.origin}${url.startsWith('/') ? url : `/${url}`}`;
    const newWindow = window.open(absoluteUrl, '_blank', 'noopener,noreferrer');
    if (newWindow) {
      newWindow.opener = null;
    } else {
      this.router.navigateByUrl(tree);
    }
  }

  onDelete(propertyId: number, pnumber: string | number | undefined) {
    const dialogRef = this.dialog.open(DeleteConfirmationDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        title: 'تأیید حذف سند ملکیت',
        message: 'آیا مطمئن هستید که می‌خواهید این سند ملکیت را حذف کنید؟',
        itemName: pnumber?.toString() || 'سند ملکیت'
      },
      disableClose: true,
      panelClass: 'delete-dialog'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.propertyService.deleteProperty(propertyId).subscribe({
          next: (response) => {
            this.toastr.success('سند ملکیت با موفقیت حذف شد', 'موفق');
            this.loadData();
          },
          error: (error) => {
            console.error('Error deleting property:', error);
            if (error.status === 403) {
              this.toastr.error('شما اجازه حذف سند ملکیت را ندارید', 'خطا');
            } else {
              this.toastr.error('خطا در حذف سند ملکیت', 'خطا');
            }
          }
        });
      }
    });
  }
}
