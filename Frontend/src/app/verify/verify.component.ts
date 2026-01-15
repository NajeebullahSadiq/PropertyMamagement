import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VerificationService, DocumentVerificationDto } from '../shared/verification.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-verify',
  templateUrl: './verify.component.html',
  styleUrls: ['./verify.component.scss']
})
export class VerifyComponent implements OnInit {
  verificationCode: string = '';
  isLoading: boolean = false;
  hasSearched: boolean = false;
  result: DocumentVerificationDto | null = null;
  error: string | null = null;
  baseUrl = environment.apiURL + '/';

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
        this.verificationCode = code;
        this.verifyDocument();
      }
    });
  }

  verifyDocument(): void {
    if (!this.verificationCode || this.verificationCode.trim() === '') {
      this.error = 'لطفاً کود تصدیق را وارد کنید';
      return;
    }

    this.isLoading = true;
    this.error = null;
    this.result = null;
    this.hasSearched = true;

    this.verificationService.verifyDocument(this.verificationCode.trim()).subscribe({
      next: (response) => {
        this.result = response;
        this.isLoading = false;
        
        // Update URL with the code for sharing
        if (!this.route.snapshot.paramMap.get('code')) {
          this.router.navigate(['/verify', this.verificationCode], { replaceUrl: true });
        }
      },
      error: (err) => {
        console.error('Verification error:', err);
        this.error = 'خطا در تصدیق سند. لطفاً دوباره تلاش کنید.';
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
        return 'مع��بر';
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
      default:
        return this.result.documentType;
    }
  }

  getPhotoUrl(): string {
    if (this.result?.holderPhoto) {
      return this.baseUrl + this.result.holderPhoto;
    }
    return 'assets/img/avatar2.png';
  }

  resetSearch(): void {
    this.verificationCode = '';
    this.result = null;
    this.error = null;
    this.hasSearched = false;
    this.router.navigate(['/verify']);
  }
}
