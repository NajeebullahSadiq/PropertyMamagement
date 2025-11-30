import { DatePipe } from '@angular/common';
import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import {environment } from 'src/app/environments/environment';

@Component({
  selector: 'app-nationalid-upload',
  templateUrl: './nationalid-upload.component.html',
  styleUrls: ['./nationalid-upload.component.scss'],
  providers: [DatePipe]
})
export class NationalidUploadComponent implements OnInit {

  progress:number =0;
  message: string='';
  @Output() sendMessage=new EventEmitter<string>();
  myDate=new Date();
  date:any;
  hour:any;
  minutes:any;
  second:any;
  fileName = '';
 
  constructor(private http: HttpClient,private datePipe: DatePipe) { 
   
  }

  ngOnInit() {
    var date = new Date;
    this.date = this.datePipe.transform(this.myDate, 'yyyy-MM-dd');
    this.hour=date.getHours();
    this.minutes=date.getMinutes();
    this.second=date.getSeconds();
  }

  uploadFile = (files:any) => {
    console.log('National ID Upload: Starting upload process', files);
    if (files.length === 0) {
      console.log('National ID Upload: No files selected');
      return;
    }

    let fileToUpload = <File>files[0];
    console.log('National ID Upload: File selected', fileToUpload.name, fileToUpload.size);
    const formData = new FormData();
    formData.append('file', fileToUpload,this.date+this.hour+this.minutes+this.second+fileToUpload.name);
    this.fileName=fileToUpload.name;
    
    const uploadUrl = environment.apiURL+'/upload';
    console.log('National ID Upload: Calling API', uploadUrl);
    
    this.http.post(uploadUrl, formData, {reportProgress: true, observe: 'events'})
      .subscribe({
        next: (event) => {
        console.log('National ID Upload: API Response', event);
        if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * (event.loaded || 1) / (event.total || 1))
         if (event.type === HttpEventType.Response) {
          this.message = fileToUpload.name+' '+'موفقانه آپلود شد';
          this.progress = 0;
          console.log('National ID Upload: Upload completed, emitting event');
          this.sendMessage.emit(this.date+this.hour+this.minutes+this.second+fileToUpload.name);
        }
      },
      error: (err: HttpErrorResponse) => {
        console.error('National ID Upload: API Error', err);
        this.progress = 0;
        console.log(err);
      }
    });
  }
  
  reset(): void {
    this.message = '';
    this.progress=0;
  }

}
