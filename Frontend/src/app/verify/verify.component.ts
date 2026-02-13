import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VerificationService, DocumentVerificationDto } from '../shared/verification.service';
import { environment } from 'src/environments/environment';
import jsQR from 'jsqr';

@Component({
  selector: 'app-verify',
  templateUrl: './verify.component.html',
  styleUrls: ['./verify.component.scss']
})
export class VerifyComponent implements OnInit, OnDestroy {
  @ViewChild('videoElement') videoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild('canvasElement') canvasElement!: ElementRef<HTMLCanvasElement>;

  verificationCode: string = '';
  isLoading: boolean = false;
  hasSearched: boolean = false;
  result: DocumentVerificationDto | null = null;
  error: string | null = null;
  baseUrl = environment.apiURL + '/';
  currentYear: number = new Date().getFullYear();

  // QR Scanner properties
  inputMode: 'manual' | 'scanner' = 'manual';
  isScanning: boolean = false;
  scannerError: string | null = null;
  private stream: MediaStream | null = null;
  private animationFrameId: number | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private verificationService: VerificationService
  ) {}

  ngOnInit(): void {
    // Check if code is provided in URL
    this.route.paramMap.subscribe(params => {
      const code = params.get('code');
      if (code) {
        // Extract just the verification code if a full URL was somehow passed
        const extractedCode = this.extractVerificationCode(code);
        this.verificationCode = extractedCode || code;
        this.verifyDocument();
      }
    });
  }

  ngOnDestroy(): void {
    this.stopScanner();
  }

  switchToManual(): void {
    this.inputMode = 'manual';
    this.stopScanner();
  }

  switchToScanner(): void {
    this.inputMode = 'scanner';
    this.scannerError = null;
    // Start scanner after view updates
    setTimeout(() => this.startScanner(), 100);
  }

  async startScanner(): Promise<void> {
    try {
      this.scannerError = null;
      
      // Check if camera is supported
      if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        this.scannerError = 'دستگاه شما از کمره پشتیبانی نمی‌کند';
        return;
      }

      // Request camera access
      this.stream = await navigator.mediaDevices.getUserMedia({
        video: { 
          facingMode: 'environment', // Prefer back camera
          width: { ideal: 640 },
          height: { ideal: 480 }
        }
      });

      if (this.videoElement?.nativeElement) {
        this.videoElement.nativeElement.srcObject = this.stream;
        this.videoElement.nativeElement.play();
        this.isScanning = true;
        
        // Wait for video to be ready
        this.videoElement.nativeElement.onloadedmetadata = () => {
          this.scanQRCode();
        };
      }
    } catch (err: any) {
      console.error('Camera error:', err);
      if (err.name === 'NotAllowedError') {
        this.scannerError = 'دسترسی به کمره رد شد. لطفاً اجازه دسترسی بدهید.';
      } else if (err.name === 'NotFoundError') {
        this.scannerError = 'کمره یافت نشد';
      } else {
        this.scannerError = 'خطا در دسترسی به کمره';
      }
      this.isScanning = false;
    }
  }

  stopScanner(): void {
    this.isScanning = false;
    
    if (this.animationFrameId) {
      cancelAnimationFrame(this.animationFrameId);
      this.animationFrameId = null;
    }
    
    if (this.stream) {
      this.stream.getTracks().forEach(track => track.stop());
      this.stream = null;
    }
    
    if (this.videoElement?.nativeElement) {
      this.videoElement.nativeElement.srcObject = null;
    }
  }

  private scanQRCode(): void {
    if (!this.isScanning || !this.videoElement?.nativeElement || !this.canvasElement?.nativeElement) {
      return;
    }

    const video = this.videoElement.nativeElement;
    const canvas = this.canvasElement.nativeElement;
    const ctx = canvas.getContext('2d');

    if (!ctx || video.readyState !== video.HAVE_ENOUGH_DATA) {
      this.animationFrameId = requestAnimationFrame(() => this.scanQRCode());
      return;
    }

    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

    const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
    const code = jsQR(imageData.data, imageData.width, imageData.height, {
      inversionAttempts: 'dontInvert'
    });

    if (code) {
      // QR code found - extract verification code
      const extractedCode = this.extractVerificationCode(code.data);
      if (extractedCode) {
        this.stopScanner();
        this.verificationCode = extractedCode;
        this.inputMode = 'manual';
        this.verifyDocument();
        return;
      }
    }

    // Continue scanning
    this.animationFrameId = requestAnimationFrame(() => this.scanQRCode());
  }

  private extractVerificationCode(data: string): string | null {
    // Check if it's a URL containing the verification code
    // Expected format: https://domain.com/verify/LIC-2026-XXXXXX
    const urlMatch = data.match(/\/verify\/([A-Z]{3}-\d{4}-[A-Z0-9]{6})/i);
    if (urlMatch) {
      return urlMatch[1].toUpperCase();
    }

    // Check if it's just the code itself
    const codeMatch = data.match(/^([A-Z]{3}-\d{4}-[A-Z0-9]{6})$/i);
    if (codeMatch) {
      return codeMatch[1].toUpperCase();
    }

    // Try to find the code pattern anywhere in the data
    const anyMatch = data.match(/([A-Z]{3}-\d{4}-[A-Z0-9]{6})/i);
    if (anyMatch) {
      return anyMatch[1].toUpperCase();
    }

    return null;
  }

  verifyDocument(): void {
    if (!this.verificationCode || this.verificationCode.trim() === '') {
      this.error = 'لطفاً کود تصدیق را وارد کنید';
      return;
    }

    // Extract verification code if a full URL was entered
    const extractedCode = this.extractVerificationCode(this.verificationCode.trim());
    const codeToVerify = extractedCode || this.verificationCode.trim();

    console.log('[Verify] Original input:', this.verificationCode);
    console.log('[Verify] Code to verify:', codeToVerify);

    this.isLoading = true;
    this.error = null;
    this.result = null;
    this.hasSearched = true;

    this.verificationService.verifyDocument(codeToVerify).subscribe({
      next: (response) => {
        console.log('[Verify] Verification response:', response);
        this.result = response;
        this.isLoading = false;
        
        // Update URL with the code for sharing
        if (!this.route.snapshot.paramMap.get('code')) {
          this.router.navigate(['/verify', codeToVerify], { replaceUrl: true });
        }
      },
      error: (err) => {
        console.error('Verification error:', err);
        console.error('Error details:', err?.error);
        
        if (err.status === 404) {
          this.error = 'کود تصدیق یافت نشد. لطفاً کود را بررسی کنید.';
        } else if (err.status === 400) {
          this.error = 'کود تصدیق نامعتبر است.';
        } else {
          this.error = 'خطا در تصدیق سند. لطفاً دوباره تلاش کنید.';
        }
        this.isLoading = false;
      }
    });
  }

  getStatusClass(): string {
    if (!this.result) return '';
    
    switch (this.result.status) {
      case 'Valid':
        return 'status-valid';
      case 'Expired':
        return 'status-expired';
      case 'Revoked':
        return 'status-revoked';
      case 'Invalid':
      default:
        return 'status-invalid';
    }
  }

  getStatusText(): string {
    if (!this.result) return '';
    
    switch (this.result.status) {
      case 'Valid':
        return 'معتبر';
      case 'Expired':
        return 'منقضی شده';
      case 'Revoked':
        return 'لغو شده';
      case 'Invalid':
      default:
        return 'نامعتبر';
    }
  }

  getDocumentTypeText(): string {
    if (!this.result?.documentType) return '';
    
    switch (this.result.documentType) {
      case 'RealEstateLicense':
        return 'جواز رهنمای معاملات';
      case 'PetitionWriterLicense':
        return 'جواز عریضه نویسی';
      case 'Securities':
        return 'تضمین نامه';
      case 'PetitionWriterSecurities':
        return 'تضمین نامه عریضه نویس';
      case 'PropertyDocument':
        return 'سند معامله ملکیت';
      case 'VehicleDocument':
        return 'سند معامله واسطه';
      default:
        return this.result.documentType;
    }
  }

  getPhotoUrl(): string {
    if (this.result?.holderPhoto) {
      // Use the same logic as print components for photo URLs
      if (this.result.holderPhoto.startsWith('Resources/') || this.result.holderPhoto.startsWith('/Resources/')) {
        const cleanPath = this.result.holderPhoto.startsWith('/') ? this.result.holderPhoto.substring(1) : this.result.holderPhoto;
        return `${this.baseUrl}${cleanPath}`;
      }
      return this.baseUrl + this.result.holderPhoto;
    }
    return 'assets/img/avatar2.png';
  }

  getSellerPhotoUrl(): string {
    if (this.result?.sellerInfo?.photo) {
      if (this.result.sellerInfo.photo.startsWith('Resources/') || this.result.sellerInfo.photo.startsWith('/Resources/')) {
        const cleanPath = this.result.sellerInfo.photo.startsWith('/') ? this.result.sellerInfo.photo.substring(1) : this.result.sellerInfo.photo;
        return `${this.baseUrl}${cleanPath}`;
      }
      return this.baseUrl + this.result.sellerInfo.photo;
    }
    return 'assets/img/avatar2.png';
  }

  getBuyerPhotoUrl(): string {
    if (this.result?.buyerInfo?.photo) {
      if (this.result.buyerInfo.photo.startsWith('Resources/') || this.result.buyerInfo.photo.startsWith('/Resources/')) {
        const cleanPath = this.result.buyerInfo.photo.startsWith('/') ? this.result.buyerInfo.photo.substring(1) : this.result.buyerInfo.photo;
        return `${this.baseUrl}${cleanPath}`;
      }
      return this.baseUrl + this.result.buyerInfo.photo;
    }
    return 'assets/img/avatar2.png';
  }

  getCompetencyDisplay(competency: string | undefined): string {
    if (!competency) return '-';
    switch (competency.toLowerCase()) {
      case 'high':
        return 'اعلی';
      case 'medium':
        return 'اوسط';
      case 'low':
        return 'ادنی';
      default:
        return competency;
    }
  }

  getLicenseTypeDisplay(licenseType: string | undefined): string {
    if (!licenseType) return '-';
    switch (licenseType.toLowerCase()) {
      case 'new':
        return 'جدید';
      case 'renewal':
        return 'تمدید';
      case 'duplicate':
        return 'مثنی';
      default:
        return licenseType;
    }
  }

  resetSearch(): void {
    this.verificationCode = '';
    this.result = null;
    this.error = null;
    this.hasSearched = false;
    this.stopScanner();
    this.inputMode = 'manual';
    this.router.navigate(['/verify']);
  }
}
