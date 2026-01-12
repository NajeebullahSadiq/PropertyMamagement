import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { companydetailsList } from 'src/app/models/companydetails';
import { CompnaydetailService } from 'src/app/shared/compnaydetail.service';
import { RbacService } from 'src/app/shared/rbac.service';

@Component({
  selector: 'app-realestatelist',
  templateUrl: './realestatelist.component.html',
  styleUrls: ['./realestatelist.component.scss']
})
export class RealestatelistComponent {
  properties!: companydetailsList[];
  filteredProperties!: companydetailsList[];
  searchTerm: string = '';
  page: number = 1;
  count: number = 0;
  tableSize: number = 10;
  tableSizes: any = [10,50,100];
  isViewOnly: boolean = false;

  constructor(
    private http: HttpClient, 
    private comservice: CompnaydetailService, 
    private toastr: ToastrService, 
    private router: Router,
    private rbacService: RbacService
  ) {
    this.isViewOnly = this.rbacService.isViewOnly();
  }

  ngOnInit() {
    this.loadData();
   
  }

  loadData(){
    this.comservice.getComapaniesList().subscribe(properties => {
      this.properties = properties;
      this.filteredProperties = properties;
    });
  }

  onEdit(propertyId: number) {
    this.router.navigate(['/realestate'], { queryParams: { id: propertyId } });
  }

  onView(propertyId: number) {
    this.router.navigate(['/realestate/view', propertyId]);
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

  filterProperties(properties: companydetailsList[], searchTerm: string): companydetailsList[] {
    return properties.filter(property =>
      property.title.toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
      property.licenseNumber.toString().toLowerCase().includes(searchTerm.toLowerCase()) ||
      property.ownerFullName.toString().toLowerCase().includes(searchTerm.toLowerCase().toString()) 
    );
  }
}
