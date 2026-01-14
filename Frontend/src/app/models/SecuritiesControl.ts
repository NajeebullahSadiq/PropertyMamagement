/**
 * کنټرول ورودی و خروجی اسناد بهادار - Securities Inbound & Outbound Control
 */
export interface SecuritiesControl {
    id?: number;

    // Tab 1: معلومات عمومی و ثبت پیشنهاد
    serialNumber: string;
    securityDocumentType: number;
    securityDocumentTypeName?: string;
    proposalNumber?: string;
    proposalDate?: Date | string;
    proposalDateFormatted?: string;
    distributionTicketNumber?: string;
    deliveryDate?: Date | string;
    deliveryDateFormatted?: string;

    // Tab 2: مشخصات اسناد بهادار و تعداد آنها
    securitiesType?: number;
    securitiesTypeName?: string;

    // Property Sale Fields
    propertySaleCount?: number;
    propertySaleSerialStart?: string;
    propertySaleSerialEnd?: string;

    // Bay Wafa Fields
    bayWafaCount?: number;
    bayWafaSerialStart?: string;
    bayWafaSerialEnd?: string;

    // Rent Fields
    rentCount?: number;
    rentSerialStart?: string;
    rentSerialEnd?: string;

    // Vehicle Sale Fields
    vehicleSaleCount?: number;
    vehicleSaleSerialStart?: string;
    vehicleSaleSerialEnd?: string;

    // Exchange Fields
    exchangeCount?: number;
    exchangeSerialStart?: string;
    exchangeSerialEnd?: string;

    // Registration Book Fields
    registrationBookCount?: number;
    registrationBookSerialStart?: string;
    registrationBookSerialEnd?: string;

    // Printed Petition Fields
    printedPetitionCount?: number;
    printedPetitionSerialStart?: string;
    printedPetitionSerialEnd?: string;

    // Tab 3: ثبت و کنترول توزیع اسناد
    distributionStartNumber?: string;
    distributionEndNumber?: string;
    distributedPersonsCount?: number;

    // Tab 4: ملاحظات و توضیحات
    remarks?: string;

    // Additional computed fields for display
    totalDocumentsCount?: number;
    serialRangeStart?: string;
    serialRangeEnd?: string;

    // Audit Fields
    createdAt?: Date;
    createdBy?: string;
    updatedAt?: Date;
    updatedBy?: string;
    status?: boolean;
}

/**
 * DTO for creating/updating securities control
 */
export interface SecuritiesControlData {
    id?: number;
    serialNumber: string;
    securityDocumentType: number;
    proposalNumber?: string;
    proposalDate?: string;
    distributionTicketNumber?: string;
    deliveryDate?: string;
    securitiesType?: number;

    // Property Sale Fields
    propertySaleCount?: number;
    propertySaleSerialStart?: string;
    propertySaleSerialEnd?: string;

    // Bay Wafa Fields
    bayWafaCount?: number;
    bayWafaSerialStart?: string;
    bayWafaSerialEnd?: string;

    // Rent Fields
    rentCount?: number;
    rentSerialStart?: string;
    rentSerialEnd?: string;

    // Vehicle Sale Fields
    vehicleSaleCount?: number;
    vehicleSaleSerialStart?: string;
    vehicleSaleSerialEnd?: string;

    // Exchange Fields
    exchangeCount?: number;
    exchangeSerialStart?: string;
    exchangeSerialEnd?: string;

    // Registration Book Fields
    registrationBookCount?: number;
    registrationBookSerialStart?: string;
    registrationBookSerialEnd?: string;

    // Printed Petition Fields
    printedPetitionCount?: number;
    printedPetitionSerialStart?: string;
    printedPetitionSerialEnd?: string;

    // Tab 3: Distribution fields
    distributionStartNumber?: string;
    distributionEndNumber?: string;
    distributedPersonsCount?: number;

    // Tab 4: Remarks
    remarks?: string;
}

/**
 * Paginated response for securities control list
 */
export interface SecuritiesControlListResponse {
    items: SecuritiesControl[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

/**
 * Security document type options
 */
export const SecurityDocumentTypes = [
    { id: 1, name: 'ستههای رهنمای معاملات' },
    { id: 2, name: 'کتاب ثبت معاملات' },
    { id: 3, name: 'عرایض مطبوع' }
];

/**
 * Securities type options
 */
export const SecuritiesTypes = [
    { id: 1, name: 'ستههای خرید و فروش جایداد' },
    { id: 2, name: 'ستههای بیع وفا' },
    { id: 3, name: 'ستههای کرایی' },
    { id: 4, name: 'ستههای خرید و فروش وسایط نقلیه' },
    { id: 5, name: 'ستههای تبادله' },
    { id: 6, name: 'کتاب ثبت معاملات' },
    { id: 7, name: 'عرایض مطبوع' },
    { id: 8, name: 'ستههای خرید و فروش جایداد و بیع وفا' },
    { id: 9, name: 'ستههای خرید و فروش جایداد و کرایی' },
    { id: 10, name: 'ستههای بیع وفا و کرایی' },
    { id: 11, name: 'تمام انواع ستههای جایداد' }
];