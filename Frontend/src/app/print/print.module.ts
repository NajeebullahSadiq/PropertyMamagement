import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { GeolocationGuard } from '../auth/authSetting/geolocation.guard';
import { SharedModule } from '../shared/shared.module';
import { PrintComponent } from './print.component';
import { PrintvehicledataComponent } from '../printvehicledata/printvehicledata.component';
import { PrintLicenseComponent } from '../print-license/print-license.component';
import { PrintSecuritiesComponent } from '../print-securities/print-securities.component';
import { PrintPetitionWriterSecuritiesComponent } from '../print-petition-writer-securities/print-petition-writer-securities.component';
import { PrintPetitionWriterLicenseComponent } from '../print-petition-writer-license/print-petition-writer-license.component';

const routes: Routes = [
  { path: ':id', component: PrintComponent, canActivate: [GeolocationGuard] },
  { path: '', component: PrintComponent, canActivate: [GeolocationGuard] },
  { path: 'vehicle/:id', component: PrintvehicledataComponent, canActivate: [GeolocationGuard] },
  { path: 'vehicle', component: PrintvehicledataComponent, canActivate: [GeolocationGuard] },
  { path: 'license/:id', component: PrintLicenseComponent, canActivate: [GeolocationGuard] },
  { path: 'license', component: PrintLicenseComponent, canActivate: [GeolocationGuard] },
  { path: 'securities/:id', component: PrintSecuritiesComponent, canActivate: [GeolocationGuard] },
  { path: 'securities', component: PrintSecuritiesComponent, canActivate: [GeolocationGuard] },
  { path: 'petition-writer-securities/:id', component: PrintPetitionWriterSecuritiesComponent, canActivate: [GeolocationGuard] },
  { path: 'petition-writer-securities', component: PrintPetitionWriterSecuritiesComponent, canActivate: [GeolocationGuard] },
  { path: 'petition-writer-license/:id', component: PrintPetitionWriterLicenseComponent, canActivate: [GeolocationGuard] },
  { path: 'petition-writer-license', component: PrintPetitionWriterLicenseComponent, canActivate: [GeolocationGuard] },
];

@NgModule({
  declarations: [
    PrintComponent,
    PrintvehicledataComponent,
    PrintLicenseComponent,
    PrintSecuritiesComponent,
    PrintPetitionWriterSecuritiesComponent,
    PrintPetitionWriterLicenseComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forChild(routes),
    SharedModule,
    TranslateModule.forChild(),
  ],
})
export class PrintModule {}
