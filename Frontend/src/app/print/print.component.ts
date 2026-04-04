import { Component, OnInit, ElementRef, Renderer2 } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../shared/auth.service';
import { PropertyService } from '../shared/property.service';
import { LocalizationService } from '../shared/localization.service';
import { VerificationService } from '../shared/verification.service';
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
  
  // Verification properties
  verificationCode: string = '';
  verificationUrl: string = '';
  qrCodeUrl: string = '';
  verificationError: string | null = null;

  // Print mode: 'full' = with design, 'data-only' = only data for pre-printed forms, 'new-design' = modern table layout, 'old-design' = traditional form overlay, 'upload-docts' = upload documents
  printMode: 'full' | 'data-only' | 'new-design' | 'old-design' | 'upload-docts' = 'full';
  window = window; // Make window available in template

  // Upload document properties
  uploadSetaNumber: string = '';
  selectedFile: File | null = null;
  uploadMessage: string = '';
  uploadError: string = '';
  isUploading: boolean = false;

  // Old design data toggle
  showOldDesignData: boolean = true;
  showOldDesignBackground: boolean = true;

  constructor(
    public service: AuthService,
    private route: ActivatedRoute,
    private pservice: PropertyService,
    private localizationService: LocalizationService,
    private verificationService: VerificationService,
    private elementRef: ElementRef,
    private renderer: Renderer2
  ) { }

  ngOnInit(): void {
    // Check if print mode is specified in URL
    const mode = this.route.snapshot.queryParamMap.get('mode');
    if (mode === 'data-only' || mode === 'full' || mode === 'new-design' || mode === 'old-design') {
      this.printMode = mode as 'full' | 'data-only' | 'new-design' | 'old-design';
    }
    this.loadPrintData();
  }

  setPrintMode(mode: 'full' | 'data-only' | 'new-design' | 'old-design' | 'upload-docts'): void {
    this.printMode = mode;
    
    // Reset data visibility when switching to old design
    if (mode === 'old-design') {
      this.showOldDesignData = true;
      this.showOldDesignBackground = true;
    }
    
    // If switching to data-only mode, process the DOM to remove labels
    if (mode === 'data-only') {
      setTimeout(() => {
        this.processDataOnlyMode();
      }, 100);
    }
  }

  toggleOldDesignData(): void {
    this.showOldDesignData = !this.showOldDesignData;
  }

  toggleOldDesignBackground(): void {
    this.showOldDesignBackground = !this.showOldDesignBackground;
  }

  // File selection handler
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.uploadError = '';
    }
  }

  // Upload document handler
  uploadDocument(): void {
    if (!this.selectedFile) {
      this.uploadError = 'لطفاً فایل را انتخاب کنید';
      return;
    }
    if (!this.uploadSetaNumber.trim()) {
      this.uploadError = 'لطفاً سټه نمبر را وارد کنید';
      return;
    }

    this.isUploading = true;
    this.uploadMessage = '';
    this.uploadError = '';

    const formData = new FormData();
    formData.append('file', this.selectedFile);
    formData.append('setaNumber', this.uploadSetaNumber);

    this.pservice.uploadSetaDocument(formData).subscribe({
      next: (response: any) => {
        this.isUploading = false;
        this.uploadMessage = 'فایل با موفقیت آپلود شد';
        this.selectedFile = null;
        this.uploadSetaNumber = '';
        // Reset file input
        const fileInput = document.getElementById('fileInput') as HTMLInputElement;
        if (fileInput) fileInput.value = '';
      },
      error: (err: any) => {
        this.isUploading = false;
        this.uploadError = err?.error?.message || 'خطا در آپلود فایل';
      }
    });
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

      // Helper function to create positioned image
      const createImage = (src: string, top: string, left: string, right: string = '') => {
        const img = document.createElement('img');
        img.src = src;
        img.style.position = 'absolute';
        img.style.top = top;
        if (right) {
          img.style.right = right;
        } else {
          img.style.left = left;
        }
        img.style.width = '80px';
        img.style.height = '85px';
        img.style.objectFit = 'cover';
        return img;
      };

      // Add seller photo (left side) - top: 15mm, left: 10mm
      if (this.SellerfilePath) {
        wrapper.appendChild(createImage(this.SellerfilePath, '15mm', '10mm'));
      }

      // Add buyer photo (right side) - top: 15mm, right: 10mm
      if (this.BuyerfilePath) {
        wrapper.appendChild(createImage(this.BuyerfilePath, '15mm', '', '10mm'));
      }

      // Map data to exact positions based on the pre-printed form
      // These positions need to be adjusted to match your exact form
      
      // Company name and phone (top section)
      if (this.userDetails.companyName) {
        wrapper.appendChild(createField(this.userDetails.companyName, '25mm', '80mm', '14pt'));
      }
      if (this.userDetails.phoneNumber) {
        wrapper.appendChild(createField(this.userDetails.phoneNumber, '32mm', '80mm', '12pt'));
      }
      
      // Document type and number
      if (this.documentData.documentType || this.documentData.customDocumentType) {
        const docType = this.getDocumentTypeDisplay();
        wrapper.appendChild(createField(docType, '50mm', '80mm'));
      }
      if (this.documentData.issuanceNumber || this.documentData.pNumber) {
        wrapper.appendChild(createField(this.documentData.issuanceNumber || this.documentData.pNumber, '57mm', '80mm'));
      }
      
      // Property details
      if (this.documentData.pArea) {
        wrapper.appendChild(createField(this.documentData.pArea + ' ' + (this.documentData.unitType || ''), '64mm', '80mm'));
      }
      if (this.documentData.propertypeType) {
        wrapper.appendChild(createField(this.documentData.propertypeType, '78mm', '80mm'));
      }
      if (this.documentData.numofRooms) {
        wrapper.appendChild(createField(this.documentData.numofRooms.toString(), '85mm', '80mm'));
      }
      
      // Property location
      if (this.documentData.provinceDari || this.documentData.province) {
        wrapper.appendChild(createField(this.documentData.provinceDari || this.documentData.province, '71mm', '100mm', '11pt'));
      }
      if (this.documentData.districtDari || this.documentData.district) {
        wrapper.appendChild(createField(this.documentData.districtDari || this.documentData.district, '71mm', '130mm', '11pt'));
      }
      if (this.documentData.village) {
        wrapper.appendChild(createField(this.documentData.village, '78mm', '80mm', '11pt'));
      }
      
      // Boundaries (حدود اربعه)
      if (this.documentData.north) {
        wrapper.appendChild(createField(this.documentData.north, '92mm', '100mm', '11pt'));
      }
      if (this.documentData.south) {
        wrapper.appendChild(createField(this.documentData.south, '100mm', '100mm', '11pt'));
      }
      if (this.documentData.east) {
        wrapper.appendChild(createField(this.documentData.east, '108mm', '100mm', '11pt'));
      }
      if (this.documentData.west) {
        wrapper.appendChild(createField(this.documentData.west, '116mm', '100mm', '11pt'));
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
      
      // Price information
      if (this.documentData.priceText) {
        wrapper.appendChild(createField(this.documentData.priceText, '125mm', '100mm', '11pt'));
      }
      if (this.documentData.price) {
        wrapper.appendChild(createField(this.documentData.price, '133mm', '100mm'));
      }
      if (this.documentData.price) {
        const halfPrice = (parseFloat(this.documentData.price) / 2).toString();
        wrapper.appendChild(createField(halfPrice, '141mm', '100mm'));
      }
      if (this.documentData.royaltyAmount) {
        wrapper.appendChild(createField(this.documentData.royaltyAmount, '149mm', '100mm'));
      }
      
      // Seller permanent address (سکونت اصلی)
      if (this.documentData.sellerProvinceDari || this.documentData.sellerProvince) {
        wrapper.appendChild(createField(this.documentData.sellerProvinceDari || this.documentData.sellerProvince, '125mm', '140mm', '11pt'));
      }
      if (this.documentData.sellerDistrictDari || this.documentData.sellerDistrict) {
        wrapper.appendChild(createField(this.documentData.sellerDistrictDari || this.documentData.sellerDistrict, '132mm', '140mm', '11pt'));
      }
      if (this.documentData.sellerVillage) {
        wrapper.appendChild(createField(this.documentData.sellerVillage, '139mm', '140mm', '11pt'));
      }
      
      // Buyer permanent address (سکونت اصلی)
      if (this.documentData.buyerProvinceDari || this.documentData.buyerProvince) {
        wrapper.appendChild(createField(this.documentData.buyerProvinceDari || this.documentData.buyerProvince, '125mm', '20mm', '11pt'));
      }
      if (this.documentData.buyerDistrictDari || this.documentData.buyerDistrict) {
        wrapper.appendChild(createField(this.documentData.buyerDistrictDari || this.documentData.buyerDistrict, '132mm', '20mm', '11pt'));
      }
      if (this.documentData.buyerVillage) {
        wrapper.appendChild(createField(this.documentData.buyerVillage, '139mm', '20mm', '11pt'));
      }
      
      // Seller temporary address (سکونت فعلی)
      if (this.documentData.tSellerProvinceDari || this.documentData.tSellerProvince) {
        wrapper.appendChild(createField(this.documentData.tSellerProvinceDari || this.documentData.tSellerProvince, '165mm', '140mm', '11pt'));
      }
      if (this.documentData.tSellerDistrictDari || this.documentData.tSellerDistrict) {
        wrapper.appendChild(createField(this.documentData.tSellerDistrictDari || this.documentData.tSellerDistrict, '172mm', '140mm', '11pt'));
      }
      if (this.documentData.tSellerVillage) {
        wrapper.appendChild(createField(this.documentData.tSellerVillage, '179mm', '140mm', '11pt'));
      }
      
      // Buyer temporary address (سکونت فعلی)
      if (this.documentData.tBuyerProvinceDari || this.documentData.tBuyerProvince) {
        wrapper.appendChild(createField(this.documentData.tBuyerProvinceDari || this.documentData.tBuyerProvince, '165mm', '20mm', '11pt'));
      }
      if (this.documentData.tBuyerDistrictDari || this.documentData.tBuyerDistrict) {
        wrapper.appendChild(createField(this.documentData.tBuyerDistrictDari || this.documentData.tBuyerDistrict, '172mm', '20mm', '11pt'));
      }
      if (this.documentData.tBuyerVillage) {
        wrapper.appendChild(createField(this.documentData.tBuyerVillage, '179mm', '20mm', '11pt'));
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
          this.SellerfilePath = this.constructImageUrl(this.documentData.sellerPhoto);
        }
        
        if (this.documentData.buyerPhoto) {
          this.BuyerfilePath = this.constructImageUrl(this.documentData.buyerPhoto);
        }

        if (this.documentData.filePath) {
          this.propertyImagePath = this.constructImageUrl(this.documentData.filePath);
        }

        if (this.documentData.previousDocumentsPath) {
          this.previousDocsPath = this.constructImageUrl(this.documentData.previousDocumentsPath);
        }

        if (this.documentData.existingDocumentsPath) {
          this.existingDocsPath = this.constructImageUrl(this.documentData.existingDocumentsPath);
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

        // Fetch verification code after loading property data
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

  /**
   * Get the appropriate label for issuance number based on document type
   */
  getIssuanceNumberLabel(): string {
    const docType = this.documentData?.documentType;
    if (docType === 'سند ملکیت') {
      return 'نمبر سند ملکیت';
    } else if (docType === 'قباله شرعی') {
      return 'نمبر قباله';
    }
    return 'نمبر سند';
  }

  /**
   * Get the document type display value (shows custom type if "سایر" is selected)
   */
  getDocumentTypeDisplay(): string {
    const docType = this.documentData?.documentType;
    if (docType === 'سایر' && this.documentData?.customDocumentType) {
      return this.documentData.customDocumentType;
    }
    return docType || this.documentData?.doctype || '';
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
    return `${this.baseUrl}Upload/view/${path}`;
  }

  // Make constructImageUrl available in template
  public constructImageUrlPublic(path: string): string {
    return this.constructImageUrl(path);
  }

  private fetchVerificationCode(): void {
    // Get the property ID from the data
    const propertyId = this.documentData?.propertyId || this.documentData?.id || this.documentData?.propertyDetailId;
    
    console.log('[PrintProperty] Data object keys:', Object.keys(this.documentData || {}));
    console.log('[PrintProperty] Looking for property ID, found:', propertyId);

    if (!propertyId) {
      console.warn('[PrintProperty] No property ID found for verification. Data:', this.documentData);
      this.waitForImagesToLoad();
      return;
    }

    console.log('[PrintProperty] Calling generateVerificationCode with ID:', propertyId);

    this.verificationService.generateVerificationCode(propertyId, 'PropertyDocument').subscribe({
      next: (result) => {
        console.log('[PrintProperty] Verification result:', result);
        
        // Validate the result
        if (!result.verificationCode) {
          console.error('[PrintProperty] No verification code in result');
          this.verificationError = 'خطا در دریافت کود تصدیق';
          this.waitForImagesToLoad();
          return;
        }
        
        if (!result.verificationUrl) {
          console.error('[PrintProperty] No verification URL in result');
          this.verificationError = 'خطا در دریافت لینک تصدیق';
          this.waitForImagesToLoad();
          return;
        }
        
        // Ensure the verification URL contains the code
        if (!result.verificationUrl.includes(result.verificationCode)) {
          console.warn('[PrintProperty] Verification URL does not contain code, fixing...');
          result.verificationUrl = `${result.verificationUrl}/${result.verificationCode}`;
        }
        
        this.verificationCode = result.verificationCode;
        this.verificationUrl = result.verificationUrl;
        
        // Generate QR code with full document information
        this.qrCodeUrl = this.verificationService.generateDocumentQrCodeUrl(
          this.documentData,
          result.verificationCode,
          result.verificationUrl
        );
        
        console.log('[PrintProperty] Verification Code:', this.verificationCode);
        console.log('[PrintProperty] Verification URL:', this.verificationUrl);
        console.log('[PrintProperty] QR Code URL:', this.qrCodeUrl);
        
        this.waitForImagesToLoad();
      },
      error: (err) => {
        console.error('[PrintProperty] Error fetching verification code:', err);
        console.error('[PrintProperty] Error details:', err?.error || err?.message);
        this.verificationError = 'خطا در دریافت کود تصدیق';
        this.waitForImagesToLoad();
      }
    });
  }

}
