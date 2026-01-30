export interface witnessDetail {
    id:number,
    firstName:string;
    fatherName:string;
    grandFather:string;
    electronicNationalIdNumber?:string;
    phoneNumber:string;
    propertyDetailsId:number;
    nationalIdCard:string;
    nationalIdCardPath?:string;
    paddressProvinceId?:number;
    paddressDistrictId?:number;
    paddressVillage?:string;
    relationshipToParties?:string;
    witnessType?:string;
}
