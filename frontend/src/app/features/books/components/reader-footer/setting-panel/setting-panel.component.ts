import { Component, input, Input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReadingSettings } from './../../../interfaces/reading-setting.interface';

@Component({
  selector: 'reader-settings-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: `./setting-panel.component.html`
})
export class ReaderSettingsMenuComponent {
  isOpen = input(false);

  closeEvent = output();
  settingsChange = output<ReadingSettings>();

  // Configuraciones
  fonts = [
    { label: 'OpenSans', value: 'opensans' },
    { label: 'Source serif', value: 'source-serif' },
    { label: 'Inter', value: 'inter' },
    { label: 'Merriweather', value: 'merriweather' },
    { label: 'Lato', value: 'lato' },
    { label: 'Montserrat', value: 'montserrat' }
  ];

  selectedFont = 'opensans';
  textSize = 16;
  lineHeight = 24;
  highContrast = false;

  selectFont(font: any) {
    this.selectedFont = font.value;
    this.emitSettingsChange();
  }

  increaseTextSize() {
    if (this.textSize < 24) {
      this.textSize++;
      this.emitSettingsChange();
    }
  }

  decreaseTextSize() {
    if (this.textSize > 12) {
      this.textSize--;
      this.emitSettingsChange();
    }
  }

  increaseLineHeight() {
    if (this.lineHeight < 36) {
      this.lineHeight++;
      this.emitSettingsChange();
    }
  }

  decreaseLineHeight() {
    if (this.lineHeight > 16) {
      this.lineHeight--;
      this.emitSettingsChange();
    }
  }

  toggleContrast(high: boolean) {
    this.highContrast = high;
    this.emitSettingsChange();
  }

  resetSettings() {
    this.selectedFont = 'opensans';
    this.textSize = 16;
    this.lineHeight = 24;
    this.highContrast = false;
    this.emitSettingsChange();
  }

  closeMenu() {
    this.closeEvent.emit();
  }

  private emitSettingsChange() {

    this.settingsChange.emit({
      font: this.selectedFont,
      textSize: this.textSize,
      lineHeight: this.lineHeight,
      highContrast: this.highContrast
    });
  }


}
