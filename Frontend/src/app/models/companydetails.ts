export interface companydetails{
    id: number;
    title:string;
    phoneNumber: number;
    licenseNumber:string;
    petitionDate:string;
    petitionNumber:number;
    tin:number,
    docPath:string;
    calendarType?:string;
}

export interface companydetailsList {
   
    id: number;
    title:string;
    ownerFullName:string;
    ownerFatherName:string;
    ownerIdNumber:number;
    licenseNumber:string;
    phoneNumber:string;
    granator:string;


  }
