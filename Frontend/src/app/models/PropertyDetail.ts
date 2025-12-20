
export interface PropertyDetails {
   
    id: number;
    pnumber:number;
    parea: number;
    punitTypeId:number;
    numofFloor:number;
    numofRooms:number;
    propertyTypeId:number;
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
    issuanceDate?:Date;
    serialNumber?:string;
    transactionDate?:Date;

  }

  export interface PropertyDetailsList {
   
    id: number;
    pnumber:number;
    parea: number;
    punitTypeId:number;
    numofFloor:number;
    numofRooms:number;
    propertyTypeId:number;
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
 
