export interface companyOwnerAddress{
    id:number;
    addressTypeId:number;
    provinceId:number;
    districtId:number;
    village:string;
    companyOwnerId:number;
}

export interface companyOwnerAddressData{
    id:number;
    addressTypeId:number;
    provinceId:number;
    districtId:number;
    village:string;
    companyOwnerId:number;
    addressType:string;
    province:string;
    district:string;
}
