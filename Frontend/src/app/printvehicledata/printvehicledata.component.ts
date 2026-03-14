import { Component, OnInit, ElementRef, Renderer2 } from '@angular/core';
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

  // Print mode: 'full' = with design, 'data-only' = only data for pre-printed forms
  printMode: 'full' | 'data-only' = 'full';
  showPrintOptions: boolean = true;

  constructor(
    public service: AuthService,
    private route: ActivatedRoute,
    private pservice: VehicleService,
    private verificationService: VerificationService,
    private elementRef: ElementRef,
    private renderer: Renderer2
  ) { 
  
  }

  ngOnInit(): void {
    // Check if print mode is specified in URL
    const mode = this.route.snapshot.queryParamMap.get('mode');
    if (mode === 'data-only' || mode === 'full') {
      this.printMode = mode as 'full' | 'data-only';
      this.showPrintOptions = false;
    }
    console.log('Vehicle print component initialized. showPrintOptions:', this.showPrintOptions, 'mode:', mode);
    this.loadPrintData();
  }

  setPrintMode(mode: 'full' | 'data-only'): void {
    this.printMode = mode;
    this.showPrintOptions = false;
    
    // If switching to data-only mode, process the DOM to remove labels
    if (mode === 'data-only') {
      setTimeout(() => {
        this.processDataOnlyMode();
        window.print();
      }, 100);
    } else {
      setTimeout(() => {
        window.print();
      }, 100);
    }
  }

  /**
   * Process the DOM to remove all label text and keep only dynamic data values
   */
  private processDataOnlyMode(): void {
    const container = this.elementRef.nativeElement.querySelector('.container');
    if (!container) return;

    // Remove the second table (rights and obligations + signatures) completely
    const tables = container.querySelectorAll('table');
    if (tables.length > 1) {
      // Remove all tables after the first one
      for (let i = 1; i < tables.length; i++) {
        tables[i].remove();
      }
    }

    // Remove all existing content and create positioned layout
    const firstTable = tables[0];
    if (firstTable) {
      // Clear the container and create absolute positioned layout
      container.innerHTML = '';
      
      // Create a positioned wrapper for exact placement
      const wrapper = document.createElement('div');
      wrapper.style.position = 'relative';
      wrapper.style.width = '210mm';
      wrapper.style.height = '297mm';
      wrapper.style.margin = '0';
      wrapper.style.padding = '0';
      
      // Helper function to create positioned text
      const createField = (text: string, top: string, left: string, fontSize: string = '12pt') => {
        const field = document.createElement('div');
        field.style.position = 'absolute';
        field.style.top = top;
        field.style.left = left;
        field.style.fontSize = fontSize;
        field.style.fontFamily = 'B Nazanin, Arial';
        field.style.whiteSpace = 'nowrap';
        field.textContent = text || '';
        return field;
      };

      // Map data to exact positions based on the pre-printed form
      // These positions need to be adjusted to match your exact form
      
      // Company name and phone (top section)
      if (this.userDetails.companyName) {
        wrapper.appendChild(createField(this.userDetails.companyName, '25mm', '80mm', '14pt'));
      }
      if (this.userDetails.phoneNumber) {
        wrapper.appendChild(createField(this.userDetails.phoneNumber, '32mm', '80mm', '12pt'));
      }
      
      // Document number
      if (this.documentData.permitNo) {
        wrapper.appendChild(createField(this.documentData.permitNo, '50mm', '120mm'));
      }
      
      // Seller information (left column)
      if (this.documentData.sellerFirstName) {
        wrapper.appendChild(createField(this.documentData.sellerFirstName, '65mm', '140mm'));
      }
      if (this.documentData.sellerFatherName) {
        wrapper.appendChild(createField(this.documentData.sellerFatherName, '75mm', '140mm'));
      }
      if (this.documentData.sellerIndentityCardNumber) {
        wrapper.appendChild(createField(this.documentData.sellerIndentityCardNumber, '85mm', '140mm'));
      }
      
      // Vehicle information (center column)
      if (this.documentData.pilateNo) {
        wrapper.appendChild(createField(this.documentData.pilateNo, '60mm', '80mm'));
      }
      if (this.documentData.typeOfVehicle) {
        wrapper.appendChild(createField(this.documentData.typeOfVehicle, '68mm', '80mm'));
      }
      if (this.documentData.model) {
        wrapper.appendChild(createField(this.documentData.model, '76mm', '80mm'));
      }
      if (this.documentData.enginNo) {
        wrapper.appendChild(createField(this.documentData.enginNo, '84mm', '80mm'));
      }
      if (this.documentData.shasiNo) {
        wrapper.appendChild(createField(this.documentData.shasiNo, '92mm', '80mm'));
      }
      if (this.documentData.color) {
        wrapper.appendChild(createField(this.documentData.color, '100mm', '80mm'));
      }
      if (this.documentData.price) {
        wrapper.appendChild(createField(this.documentData.price, '108mm', '80mm'));
      }
      if (this.documentData.priceText) {
        wrapper.appendChild(createField(this.documentData.priceText, '116mm', '80mm'));
      }
      
      // Buyer information (right column)
      if (this.documentData.buyerFirstName) {
        wrapper.appendChild(createField(this.documentData.buyerFirstName, '65mm', '20mm'));
      }
      if (this.documentData.buyerFatherName) {
        wrapper.appendChild(createField(this.documentData.buyerFatherName, '75mm', '20mm'));
      }
      if (this.documentData.buyerIndentityCardNumber) {
        wrapper.appendChild(createField(this.documentData.buyerIndentityCardNumber, '85mm', '20mm'));
      }
      
      // Seller address
      if (this.documentData.sellerProvince) {
        wrapper.appendChild(createField(this.documentData.sellerProvince, '135mm', '140mm', '11pt'));
      }
      if (this.documentData.sellerDistrict) {
        wrapper.appendChild(createField(this.documentData.sellerDistrict, '142mm', '140mm', '11pt'));
      }
      if (this.documentData.sellerVillage) {
        wrapper.appendChild(createField(this.documentData.sellerVillage, '149mm', '140mm', '11pt'));
      }
      
      // Buyer address
      if (this.documentData.buyerProvince) {
        wrapper.appendChild(createField(this.documentData.buyerProvince, '135mm', '20mm', '11pt'));
      }
      if (this.documentData.buyerDistrict) {
        wrapper.appendChild(createField(this.documentData.buyerDistrict, '142mm', '20mm', '11pt'));
      }
      if (this.documentData.buyerVillage) {
        wrapper.appendChild(createField(this.documentData.buyerVillage, '149mm', '20mm', '11pt'));
      }
      
      // Temporary address (سکونت فعلی)
      if (this.documentData.tSellerProvince) {
        wrapper.appendChild(createField(this.documentData.tSellerProvince, '170mm', '140mm', '11pt'));
      }
      if (this.documentData.tSellerDistrict) {
        wrapper.appendChild(createField(this.documentData.tSellerDistrict, '177mm', '140mm', '11pt'));
      }
      if (this.documentData.tSellerVillage) {
        wrapper.appendChild(createField(this.documentData.tSellerVillage, '184mm', '140mm', '11pt'));
      }
      
      if (this.documentData.tBuyerProvince) {
        wrapper.appendChild(createField(this.documentData.tBuyerProvince, '170mm', '20mm', '11pt'));
      }
      if (this.documentData.tBuyerDistrict) {
        wrapper.appendChild(createField(this.documentData.tBuyerDistrict, '177mm', '20mm', '11pt'));
      }
      if (this.documentData.tBuyerVillage) {
        wrapper.appendChild(createField(this.documentData.tBuyerVillage, '184mm', '20mm', '11pt'));
      }
      
      // Witnesses
      if (this.documentData.witnessOneFirstName) {
        wrapper.appendChild(createField(this.documentData.witnessOneFirstName, '195mm', '90mm', '11pt'));
      }
      if (this.documentData.witnessOneFatherName) {
        wrapper.appendChild(createField(this.documentData.witnessOneFatherName, '195mm', '70mm', '11pt'));
      }
      if (this.documentData.witnessOneIndentityCardNumber) {
        wrapper.appendChild(createField(this.documentData.witnessOneIndentityCardNumber, '195mm', '50mm', '11pt'));
      }
      
      if (this.documentData.witnessTwoFirstName) {
        wrapper.appendChild(createField(this.documentData.witnessTwoFirstName, '205mm', '90mm', '11pt'));
      }
      if (this.documentData.witnessTwoFatherName) {
        wrapper.appendChild(createField(this.documentData.witnessTwoFatherName, '205mm', '70mm', '11pt'));
      }
      if (this.documentData.witnessTwoIndentityCardNumber) {
        wrapper.appendChild(createField(this.documentData.witnessTwoIndentityCardNumber, '205mm', '50mm', '11pt'));
      }
      
      container.appendChild(wrapper);
    }
  }

  cancelPrint(): void {
    window.close();
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
        console.log('Vehicle images loaded. showPrintOptions:', this.showPrintOptions, 'isLoading:', this.isLoading);
        // Don't auto-print if showing options
        if (!this.showPrintOptions) {
          setTimeout(() => {
            window.print();
          }, 500);
        }
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
        
        // Validate the result
        if (!result.verificationCode) {
          console.error('[PrintVehicle] No verification code in result');
          this.verificationError = 'خطا در دریافت کود تصدیق';
          this.waitForImagesToLoad();
          return;
        }
        
        if (!result.verificationUrl) {
          console.error('[PrintVehicle] No verification URL in result');
          this.verificationError = 'خطا در دریافت لینک تصدیق';
          this.waitForImagesToLoad();
          return;
        }
        
        // Ensure the verification URL contains the code
        if (!result.verificationUrl.includes(result.verificationCode)) {
          console.warn('[PrintVehicle] Verification URL does not contain code, fixing...');
          result.verificationUrl = `${result.verificationUrl}/${result.verificationCode}`;
        }
        
        this.verificationCode = result.verificationCode;
        this.verificationUrl = result.verificationUrl;
        this.qrCodeUrl = this.verificationService.generateQrCodeUrl(result.verificationUrl);
        
        console.log('[PrintVehicle] Verification Code:', this.verificationCode);
        console.log('[PrintVehicle] Verification URL:', this.verificationUrl);
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
