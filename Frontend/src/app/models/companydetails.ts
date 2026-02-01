export interface companydetails{
    id: number;
    title:string;
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
    granator:string;
    isComplete:boolean;


  }
