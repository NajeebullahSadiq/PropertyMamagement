/**
 * اسناد بهادار رهنمای معاملات - Securities Distribution for Transaction Guides
 */
export interface SecuritiesDistribution {
    id?: number;

    // Tab 1: مشخصات رهنمای معاملات
    registrationNumber: string;
    licenseOwnerName: string;
    licenseOwnerFatherName: string;
    transactionGuideName: string;
    licenseNumber: string;

    // Tab 2: مشخصات اسناد توزیعی
    documentType?: number;
    propertySubType?: number;
    vehicleSubType?: number;

    // Property Document Fields
    propertySaleCount?: number;
    propertySaleSerialStart?: string;
    propertySaleSerialEnd?: string;
    bayWafaCount?: number;
    bayWafaSerialStart?: string;
    bayWafaSerialEnd?: string;
    rentCount?: number;
    rentSerialStart?: string;
    rentSerialEnd?: string;

    // Vehicle Document Fields
    vehicleSaleCount?: number;
    vehicleSaleSerialStart?: string;
    vehicleSaleSerialEnd?: string;
    vehicleExchangeCount?: number;
    vehicleExchangeSerialStart?: string;
    vehicleExchangeSerialEnd?: string;

    // Registration Book Fields
    registrationBookType?: number;
    registrationBookCount?: number;
    duplicateBookCount?: number;

    // Tab 3: قیمت اسناد بهادار
    pricePerDocument?: number;
    totalDocumentsPrice?: number;
    registrationBookPrice?: number;
    totalSecuritiesPrice?: number;

    // Tab 4: مشخصات آویز تحویلی و تاریخ توزیع
    bankReceiptNumber?: string;
    deliveryDate?: Date | string;
    deliveryDateFormatted?: string;
    distributionDate?: Date | string;
    distributionDateFormatted?: string;

    // Audit Fields
    createdAt?: Date;
    createdBy?: string;
    updatedAt?: Date;
    updatedBy?: string;
    status?: boolean;
}

/**
 * DTO for creating/updating securities distribution
 */
export interface SecuritiesDistributionData {
    id?: number;
    registrationNumber: string;
    licenseOwnerName: string;
    licenseOwnerFatherName: string;
    transactionGuideName: string;
    licenseNumber: string;
    documentType?: number;
    propertySubType?: number;
    vehicleSubType?: number;
    propertySaleCount?: number;
    propertySaleSerialStart?: string;
    propertySaleSerialEnd?: string;
    bayWafaCount?: number;
    bayWafaSerialStart?: string;
    bayWafaSerialEnd?: string;
    rentCount?: number;
    rentSerialStart?: string;
    rentSerialEnd?: string;
    vehicleSaleCount?: number;
    vehicleSaleSerialStart?: string;
    vehicleSaleSerialEnd?: string;
    vehicleExchangeCount?: number;
    vehicleExchangeSerialStart?: string;
    vehicleExchangeSerialEnd?: string;
    registrationBookType?: number;
    registrationBookCount?: number;
    duplicateBookCount?: number;
    pricePerDocument?: number;
    totalDocumentsPrice?: number;
    registrationBookPrice?: number;
    totalSecuritiesPrice?: number;
    bankReceiptNumber?: string;
    deliveryDate?: string;
    distributionDate?: string;
}

/**
 * Paginated response for securities distribution list
 */
export interface SecuritiesDistributionListResponse {
    items: SecuritiesDistribution[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

/**
 * Document type options
 */
export const DocumentTypes = [
    { id: 1, name: 'سته‌های معاملات جایداد' },
    { id: 2, name: 'سته‌های معاملات وسایط نقلیه' }
];

/**
 * Property sub-type options
 */
export const PropertySubTypes = [
    { id: 1, name: 'سته خرید و فروش جایداد' },
    { id: 2, name: 'سته بیع وفا' },
    { id: 3, name: 'سته کرایی' },
    { id: 4, name: 'همه انواع' }
];

/**
 * Vehicle sub-type options
 */
export const VehicleSubTypes = [
    { id: 1, name: 'سته خرید و فروش وسایط نقلیه' },
    { id: 2, name: 'سته تبادله وسایط نقلیه' }
];

/**
 * Registration book type options
 */
export const RegistrationBookTypes = [
    { id: 1, name: 'کتاب ثبت' },
    { id: 2, name: 'کتاب ثبت مثنی' }
];
