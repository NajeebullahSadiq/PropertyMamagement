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
  
  // Property details
  serialNumber?: string;
  customDocumentType?: string;
  propertyType?: string;
  propertyTypeName?: string;
  propertyTypeDari?: string;
  area?: number;
  unitType?: string;
  unitTypeDari?: string;
  province?: string;
  provinceDari?: string;
  district?: string;
  districtDari?: string;
  village?: string;
  
  // Boundaries
  north?: string;
  south?: string;
  east?: string;
  west?: string;
  
  // Price info
  price?: number;
  priceText?: string;
  royaltyAmount?: number;
  halfPrice?: number;
  
  // Witnesses
  witnessOne?: WitnessInfoDto;
  witnessTwo?: WitnessInfoDto;
  
  sellerInfo?: SellerInfoDto;
  buyerInfo?: BuyerInfoDto;
  petitionWriterInfo?: PetitionWriterInfoDto;
  
  // Vehicle details
  plateNumber?: string;
  vehicleType?: string;
  vehicleModel?: string;
  engineNumber?: string;
  chassisNumber?: string;
  vehicleColor?: string;
  description?: string;
}

export interface WitnessInfoDto {
  firstName?: string;
  fatherName?: string;
  electronicNationalIdNumber?: string;
}

export interface SellerInfoDto {
  firstName?: string;
  fatherName?: string;
  grandFatherName?: string;
  electronicNationalIdNumber?: string;
  phoneNumber?: string;
  photo?: string;
  province?: string;
  district?: string;
  village?: string;
}

export interface BuyerInfoDto {
  firstName?: string;
  fatherName?: string;
  grandFatherName?: string;
  electronicNationalIdNumber?: string;
  phoneNumber?: string;
  photo?: string;
  province?: string;
  district?: string;
  village?: string;
}

export interface PetitionWriterInfoDto {
  applicantFatherName?: string;
  applicantGrandFatherName?: string;
  electronicNationalIdNumber?: string;
  mobileNumber?: string;
  competency?: string;
  district?: string;
  licenseType?: string;
  licensePrice?: number;
  permanentProvinceName?: string;
  permanentDistrictName?: string;
  permanentVillage?: string;
  currentProvinceName?: string;
  currentDistrictName?: string;
  currentVillage?: string;
  detailedAddress?: string;
  latestRelocation?: string;
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
    // Use QR Server API for QR code generation (free, reliable, no library needed)
    const encodedUrl = encodeURIComponent(verificationUrl);
    return `https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=${encodedUrl}`;
  }

  /**
   * Generate QR code with full document information
   */
  generateDocumentQrCodeUrl(documentData: any, verificationCode: string, verificationUrl: string): string {
    // Build comprehensive document info for QR code
    const lines: string[] = [];
    
    // Header
    lines.push('═══════════════════════════════');
    lines.push('      سند معاملات ملکیت');
    lines.push('═══════════════════════════════');
    lines.push('');
    
    // Document info
    lines.push(`کود تصدیق: ${verificationCode}`);
    lines.push(`نوعیت سند: ${documentData.documentType || documentData.customDocumentType || '-'}`);
    lines.push(`نمبر مسلسل: ${documentData.serialNumber || documentData.id || '-'}`);
    lines.push(`نوعیت ملکیت: ${documentData.propertypeType || documentData.customPropertyType || '-'}`);
    lines.push('');
    
    // Property info
    lines.push('─── معلومات ملکیت ───');
    lines.push(`مساحت: ${documentData.pArea || '-'} ${documentData.unitTypeDari || documentData.unitType || ''}`);
    lines.push(`ولایت: ${documentData.provinceDari || documentData.province || '-'}`);
    lines.push(`ناحیه: ${documentData.districtDari || documentData.district || '-'}`);
    lines.push(`قریه: ${documentData.village || '-'}`);
    lines.push('');
    
    // Boundaries
    lines.push('─── حدود اربعه ───');
    lines.push(`شمال: ${documentData.north || '-'} | جنوب: ${documentData.south || '-'}`);
    lines.push(`شرق: ${documentData.east || '-'} | غرب: ${documentData.west || '-'}`);
    lines.push('');
    
    // Price
    lines.push('─── معلومات معاملہ ───');
    lines.push(`قیمت: ${documentData.price || '-'} افغانی`);
    lines.push(`قیمت (حروف): ${documentData.priceText || '-'}`);
    lines.push(`حق العمل: ${documentData.royaltyAmount || '-'}`);
    lines.push('');
    
    // Sellers
    if (documentData.sellers && documentData.sellers.length > 0) {
      lines.push('─── فروشندگان ───');
      documentData.sellers.forEach((seller: any, i: number) => {
        lines.push(`${i + 1}. ${seller.firstName || '-'} ولد ${seller.fatherName || '-'}`);
        lines.push(`   تذکره: ${seller.electronicNationalIdNumber || '-'}`);
      });
      lines.push('');
    }
    
    // Buyers
    if (documentData.buyers && documentData.buyers.length > 0) {
      lines.push('─── خریداران ───');
      documentData.buyers.forEach((buyer: any, i: number) => {
        lines.push(`${i + 1}. ${buyer.firstName || '-'} ولد ${buyer.fatherName || '-'}`);
        lines.push(`   تذکره: ${buyer.electronicNationalIdNumber || '-'}`);
      });
      lines.push('');
    }
    
    // Witnesses
    lines.push('─── شاهدان ───');
    lines.push(`شاهد اول: ${documentData.witnessOneFirstName || '-'} ولد ${documentData.witnessOneFatherName || '-'}`);
    lines.push(`شاهد دوم: ${documentData.witnessTwoFirstName || '-'} ولد ${documentData.witnessTwoFatherName || '-'}`);
    lines.push('');
    
    // Footer
    lines.push('───────────────────────────────');
    lines.push(`تاریخ: ${documentData.createdAtFormatted || documentData.createdAt || '-'}`);
    lines.push('');
    lines.push(`لینک تصدیق: ${verificationUrl}`);
    lines.push('═══════════════════════════════');
    
    // Encode the content for QR code
    const qrContent = lines.join('\n');
    const encodedContent = encodeURIComponent(qrContent);
    
    // Use larger QR code for more data
    return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodedContent}`;
  }
}
