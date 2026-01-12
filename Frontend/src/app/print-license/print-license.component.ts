import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../shared/auth.service';
import { PropertyService } from '../shared/property.service';
import { CompnaydetailService } from '../shared/compnaydetail.service';
import { environment } from 'src/environments/environment';
import { AccountInfo } from '../models/AccountInfo';

@Component({
  selector: 'app-print-license',
  templateUrl: './print-license.component.html',
  styleUrls: ['./print-license.component.scss']
})
export class PrintLicenseComponent implements OnInit {
  filePath: string = 'assets/img/avatar2.png';
  baseUrl = environment.apiURL + '/';
  data: any = {};
  accountInfo: AccountInfo | null = null;
  isLoading: boolean = true;

  constructor(
    public service: AuthService,
    private route: ActivatedRoute,
    private pservice: PropertyService,
    private companyService: CompnaydetailService
  ) {}

  ngOnInit(): void {
    const code = this.route.snapshot.paramMap.get('id');
    if (code) {
      this.pservice.getLicensePrintData(code).subscribe({
        next: (res) => {
          this.data = res;
          if (res.ownerPhoto) {
            this.filePath = this.baseUrl + res.ownerPhoto;
          }

          // Fetch Account Info for the company
          if (res.companyId) {
            this.companyService.getAccountInfoByCompanyId(res.companyId).subscribe({
              next: (info) => {
                this.accountInfo = info;
                this.isLoading = false;
                this.triggerPrint();
              },
              error: (err) => {
                console.log('No account info found for this company');
                this.isLoading = false;
                this.triggerPrint();
              }
            });
          } else {
            this.isLoading = false;
            this.triggerPrint();
          }
        },
        error: (err) => {
          console.error('Error loading license data:', err);
          this.isLoading = false;
        }
      });
    } else {
      this.isLoading = false;
    }
  }

  private triggerPrint(): void {
    // Wait for images to load before printing
    setTimeout(() => {
      window.print();
    }, 500);
  }

  printPage(): void {
    window.print();
  }
}
