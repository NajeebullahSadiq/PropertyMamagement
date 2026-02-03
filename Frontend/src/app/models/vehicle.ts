export interface VehicleDetails{
    id: number;
    permitNo:string;
    pilateNo: string;
    typeOfVehicle:string;
    model:string;
    enginNo:string;
    shasiNo:string;
    color:string;
    price:number;
    priceText:string;
    halfPrice:string;  // Changed to string to match backend
    royaltyAmount:string;  // Changed to string to match backend
    des:string;
    filePath:string;
    vehicleHand:string;
    companyId?:number;
    iscomplete?:boolean;
    iseditable?:boolean; 
}
