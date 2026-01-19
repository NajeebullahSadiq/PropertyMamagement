/**
 * ثبت جواز عریضه‌نویسان
 * Petition Writer License Registration
 */
export interface PetitionWriterLicense {
    id?: number;
    
    // Tab 1: مشخصات عریضه‌نویس
    licenseNumber: string;
    applicantName: string;
    applicantFatherName?: string;
    applicantGrandFatherName?: string;
    
    // Identity Card - تذکره
    electronicIdNumber?: string;
    
    // Addresses
    permanentProvinceId?: number;
    permanentDistrictId?: number;
    permanentVillage?: string;
    permanentProvinceName?: string;
    permanentDistrictName?: string;
    
    currentProvinceId?: number;
    currentDistrictId?: number;
    currentVillage?: string;
    currentProvinceName?: string;
    currentDistrictName?: string;
    
    // Activity Location
    activityLocation?: string;
    
    // Picture Path
    picturePath?: string;
    
    // Tab 2: ثبت مالیه و مشخصات جواز
    bankReceiptNumber?: string;
    bankReceiptDate?: Date | string;
    bankReceiptDateFormatted?: string;
    licenseType?: string;
    licenseIssueDate?: Date | string;
    licenseIssueDateFormatted?: string;
    licenseExpiryDate?: Date | string;
    licenseExpiryDateFormatted?: string;
    
    // Tab 3: لغو / انصراف
    licenseStatus: LicenseStatusEnum;
    cancellationDate?: Date | string;
    cancellationDateFormatted?: string;
    
    // Audit Fields
    status?: boolean;
    createdAt?: Date;
    createdBy?: string;
    updatedAt?: Date;
    updatedBy?: string;
    
    // Related data
    relocations?: PetitionWriterRelocation[];
}

/**
 * Relocation History
 */
export interface PetitionWriterRelocation {
    id?: number;
    petitionWriterLicenseId?: number;
    newActivityLocation: string;
    relocationDate?: Date | string;
    relocationDateFormatted?: string;
    remarks?: string;
    createdAt?: Date;
    createdBy?: string;
}

/**
 * License Status Enum
 */
export enum LicenseStatusEnum {
    Active = 1,
    Cancelled = 2,
    Withdrawn = 3
}

/**
 * License Status Types for dropdown
 */
export const LicenseStatusTypes = [
    { id: LicenseStatusEnum.Active, name: 'فعال' },
    { id: LicenseStatusEnum.Cancelled, name: 'لغو' },
    { id: LicenseStatusEnum.Withdrawn, name: 'انصراف' }
];

/**
 * License Types for dropdown
 */
export const LicenseTypes = [
    { id: 'new', name: 'جدید' },
    { id: 'renewal', name: 'تمدید' }
];

/**
 * DTO for creating/updating petition writer license
 */
export interface PetitionWriterLicenseData {
    id?: number;
    licenseNumber: string;
    applicantName: string;
    applicantFatherName?: string;
    applicantGrandFatherName?: string;
    electronicIdNumber: string;
    permanentProvinceId?: number;
    permanentDistrictId?: number;
    permanentVillage?: string;
    currentProvinceId?: number;
    currentDistrictId?: number;
    currentVillage?: string;
    activityLocation?: string;
    picturePath?: string;
    bankReceiptNumber?: string;
    bankReceiptDate?: string;
    licenseType?: string;
    licenseIssueDate?: string;
    licenseExpiryDate?: string;
    licenseStatus?: number;
    cancellationDate?: string;
    calendarType?: string;
}

/**
 * DTO for relocation
 */
export interface PetitionWriterRelocationData {
    id?: number;
    petitionWriterLicenseId?: number;
    newActivityLocation: string;
    relocationDate?: string;
    remarks?: string;
    calendarType?: string;
}

/**
 * List response interface
 */
export interface PetitionWriterLicenseListResponse {
    items: PetitionWriterLicense[];
    totalCount: number;
    page: number;
    pageSize: number;
}
