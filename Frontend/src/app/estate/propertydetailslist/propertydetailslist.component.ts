import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PropertyDetails, PropertyDetailsList } from 'src/app/models/PropertyDetail';
import { PropertyService } from 'src/app/shared/property.service';

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

  constructor(private http: HttpClient, private propertyService: PropertyService, private toastr: ToastrService, private router: Router) {}

  ngOnInit() {
    this.loadData();
    // Subscribe to property added event to reload list immediately
    this.propertyService.propertyAdded.subscribe(() => {
      this.loadData();
    });
  }

  loadData(){
    this.propertyService.getPropertyDetails().subscribe(properties => {
      this.properties = properties;
      this.filteredProperties = properties;
    });
  }

  onEdit(propertyId: number) {
    this.router.navigate(['/dashboard/estate'], { queryParams: { id: propertyId } });
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
    return properties.filter(property =>
      property.propertyTypeText.toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
      property.transactionTypeText.toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
      property.pnumber.toString().toLowerCase().includes(searchTerm.toLowerCase().toString()) ||
      (property.buyerName && property.buyerName.toString().toLowerCase().includes(searchTerm.toLowerCase())) || // add null check for buyerName
      (property.sellerName && property.sellerName.toString().toLowerCase().includes(searchTerm.toLowerCase()))
    );
  }
  onPrint(id:any):void{
    const url = this.router.createUrlTree(['print', id]).toString();
    const newWindow = window.open(url, '_blank', 'noopener,noreferrer');
    if (newWindow) {
        newWindow.opener = null;
    }
  }
}
