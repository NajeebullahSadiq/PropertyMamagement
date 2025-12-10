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
    revocableSaleSeller: { value: 'Seller in a revocable sale', label: 'بایع بیع جایزی' },
    heirs: { value: 'Heirs', label: 'ورثه' },
    sellerAgent: { value: 'Sales Agent', label: 'وکیل فروش' },
    leaseAgent: { value: 'Lease Agent', label: 'وکیل کرایه' },
    revocableSaleAgent: { value: 'Agent for a revocable sale', label: 'وکیل بیع جایزی' },
    buyer: { value: 'Buyer', label: 'خریدار' },
    buyerAgent: { value: 'Authorized Agent (Buyer)', label: 'وکیل خریدار' }
  };

  // Vehicle Hand Options
  vehicleHandOptions = [
    { value: 'afghanistan_hand', label: 'دست افغانستان' },
    { value: 'against_afghanistan_hand', label: 'دست خلاف افغانستان' }
  ];

  // Document Type Options
  docTypes = [
    { value: 'رسمی', label: 'رسمی' },
    { value: 'عرفی', label: 'عرفی' }
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
    { value: 'Land', label: 'زمین' },
    { value: 'Commercial Building', label: 'ساختمان تجاری' },
    { value: 'Office', label: 'دفتر' },
    { value: 'Shop', label: 'دکان' },
    { value: 'Warehouse', label: 'انبار' },
    { value: 'Factory', label: 'کارخانه' },
    { value: 'Farm', label: 'مزرعه' },
    { value: 'Villa', label: 'ویلا' },
    { value: 'Block', label: 'بلاک' }
  ];

  // Transaction Types (from backend lookup table)
  transactionTypes = [
    { value: 'Sale', label: 'فروخت' },
    { value: 'Rent', label: 'اجاره' },
    { value: 'Lease', label: 'اجاره دراز مدت' },
    { value: 'Mortgage', label: 'رہن' },
    { value: 'Exchange', label: 'تبادله' },
    { value: 'Gift', label: 'هدیه' },
    { value: 'Inheritance', label: 'وراثت' }
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

  // Identity Card Types (from backend lookup table)
  identityCardTypes = [
    { value: 'National ID Card (Tazkira)', label: 'تذکره ملی' },
    { value: 'Passport', label: 'پاسپورت' },
    { value: 'Driver\'s License', label: 'رخصت رانندگی' },
    { value: 'Birth Certificate', label: 'سند تولد' },
    { value: 'Military ID', label: 'شناسنامه نظامی' },
    { value: 'Student ID', label: 'شناسنامه دانشجویی' },
    { value: 'Employee ID', label: 'شناسنامه کارمندی' }
  ];

  // Address Types (from backend lookup table)
  addressTypes = [
    { value: 'Permanent Address', label: 'آدرس دائمی' },
    { value: 'Temporary Address', label: 'آدرس موقتی' },
    { value: 'Business Address', label: 'آدرس تجاری' },
    { value: 'Mailing Address', label: 'آدرس پستی' },
    { value: 'Emergency Contact Address', label: 'آدرس تماس اضطراری' }
  ];

  // Guarantee Types (from backend lookup table)
  guaranteeTypes = [
    { value: 'Bank Guarantee', label: 'ضمانت بانکی' },
    { value: 'Personal Guarantee', label: 'ضمانت شخصی' },
    { value: 'Corporate Guarantee', label: 'ضمانت شرکتی' },
    { value: 'Property Guarantee', label: 'ضمانت ملکی' },
    { value: 'Cash Deposit', label: 'سپردهٔ نقد' },
    { value: 'Government Guarantee', label: 'ضمانت دولتی' },
    { value: 'Insurance Guarantee', label: 'ضمانت بیمهٔ' }
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
