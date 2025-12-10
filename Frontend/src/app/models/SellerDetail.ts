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
}
