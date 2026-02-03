export interface companydetails{
    id: number;
    title:string;
    tin:number,
    docPath:string;
    provinceId?:number;
    calendarType?:string;
}

export interface companydetailsList {
   
    id: number;
    title:string;
    ownerFullName:string;
    ownerFatherName:string;
    ownerElectronicNationalIdNumber:string;
    licenseNumber:string;
    granator:string;
    isComplete:boolean;


  }
