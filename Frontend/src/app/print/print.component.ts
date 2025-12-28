import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../shared/auth.service';
import { PropertyService } from '../shared/property.service';
import { LocalizationService } from '../shared/localization.service';
import { environment } from 'src/environments/environment';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-print',
  templateUrl: './print.component.html',
  styleUrls: ['./print.component.scss']
})
export class PrintComponent implements OnInit {
  userDetails: any = {};
  SellerfilePath: string = 'assets/img/avatar2.png';
  BuyerfilePath: string = 'assets/img/avatar2.png';
  baseUrl = environment.apiURL + '/';
  documentData: any = {};
  isLoading: boolean = true;
  loadError: boolean = false;
  errorMessage: string = '';
  propertyImagePath: string = '';
  previousDocsPath: string = '';
  existingDocsPath: string = '';

  constructor(
    public service: AuthService,
    private route: ActivatedRoute,
    private pservice: PropertyService,
    private localizationService: LocalizationService
  ) { }

  ngOnInit(): void {
    this.loadPrintData();
  }

  private loadPrintData(): void {
    const code = this.route.snapshot.paramMap.get('id');
    
    if (!code) {
      this.handleError('No property ID provided');
      return;
    }

    const propertyData$ = this.pservice.getPropertyPrintData(code);
    propertyData$.subscribe({
      next: (property) => {
        this.documentData = property || {};

        if (this.documentData.sellerPhoto) {
          this.SellerfilePath = this.baseUrl + this.documentData.sellerPhoto;
        }
        
        if (this.documentData.buyerPhoto) {
          this.BuyerfilePath = this.baseUrl + this.documentData.buyerPhoto;
        }

        if (this.documentData.filePath) {
          this.propertyImagePath = this.baseUrl + this.documentData.filePath;
        }

        if (this.documentData.previousDocumentsPath) {
          this.previousDocsPath = this.baseUrl + this.documentData.previousDocumentsPath;
        }

        if (this.documentData.existingDocumentsPath) {
          this.existingDocsPath = this.baseUrl + this.documentData.existingDocumentsPath;
        }

        if (this.documentData.propertypeType) {
          this.documentData.propertypeType = this.getDariPropertyTypeLabel(this.documentData.propertypeType);
        }

        this.service.getCurrentUserProfile().pipe(
          catchError((err) => {
            console.warn('User profile failed to load (print will continue):', err);
            return of({});
          })
        ).subscribe((user) => {
          this.userDetails = user || {};
        });

        this.waitForImagesToLoad();
      },
      error: (err) => {
        console.error('Error loading print data:', err);
        const status = err?.status;
        if (status === 0) {
          this.handleError('ارتباط با سرور برقرار نشد. لطفاً سرور را بررسی کنید و دوباره تلاش نمایید.');
          return;
        }
        if (status === 404) {
          this.handleError('اطلاعات چاپ پیدا نشد. ممکن است این معامله هنوز تکمیل نشده باشد.');
          return;
        }
        if (status === 401) {
          this.handleError('جلسه شما ختم شده است. لطفاً دوباره وارد شوید.');
          return;
        }
        if (status === 403) {
          this.handleError('شما اجازه دسترسی به این صفحه را ندارید.');
          return;
        }
        if (status === 500) {
          const serverMessage = err?.error?.message || err?.error?.title;
          const serverHint = err?.error?.hint;
          this.handleError(serverHint || serverMessage || 'خطای داخلی سرور رخ داده است. لطفاً دوباره تلاش کنید.');
          return;
        }
        this.handleError('Failed to load property data. Please try again.');
      }
    });
  }

  private waitForImagesToLoad(): void {
    const imagesToLoad: Array<{ img: HTMLImageElement; path: string; name: string }> = [];

    const sellerImg = new Image();
    imagesToLoad.push({ img: sellerImg, path: this.SellerfilePath, name: 'Seller' });

    const buyerImg = new Image();
    imagesToLoad.push({ img: buyerImg, path: this.BuyerfilePath, name: 'Buyer' });

    if (this.propertyImagePath) {
      const propImg = new Image();
      imagesToLoad.push({ img: propImg, path: this.propertyImagePath, name: 'Property' });
    }

    if (this.previousDocsPath) {
      const prevImg = new Image();
      imagesToLoad.push({ img: prevImg, path: this.previousDocsPath, name: 'PreviousDocs' });
    }

    if (this.existingDocsPath) {
      const existImg = new Image();
      imagesToLoad.push({ img: existImg, path: this.existingDocsPath, name: 'ExistingDocs' });
    }

    let loadedCount = 0;
    const totalImages = imagesToLoad.length;

    const checkAllLoaded = () => {
      loadedCount++;
      if (loadedCount >= totalImages) {
        this.isLoading = false;
        setTimeout(() => {
          window.print();
        }, 500);
      }
    };

    imagesToLoad.forEach(({ img, path, name }) => {
      img.onload = () => {
        console.log(`${name} image loaded successfully`);
        checkAllLoaded();
      };
      img.onerror = () => {
        console.warn(`${name} image failed to load`);
        if (name === 'Seller') {
          this.SellerfilePath = 'assets/img/avatar2.png';
        } else if (name === 'Buyer') {
          this.BuyerfilePath = 'assets/img/avatar2.png';
        }
        checkAllLoaded();
      };
      img.src = path;
    });
  }

  private handleError(message: string): void {
    this.isLoading = false;
    this.loadError = true;
    this.errorMessage = message;
  }

  private getDariPropertyTypeLabel(propertyTypeValue: any): string {
    const value = (propertyTypeValue ?? '').toString();
    const match = this.localizationService.propertyTypes.find(pt => pt.value === value);
    return match?.label || 'سایر';
  }

}
