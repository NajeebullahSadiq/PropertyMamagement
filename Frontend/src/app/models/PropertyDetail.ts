import { CalendarType } from './calendar-type';

export interface PropertyDetails {
   
    id: number;
    parea: number;
    punitTypeId:number;
    numofFloor:number;
    numofRooms:number;
    propertyTypeId:number;
    customPropertyType?:string;
    price:number;
    priceText:string;
    royaltyAmount:number;
    transactionTypeId:number;
    des:string;
    filePath:string;
    previousDocumentsPath?:string;
    existingDocumentsPath?:string;
    iscomplete:boolean;
    iseditable:boolean;
    west:string;
    south:string;
    east:string;
    north:string;
    documentType?:string;
    issuanceNumber?:string;
    issuanceDate?:string;
    serialNumber?:string;
    transactionDate?:string;
    calendarType?: CalendarType;
    status?: string;
    verifiedBy?: string;
    verifiedAt?: string;
    approvedBy?: string;
    approvedAt?: string;

    propertyAddresses?: any[];

  }

  export interface PropertyDetailsList {
   
    id: number;
    pnumber?: number;
    parea: number;
    punitTypeId:number;
    numofFloor:number;
    numofRooms:number;
    propertyTypeId:number;
    customPropertyType?:string;
    price:number;
    priceText:string;
    royaltyAmount:number;
    transactionTypeId:number;
    des:string;
    filePath:string;
    propertyTypeText:string;
    unitTypeText:string;
    transactionTypeText:string;
    iscomplete:boolean;
    buyerName:string;
    sellerName:string;
    buyerIndentityCardNumber?: number;
    sellerIndentityCardNumber?: number;
    status?: string;

  }

  export interface VehiclesDetailsList {
   
    id: number;
    permitNo:number;
    pilateNo: number;
    typeOfVehicle:string;
    model:string;
    enginNo:string;
    shasiNo:string;
    color:string;
    price:number;
    priceText:string;
    royaltyAmount:number;
    transactionTypeId:number;
    des:string;
    filePath:string;
    unitTypeText:string;
    iscomplete:boolean;
    buyerName:string;
    sellerName:string;

  }
 
