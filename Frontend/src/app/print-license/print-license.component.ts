import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs';
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
  error: string | null = null;

  constructor(
    public service: AuthService,
    private route: ActivatedRoute,
    private pservice: PropertyService,
    private companyService: CompnaydetailService
  ) {}

  ngOnInit(): void {
    console.log('[PrintLicense] init');

    combineLatest([this.route.paramMap, this.route.queryParamMap]).subscribe(([paramMap, queryParamMap]) => {
      this.error = null;
      this.isLoading = true;

      const codeFromParam = paramMap.get('id');
      const codeFromQuery = queryParamMap.get('id');
      const code = codeFromParam || codeFromQuery;

      console.log('[PrintLicense] route id:', code);

      if (!code) {
        this.error = 'شناسه چاپ موجود نیست';
        this.isLoading = false;
        return;
      }

      console.log('[PrintLicense] calling API getLicensePrintData');

      this.pservice.getLicensePrintData(code).subscribe({
        next: (res) => {
          const payload = (res && (res.data || res.result || res.payload)) ? (res.data || res.result || res.payload) : res;

          this.data = this.toCamelCaseDeep(payload || {});

          if (this.data && this.data.ownerPhoto) {
            this.filePath = this.baseUrl + this.data.ownerPhoto;
          }

          // Fetch Account Info for the company
          if (this.data && this.data.companyId) {
            this.companyService.getAccountInfoByCompanyId(this.data.companyId).subscribe({
              next: (info) => {
                this.accountInfo = info;
                this.isLoading = false;
                this.triggerPrint();
              },
              error: () => {
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

          if (err?.status === 404) {
            this.error = 'رکورد برای چاپ پیدا نشد';
          } else if (err?.status === 401 || err?.status === 403) {
            this.error = 'دسترسی برای چاپ ندارید';
          } else {
            this.error = 'خطا در دریافت معلومات چاپ';
          }

          this.isLoading = false;
        }
      });
    });
  }

  private toCamelCaseDeep(value: any): any {
    if (Array.isArray(value)) {
      return value.map(v => this.toCamelCaseDeep(v));
    }

    if (value && typeof value === 'object' && value.constructor === Object) {
      const result: any = {};
      for (const [k, v] of Object.entries(value)) {
        const camelKey = k ? (k.charAt(0).toLowerCase() + k.slice(1)) : k;
        result[camelKey] = this.toCamelCaseDeep(v);
      }
      return result;
    }

    return value;
  }

  private triggerPrint(): void {
    if (this.error) {
      return;
    }

    // Wait for images to load before printing
    setTimeout(() => {
      window.print();
    }, 500);
  }

  printPage(): void {
    window.print();
  }
}
