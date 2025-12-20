import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-document-viewer',
  templateUrl: './document-viewer.component.html',
  styleUrls: ['./document-viewer.component.scss']
})
export class DocumentViewerComponent implements OnInit {
  isLoading = true;
  error: string | null = null;
  fileUrl: SafeResourceUrl | null = null;
  rawFileUrl: string = '';
  isImage = false;
  isPdf = false;
  fileName = '';
  fileExtension = '';

  constructor(
    public dialogRef: MatDialogRef<DocumentViewerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { filePath: string; fileName: string },
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    console.log('=== DocumentViewer Init ===' , {
      filePath: this.data?.filePath,
      fileName: this.data?.fileName,
      apiUrl: environment.apiURL
    });
    
    if (!this.data?.filePath || !this.data?.fileName) {
      this.error = 'اطلاعات فایل نامعتبر است.';
      this.isLoading = false;
      return;
    }

    this.fileName = this.data.fileName;
    this.fileExtension = this.getFileExtension(this.fileName);
    this.isImage = this.checkIsImage(this.fileExtension);
    this.isPdf = this.fileExtension === 'pdf';

    console.log('File type detected:', {
      extension: this.fileExtension,
      isImage: this.isImage,
      isPdf: this.isPdf
    });

    // Build the direct URL to the file
    const cleanPath = this.data.filePath.replace(/^[\/\\]+/, '');
    this.rawFileUrl = `${environment.apiURL}/upload/view/${cleanPath}`;
    
    console.log('Generated file URL:', this.rawFileUrl);

    // Sanitize the URL for Angular security
    if (this.isPdf) {
      this.fileUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.rawFileUrl);
    } else {
      this.fileUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.rawFileUrl);
    }

    // Stop loading immediately - let the browser handle the rest
    this.isLoading = false;
    
    console.log('DocumentViewer: State after init', {
      isLoading: this.isLoading,
      error: this.error,
      isImage: this.isImage,
      isPdf: this.isPdf,
      fileUrl: this.fileUrl,
      rawFileUrl: this.rawFileUrl
    });
  }

  getFileExtension(fileName: string): string {
    return fileName.substring(fileName.lastIndexOf('.') + 1).toLowerCase();
  }

  checkIsImage(extension: string): boolean {
    const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'];
    return imageExtensions.includes(extension);
  }

  onImageLoad(): void {
    console.log('✓ Image loaded successfully');
    this.isLoading = false;
    this.error = null;
  }

  onImageError(event: any): void {
    console.error('✗ Image failed to load:', event);
    this.error = 'خطا در بارگذاری تصویر. لطفا فایل را دانلود کنید.';
    this.isLoading = false;
  }

  onPdfLoad(): void {
    console.log('✓ PDF loaded successfully');
    this.isLoading = false;
    this.error = null;
  }

  onPdfError(event: any): void {
    console.error('✗ PDF failed to load:', event);
    this.error = 'خطا در بارگذاری PDF. لطفا فایل را دانلود کنید.';
    this.isLoading = false;
  }

  downloadFile(): void {
    window.open(this.rawFileUrl.replace('/view/', '/download/'), '_blank');
  }

  closeDialog(): void {
    this.dialogRef.close();
  }
}
