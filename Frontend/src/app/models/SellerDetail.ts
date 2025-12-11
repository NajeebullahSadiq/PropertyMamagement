export interface SellerDetail {
    id:number,
    firstName:string;
    fatherName:string;
    grandFather:string;
    indentityCardNumber:number;
    phoneNumber:string;
    paddressProvinceId:Number;
    paddressDistrictId:number;
    paddressVillage:string;
    taddressProvinceId:number;
    taddressDistrictId:number;
    taddressVillage:string;
    propertyDetailsId:number;
    photo:string;
    nationalIdCardPath:string;
    roleType:string; // Seller role type
    authorizationLetter?:string; // Path to authorization letter file (for agents)
    heirsLetter?:string; // Path to heirs letter file (for heirs)
    propertyTypeId?:number;
    price?:number;
    priceText?:string;
    royaltyAmount?:number;
    halfPrice?:number;
    rentStartDate?:Date; // Rental start date for lessee roles
    rentEndDate?:Date; // Rental end date for lessee roles
}

export interface VBuyerDetail {
    id:number,
    firstName:string;
    fatherName:string;
    grandFather:string;
    indentityCardNumber:number;
    phoneNumber:string;
    paddressProvinceId:Number;
    paddressDistrictId:number;
    paddressVillage:string;
    taddressProvinceId:number;
    taddressDistrictId:number;
    taddressVillage:string;
    propertyDetailsId:number;
    photo:string;
    nationalIdCardPath:string;
    roleType:string; // Seller role type
    authorizationLetter?:string; // Path to authorization letter file (for agents)
    heirsLetter?:string; // Path to heirs letter file (for heirs)
    propertyTypeId?:number;
    price?:number;
    priceText?:string;
    royaltyAmount?:number;
    halfPrice?:number;
    rentStartDate?:Date; // Rental start date for lessee roles
    rentEndDate?:Date; // Rental end date for lessee roles
}
