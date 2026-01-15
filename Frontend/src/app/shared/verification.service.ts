import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface GenerateVerificationRequest {
  documentId: number;
  documentType: string;
}

export interface VerificationResult {
  verificationCode: string;
  verificationUrl: string;
  isNew: boolean;
}

export interface DocumentVerificationDto {
  verificationCode: string;
  isValid: boolean;
  status: string;
  documentType?: string;
  licenseNumber?: string;
  holderName?: string;
  holderPhoto?: string;
  issueDate?: string;
  expiryDate?: string;
  companyTitle?: string;
  officeAddress?: string;
  revokedReason?: string;
  verifiedAt: string;
}

export interface VerificationStatsDto {
  verificationCode: string;
  totalAttempts: number;
  successfulAttempts: number;
  failedAttempts: number;
  lastVerifiedAt?: string;
  createdAt?: string;
  isRevoked: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class VerificationService {
  private baseUrl = environment.apiURL + '/Verification';

  constructor(private http: HttpClient) { }

  /**
   * Generate or retrieve verification code for a document
   */
  generateVerificationCode(documentId: number, documentType: string): Observable<VerificationResult> {
    const request: GenerateVerificationRequest = { documentId, documentType };
    return this.http.post<VerificationResult>(`${this.baseUrl}/generate`, request);
  }

  /**
   * Verify a document using its verification code (public endpoint)
   */
  verifyDocument(code: string): Observable<DocumentVerificationDto> {
    return this.http.get<DocumentVerificationDto>(`${this.baseUrl}/verify/${code}`);
  }

  /**
   * Revoke a verification code (admin only)
   */
  revokeVerification(verificationCode: string, reason: string): Observable<{ success: boolean; message: string }> {
    return this.http.post<{ success: boolean; message: string }>(`${this.baseUrl}/revoke`, {
      verificationCode,
      reason
    });
  }

  /**
   * Get verification statistics for a document
   */
  getVerificationStats(code: string): Observable<VerificationStatsDto> {
    return this.http.get<VerificationStatsDto>(`${this.baseUrl}/stats/${code}`);
  }

  /**
   * Generate QR code data URL from verification URL
   */
  generateQrCodeUrl(verificationUrl: string): string {
    // Use Google Charts API for QR code generation (simple, no library needed)
    const encodedUrl = encodeURIComponent(verificationUrl);
    return `https://chart.googleapis.com/chart?cht=qr&chs=150x150&chl=${encodedUrl}&choe=UTF-8`;
  }
}
