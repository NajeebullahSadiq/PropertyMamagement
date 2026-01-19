import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs';
import { PetitionWriterLicenseService } from '../shared/petition-writer-license.service';
import { CalendarService } from '../shared/calendar.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-print-petition-writer-license',
  templateUrl: './print-petition-writer-license.component.html',
  styleUrls: ['./print-petition-writer-license.component.scss']
})
export class PrintPetitionWriterLicenseComponent implements OnInit {
  baseUrl = environment.apiURL + '/';
  data: any = {};
  isLoading: boolean = true;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private licenseService: PetitionWriterLicenseService,
    private calendarService: CalendarService
  ) {}

  ngOnInit(): void {
    console.log('[PrintPetitionWriterLicense] init');

    combineLatest([this.route.paramMap, this.route.queryParamMap]).subscribe(([paramMap, queryParamMap]) => {
      this.error = null;
      this.isLoading = true;

      const idFromParam = paramMap.get('id');
      const idFromQuery = queryParamMap.get('id');
      const id = idFromParam || idFromQuery;

      console.log('[PrintPetitionWriterLicense] route id:', id);

      if (!id) {
        this.error = 'شناسه چاپ موجود نیست';
        this.isLoading = false;
        return;
      }

      console.log('[PrintPetitionWriterLicense] calling API getById');
      const calendar = this.calendarService.getSelectedCalendar();

      this.licenseService.getById(+id, calendar).subscribe({
        next: (res) => {
          this.data = res || {};
          this.isLoading = false;
          this.triggerPrint();
        },
        error: (err) => {
          console.error('Error loading petition writer license data:', err);

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

  private triggerPrint(): void {
    if (this.error) {
      return;
    }

    // Wait for images to load before printing
    setTimeout(() => {
      // Store original title and set empty to hide from print header
      const originalTitle = document.title;
      document.title = ' ';
      
      window.print();
      
      // Restore original title after print dialog
      setTimeout(() => {
        document.title = originalTitle;
      }, 100);
    }, 500);
  }

  printPage(): void {
    window.print();
  }
}
