// import { Injectable } from '@angular/core';
// import jsPDF from 'jspdf';

// @Injectable({
//   providedIn: 'root',
// })
// export class PdfService {
//   generatePdf(htmlContent: string) {
//     const pdf = new jsPDF('p', 'mm', 'a4');

//     // Convert the HTML content to a canvas
//     const canvas = document.createElement('canvas');
//     canvas.width = pdf.internal.pageSize.getWidth();
//     canvas.height = pdf.internal.pageSize.getHeight();
//     const ctx = canvas.getContext('2d');

//     if (ctx) { // Check if ctx is not null
//       const dataUrl = 'data:image/svg+xml,' + encodeURIComponent(htmlContent);
//       const img = new Image();

//       img.onload = () => {
//         ctx.drawImage(img, 0, 0);
//         pdf.addImage(canvas.toDataURL('image/jpeg'), 'JPEG', 0, 0, pdf.internal.pageSize.getWidth(), pdf.internal.pageSize.getHeight());
//         pdf.save('document.pdf');
//       };

//       img.src = dataUrl;
//     } else {
//       console.error('Canvas context is null. PDF generation failed.');
//     }
//   }
// }
