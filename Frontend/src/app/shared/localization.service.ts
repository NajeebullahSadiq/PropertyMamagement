import { Injectable } from '@angular/core';

/**
 * Localization Service
 * Provides centralized translation management for dropdown labels and UI text
 * All user-facing labels are in Dari/Pashto, while internal values remain in English
 */
@Injectable({
  providedIn: 'root'
})
export class LocalizationService {

  // Role Types Translations
  roleTypes = {
    seller: { value: 'Seller', label: 'فروشنده' },
    sellers: { value: 'Sellers', label: 'فروشندگان' },
    lessor: { value: 'Lessor', label: 'کرایه‌دهنده' },
    lessors: { value: 'Lessors', label: 'کرایه‌دهندگان' },
    revocableSaleSeller: { value: 'Seller in a revocable sale', label: 'بایع بیع جایزی' },
    revocableSaleSellers: { value: 'Sellers in a revocable sale', label: 'بایعین بیع جایزی' },
    heirs: { value: 'Heirs', label: 'ورثه' },
    sellerAgent: { value: 'Sales Agent', label: 'وکیل فروش' },
    leaseAgent: { value: 'Lease Agent', label: 'وکیل کرایه' },
    revocableSaleAgent: { value: 'Agent for a revocable sale', label: 'وکیل بیع جایزی' },
    // Buyer Role Types - Vehicle Module (Restricted to 3 options only)
    buyer: { value: 'Buyer', label: 'خریدار' },
    buyers: { value: 'Buyers', label: 'خریداران' },
    buyerAgent: { value: 'Purchase Agent', label: 'وکیل خرید' },
    // Legacy buyer types (kept for backward compatibility in other modules)
    revocableSaleBuyer: { value: 'Buyer in a revocable sale', label: 'مشتری بیع جایزی' },
    revocableSaleBuyers: { value: 'Buyers in a revocable sale', label: 'مشتریان بیع جایزی' },
    lessee: { value: 'Lessee', label: 'کرایه‌گیرنده' },
    lessees: { value: 'Lessees', label: 'کرایه‌گیرندگان' },
    revocableSaleBuyerAgent: { value: 'Agent for buyer in a revocable sale', label: 'وکیل مشتری بیع جایزی' },
    leaseReceiverAgent: { value: 'Agent for lessee', label: 'وکیل کرایه‌گیرنده' }
  };

  // Vehicle Buyer Role Types - Restricted to 3 approved options only
  vehicleBuyerRoleTypes = [
    { value: 'Buyer', label: 'خریدار', allowMultiple: false },
    { value: 'Buyers', label: 'خریداران', allowMultiple: true },
    { value: 'Purchase Agent', label: 'وکیل خرید', allowMultiple: false }
  ];

  // Vehicle Hand Options
  vehicleHandOptions = [
    { value: 'afghanistan_hand', label: 'دست افغانستان' },
    { value: 'against_afghanistan_hand', label: 'دست خلاف افغانستان' }
  ];

  // Document Type Options (Dynamic Deed Types)
  documentTypes = [
    { value: 'قباله شرعی', label: 'قباله شرعی' },
    { value: 'سند ملکیت', label: 'سند ملکیت' },
    { value: 'سټه رهنمای معاملات', label: 'سټه رهنمای معاملات' },
    { value: 'سند دست‌نویس', label: 'سند دست‌نویس' }
  ];

  // Table Size Options
  tableSizes = [
    { value: 10, label: '10' },
    { value: 50, label: '50' },
    { value: 100, label: '100' }
  ];

  // Property Types (from backend lookup table)
  propertyTypes = [
    { value: 'House', label: 'حویلی' },
    { value: 'Apartment', label: 'آپارتمان' },
    { value: 'Shop', label: 'دکان' },
    { value: 'Block', label: 'بلاک' },
    { value: 'Land', label: 'زمین' },
    { value: 'Garden', label: 'باغ' },
    { value: 'Hill', label: 'مارکیټ' },
    { value: 'Other', label: 'سایر' }
  ];

  // Transaction Types (limited to 4 options for buyer details)
  transactionTypes = [
    { value: 'Purchase', label: 'خرید و فروش' },
    { value: 'Rent', label: 'کرایه' },
    { value: 'Revocable Sale', label: 'بیع جایزی' },
    { value: 'Other', label: 'سایر' }
  ];

  cancellationReasons = [
    { value: 'Coercion', label: 'اکراه' },
    { value: 'Fraud', label: 'فریب' },
    { value: 'Lesion', label: 'غبن' },
    { value: 'Defect', label: 'عیب' },
    { value: 'LackOfLegalCapacity', label: 'عدم اهلیت' },
    { value: 'LackOfCompetence', label: 'عدم صلاحیت' },
    { value: 'AbsenceOfDocuments', label: 'عدم موجودیت سند یا مدارک اعتباری' },
    { value: 'MutualConsent', label: 'رضایت طرفین معامله' },
    { value: 'Other', label: 'سایر' }
  ];

  // Education Levels (from backend lookup table)
  educationLevels = [
    { value: 'Illiterate', label: 'بی سواد' },
    { value: 'Primary School', label: 'مکتب ابتدائی' },
    { value: 'Secondary School', label: 'مکتب متوسطه' },
    { value: 'High School', label: 'لیسه' },
    { value: 'Diploma', label: 'دیپلوما' },
    { value: 'Bachelor\'s Degree', label: 'لیسانس' },
    { value: 'Master\'s Degree', label: 'فوق لیسانس' },
    { value: 'PhD/Doctorate', label: 'دکتورا' },
    { value: 'Religious Education', label: 'تعلیم دینی' },
    { value: 'Technical/Vocational', label: 'تعلیم فنی' }
  ];

  // Address Types (from backend lookup table)
  addressTypes = [
    { value: 'Permanent Address', label: 'آدرس دائمی' },
    { value: 'Temporary Address', label: 'آدرس موقتی' },
    { value: 'Business Address', label: 'آدرس تجاری' },
    { value: 'Mailing Address', label: 'آدرس پستی' },
    { value: 'Emergency Contact Address', label: 'آدرس تماس اضطراری' }
  ];

  // Guarantee Types - Standardized to 3 options only
  // 1 = Cash (پول نقد), 2 = Sharia Deed (قباله شرعی), 3 = Customary Deed (قباله عرفی)
  guaranteeTypes = [
    { value: 'Cash', label: 'پول نقد', id: 1 },
    { value: 'ShariaDeed', label: 'قباله شرعی', id: 2 },
    { value: 'CustomaryDeed', label: 'قباله عرفی', id: 3 }
  ];

  // Property Unit Types (from backend lookup table)
  propertyUnitTypes = [
    { value: 'Square Meter (m²)', label: 'متر مربع' },
    { value: 'Square Foot (ft²)', label: 'فوت مربع' },
    { value: 'Jerib', label: 'جریب' },
    { value: 'Acre', label: 'ایکر' },
    { value: 'Hectare', label: 'هکتار' },
    { value: 'Biswa', label: 'بسوہ' },
    { value: 'Kanal', label: 'کنال' },
    { value: 'Marla', label: 'مرلہ' }
  ];

  // Business Areas (from backend lookup table)
  businessAreas = [
    { value: 'Construction', label: 'ساختمان سازی' },
    { value: 'Real Estate', label: 'املاک' },
    { value: 'Import/Export', label: 'واردات/صادرات' },
    { value: 'Manufacturing', label: 'تولید' },
    { value: 'Retail Trade', label: 'تجارت خرده ای' },
    { value: 'Wholesale Trade', label: 'تجارت عمده ای' },
    { value: 'Transportation', label: 'حمل و نقل' },
    { value: 'Agriculture', label: 'کشاورزی' },
    { value: 'Mining', label: 'معدن کاری' },
    { value: 'Tourism', label: 'گردشگری' },
    { value: 'Healthcare', label: 'خدمات صحی' },
    { value: 'Education', label: 'تعلیم' },
    { value: 'Financial Services', label: 'خدمات مالی' },
    { value: 'Technology', label: 'تکنالوژی' },
    { value: 'Consulting', label: 'مشاوره' }
  ];

  // Violation Types (from backend lookup table)
  violationTypes = [
    { value: 'License Violation', label: 'نقض جواز' },
    { value: 'Tax Evasion', label: 'فرار مالیاتی' },
    { value: 'Building Code Violation', label: 'نقض کوڈ ساختمان' },
    { value: 'Environmental Violation', label: 'نقض محیط زیست' },
    { value: 'Safety Violation', label: 'نقض ایمنی' },
    { value: 'Documentation Violation', label: 'نقض مستندات' },
    { value: 'Zoning Violation', label: 'نقض منطقه بندی' },
    { value: 'Contract Violation', label: 'نقض قرارداد' },
    { value: 'Quality Standards Violation', label: 'نقض معیارهای کیفیت' },
    { value: 'Permit Violation', label: 'نقض مجوز' }
  ];

  // Lost Document Types (from backend lookup table)
  lostDocumentTypes = [
    { value: 'Property Deed', label: 'سند ملکیت' },
    { value: 'Business License', label: 'جواز تجاری' },
    { value: 'Tax Certificate', label: 'گواهی مالیاتی' },
    { value: 'Construction Permit', label: 'مجوز ساختمان' },
    { value: 'Identity Card', label: 'کارت شناسایی' },
    { value: 'Passport', label: 'پاسپورت' },
    { value: 'Vehicle Registration', label: 'ثبت نام وسیله نقلیه' },
    { value: 'Insurance Policy', label: 'پالیسی بیمه' },
    { value: 'Contract Agreement', label: 'قرارداد' },
    { value: 'Bank Statement', label: 'صورت حساب بانکی' }
  ];

  constructor() { }

  /**
   * Get label for a given value from a dropdown options array
   * @param value The internal English value
   * @param options The options array
   * @returns The Dari/Pashto label or the value if not found
   */
  getLabel(value: any, options: any[]): string {
    const option = options.find(opt => opt.value === value);
    return option ? option.label : value;
  }

  /**
   * Get value for a given label from a dropdown options array
   * @param label The Dari/Pashto label
   * @param options The options array
   * @returns The internal English value or the label if not found
   */
  getValue(label: string, options: any[]): any {
    const option = options.find(opt => opt.label === label);
    return option ? option.value : label;
  }
}
