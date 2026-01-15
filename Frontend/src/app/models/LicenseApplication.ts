/**
 * ثبت درخواست متقاضیان جواز رهنمای معاملات
 * License Application for Real Estate Guides
 */
export interface LicenseApplication {
    id?: number;
    
    // Tab 1: مشخصات درخواست متقاضی
    requestDate?: Date | string;
    requestDateFormatted?: string;
    requestSerialNumber: string;
    applicantName: string;
    proposedGuideName: string;
    
    // Permanent Address (سکونت اصلی)
    permanentProvinceId?: number;
    permanentDistrictId?: number;
    permanentVillage?: string;
    permanentProvinceName?: string;
    permanentDistrictName?: string;
    
    // Current Address (سکونت فعلی)
    currentProvinceId?: number;
    currentDistrictId?: number;
    currentVillage?: string;
    currentProvinceName?: string;
    currentDistrictName?: string;
    
    // Status
    status?: boolean;
    isWithdrawn?: boolean;
    
    // Audit Fields
    createdAt?: Date;
    createdBy?: string;
    updatedAt?: Date;
    updatedBy?: string;
    
    // Related data
    guarantors?: LicenseApplicationGuarantor[];
    withdrawal?: LicenseApplicationWithdrawal;
}

/**
 * Tab 2: تضمین‌کنندگان - Guarantors
 */
export interface LicenseApplicationGuarantor {
    id?: number;
    licenseApplicationId?: number;
    
    // Guarantor Identity
    guarantorName: string;
    guarantorFatherName?: string;
    
    // Guarantee Type
    guaranteeTypeId: number;
    
    // Conditional Fields - Cash (پول نقد)
    cashAmount?: number;
    
    // Conditional Fields - Sharia Deed (قباله شرعی)
    shariaDeedNumber?: string;
    shariaDeedDate?: Date | string;
    shariaDeedDateFormatted?: string;
    
    // Conditional Fields - Customary Deed (قباله عرفی)
    customaryDeedSerialNumber?: string;
    
    // Permanent Address (سکونت اصلی)
    permanentProvinceId?: number;
    permanentDistrictId?: number;
    permanentVillage?: string;
    permanentProvinceName?: string;
    permanentDistrictName?: string;
    
    // Current Address (سکونت فعلی)
    currentProvinceId?: number;
    currentDistrictId?: number;
    currentVillage?: string;
    currentProvinceName?: string;
    currentDistrictName?: string;
    
    // Audit
    createdAt?: Date;
    createdBy?: string;
}

/**
 * Tab 3: انصراف - Withdrawal
 */
export interface LicenseApplicationWithdrawal {
    id?: number;
    licenseApplicationId?: number;
    
    withdrawalReason: string;
    withdrawalDate?: Date | string;
    withdrawalDateFormatted?: string;
    
    // Audit
    createdAt?: Date;
    createdBy?: string;
}

/**
 * DTO for creating/updating license application
 */
export interface LicenseApplicationData {
    id?: number;
    requestDate?: string;
    requestSerialNumber: string;
    applicantName: string;
    proposedGuideName: string;
    permanentProvinceId?: number;
    permanentDistrictId?: number;
    permanentVillage?: string;
    currentProvinceId?: number;
    currentDistrictId?: number;
    currentVillage?: string;
    calendarType?: string;
}

/**
 * DTO for guarantor
 */
export interface LicenseApplicationGuarantorData {
    id?: number;
    licenseApplicationId?: number;
    guarantorName: string;
    guarantorFatherName?: string;
    guaranteeTypeId: number;
    cashAmount?: number;
    shariaDeedNumber?: string;
    shariaDeedDate?: string;
    customaryDeedSerialNumber?: string;
    permanentProvinceId?: number;
    permanentDistrictId?: number;
    permanentVillage?: string;
    currentProvinceId?: number;
    currentDistrictId?: number;
    currentVillage?: string;
    calendarType?: string;
}

/**
 * DTO for withdrawal
 */
export interface LicenseApplicationWithdrawalData {
    id?: number;
    licenseApplicationId?: number;
    withdrawalReason: string;
    withdrawalDate?: string;
    calendarType?: string;
}

/**
 * List response
 */
export interface LicenseApplicationListResponse {
    items: LicenseApplication[];
    totalCount: number;
    page: number;
    pageSize: number;
}

/**
 * Guarantee Type Constants
 */
export enum LicenseGuaranteeTypeEnum {
    Cash = 1,          // پول نقد
    ShariaDeed = 2,    // قباله شرعی
    CustomaryDeed = 3  // قباله عرفی
}

export const LicenseGuaranteeTypes = [
    { id: 1, name: 'پول نقد' },
    { id: 2, name: 'قباله شرعی' },
    { id: 3, name: 'قباله عرفی' }
];

/**
 * Application Status
 */
export const ApplicationStatuses = [
    { id: 1, name: 'فعال', value: 'active' },
    { id: 2, name: 'منصرف‌شده', value: 'withdrawn' }
];
