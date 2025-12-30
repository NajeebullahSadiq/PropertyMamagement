import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  private apiUrl = environment.apiURL;

  constructor(private http: HttpClient) { }

  private encodeFilePath(filePath: string): string {
    let cleanPath = filePath || '';
    cleanPath = cleanPath.replace(/^[\/\\]+/, '');
    cleanPath = cleanPath.replace(/\\/g, '/');
    cleanPath = cleanPath.replace(/\/{2,}/g, '/');
    return cleanPath
      .split('/')
      .filter(Boolean)
      .map(segment => encodeURIComponent(segment))
      .join('/');
  }

  uploadFile(file: File, documentType?: string): Observable<HttpEvent<any>> {
    const formData = new FormData();
    formData.append('file', file);

    let url = `${this.apiUrl}/upload`;
    if (documentType) {
      url += `?documentType=${documentType}`;
    }

    const request = new HttpRequest('POST', url, formData, {
      reportProgress: true
    });

    return this.http.request(request);
  }

  viewFile(filePath: string): Observable<Blob> {
    const encodedPath = this.encodeFilePath(filePath);
    const url = `${this.apiUrl}/upload/view/${encodedPath}`;
    console.log('FileService.viewFile: Calling API', {
      filePath: filePath,
      encodedPath: encodedPath,
      url: url
    });
    return this.http.get(url, {
      responseType: 'blob'
    });
  }

  downloadFile(filePath: string): Observable<Blob> {
    const encodedPath = this.encodeFilePath(filePath);
    return this.http.get(`${this.apiUrl}/upload/download/${encodedPath}`, {
      responseType: 'blob'
    });
  }

  deleteFile(filePath: string): Observable<any> {
    const encodedPath = this.encodeFilePath(filePath);
    return this.http.delete(`${this.apiUrl}/upload/delete/${encodedPath}`);
  }

  getFileUrl(filePath: string): string {
    const encodedPath = this.encodeFilePath(filePath);
    const fileUrl = `${this.apiUrl}/upload/view/${encodedPath}`;
    console.log('FileService.getFileUrl:', {
      apiUrl: this.apiUrl,
      originalPath: filePath,
      encodedPath: encodedPath,
      finalUrl: fileUrl
    });
    return fileUrl;
  }

  getDownloadUrl(filePath: string): string {
    const encodedPath = this.encodeFilePath(filePath);
    return `${this.apiUrl}/upload/download/${encodedPath}`;
  }

  getFileExtension(fileName: string): string {
    return fileName.substring(fileName.lastIndexOf('.') + 1).toLowerCase();
  }

  isImageFile(fileName: string): boolean {
    const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'];
    const extension = this.getFileExtension(fileName);
    return imageExtensions.includes(extension);
  }

  isPdfFile(fileName: string): boolean {
    return this.getFileExtension(fileName) === 'pdf';
  }

  getFileIcon(fileName: string): string {
    const extension = this.getFileExtension(fileName);
    switch (extension) {
      case 'pdf':
        return 'fas fa-file-pdf';
      case 'doc':
      case 'docx':
        return 'fas fa-file-word';
      case 'xls':
      case 'xlsx':
        return 'fas fa-file-excel';
      case 'jpg':
      case 'jpeg':
      case 'png':
      case 'gif':
      case 'bmp':
      case 'webp':
        return 'fas fa-image';
      default:
        return 'fas fa-file';
    }
  }

  getFileTypeLabel(fileName: string): string {
    const extension = this.getFileExtension(fileName);
    switch (extension) {
      case 'pdf':
        return 'PDF';
      case 'doc':
      case 'docx':
        return 'Word Document';
      case 'xls':
      case 'xlsx':
        return 'Excel Spreadsheet';
      case 'jpg':
      case 'jpeg':
      case 'png':
      case 'gif':
      case 'bmp':
      case 'webp':
        return 'Image';
      case 'txt':
        return 'Text File';
      default:
        return 'Document';
    }
  }
}
