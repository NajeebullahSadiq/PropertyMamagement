export interface LicenseDetail{
    id: number;
    provinceId?: number;        // Province for license numbering
    licenseNumber:number;
    issueDate:string;
    expireDate:string;
    transferLocation?:string;   // محل انتقال (Transfer Location) - replaces areaId
    officeAddress:string;
    companyId:number;
    docPath:string;
    licenseType:string;
    // License Category (نوعیت جواز): جدید, تجدید, مثنی
    licenseCategory?:string;
    // Renewal Round (دور تجدید) - only applicable when licenseCategory is تجدید
    renewalRound?:number;
    calendarType?:string;
    // Financial and Administrative Fields (جزئیات مالی و اسناد جواز)
    royaltyAmount?:number;      // مبلغ حق‌الامتیاز
    royaltyDate?:string;        // تاریخ پرداخت تعرفه
    tariffNumber?:string;       // نمبر تعرفه
    penaltyAmount?:number;      // مبلغ جریمه
    penaltyDate?:string;        // تاریخ جریمه
    hrLetter?:string;           // مکتوب قوای بشری
    hrLetterDate?:string;       // تاریخ مکتوب قوای بشری
}
