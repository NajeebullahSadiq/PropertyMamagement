import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Property-Registeration-MIS';

  constructor(private translate: TranslateService) {
    // Set default language
    this.translate.setDefaultLang('دری');
  }

  ngOnInit() {
    // Use the default language
    this.translate.use('دری');
  }
}
