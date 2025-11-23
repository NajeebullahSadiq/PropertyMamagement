import { DatePipe } from '@angular/common';
import { HttpClient, HttpErrorResponse, HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss'],
  providers: [DatePipe]
})
export class UploadComponent implements OnInit {

  progress = 0;
  message: string='';
  @Output() sendMessage=new EventEmitter<string>();
  myDate=new Date();
  date:any;
  hour:any;
  minutes:any;
  second:any;
  fileName = '';
  apiURL= 'http://localhost:5143/api'
  constructor(private http: HttpClient,private datePipe: DatePipe,private translateService: TranslateService) { 
   
  }

  ngOnInit() {
    var date = new Date;
    this.date = this.datePipe.transform(this.myDate, 'yyyy-MM-dd');
    this.hour=date.getHours();
    this.minutes=date.getMinutes();
    this.second=date.getSeconds();
  }

  uploadFile = (files:any) => {
    if (files.length === 0) {
      return;
    }

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload,this.date+this.hour+this.minutes+this.second+fileToUpload.name);
    this.fileName=fileToUpload.name;
    this.http.post(this.apiURL+'/upload', formData, {reportProgress: true, observe: 'events'})
      .subscribe({
        next: (event) => {
        if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * (event.loaded || 1) / (event.total || 1))
         if (event.type === HttpEventType.Response) {
          this.message = fileToUpload.name + ' ' + this.translateService.instant('success');
          this.sendMessage.emit(this.date+this.hour+this.minutes+this.second+fileToUpload.name);
        }
      },
      error: (err: HttpErrorResponse) => console.log(err)
    });
  }
  childMethod() {
    this.message="";
    this.fileName="";
    this.progress=0;
  }

}
