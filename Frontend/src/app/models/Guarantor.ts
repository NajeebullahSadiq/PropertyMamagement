export interface Guarantor{
    id: number;
    firstName:string;
    fatherName:string;
    grandFatherName?:string;
    identityCardTypeId:number;
    indentityCardNumber:number;
    jild:string;
    safha:string;
    companyId:number;
    sabtNumber:string;
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
}
