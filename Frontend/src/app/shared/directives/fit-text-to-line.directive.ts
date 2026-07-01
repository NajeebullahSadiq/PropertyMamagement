import { AfterViewInit, Directive, ElementRef, Input, OnDestroy } from '@angular/core';

/**
 * Shrinks font size so text stays on a single line within its container.
 */
@Directive({
  selector: '[appFitTextToLine]'
})
export class FitTextToLineDirective implements AfterViewInit, OnDestroy {
  @Input() minFontSize = 10;
  @Input() maxFontSize = 18;

  private resizeObserver?: ResizeObserver;
  private mutationObserver?: MutationObserver;
  private onWindowResize = () => this.fitText();

  constructor(private el: ElementRef<HTMLElement>) {}

  ngAfterViewInit(): void {
    const element = this.el.nativeElement;
    element.style.whiteSpace = 'nowrap';
    element.style.overflow = 'hidden';

    requestAnimationFrame(() => this.fitText());

    if (document.fonts?.ready) {
      document.fonts.ready.then(() => this.fitText());
    }

    window.addEventListener('resize', this.onWindowResize);

    this.resizeObserver = new ResizeObserver(() => this.fitText());
    this.resizeObserver.observe(element);

    const parent = element.parentElement;
    if (parent) {
      this.resizeObserver.observe(parent);
    }

    this.mutationObserver = new MutationObserver(() => this.fitText());
    this.mutationObserver.observe(element, {
      childList: true,
      characterData: true,
      subtree: true
    });
  }

  private fitText(): void {
    const element = this.el.nativeElement;
    let fontSize = this.maxFontSize;

    element.style.fontSize = `${fontSize}px`;

    while (element.scrollWidth > element.clientWidth && fontSize > this.minFontSize) {
      fontSize -= 0.5;
      element.style.fontSize = `${fontSize}px`;
    }
  }

  ngOnDestroy(): void {
    window.removeEventListener('resize', this.onWindowResize);
    this.resizeObserver?.disconnect();
    this.mutationObserver?.disconnect();
  }
}
