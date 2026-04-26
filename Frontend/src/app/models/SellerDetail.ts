export interface SellerDetail {
    id:number,
    firstName:string;
    fatherName:string;
    grandFather:string;
    electronicNationalIdNumber?:string;
    phoneNumber:string;
    paddressProvinceId:Number;
    paddressDistrictId:number;
    paddressVillage:string;
    taddressProvinceId:number;
    taddressDistrictId:number;
    taddressVillage:string;
    propertyDetailsId:number;
    photo:string;
    nationalIdCard?:string;
    nationalIdCardPath?:string;
    roleType:string; // Seller role type
    authorizationLetter?:string; // Path to authorization letter file (for agents)
    heirsLetter?:string; // Path to heirs letter file (for heirs)
    propertyTypeId?:number;
    customPropertyType?:string;
    price?:number;
    priceText?:string;
    royaltyAmount?:number;
    halfPrice?:number;
    rentStartDate?:string; // Rental start date as Hijri Shamsi string
    rentEndDate?:string; // Rental end date as Hijri Shamsi string
    rentStartDateStr?:string; // Rent start date string for backend
    rentEndDateStr?:string; // Rent end date string for backend
    calendarType?:string; // Calendar type for date conversion
    transactionType?:string; // Transaction type (Purchase, Rent, Revocable Sale, Other)
    transactionTypeDescription?:string; // Custom description when transactionType is 'Other'
    taxIdentificationNumber?:string;
    additionalDetails?:string;
    sharePercentage?:number;
    shareAmount?:number;
}

export interface VBuyerDetail {
    id:number,
    firstName:string;
    fatherName:string;
    grandFather:string;
    electronicNationalIdNumber?:string;
    phoneNumber:string;
    paddressProvinceId:Number;
    paddressDistrictId:number;
    paddressVillage:string;
    taddressProvinceId:number;
    taddressDistrictId:number;
    taddressVillage:string;
    propertyDetailsId:number;
    photo:string;
    nationalIdCard?:string;
    nationalIdCardPath?:string;
    roleType:string; // Seller role type
    authorizationLetter?:string; // Path to authorization letter file (for agents)
    heirsLetter?:string; // Path to heirs letter file (for heirs)
    propertyTypeId?:number;
    customPropertyType?:string;
    price?:number;
    priceText?:string;
    royaltyAmount?:number;
    halfPrice?:number;
    rentStartDate?:string; // Rental start date as Hijri Shamsi string
    rentEndDate?:string; // Rental end date as Hijri Shamsi string
    rentStartDateStr?:string; // Rent start date string for backend
    rentEndDateStr?:string; // Rent end date string for backend
    calendarType?:string; // Calendar type for date conversion
    transactionType?:string; // Transaction type (Purchase, Rent, Revocable Sale, Other)
    transactionTypeDescription?:string; // Custom description when transactionType is 'Other'
    sharePercentage?:number;
    shareAmount?:number;
}
