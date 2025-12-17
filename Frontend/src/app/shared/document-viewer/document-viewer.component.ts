import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FileService } from '../file.service';
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-document-viewer',
  templateUrl: './document-viewer.component.html',
  styleUrls: ['./document-viewer.component.scss']
})
export class DocumentViewerComponent implements OnInit, OnDestroy {
  isLoading = false;
  error: string | null = null;
  fileUrl: SafeResourceUrl | SafeUrl | null = null;
  imageUrl: SafeUrl | null = null;
  pdfUrl: SafeResourceUrl | null = null;
  isImage = false;
  isPdf = false;
  fileName = '';
  private blobUrls: string[] = [];

  constructor(
    public dialogRef: MatDialogRef<DocumentViewerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { filePath: string; fileName: string },
    private fileService: FileService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    console.log('DocumentViewerComponent.ngOnInit - data received:', {
      filePath: this.data?.filePath,
      fileName: this.data?.fileName
    });
    this.loadFile();
  }

  ngOnDestroy(): void {
    // Clean up blob URLs to prevent memory leaks
    this.blobUrls.forEach(url => {
      URL.revokeObjectURL(url);
    });
    this.blobUrls = [];
  }

  loadFile(): void {
    this.isLoading = true;
    this.error = null;
    this.fileName = this.data.fileName;

    console.log('DocumentViewer: Loading file', {
      filePath: this.data.filePath,
      fileName: this.data.fileName
    });

    // Check if we have valid file data
    if (!this.data.filePath || !this.data.fileName) {
      this.error = 'اطلاعات فایل نامعتبر است.';
      this.isLoading = false;
      console.error('DocumentViewer: Invalid file data', this.data);
      return;
    }

    this.isImage = this.fileService.isImageFile(this.data.fileName);
    this.isPdf = this.fileService.isPdfFile(this.data.fileName);

    console.log('DocumentViewer: File type check', {
      isImage: this.isImage,
      isPdf: this.isPdf,
      filePath: this.data.filePath,
      fileName: this.data.fileName
    });

    if (this.isImage) {
      // Use bypassSecurityTrustUrl for images (not bypassSecurityTrustResourceUrl)
      const fileUrl = this.fileService.getFileUrl(this.data.filePath);
      console.log('DocumentViewer: Generated image URL:', fileUrl);
      
      // Test if the URL is accessible before setting it
      const img = new Image();
      img.onload = () => {
        console.log('DocumentViewer: Image loaded successfully');
        this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(fileUrl);
        this.fileUrl = this.imageUrl;
        this.isLoading = false;
      };
      img.onerror = (event) => {
        console.error('DocumentViewer: Failed to load image via direct URL, trying blob method', { fileUrl, event });
        this.loadImageAsBlob();
      };
      img.src = fileUrl;
      
    } else if (this.isPdf) {
      // For PDFs, try blob method directly as it's more reliable for cross-origin content
      console.log('DocumentViewer: Loading PDF using blob method');
      this.loadPdfAsBlob();
    } else {
      this.error = 'نوع فایل پشتیبانی نشده است. لطفا فایل را دانلود کنید.';
      this.isLoading = false;
      console.log('DocumentViewer: Unsupported file type');
    }
  }

  downloadFile(): void {
    const link = document.createElement('a');
    link.href = this.fileService.getDownloadUrl(this.data.filePath);
    link.download = this.data.fileName;
    link.click();
  }

  closeDialog(): void {
    this.dialogRef.close();
  }

  onImageError(): void {
    console.error('DocumentViewer: Image failed to load', {
      fileUrl: this.fileUrl,
      filePath: this.data.filePath,
      fileName: this.data.fileName
    });
    this.error = 'خطا در بارگذاری تصویر. لطفا فایل را دانلود کنید.';
    this.isLoading = false;
  }

  onImageLoad(): void {
    console.log('DocumentViewer: Image loaded successfully');
  }

  onPdfLoad(): void {
    console.log('DocumentViewer: PDF iframe loaded successfully');
  }

  onPdfError(): void {
    console.error('DocumentViewer: PDF iframe failed to load');
    this.error = 'خطا در بارگذاری PDF. لطفا فایل را دانلود کنید.';
    this.isLoading = false;
  }

  loadImageAsBlob(): void {
    console.log('DocumentViewer: Loading image as blob for', this.data.filePath);
    this.fileService.viewFile(this.data.filePath).subscribe({
      next: (blob) => {
        console.log('DocumentViewer: Successfully loaded image blob');
        const blobUrl = URL.createObjectURL(blob);
        this.blobUrls.push(blobUrl); // Track for cleanup
        this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(blobUrl);
        this.fileUrl = this.imageUrl;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('DocumentViewer: Failed to load image as blob', error);
        this.error = 'خطا در بارگذاری تصویر. لطفا فایل را دانلود کنید.';
        this.isLoading = false;
      }
    });
  }

  loadPdfAsBlob(): void {
    console.log('DocumentViewer: Loading PDF as blob for', this.data.filePath);
    this.fileService.viewFile(this.data.filePath).subscribe({
      next: (blob) => {
        console.log('DocumentViewer: Successfully loaded PDF blob', {
          blobSize: blob.size,
          blobType: blob.type
        });
        
        // Check if blob is valid
        if (blob.size === 0) {
          console.error('DocumentViewer: Empty blob received for PDF');
          this.error = 'فایل PDF خالی است یا در دسترس نمی‌باشد.';
          this.isLoading = false;
          return;
        }
        
        const blobUrl = URL.createObjectURL(blob);
        this.blobUrls.push(blobUrl); // Track for cleanup
        console.log('DocumentViewer: Created blob URL for PDF:', blobUrl);
        
        this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(blobUrl);
        this.fileUrl = this.pdfUrl;
        this.isLoading = false;
        
        // Set a timeout to check if PDF loaded
        setTimeout(() => {
          if (this.pdfUrl && !this.error) {
            console.log('DocumentViewer: PDF should be visible now');
          }
        }, 2000);
      },
      error: (error) => {
        console.error('DocumentViewer: Failed to load PDF as blob', error);
        this.error = 'خطا در بارگذاری PDF. لطفا فایل را دانلود کنید.';
        this.isLoading = false;
      }
    });
  }
}
