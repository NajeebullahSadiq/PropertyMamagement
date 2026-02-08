export interface SecuritiesDistributionItem {
  id?: number;
  documentType: number;
  serialStart?: string;
  serialEnd?: string;
  count: number;
  price: number;
}

export interface SecuritiesDistributionData {
  id?: number;
  // Tab 1: مشخصات رهنمای معاملات
  registrationNumber: string;
  licenseOwnerName: string;
  licenseOwnerFatherName: string;
  transactionGuideName: string;
  licenseNumber: string;
  // Tab 2: مشخصات اسناد توزیعی
  items: SecuritiesDistributionItem[];
  // Tab 3: قیمت اسناد بهادار
  pricePerDocument?: number;
  totalDocumentsPrice?: number;
  totalSecuritiesPrice?: number;
  // Tab 4: مشخصات آویز تحویلی و تاریخ توزیع
  bankReceiptNumber?: string;
  deliveryDate?: string;
  distributionDate?: string;
  // Formatted dates for display
  deliveryDateFormatted?: string;
  distributionDateFormatted?: string;
  // Legacy fields for backward compatibility with old templates
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
  registrationBookPrice?: number;
  duplicateBookCount?: number;
  // Audit fields
  createdAt?: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
  status?: boolean;
}

// Alias for backward compatibility
export type SecuritiesDistribution = SecuritiesDistributionData;

// List response interface
export interface SecuritiesDistributionListResponse {
  items: SecuritiesDistributionData[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface DocumentTypeInfo {
  id: number;
  name: string;
  nameEn: string;
  pricePerUnit: number;
  hasSerial: boolean;
}

export const DocumentTypes: DocumentTypeInfo[] = [
  { id: 1, name: 'سټه یی خرید و فروش', nameEn: 'Property Sale', pricePerUnit: 4000, hasSerial: true },
  { id: 2, name: 'سټه یی بیع وفا', nameEn: 'Bay Wafa', pricePerUnit: 4000, hasSerial: true },
  { id: 3, name: 'سټه یی کرایی', nameEn: 'Rent', pricePerUnit: 4000, hasSerial: true },
  { id: 4, name: 'سټه وسایط نقلیه', nameEn: 'Vehicle', pricePerUnit: 4000, hasSerial: true },
  { id: 5, name: 'کتاب ثبت', nameEn: 'Registration Book', pricePerUnit: 1000, hasSerial: false },
  { id: 6, name: 'کتاب ثبت مثنی', nameEn: 'Duplicate Book', pricePerUnit: 20000, hasSerial: false }
];

// Legacy types - kept for backward compatibility but not used in new implementation
export const PropertySubTypes = [
  { id: 1, name: 'سته خرید و فروش' },
  { id: 2, name: 'سته بیع وفا' },
  { id: 3, name: 'سته کرایی' },
  { id: 4, name: 'همه' }
];

export const VehicleSubTypes = [
  { id: 1, name: 'سته خرید و فروش' },
  { id: 2, name: 'سته تبادله' }
];

export const RegistrationBookTypes = [
  { id: 1, name: 'کتاب ثبت' },
  { id: 2, name: 'کتاب ثبت مثنی' }
];
