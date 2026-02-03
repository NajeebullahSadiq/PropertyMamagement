import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../shared/auth.service';
import { PropertyService } from '../shared/property.service';
import { VehicleService } from '../shared/vehicle.service';
import { VerificationService } from '../shared/verification.service';
import { environment } from 'src/environments/environment';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-printvehicledata',
  templateUrl: './printvehicledata.component.html',
  styleUrls: ['./printvehicledata.component.scss']
})
export class PrintvehicledataComponent implements OnInit {
  userDetails: any = {};
  SellerfilePath: string = 'assets/img/avatar2.png';
  BuyerfilePath: string = 'assets/img/avatar2.png';
  baseUrl = environment.apiURL + '/';
  documentData: any = {};
  isLoading: boolean = true;
  loadError: boolean = false;
  errorMessage: string = '';
  
  // Verification properties
  verificationCode: string = '';
  verificationUrl: string = '';
  qrCodeUrl: string = '';
  verificationError: string | null = null;

  constructor(
    public service: AuthService,
    private route: ActivatedRoute,
    private pservice: VehicleService,
    private verificationService: VerificationService
  ) { 
  
  }

  ngOnInit(): void {
    this.loadPrintData();
  }

  private loadPrintData(): void {
    const code = this.route.snapshot.paramMap.get('id');
    
    if (!code) {
      this.handleError('No vehicle ID provided');
      return;
    }

    const vehicleData$ = this.pservice.getVehiclePropertyPrintData(code);
    vehicleData$.subscribe({
      next: (vehicle) => {
        this.documentData = vehicle || {};

        if (this.documentData.sellerPhoto) {
          this.SellerfilePath = this.constructImageUrl(this.documentData.sellerPhoto);
        }
        
        if (this.documentData.buyerPhoto) {
          this.BuyerfilePath = this.constructImageUrl(this.documentData.buyerPhoto);
        }

        this.service.getCurrentUserProfile().pipe(
          catchError((err) => {
            console.warn('User profile failed to load (print will continue):', err);
            return of({});
          })
        ).subscribe((user) => {
          this.userDetails = user || {};
        });

        // Fetch verification code after loading vehicle data
        this.fetchVerificationCode();
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
        this.handleError('Failed to load vehicle data. Please try again.');
      }
    });
  }

  private waitForImagesToLoad(): void {
    const imagesToLoad: Array<{ img: HTMLImageElement; path: string; name: string }> = [];

    const sellerImg = new Image();
    imagesToLoad.push({ img: sellerImg, path: this.SellerfilePath, name: 'Seller' });

    const buyerImg = new Image();
    imagesToLoad.push({ img: buyerImg, path: this.BuyerfilePath, name: 'Buyer' });

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

  private constructImageUrl(path: string): string {
    if (!path) return 'assets/img/avatar2.png';
    
    // If path already starts with http/https or is a blob URL, return as is
    if (path.startsWith('http://') || path.startsWith('https://') || path.startsWith('blob:')) {
      return path;
    }
    
    // If it's an assets path, return as is
    if (path.startsWith('assets/')) {
      return path;
    }
    
    // If path starts with Resources/, it's a full path from DB - use static file serving
    if (path.startsWith('Resources/') || path.startsWith('/Resources/')) {
      const cleanPath = path.startsWith('/') ? path.substring(1) : path;
      return `${this.baseUrl}${cleanPath}`;
    }
    
    // Otherwise, use Upload/view endpoint
    return `${this.baseUrl}api/Upload/view/${path}`;
  }

  // Make constructImageUrl available in template
  public constructImageUrlPublic(path: string): string {
    return this.constructImageUrl(path);
  }

  private fetchVerificationCode(): void {
    // Get the vehicle ID from the data
    const vehicleId = this.documentData?.id || this.documentData?.Id;
    
    console.log('[PrintVehicle] Data object keys:', Object.keys(this.documentData || {}));
    console.log('[PrintVehicle] Looking for vehicle ID, found:', vehicleId);

    if (!vehicleId) {
      console.warn('[PrintVehicle] No vehicle ID found for verification. Data:', this.documentData);
      this.waitForImagesToLoad();
      return;
    }

    console.log('[PrintVehicle] Calling generateVerificationCode with ID:', vehicleId);

    this.verificationService.generateVerificationCode(vehicleId, 'VehicleDocument').subscribe({
      next: (result) => {
        console.log('[PrintVehicle] Verification result:', result);
        this.verificationCode = result.verificationCode;
        this.verificationUrl = result.verificationUrl;
        this.qrCodeUrl = this.verificationService.generateQrCodeUrl(result.verificationUrl);
        console.log('[PrintVehicle] QR Code URL:', this.qrCodeUrl);
        this.waitForImagesToLoad();
      },
      error: (err) => {
        console.error('[PrintVehicle] Error fetching verification code:', err);
        console.error('[PrintVehicle] Error details:', err?.error || err?.message);
        this.verificationError = 'خطا در دریافت کود تصدیق';
        this.waitForImagesToLoad();
      }
    });
  }
}
