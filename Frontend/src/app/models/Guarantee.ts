export interface Guarantee{
    id: number;
    guaranteeTypeId:number;
    propertyDocumentNumber:number;
    propertyDocumentDate:string;
    senderMaktobNumber:string;
    senderMaktobDate:string;
    answerdMaktobNumber:number;
    answerdMaktobDate:string;
    dateofGuarantee:string;
    guaranteeDocNumber:number;
    guaranteeDate:string;
    companyId:number;
    docPath:string;
    calendarType?:string;
}
