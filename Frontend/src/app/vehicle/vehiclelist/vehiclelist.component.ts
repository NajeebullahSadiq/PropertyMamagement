import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { VehiclesDetailsList } from 'src/app/models/PropertyDetail';
import { VehicleService } from 'src/app/shared/vehicle.service';
import { RbacService } from 'src/app/shared/rbac.service';

@Component({
  selector: 'app-vehiclelist',
  templateUrl: './vehiclelist.component.html',
  styleUrls: ['./vehiclelist.component.scss']
})
export class VehiclelistComponent {
  properties!: VehiclesDetailsList[];
  filteredProperties!: VehiclesDetailsList[];
  searchTerm: string = '';
  page: number = 1;
  count: number = 0;
  tableSize: number = 10;
  tableSizes: any = [10,50,100];
  isViewOnly: boolean = false;
  canEdit: boolean = false;

  constructor(
    private http: HttpClient, 
    private vehicleservice: VehicleService, 
    private toastr: ToastrService, 
    private router: Router,
    private rbacService: RbacService
  ) {
    this.isViewOnly = this.rbacService.isViewOnly();
    this.canEdit = this.rbacService.canCreateVehicle();
  }

  ngOnInit() {
    this.loadData();
   
  }

  loadData(){
    this.vehicleservice.getPropertyDetails().subscribe(properties => {
      this.properties = properties;
      this.filteredProperties = properties;
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
    this.filteredProperties = this.filterProperties(this.properties, this.searchTerm);
    this.count = this.filteredProperties.length;
    this.page = 1;
  }

  filterProperties(properties: VehiclesDetailsList[], searchTerm: string): VehiclesDetailsList[] {
    return properties.filter(property =>
      property.permitNo.toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
      property.pilateNo.toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
      property.shasiNo.toString().toLowerCase().includes(searchTerm.toLowerCase().toString()) ||
      (property.buyerName && property.buyerName.toString().toLowerCase().includes(searchTerm.toLowerCase())) || // add null check for buyerName
      (property.sellerName && property.sellerName.toString().toLowerCase().includes(searchTerm.toLowerCase()))
    );
  }
  onPrint(id:any):void{
    const url = this.router.createUrlTree(['printvehicledata', id]).toString();
    const newWindow = window.open(url, '_blank', 'noopener,noreferrer');
    if (newWindow) {
        newWindow.opener = null;
    }
  }
}
