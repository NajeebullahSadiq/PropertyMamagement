import { DatePipe } from '@angular/common';
import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges, OnChanges } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DocumentViewerComponent } from 'src/app/shared/document-viewer/document-viewer.component';
import { FileService } from 'src/app/shared/file.service';
import {environment } from 'src/environments/environment';

@Component({
  selector: 'app-nationalid-upload',
  templateUrl: './nationalid-upload.component.html',
  styleUrls: ['./nationalid-upload.component.scss'],
  providers: [DatePipe]
})
export class NationalidUploadComponent implements OnInit, OnChanges {

  progress:number =0;
  message: string='';
  uploadedFilePath: string = '';
  uploadedFileName: string = '';
  isUploading: boolean = false;
  @Input() existingFilePath: string = '';
  @Input() existingFileName: string = '';
  @Output() sendMessage=new EventEmitter<string>();
  myDate=new Date();
  date:any;
  hour:any;
  minutes:any;
  second:any;
  fileName = '';
 
  constructor(
    private http: HttpClient,
    private datePipe: DatePipe,
    private dialog: MatDialog,
    private fileService: FileService
  ) { 
   
  }

  ngOnInit() {
    var date = new Date;
    this.date = this.datePipe.transform(this.myDate, 'yyyy-MM-dd');
    this.hour=date.getHours();
    this.minutes=date.getMinutes();
    this.second=date.getSeconds();
    this.initializeExistingFile();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['existingFilePath'] || changes['existingFileName']) {
      this.initializeExistingFile();
    }
  }

  private initializeExistingFile(): void {
    if (this.isUploading) {
      return;
    }

    if (this.existingFilePath) {
      if (this.uploadedFilePath !== this.existingFilePath) {
        this.uploadedFilePath = this.existingFilePath;
        this.uploadedFileName = this.existingFileName || this.extractFileName(this.existingFilePath);
        this.message = this.uploadedFileName ? 'فایل موجود' : '';
        this.progress = 0;
      }
      return;
    }

    if (this.uploadedFilePath || this.uploadedFileName || this.message) {
      this.uploadedFilePath = '';
      this.uploadedFileName = '';
      this.message = '';
      this.progress = 0;
    }
  }

  private extractFileName(filePath: string): string {
    if (!filePath) return '';
    const parts = filePath.split(/[\\\/]/);
    return parts[parts.length - 1] || '';
  }

  uploadFile = (files:any) => {
    if (files.length === 0) {
      return;
    }

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload);
    this.fileName = fileToUpload.name;
    this.isUploading = true;
    
    const uploadUrl = environment.apiURL+'/upload?documentType=identity';
    
    this.http.post(uploadUrl, formData, {reportProgress: true, observe: 'events'})
      .subscribe({
        next: (event) => {
          if (event.type === HttpEventType.UploadProgress) {
            this.progress = Math.round(100 * (event.loaded || 1) / (event.total || 1));
          }
          if (event.type === HttpEventType.Response) {
            const response: any = event.body;
            this.uploadedFilePath = response.dbPath;
            this.uploadedFileName = response.originalFileName;
            this.message = fileToUpload.name + ' ' + 'موفقانه آپلود شد';
            this.isUploading = false;
            this.sendMessage.emit(response.dbPath);
          }
        },
        error: (err: HttpErrorResponse) => {
          console.error('National ID Upload: API Error', err);
          this.isUploading = false;
          this.progress = 0;
          this.message = 'خطا در آپلود فایل';
        }
      });
  }

  viewFile(): void {
    if (this.uploadedFilePath) {
      this.dialog.open(DocumentViewerComponent, {
        width: '90%',
        maxWidth: '1200px',
        height: '90vh',
        data: {
          filePath: this.uploadedFilePath,
          fileName: this.uploadedFileName
        }
      });
    }
  }

  downloadFile(): void {
    if (this.uploadedFilePath) {
      const link = document.createElement('a');
      link.href = this.fileService.getDownloadUrl(this.uploadedFilePath);
      link.download = this.uploadedFileName;
      link.click();
    }
  }

  deleteFile(): void {
    if (this.uploadedFilePath) {
      this.fileService.deleteFile(this.uploadedFilePath).subscribe({
        next: () => {
          this.uploadedFilePath = '';
          this.uploadedFileName = '';
          this.message = '';
          this.progress = 0;
        },
        error: (err) => {
          console.error('Error deleting file:', err);
        }
      });
    }
  }

  getFileIcon(fileName: string): string {
    return this.fileService.getFileIcon(fileName);
  }

  getFileTypeLabel(fileName: string): string {
    return this.fileService.getFileTypeLabel(fileName);
  }
  
  reset(): void {
    this.message = '';
    this.progress = 0;
    this.uploadedFilePath = '';
    this.uploadedFileName = '';
    this.isUploading = false;
  }

}
