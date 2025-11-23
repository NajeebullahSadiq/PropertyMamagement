export interface VehicleDetails{
    id: number;
    permitNo:number;
    pilateNo: number;
    typeOfVehicle:string;
    model:string;
    enginNo:number;
    shasiNo:number;
    color:string;
    price:number;
    priceText:string;
    royaltyAmount:number;
    des:string;
    filePath:string;
    vehicleHand:string;
    iscomplete?:boolean;
    iseditable?:boolean; 
}