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
    roleType:string; // "Seller" or "Authorized Agent (Seller)"
    authorizationLetter:string; // Path to authorization letter file
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
    roleType:string; // "Buyer" or "Authorized Agent (Buyer)"
    authorizationLetter:string; // Path to authorization letter file
}