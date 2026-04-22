import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BaseComponent } from 'src/app/shared/base-component';
import { VehiclesDetailsList } from 'src/app/models/PropertyDetail';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { RbacService } from 'src/app/shared/rbac.service';
import { NotificationService } from 'src/app/shared/notification.service';

@Component({
  selector: 'app-vehiclelist',
  templateUrl: './vehiclelist.component.html',
  styleUrls: ['./vehiclelist.component.scss']
})
export class VehiclelistComponent extends BaseComponent {
  properties!: VehiclesDetailsList[];
  filteredProperties!: VehiclesDetailsList[];
  searchTerm: string = '';
  page: number = 1;
  count: number = 0;
  tableSize: number = 20;
  tableSizes: any = [10,20,50,100];
  isLoading = false;
  errorMessage: string | null = null;
  isViewOnly: boolean = false;
  canEdit: boolean = false;
  canPrint: boolean = false;

  constructor(
    private http: HttpClient,
    private vehicleservice: VehicleService,
    private notification: NotificationService,
    private router: Router,
    private rbacService: RbacService
  ) {
    super();
    this.isViewOnly = this.rbacService.isViewOnly();
    this.canEdit = this.rbacService.hasPermission('vehicle.create') || this.rbacService.hasPermission('vehicle.edit');
    this.canPrint = this.rbacService.hasPermission('vehicle.view');
  }

  ngOnInit() {
    this.loadData();
   
  }

  loadData(){
    this.isLoading = true;
    this.errorMessage = null;
    this.vehicleservice.getPropertyDetails(this.page, this.tableSize, this.searchTerm).pipe(takeUntil(this.destroy$)).subscribe({
      next: (response) => {
        this.properties = response?.items || [];
        this.filteredProperties = this.properties;
        this.count = response?.totalCount || 0;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'خطا در بارگذاری اطلاعات وسایط نقلیه. لطفاً دوباره تلاش کنید';
        this.notification.showHttpError(err);
      }
    });
  }

  onEdit(propertyId: number) {
    this.router.navigate(['/vehicle'], { queryParams: { id: propertyId } });
  }

  onView(propertyId: number) {
    this.router.navigate(['/vehicle/view', propertyId]);
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
    this.page = 1;
    this.loadData();
  }

  filterProperties(properties: VehiclesDetailsList[], searchTerm: string): VehiclesDetailsList[] {
    return properties.filter(property =>
      (property.permitNo && property.permitNo.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (property.pilateNo && property.pilateNo.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (property.shasiNo && property.shasiNo.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (property.enginNo && property.enginNo.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (property.buyerName && property.buyerName.toString().toLowerCase().includes(searchTerm.toLowerCase())) ||
      (property.sellerName && property.sellerName.toString().toLowerCase().includes(searchTerm.toLowerCase()))
    );
  }
  onPrint(id:any):void{
    const url = this.router.createUrlTree(['/print/vehicle', id]).toString();
    const newWindow = window.open(url, '_blank', 'noopener,noreferrer');
    if (newWindow) {
        newWindow.opener = null;
    }
  }
}
