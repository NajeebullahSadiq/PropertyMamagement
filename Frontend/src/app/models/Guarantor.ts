export interface Guarantor{
    id: number;
    firstName:string;
    fatherName:string;
    grandFatherName?:string;
    electronicNationalIdNumber?:string;
    companyId:number;
    pothoPath:string;
    phoneNumber:string;
    paddressProvinceId:number;
    paddressDistrictId:number;
    paddressVillage:string;
    taddressProvinceId:number;
    taddressDistrictId:number;
    taddressVillage:string;
    // Guarantee Information
    guaranteeTypeId?:number;
    propertyDocumentNumber?:number;
    propertyDocumentDate?:string;
    senderMaktobNumber?:string;
    senderMaktobDate?:string;
    answerdMaktobNumber?:number;
    answerdMaktobDate?:string;
    dateofGuarantee?:string;
    guaranteeDocNumber?:number;
    guaranteeDate?:string;
    guaranteeDocPath?:string;
    calendarType?:string;
    // Conditional fields - Sharia Deed (قباله شرعی)
    courtName?:string;           // محکمه نوم
    collateralNumber?:string;    // نمبر وثیقه
    // Conditional fields - Customary Deed (قباله عرفی)
    setSerialNumber?:string;     // نمبر سریال سټه
    guaranteeDistrictId?:number; // ناحیه
    // Conditional fields - Cash (پول نقد)
    bankName?:string;            // بانک
    depositNumber?:string;       // نمبر اویز
    depositDate?:string;         // تاریخ اویز
}

// Guarantee Type Constants
export enum GuaranteeTypeEnum {
    Cash = 1,          // پول نقد
    ShariaDeed = 2,    // قباله شرعی
    CustomaryDeed = 3  // قباله عرفی
}
