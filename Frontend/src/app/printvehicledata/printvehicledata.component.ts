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

    // List of Dari label keywords to remove
    const labelKeywords = [
      'اسم', 'ولد', 'شماره تذکره', 'نمبر جواز سیر', 'نمبر پلیت', 'نوعیت',
      'مودل', 'نمبر انجن', 'نمبر شاسی', 'رنگ', 'قیمت به عدد', 'قیمت به حروف',
      'مناصفه قیمت', 'مبلغ حق الامتیاز', 'ولایت', 'ناحیه', 'ولسوالی', 'قریه', 'گذر',
      'سکونت اصلی', 'سکونت فعلی', 'شهرت شهود', 'شماره', 'جزئیات',
      'شهرت مکمل مالک ملکیت', 'شهرت مکمل مشتری', 'مشخصات و قیمت واسطه',
      'شصت و امضای بایع', 'شصت و امضای مشتری', 'شصت و امضای شاهد',
      'مهر و امضای دارنده جواز', 'حقوق و مکلفیت های بایع و مشتری',
      'مکلفیت های دارنده جواز رهنمای معاملات'
    ];

    // Process all text nodes
    const walker = document.createTreeWalker(
      container,
      NodeFilter.SHOW_TEXT,
      null
    );

    const nodesToProcess: { node: Node; parent: Node }[] = [];
    let node: Node | null;
    
    while (node = walker.nextNode()) {
      if (node.parentNode) {
        nodesToProcess.push({ node, parent: node.parentNode });
      }
    }

    nodesToProcess.forEach(({ node, parent }) => {
      const text = node.textContent || '';
      let processedText = text;

      // Remove label keywords
      labelKeywords.forEach(keyword => {
        const regex = new RegExp(keyword + '\\s*:?\\s*', 'g');
        processedText = processedText.replace(regex, '');
      });

      // Remove common patterns like ":" followed by spaces
      processedText = processedText.replace(/:\s*/g, '');
      
      // Remove parentheses if they're empty or contain only spaces
      processedText = processedText.replace(/\(\s*\)/g, '');

      // Update the text node if it changed
      if (processedText !== text && processedText.trim()) {
        node.textContent = processedText.trim();
      } else if (!processedText.trim()) {
        // If the text is now empty, remove the parent element
        if (parent instanceof HTMLElement && parent.parentNode) {
          parent.parentNode.removeChild(parent);
        }
      }
    });
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
