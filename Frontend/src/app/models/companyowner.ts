export interface companyowner{
    id: number;
    firstName:string;
    fatherName:string;
    grandFatherName:string;
    educationLevelId:number;
    dateofBirth:string;
    electronicNationalIdNumber?:string;
    companyId:number;
    pothoPath:string;
    calendarType?:string;
    // Contact Information
    phoneNumber?:string;
    whatsAppNumber?:string;
    // Owner's Own Address Fields (آدرس اصلی مالک)
    ownerProvinceId?:number;
    ownerDistrictId?:number;
    ownerVillage?:string;
    // Permanent Address Fields (آدرس دایمی) - Current Residence
    permanentProvinceId?:number;
    permanentDistrictId?:number;
    permanentVillage?:string;
    // Location names for display
    ownerProvinceName?:string;
    ownerDistrictName?:string;
    permanentProvinceName?:string;
    permanentDistrictName?:string;
    // Flag for address change operation
    isAddressChange?:boolean;
}

export interface companyOwnerAddressHistory {
    id: number;
    addressType: string;
    provinceId?: number;
    districtId?: number;
    village?: string;
    provinceName?: string;
    districtName?: string;
    effectiveFrom: string;
    effectiveTo?: string;
    isActive: boolean;
}
